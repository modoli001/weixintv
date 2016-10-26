using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Service.IService;
using Common.Enums;
using System.Threading.Tasks;
using Common;

namespace WebPage.Hubs
{
    /// <summary>
    /// 聊天室
    /// </summary>
    public class ChatHub : Hub
    {
        #region 声明容器
        /// <summary>
        /// 用户管理
        /// </summary>
        public IUserManage UserManage = Spring.Context.Support.ContextRegistry.GetContext().GetObject("Service.User") as IUserManage;
        /// <summary>
        /// 部门管理
        /// </summary>
        public IDepartmentManage DepartmentManage = Spring.Context.Support.ContextRegistry.GetContext().GetObject("Service.Department") as IDepartmentManage;
        /// <summary>
        /// 编码管理
        /// </summary>
        public ICodeManage CodeManage = Spring.Context.Support.ContextRegistry.GetContext().GetObject("Service.Code") as ICodeManage;
        /// <summary>
        /// 用户在线管理
        /// </summary>
        public IUserOnlineManage UserOnlineManage = Spring.Context.Support.ContextRegistry.GetContext().GetObject("Service.UserOnlineManage") as IUserOnlineManage;
        /// <summary>
        /// 聊天消息
        /// </summary>
        public IChatMessageManage ChatMessageManage = Spring.Context.Support.ContextRegistry.GetContext().GetObject("Service.ChatMessageManage") as IChatMessageManage;
        #endregion


        /// <summary>
        /// 用户登录注册信息
        /// </summary>
        /// <param name="id"></param>
        public void Register(string account,string password)
        {
            try
            {
                //获取用户信息
                var User = UserManage.Get(p => p.ACCOUNT == account);
                if (User != null && User.PASSWORD == password)
                {
                    //更新在线状态
                    var UserOnline = UserOnlineManage.LoadListAll(p => p.FK_UserId == User.ID).FirstOrDefault();
                    UserOnline.ConnectId = Context.ConnectionId;
                    UserOnline.OnlineDate = DateTime.Now;
                    UserOnline.IsOnline = true;
                    UserOnline.UserIP = Utils.GetIP();
                    UserOnlineManage.Update(UserOnline);

                    //获取历史消息
                    int days = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["HistoryDays"]);
                    DateTime dtHistory = DateTime.Now.AddDays(-days);
                    var ChatMessageList = ChatMessageManage.LoadAll(p => p.MessageDate > dtHistory);                    

                    //超级管理员
                    if (User.ID == ClsDic.DicRole["超级管理员"])
                    {
                        //通知用户上线
                        Clients.All.UserLoginNotice("超级管理员：" + User.NAME + " 上线了!");

                        var HistoryMessage = ChatMessageList.OrderBy(p=>p.MessageDate).ToList().Select(p => new
                        {
                            UserName = UserManage.Get(m => m.ID == p.FromUser).NAME,
                            UserFace = string.IsNullOrEmpty(UserManage.Get(m => m.ID == p.FromUser).FACE_IMG) ? "/sys/user/User_Default_Avatat?name=" + UserManage.Get(m => m.ID == p.FromUser).NAME.Substring(0, 1) : UserManage.Get(m => m.ID == p.FromUser).FACE_IMG,
                            MessageType=GetMessageType(p.MessageType),
                            p.FromUser,
                            p.MessageContent,
                            MessageDate = p.MessageDate.GetDateTimeFormats('D')[1].ToString() + " - " + p.MessageDate.ToString("HH:mm:ss"),
                            ConnectId = UserOnlineManage.LoadListAll(m => m.FK_UserId == p.FromUser).FirstOrDefault().ConnectId
                        }).ToList();

                        //推送历史消息
                        Clients.Client(Context.ConnectionId).addHistoryMessageToPage(JsonConverter.Serialize(HistoryMessage));
                    }
                    else
                    {
                        //获取用户一级部门信息
                        var Depart = GetUserDepart(User.DPTID);
                        if (Depart != null && !string.IsNullOrEmpty(Depart.ID))
                        {
                            //添加用户到部门群组 Groups.Add（用户连接ID，群组）
                            Groups.Add(Context.ConnectionId, Depart.ID);
                            //通知用户上线
                            Clients.All.UserLoginNotice(Depart.NAME + " - " + CodeManage.Get(m => m.CODEVALUE == User.LEVELS && m.CODETYPE == "ZW").NAMETEXT + "：" + User.NAME + " 上线了!");
                            //用户历史消息
                            int typeOfpublic = Common.Enums.ClsDic.DicMessageType["广播"];
                            int typeOfgroup = Common.Enums.ClsDic.DicMessageType["群组"];
                            int typeOfprivate = Common.Enums.ClsDic.DicMessageType["私聊"];
                            var HistoryMessage = ChatMessageList.Where(p => p.MessageType == typeOfpublic || p.FromUser == User.ID || (p.MessageType == typeOfgroup && p.ToGroup == Depart.ID) || (p.MessageType == typeOfprivate && p.ToGroup == User.ID.ToString())).OrderBy(p => p.MessageDate).ToList().Select(p => new
                            {
                                UserName = UserManage.Get(m => m.ID == p.FromUser).NAME,
                                UserFace = string.IsNullOrEmpty(UserManage.Get(m => m.ID == p.FromUser).FACE_IMG) ? "/sys/user/User_Default_Avatat?name=" + UserManage.Get(m => m.ID == p.FromUser).NAME.Substring(0, 1) : UserManage.Get(m => m.ID == p.FromUser).FACE_IMG,
                                MessageType = GetMessageType(p.MessageType),
                                p.FromUser,
                                p.MessageContent,
                                MessageDate = p.MessageDate.GetDateTimeFormats('D')[1].ToString() + " - " + p.MessageDate.ToString("HH:mm:ss"),
                                ConnectId = UserOnlineManage.LoadListAll(m => m.FK_UserId == p.FromUser).FirstOrDefault().ConnectId
                            }).ToList();
                           
                            //推送历史消息
                            Clients.Client(Context.ConnectionId).addHistoryMessageToPage(JsonConverter.Serialize(HistoryMessage));

                        }
                    }
                    //刷新用户通讯录
                    Clients.All.ContactsNotice(JsonConverter.Serialize(UserOnline));                    
                }
            }
            catch(Exception ex)
            {
                Clients.Client(Context.ConnectionId).UserLoginNotice("出错了：" + ex.Message);
                throw ex.InnerException;
            }
            
        }

        /// <summary>
        /// 发送消息（广播、组播）
        /// </summary>
        /// <param name="message">发送的消息</param>
        /// <param name="message">发送的群组</param>
        public void Send(string message,string groupId)
        {
            try 
            {
                //消息用户主体
                var UserOnline = UserOnlineManage.LoadListAll(p => p.ConnectId == Context.ConnectionId).FirstOrDefault();
                var Users = UserManage.Get(p => p.ID == UserOnline.FK_UserId);
                
                //广播
                if(string.IsNullOrEmpty(groupId))
                {
                    //保存消息
                    ChatMessageManage.Save(new Domain.SYS_CHATMESSAGE() { FromUser = UserOnline.FK_UserId, MessageType = Common.Enums.ClsDic.DicMessageType["广播"], MessageContent = message, MessageDate = DateTime.Now, MessageIP = Utils.GetIP() });

                    //返回消息实体
                    var Message = new Message() { ConnectId = UserOnline.ConnectId, UserName = Users.NAME, UserFace = string.IsNullOrEmpty(Users.FACE_IMG) ? "/sys/user/User_Default_Avatat?name=" + Users.NAME.Substring(0, 1) : Users.FACE_IMG, MessageDate = DateTime.Now.GetDateTimeFormats('D')[1].ToString() + " - " + DateTime.Now.ToString("HH:mm:ss"), MessageContent = message, MessageType = "public", UserId = Users.ID };

                    //推送消息
                    Clients.All.addNewMessageToPage(JsonConverter.Serialize(Message));
                }
                //组播
                else
                {
                    //保存消息
                    ChatMessageManage.Save(new Domain.SYS_CHATMESSAGE() { FromUser = UserOnline.FK_UserId, MessageType = Common.Enums.ClsDic.DicMessageType["群组"], MessageContent = message, MessageDate = DateTime.Now, MessageIP = Utils.GetIP(), ToGroup = groupId });
                    //返回消息实体
                    var Message = new Message() { ConnectId = UserOnline.ConnectId, UserName = Users.NAME, UserFace = string.IsNullOrEmpty(Users.FACE_IMG) ? "/sys/user/User_Default_Avatat?name=" + Users.NAME.Substring(0, 1) : Users.FACE_IMG, MessageDate = DateTime.Now.GetDateTimeFormats('D')[1].ToString() + " - " + DateTime.Now.ToString("HH:mm:ss"), MessageContent = message, MessageType = "group", UserId = Users.ID };

                    //推送消息
                    Clients.Group(groupId).addNewMessageToPage(JsonConverter.Serialize(Message));
                    //如果用户不在群组中则单独推送消息给用户
                    var Depart = GetUserDepart(Users.DPTID);
                    if(Depart==null)
                    {
                        //推送给用户
                        Clients.Client(Context.ConnectionId).addNewMessageToPage(JsonConverter.Serialize(Message));
                    }
                    else if(Depart.ID!=groupId)
                    {
                        //推送给用户
                        Clients.Client(Context.ConnectionId).addNewMessageToPage(JsonConverter.Serialize(Message));
                    }
                }                               
            }
            catch(Exception ex)
            {
                //推送系统消息
                Clients.Client(Context.ConnectionId).addSysMessageToPage("系统消息：消息发送失败，请稍后再试！");
                throw ex.InnerException;
            }            
        }
        /// <summary>
        /// 发送给指定用户（单播）
        /// </summary>
        /// <param name="clientId">接收用户的连接ID</param>
        /// <param name="message">发送的消息</param>
        public void SendSingle(string clientId, string message)
        {
            try
            {
                //接收用户连接为空
                if (string.IsNullOrEmpty(clientId))
                {
                    //推送系统消息
                    Clients.Client(Context.ConnectionId).addSysMessageToPage("系统消息：用户不存在！");
                }
                else
                {
                    //消息用户主体
                    var UserOnline = UserOnlineManage.LoadListAll(p => p.ConnectId == Context.ConnectionId).FirstOrDefault();
                    var Users = UserManage.Get(p => p.ID == UserOnline.FK_UserId);
                    //接收消息用户主体
                    var ReceiveUser = UserOnlineManage.LoadListAll(p => p.ConnectId == clientId).FirstOrDefault();
                    if (ReceiveUser == null)
                    {
                        //推送系统消息
                        Clients.Client(Context.ConnectionId).addSysMessageToPage("系统消息：用户不存在！");
                    }
                    else
                    {
                        //保存消息
                        ChatMessageManage.Save(new Domain.SYS_CHATMESSAGE() { FromUser = UserOnline.FK_UserId, MessageType = Common.Enums.ClsDic.DicMessageType["私聊"], MessageContent = message, MessageDate = DateTime.Now, MessageIP = Utils.GetIP(), ToGroup = Users.ID.ToString() });
                        //返回消息实体
                        var Message = new Message() { ConnectId = UserOnline.ConnectId, UserName = Users.NAME, UserFace = string.IsNullOrEmpty(Users.FACE_IMG) ? "/sys/user/User_Default_Avatat?name=" + Users.NAME.Substring(0, 1) : Users.FACE_IMG, MessageDate = DateTime.Now.GetDateTimeFormats('D')[1].ToString() + " - " + DateTime.Now.ToString("HH:mm:ss"), MessageContent = message, MessageType = "private", UserId = Users.ID };                                        
                        if (ReceiveUser.IsOnline)
                        {
                            //推送给指定用户
                            Clients.Client(clientId).addNewMessageToPage(JsonConverter.Serialize(Message));
                        }
                        //推送给用户
                        Clients.Client(Context.ConnectionId).addNewMessageToPage(JsonConverter.Serialize(Message));
                        
                    }
                }
            }
            catch (Exception ex)
            {
                //推送系统消息
                Clients.Client(Context.ConnectionId).addSysMessageToPage("系统消息：消息发送失败，请稍后再试！");
                throw ex.InnerException;
            }            
        }

        //使用者离线
        public override Task OnDisconnected(bool stopCalled)
        {
            //更新在线状态
            var UserOnline = UserOnlineManage.LoadListAll(p => p.ConnectId == Context.ConnectionId).FirstOrDefault();
            UserOnline.ConnectId = Context.ConnectionId;
            UserOnline.OfflineDate = DateTime.Now;
            UserOnline.IsOnline = false;
            UserOnlineManage.Update(UserOnline);

            //获取用户信息
            var User = UserManage.Get(p => p.ID == UserOnline.FK_UserId);

            Clients.All.UserLogOutNotice(User.NAME + "：离线了!");
            //刷新用户通讯录
            Clients.All.ContactsNotice(JsonConverter.Serialize(UserOnline));

            return base.OnDisconnected(true);
        }


        #region 帮助方法
        /// <summary>
        /// 递归获取一级部门ID
        /// </summary>
        private Domain.SYS_DEPARTMENT GetUserDepart(string departId)
        {
            var DepartMent = DepartmentManage.Get(p => p.ID == departId);

            if (DepartMent != null)
            {
                var ParentId = DepartMent.PARENTID;
                var ParentDepart = new Domain.SYS_DEPARTMENT();
                for (int? i = DepartMent.BUSINESSLEVEL; i >= 1; i--)
                {
                    ParentDepart = DepartmentManage.Get(p => p.ID == ParentId);
                    if(!string.IsNullOrEmpty(ParentDepart.PARENTID))
                    {
                        ParentId = ParentDepart.PARENTID;
                    }
                    else
                    {
                        break;
                    }
                }

                return ParentDepart;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取消息类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetMessageType(int type)
        {
            if (type == Common.Enums.ClsDic.DicMessageType["广播"]) { return "public"; }
            else if (type == Common.Enums.ClsDic.DicMessageType["群组"]) { return "group"; }
            else{return "private";}
        }
        #endregion
    }

    #region 定义消息类
    public class Message
    {
        /// <summary>
        /// 发送者ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 发送者ConnectID
        /// </summary>
        public string ConnectId { get; set; }
        /// <summary>
        /// 发送者名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 发送者头像
        /// </summary>
        public string UserFace { get; set; }
        /// <summary>
        /// 发送日期
        /// </summary>
        public string MessageDate { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public string MessageType { get; set; }
        /// <summary>
        /// 消息主体
        /// </summary>
        public string MessageContent { get; set; }
    }
    #endregion
}