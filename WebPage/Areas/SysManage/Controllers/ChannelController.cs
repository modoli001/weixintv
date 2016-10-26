using Common;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPage.Controllers;

namespace WebPage.Areas.SysManage.Controllers
{
    public class ChannelController : BaseController
    {
        IChannelManage ChannelManage { get; set; }

        [UserAuthorizeAttribute(ModuleAlias = "Channel", OperaAction = "View")]
        public ActionResult Index()
        {
            try
            {

                #region 处理查询参数
                string type = Request.QueryString["type"];
                ViewBag.Search = base.keywords;
                ViewData["Type"] = type;
                #endregion

                return View(BindList(type));
            }
            catch (Exception e)
            {
                WriteLog(Common.Enums.enumOperator.Select, "频道管理加载主页：", e);
                throw e.InnerException;
            }
        }

        /// <summary>
        /// 加载频道详情
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "Channel", OperaAction = "Detail")]
        public ActionResult Detail(int? id)
        {
            try
            {
                var entity = new Domain.SYS_CHANNEL() { IsDisplay = true };
                var query = ChannelManage.LoadListAll(null);

                if(id!=null && id>0)
                {
                    entity = ChannelManage.Get(p => p.ID == id);
                    query = query.Where(p => p.ID != id).ToList();
                }

                var parentId = Request.QueryString["parentId"];
                var typeId = Request.QueryString["typeId"];

                if (!string.IsNullOrEmpty(parentId))
                {
                    int parent = int.Parse(parentId);
                    entity.ParentID = parent;
                }
                if (!string.IsNullOrEmpty(typeId))
                {
                    int type = int.Parse(typeId);
                    entity.TypeId = type;
                }



                ViewData["Channels"] = Common.JsonConverter.JsonClass(ChannelManage.RecursiveModule(query).Select(p => new
                {
                    p.ID,
                    Title = GetTitle2(p.Tilte, p.Levels)
                }));

                return View(entity);
            }
            catch (Exception e)
            {
                WriteLog(Common.Enums.enumOperator.Select, "频道管理加载详情", e);
                throw e.InnerException;
            }
        }

        [UserAuthorizeAttribute(ModuleAlias = "Channel", OperaAction = "Add,Edit")]
        public ActionResult Save(Domain.SYS_CHANNEL entity)
        {
            bool IsEdit = false;
            var json = new JsonHelper() { Msg = "保存成功", Status = "n" };

            try {
                if (entity == null)
                    json.Msg = "频道不存在!";
                else
                {
                    if(entity.ID>0)
                    {
                        IsEdit = true;
                        entity.UpdateDate = DateTime.Now;
                        entity.UpdateUser = CurrentUser.Name;
                    }
                    else
                    {
                        entity.CreateDate = DateTime.Now;
                        entity.CreateUser = CurrentUser.Name;
                        entity.UpdateDate = DateTime.Now;
                        entity.UpdateUser = CurrentUser.Name;
                    }

                    if(entity.ParentID>0)
                    {
                        entity.Levels = ChannelManage.Get(p => p.ID == entity.ParentID).Levels + 1;
                    }
                    else
                    {
                        entity.Levels = 0;
                    }

                    if(ChannelManage.IsExist(p=>p.ParentID==entity.ParentID&&p.ID!=entity.ID&&p.Tilte==entity.Tilte))
                    {
                        json.Msg = "子频道名称不能重复！";
                    }
                    else
                    {
                        if(ChannelManage.SaveOrUpdate(entity,IsEdit))
                        {
                            if (IsEdit)
                            {
                                ChannelManage.MoreModifyModule(entity.ID, entity.Levels);
                            }
                            json.Status = "y";
                        }
                        else
                        {
                            json.Msg = "保存失败！";
                        }
                    }
                }
            }
            catch(Exception e)
            {
                json.Msg = "保存频道发生内部错误！";
                WriteLog(Common.Enums.enumOperator.None, "保存频道：", e);
            }

            return Json(json);
        }

        /// <summary>
        /// 删除频道
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "Channel", OperaAction = "Remove")]
        public ActionResult Delete(string idList)
        {
            JsonHelper json = new JsonHelper() { Msg = "删除频道成功", ReUrl = "", Status = "n" };
            try
            {
                if (!string.IsNullOrEmpty(idList))
                {
                    var idlist1 = idList.Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
                    if(ChannelManage.IsExist(p=>idlist1.Contains(p.ParentID)))
                    {
                        json.Msg = "该频道下含有子频道，请先删除子频道！";
                    }
                    else
                    {
                        if(ChannelManage.Delete(p=>idlist1.Contains(p.ID)))
                        {
                            json.Status = "y";
                        }
                        else
                        {
                            json.Msg = "删除失败！";
                        }
                    }
                }
                else
                {
                    json.Msg = "未找到要删除的频道";
                }
                WriteLog(Common.Enums.enumOperator.Remove, "删除频道，结果：" + json.Msg, Common.Enums.enumLog4net.WARN);
            }
            catch (Exception e)
            {
                json.Msg = "删除频道发生内部错误！";
                WriteLog(Common.Enums.enumOperator.Remove, "删除频道：", e);
            }
            return Json(json);
        }


        /// <summary>
        /// 查询分类列表
        /// </summary>
        /// <returns></returns>
        private object BindList(string type)
        {
            var query = this.ChannelManage.LoadAll(null);

            //直播 点播
            if (!string.IsNullOrEmpty(type))
            {
                int typeId = int.Parse(type);
                query = query.Where(p => p.TypeId == typeId);
            }            

            //递归排序（无分页）
            var entity = this.ChannelManage.RecursiveModule(query.ToList())
               .Select(p => new
               {
                   p.ID,
                   Tilte=GetTitle(p.Tilte,p.Levels),
                   IsDisplay = p.IsDisplay ? "是" : "否",
                   p.DisplayOrder,
                   p.TypeId,
                   ChannelType = p.TypeId == 0 ? "<span class=\"btn btn-danger btn-xs\" style=\"margin-right:5px;\">直播</span>" : "<span class=\"btn btn-warning btn-xs\" style=\"margin-right:5px;\">点播</span>"
               });

            //关键字查询
            if (!string.IsNullOrEmpty(keywords))
            {
                var keyword = keywords.ToLower();
                entity = entity.Where(p => p.Tilte.Contains(keyword));
            }

            return Common.JsonConverter.JsonClass(entity);
        }

        private string GetTitle(string Title,int levels)
        {
            return levels > 0 ? "<span style=\"margin-left:" + (15 * levels) + "px\">|--" + Title + "</span>" : "<span style=\"margin-left:0px\">" + Title + "</span>";
        }
        private string GetTitle2(string Title, int levels)
        {
            string padding = "&emsp;";

            for (int i = 0; i < levels;i++ )
            {
                padding += "&emsp;";
            }

            return levels > 0 ? padding + "|--" + Title : Title;
        }
    }
}