using Common;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPage.Controllers;

namespace WebPage.Areas.SysManage.Controllers
{
    public class UserController : BaseController
    {
        #region 声明容器
        /// <summary>
        /// 部门
        /// </summary>
        IDepartmentManage DepartmentManage { get; set; }
        /// <summary>
        /// 岗位管理
        /// </summary>
        IPostManage PostManage { get; set; }
        /// <summary>
        /// 用户岗位
        /// </summary>
        IPostUserManage PostUserManage { get; set; }
        /// <summary>
        /// 用户信息
        /// </summary>
        IUserInfoManage UserInfoManage { get; set; }
        /// <summary>
        /// 字典编码
        /// </summary>
        ICodeManage CodeManage { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        IRoleManage RoleManage { get; set; }
        /// <summary>
        /// 大数据字段管理
        /// </summary>
        IContentManage ContentManage { get; set; }
        /// <summary>
        /// 用户在线状态
        /// </summary>
        IUserOnlineManage UserOnlineManage { get; set; }
        #endregion

        #region 基本视图
        /// <summary>
        /// 加载首页
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "User", OperaAction = "View")]
        public ActionResult Index()
        {
            try
            {

                #region 处理查询参数
                string DepartId = Request.QueryString["DepartId"];
                ViewBag.Search = base.keywords;
                ViewData["DepartId"] = DepartId;
                #endregion

                ViewBag.dpt = this.DepartmentManage.GetDepartmentByDetail();

                return View(BindList(DepartId));
            }
            catch (Exception e)
            {
                WriteLog(Common.Enums.enumOperator.Select, "员工管理加载主页：", e);
                throw e.InnerException;
            }

        }
        /// <summary>
        /// 加载用户详情信息（基本）
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "User", OperaAction = "Detail")]
        public ActionResult Detail(int? id)
        {
            try
            {
                var _entity = new Domain.SYS_USER();

                var Postlist="";

                if (id != null && id > 0)
                {
                    _entity = UserManage.Get(p => p.ID == id);
                    Postlist = String.Join(",", _entity.SYS_POST_USER.Select(p => p.FK_POSTID).ToList());
                }
                ViewBag.dpt = this.DepartmentManage.GetDepartmentByDetail();
                ViewBag.zw = this.CodeManage.LoadAll(p => p.CODETYPE == "ZW").ToList();
                ViewData["Postlist"] = Postlist;
                return View(_entity);
            }
            catch (Exception e)
            {
                WriteLog(Common.Enums.enumOperator.Select, "加载用户详情发生错误：", e);
                throw e.InnerException;
            }
        }
        /// <summary>
        /// 保存人员基本信息
        /// </summary>
        [ValidateInput(false)]
        [UserAuthorizeAttribute(ModuleAlias = "User", OperaAction = "Add,Edit")]
        public ActionResult Save(Domain.SYS_USER entity)
        {
            bool isEdit = false;
            var json = new JsonHelper() { Msg = "保存成功", Status = "n" };
            using (System.Transactions.TransactionScope ts = new System.Transactions.TransactionScope())
            {
                try
                {
                    if (entity != null)
                    {
                        if (entity.ID <= 0) //添加
                        {
                            entity.CREATEDATE = DateTime.Now;
                            entity.CREATEPER = this.CurrentUser.Name;
                            entity.UPDATEDATE = DateTime.Now;
                            entity.UPDATEUSER = this.CurrentUser.Name;
                            entity.PASSWORD = new Common.CryptHelper.AESCrypt().Encrypt("111111");
                            entity.PINYIN1 = Common.ConvertHzToPz.Convert(entity.NAME).ToLower();
                            entity.PINYIN2 = Common.ConvertHzToPz.ConvertFirst(entity.NAME).ToLower();
                        }
                        else //修改
                        {
                            entity.UPDATEUSER = this.CurrentUser.Name;
                            entity.UPDATEDATE = DateTime.Now;
                            entity.PINYIN1 = Common.ConvertHzToPz.Convert(entity.NAME).ToLower();
                            entity.PINYIN2 = Common.ConvertHzToPz.ConvertFirst(entity.NAME).ToLower();
                            isEdit = true;
                        }
                        //检测此用户名是否重复
                        if (!this.UserManage.IsExist(p => p.ACCOUNT.Equals(entity.ACCOUNT) && p.ID != entity.ID))
                        {   
                   
                            if (this.UserManage.SaveOrUpdate(entity, isEdit))
                            {
                                if(!isEdit)
                                {
                                    UserOnlineManage.Save(new Domain.SYS_USER_ONLINE() {FK_UserId=entity.ID, OnlineDate = DateTime.Now, OfflineDate = DateTime.Now, IsOnline = false, UserIP = "0.0.0.0" });
                                }

                                //员工岗位
                                var postlist = Request.Form["postlist"];
                                if (!string.IsNullOrEmpty(postlist))
                                {
                                    //删除员工岗位
                                    if (PostUserManage.IsExist(p => p.FK_USERID == entity.ID))
                                    {
                                        PostUserManage.Delete(p => p.FK_USERID == entity.ID);
                                    }
                                    //添加新的员工岗位
                                    List<Domain.SYS_POST_USER> PostUser = new List<Domain.SYS_POST_USER>();
                                    foreach (var item in postlist.Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => p).ToList())
                                    {
                                        PostUser.Add(new Domain.SYS_POST_USER() { FK_POSTID = item, FK_USERID = entity.ID });
                                    }
                                    PostUserManage.SaveList(PostUser);
                                }

                                json.Status = "y";
                            }
                            else
                            {
                                json.Msg = "保存人员信息失败！";
                            }
                            
                        }
                        else
                        {
                            json.Msg = "登录账号已被使用，请修改后再提交!";
                        }
                    }
                    else
                    {
                        json.Msg = "未找到要操作的用户记录";
                    }
                    if (isEdit)
                    {
                        WriteLog(Common.Enums.enumOperator.Edit, "修改用户，结果：" + json.Msg, Common.Enums.enumLog4net.INFO);
                    }
                    else
                    {
                        WriteLog(Common.Enums.enumOperator.Add, "添加用户，结果：" + json.Msg, Common.Enums.enumLog4net.INFO);
                    }

                    ts.Complete();
                }
                catch (Exception e)
                {
                    json.Msg = "保存人员信息发生内部错误！";
                    WriteLog(Common.Enums.enumOperator.None, "保存用户错误：", e);
                }
            }
            return Json(json);

        }
        /// <summary>
        /// 方法注解：删除用户
        /// 验证规则：1、超级管理员不能删除
        ///           2、当前登录用户不能删除
        ///           3、正常状态用户不能删除
        ///           4、上级部门用户不能删除
        /// 删除原则：1、删除用户档案
        ///           2、删除用户角色关系
        ///           3、删除用户权限关系
        ///           4、删除用户岗位关系
        ///           5、删除用户部门关系
        ///           6、删除用户在线状态
        ///           7、删除用户聊天信息
        ///           8、删除项目团队成员
        ///           9、删除用户
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "User", OperaAction = "Remove")]
        public ActionResult Delete(string idList)
        {
            var json = new JsonHelper() { Status = "n", Msg = "删除用户成功" };
            try
            {
                //是否为空
                if (string.IsNullOrEmpty(idList)) { json.Msg = "未找到要删除的用户"; return Json(json); }
                string[] id = idList.Trim(',').Split(',');
                for (int i = 0; i < id.Length; i++)
                {
                    int userId = int.Parse(id[i]);                   
                    if (this.UserManage.IsAdmin(userId))
                    {
                        json.Msg = "被删除用户存在超级管理员，不能删除!";
                        WriteLog(Common.Enums.enumOperator.Remove, "删除用户：" + json.Msg, Common.Enums.enumLog4net.ERROR);
                        return Json(json);
                    }
                    if (this.CurrentUser.Id == userId)
                    {
                        json.Msg = "当前登录用户，不能删除!";
                        WriteLog(Common.Enums.enumOperator.Remove, "删除用户：" + json.Msg, Common.Enums.enumLog4net.ERROR);
                        return Json(json);
                    }
                    if (this.UserManage.Get(p => p.ID == userId).ISCANLOGIN)
                    {
                        json.Msg = "用户未锁定，不能删除!";
                        WriteLog(Common.Enums.enumOperator.Remove, "删除用户：" + json.Msg, Common.Enums.enumLog4net.ERROR);
                        return Json(json);
                    }
                    if (this.CurrentUser.DptInfo!=null)
                    {
                        string dptid = this.UserManage.Get(p => p.ID == userId).DPTID;
                        if (this.DepartmentManage.Get(m => m.ID == dptid).BUSINESSLEVEL < this.CurrentUser.DptInfo.BUSINESSLEVEL)
                        {
                            json.Msg = "不能删除上级部门用户!";
                            WriteLog(Common.Enums.enumOperator.Remove, "删除用户：" + json.Msg, Common.Enums.enumLog4net.ERROR);
                            return Json(json);
                        }
                    }
                    this.UserManage.Remove(userId);
                    json.Status = "y";
                    WriteLog(Common.Enums.enumOperator.Remove, "删除用户：" + json.Msg, Common.Enums.enumLog4net.WARN);
                }
            }
            catch (Exception e)
            {
                json.Msg = "删除用户发生内部错误！";
                WriteLog(Common.Enums.enumOperator.Remove, "删除用户：", e);
            }
            return Json(json);
        }
        /// <summary>
        /// 方法描述:根据传入的用户编号重置当前用户密码
        /// </summary>
        /// <param name="Id">用户编号</param>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "User", OperaAction = "PwdReset")]
        public ActionResult ResetPwd(string idList)
        {
            var json = new JsonHelper() { Status = "n", Msg = "操作成功" };
            try
            {
                //校验用户编号是否为空
                if (string.IsNullOrEmpty(idList))
                {
                    json.Msg = "校验失败，用户编号不能为空";
                    WriteLog(Common.Enums.enumOperator.Edit, "重置当前用户密码：" + json.Msg, Common.Enums.enumLog4net.ERROR);
                    return Json(json);
                }
                var idlist1 = idList.Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
                if (idlist1 != null && idlist1.Count > 0)
                {
                    foreach (var newid in idlist1)
                    {
                        var _user = UserManage.Get(p => p.ID == newid);
                        _user.PASSWORD = new Common.CryptHelper.AESCrypt().Encrypt("111111");
                        UserManage.Update(_user);
                    }
                }
                json.Status = "y";
                WriteLog(Common.Enums.enumOperator.Edit, "重置当前用户密码：" + json.Msg, Common.Enums.enumLog4net.INFO);
            }
            catch (Exception e)
            {
                json.Msg = "操作失败";
                WriteLog(Common.Enums.enumOperator.Edit, "重置当前用户密码：", e);
            }
            return Json(json);
        }
        /// <summary>
        /// 加载人员档案
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "User", OperaAction = "UserInfo")]
        public ActionResult UserInfo(int? userid)
        {
            try
            {
                //是否为人事部
                var IsMatters = true;

                var entity = new Domain.SYS_USERINFO();

                var UserName = CurrentUser.Name;

                if (userid != null && userid > 0)
                {
                    entity = UserInfoManage.Get(p => p.USERID == userid) ?? new Domain.SYS_USERINFO() { USERID = int.Parse(userid.ToString()) };
                    UserName = UserManage.Get(p => p.ID == userid).NAME;
                    if((CurrentUser.DptInfo!=null && CurrentUser.DptInfo.NAME != "人事部") && !CurrentUser.IsAdmin)
                    {
                        IsMatters = false;
                    }
                }
                else
                {
                    entity = UserInfoManage.Get(p => p.USERID == CurrentUser.Id) ?? new Domain.SYS_USERINFO() { USERID = CurrentUser.Id };
                    IsMatters = false;
                }

                ViewData["UserName"] = UserName;

                ViewBag.IsMatters = IsMatters;

                Dictionary<string, string> dic = Common.Enums.ClsDic.DicCodeType;
                var dictype = this.CodeManage.GetDicType();
                //在岗状态
                string zgzt = dic["在岗状态"];
                ViewData["zgzt"] = dictype.Where(p => p.CODETYPE == zgzt).ToList();
                //婚姻状况
                string hyzk = dic["婚姻状况"];
                ViewData["hunyin"] = dictype.Where(p => p.CODETYPE == hyzk).ToList();
                //政治面貌
                string zzmm = dic["政治面貌"];
                ViewData["zzmm"] = dictype.Where(p => p.CODETYPE == zzmm).ToList();
                //民族
                string mz = dic["民族"];
                ViewData["mz"] = dictype.Where(p => p.CODETYPE == mz).ToList();
                //职称级别
                string zcjb = dic["职称"];
                ViewData["zcjb"] = dictype.Where(p => p.CODETYPE == zcjb).ToList();
                //学历
                string xl = dic["学历"];
                ViewData["xl"] = dictype.Where(p => p.CODETYPE == xl).ToList();

                return View(entity);
            }
            catch (Exception e)
            {
                WriteLog(Common.Enums.enumOperator.Select, "加载人员档案：", e);
                throw e.InnerException;
            }

        }
        /// <summary>
        /// 保存人员档案
        /// </summary>
        public ActionResult SetUserInfo(Domain.SYS_USERINFO entity)
        {
            bool isEdit = false;
            var json = new JsonHelper() { Msg = "保存人员档案成功", Status = "n" };
            try
            {               
                    if (entity != null)
                    {
                        #region 获取html标签值

                        //籍贯
                        entity.HomeTown = Request.Form["jgprov"] + "," + Request.Form["jgcity"] + "," +
                                          Request.Form["jgcountry"];
                        //户口所在地
                        entity.HuJiSuoZaiDi = Request.Form["hkprov"] + "," + Request.Form["hkcity"] + "," +
                                              Request.Form["hkcountry"];

                        #endregion

                        //添加
                        if (entity.ID<=0)
                        {
                            entity.CREATEUSER = CurrentUser.Name;
                            entity.CREATEDATE = DateTime.Now;
                            entity.UPDATEUSER = CurrentUser.Name;
                            entity.UPDATEDATE = DateTime.Now;
                        }
                        else
                        {
                            entity.UPDATEUSER = CurrentUser.Name;
                            entity.UPDATEDATE = DateTime.Now;
                            isEdit = true;
                        }



                        //修改用户档案
                        if (this.UserInfoManage.SaveOrUpdate(entity, isEdit))
                        {
                            json.Status = "y";

                        }
                        else
                        {
                            json.Msg = "保存用户档案失败";

                        }

                    }
                    else
                    {
                        json.Msg = "未找到要编辑的用户记录";
                    }
                    if (isEdit)
                    {
                        WriteLog(Common.Enums.enumOperator.Edit, "保存人员档案：" + json.Msg, Common.Enums.enumLog4net.INFO);
                    }
                    else
                    {
                        WriteLog(Common.Enums.enumOperator.Add, "保存人员档案：" + json.Msg, Common.Enums.enumLog4net.INFO);
                    }                
            }
            catch (Exception e)
            {                
                json.Msg = e.InnerException.Message;
                WriteLog(Common.Enums.enumOperator.None, "保存人员档案：", e);
            }
            return Json(json);
        }       
        /// <summary>
        /// 上传头像
        /// </summary>
        /// <returns></returns>
        public ActionResult UserFace()
        {
            return View(CurrentUser);
        }
        [ValidateInput(false)]
        public ActionResult SaveFace()
        {
            var json = new JsonHelper() { Status = "n", Msg = "上传头像成功！" };
            try
            {
                var faceBase64 = Request.Form["UserFace"];
                var User = UserManage.Get(p => p.ID == CurrentUser.Id);
                User.FACE_IMG = faceBase64;
                UserManage.Update(User);
                CurrentUser.Face_Img = User.FACE_IMG;               
                json.Status = "y";
                
            }
            catch (Exception e)
            {
                json.Msg = "上传头像发生内部错误！";
                WriteLog(Common.Enums.enumOperator.Remove, "上传头像：", e);
            }
            return Json(json);
        }
        #endregion

        #region 帮助方法及其他控制器调用
        /// <summary>
        /// 分页查询用户列表
        /// </summary>
        private Common.PageInfo BindList(string DepartId)
        {
            //基础数据
            var query = this.UserManage.LoadAll(p => p.ID > 1);

            //部门(本部门用户及所有下级部门用户)
            if (!string.IsNullOrEmpty(DepartId))
            {
                var childDepart = this.DepartmentManage.LoadAll(p => p.PARENTID == DepartId).Select(p=>p.ID).ToList();
                query = query.Where(p => p.DPTID == DepartId || childDepart.Any(e => e == p.DPTID));
            }
            
            //查询关键字
            if (!string.IsNullOrEmpty(keywords))
            {
                keywords = keywords.ToLower();
                query = query.Where(p => p.NAME.Contains(keywords) || p.ACCOUNT.Contains(keywords) || p.PINYIN2.Contains(keywords) || p.PINYIN1.Contains(keywords));
            }
            //排序
            query = query.OrderBy(p=>p.SHOWORDER1).OrderByDescending(p => p.CREATEDATE);
            //分页
            var result = this.UserManage.Query(query, page, pagesize);

            var list = result.List.Select(p => new
            {
                p.ID,
                p.NAME,
                p.ACCOUNT,
                DPTNAME = this.DepartmentManage.GetDepartmentName(p.DPTID),
                POSTNAME = GetPostName(p.SYS_POST_USER),
                ROLENAME = GetRoleName(p.SYS_USER_ROLE),
                p.CREATEDATE,
                ZW = this.CodeManage.Get(m => m.CODEVALUE == p.LEVELS && m.CODETYPE == "ZW").NAMETEXT,
                ISCANLOGIN = !p.ISCANLOGIN ? "<i class=\"fa fa-circle text-navy\"></i>" : "<i class=\"fa fa-circle text-danger\"></i>"

            }).ToList();

            return new Common.PageInfo(result.Index, result.PageSize, result.Count, Common.JsonConverter.JsonClass(list));
        }
        /// <summary>
        /// 根据岗位集合获取岗位名称
        /// </summary>
        private string GetPostName(ICollection<Domain.SYS_POST_USER> collection)
        {
            string retval = string.Empty;
            if (collection != null && collection.Count > 0)
            {
                var postlist = String.Join(",", collection.Select(p => p.FK_POSTID).ToList()).Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => p).ToList();
                retval = String.Join(",", PostManage.LoadAll(p => postlist.Any(e => e == p.ID)).Select(p => p.POSTNAME).ToList());
            }
            return retval = retval.TrimEnd(',');
        }
        /// <summary>
        /// 根据角色集合获取角色名称
        /// </summary>
        private string GetRoleName(ICollection<Domain.SYS_USER_ROLE> collection)
        {
            string retval = string.Empty;
            if (collection != null && collection.Count > 0)
            {
                var rolelist = String.Join(",", collection.Select(p => p.FK_ROLEID).ToList()).Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
                retval = String.Join(",", RoleManage.LoadAll(p => rolelist.Any(e => e == p.ID)).Select(p => p.ROLENAME).ToList());
            }
            return retval = retval.TrimEnd(',');
        }
        /// <summary>
        /// 联系人列表
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "UserList", OperaAction = "View")]
        public ActionResult Contacts()
        {
            var UsersList = UserManage.LoadListAll(p => p.ID != CurrentUser.Id).OrderBy(p => p.LEVELS).OrderBy(p => p.CREATEDATE).Select(p => new
            {
                p.ID,
                FaceImg = string.IsNullOrEmpty(p.FACE_IMG) ? "/Pro/Project/User_Default_Avatat?name=" + p.NAME.Substring(0, 1) : p.FACE_IMG,
                p.NAME,
                HuJiSuoZaiDi = UserInfoManage.Get(m => m.USERID == p.ID) != null ? UserInfoManage.Get(m => m.USERID == p.ID).HuJiSuoZaiDi : "",
                HUJIPAICHUSUO = UserInfoManage.Get(m => m.USERID == p.ID) != null ? UserInfoManage.Get(m => m.USERID == p.ID).HUJIPAICHUSUO : "",
                InsideEmail = p.ACCOUNT + EmailDomain,
                Email = UserInfoManage.Get(m => m.USERID == p.ID) != null ? UserInfoManage.Get(m => m.USERID == p.ID).EMAILADDRESS : "",
                p.LEVELS,
                Mobile = UserInfoManage.Get(m => m.USERID == p.ID) != null ? UserInfoManage.Get(m => m.USERID == p.ID).PHONE : "",
                Mobile2 = UserInfoManage.Get(m => m.USERID == p.ID) != null ? UserInfoManage.Get(m => m.USERID == p.ID).SECONDPHONE : "",
                Tel = UserInfoManage.Get(m => m.USERID == p.ID) != null ? UserInfoManage.Get(m => m.USERID == p.ID).OFFICEPHONE : "",
                Depart = GetDepart(p.DPTID),
                p.CREATEDATE
            }).ToList();

            return View(JsonConverter.JsonClass(UsersList));
        }       
        /// <summary>
        /// 获取用户部门列表
        /// </summary>
        /// <param name="departId"></param>
        /// <returns></returns>
        private string GetDepart(string departId)
        {
            var departs = string.Empty;
            if(!string.IsNullOrEmpty(departId))
            {
                var parentdepart = DepartmentManage.Get(p => p.ID == departId).PARENTID;
                var query = this.DepartmentManage.LoadAll(p => p.ID == departId || p.ID == parentdepart).OrderBy(p => p.BUSINESSLEVEL).ToList();
                if (query != null && query.Count > 0)
                {
                    foreach (var item in query)
                    {
                        departs += item.NAME + (string.IsNullOrEmpty(item.PARENTID) ? "<i class=\"fa fa-angle-right fa-fw\"></i>" : "");
                    }
                }
            }
            return departs;
        }

        /// <summary>
        /// 用户默认头像
        /// </summary>
        /// <param name="name">首字母/姓</param>
        /// <returns></returns>
        public FileContentResult User_Default_Avatat(string name)
        {
            System.IO.MemoryStream ms = new Models.user_avatat().Create(name);
            Response.ClearContent();//清空输出流 
            return File(ms.ToArray(), @"image/png");
        }
        #endregion
    }
}