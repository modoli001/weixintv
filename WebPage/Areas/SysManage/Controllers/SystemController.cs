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
    /// <summary>
    /// 系统管理控制器
    /// </summary>
    public class SystemController : BaseController
    {
        #region  声明容器
        /// <summary>
        /// 系统管理
        /// </summary>
        ISystemManage SystemManage { get; set; }
        /// <summary>
        /// 模块管理
        /// </summary>
        IModuleManage ModuleManage { get; set; }

        #endregion

        #region 基本视图
        /// <summary>
        /// 加载主页
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "SystemSet", OperaAction = "View")]
        public ActionResult Index()
        {
            #region 处理查询参数
            //系统状态
            string status = Request.QueryString["status"];
            ViewData["status"] = status;
            ViewBag.Search = base.keywords;
            #endregion

            return View(BindList(status));
        }
        /// <summary>
        /// 加载详情
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "SystemSet", OperaAction = "Detail")]
        public ActionResult Detail(string id)
        {
            return View(this.SystemManage.Get(p => p.ID == id) ?? new Domain.SYS_SYSTEM());
        }
        /// <summary>
        /// 保存系统
        /// </summary>
        [ValidateInput(false)]
        [UserAuthorizeAttribute(ModuleAlias = "SystemSet", OperaAction = "Add,Edit")]
        public ActionResult Save(Domain.SYS_SYSTEM entity)
        {
            bool isEdit = false;
            JsonHelper json = new JsonHelper() { Msg = "保存系统成功", Status = "n" };
            try
            {
                if (entity != null)
                {
                    if (string.IsNullOrEmpty(entity.ID))
                    {
                        entity.ID = Guid.NewGuid().ToString();
                        entity.CREATEDATE = DateTime.Now;
                    }
                    else
                    {                        
                        isEdit = true;
                    }

                    //系统是否存在
                    if (!this.SystemManage.IsExist(p => p.ID != entity.ID && p.NAME == entity.NAME && p.SITEURL == entity.SITEURL))
                    {
                        if (this.SystemManage.SaveOrUpdate(entity, isEdit))
                        {
                            json.Status = "y";
                        }
                        else
                        {
                            json.Msg = "保存系统失败";
                        }
                    }
                    else
                    {
                        json.Msg = entity.NAME + "系统已存在，不能重复添加";
                    }
                }
                else
                {
                    json.Msg = "未找到需要保存的系统";
                }
                if (isEdit)
                {
                    WriteLog(Common.Enums.enumOperator.Edit, "修改系统，结果：" + json.Msg, Common.Enums.enumLog4net.INFO);
                }
                else
                {
                    WriteLog(Common.Enums.enumOperator.Add, "添加系统，结果：" + json.Msg, Common.Enums.enumLog4net.INFO);
                }
            }
            catch (Exception e)
            {
                json.Msg = "保存系统发生内部错误！";
                WriteLog(Common.Enums.enumOperator.None, "保存系统：", e);
            }
            return Json(json);

        }

        /// <summary>
        /// 删除系统
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "SystemSet", OperaAction = "Remove")]
        public ActionResult Delete(string idlist)
        {
            var json = new JsonHelper() { Msg = "删除成功", Status = "n" };
            try
            {
                idlist = idlist.TrimEnd(',');
                if (!string.IsNullOrEmpty(idlist))
                {
                    //验证系统是否为主系统
                    if (idlist.ToLower().Contains("fddeab19-3588-4fe1-83b6-c15d4abb942d"))
                    {
                        json.Msg = "不能删除主系统";
                    }
                    else
                    {
                        //验证是否是正常使用的系统
                        if (this.SystemManage.IsExist(p => idlist.Contains(p.ID) && p.IS_LOGIN))
                        {
                            json.Msg = "要删除的系统正在使用中，不能删除";
                        }
                        else
                        {
                            //验证系统是否配置了模块
                            if (this.ModuleManage.IsExist(p => idlist.Contains(p.FK_BELONGSYSTEM)))
                            {
                                json.Msg = "要删除的系统存在使用中的模块，不能删除";
                            }
                            else
                            {
                                //删除
                                this.SystemManage.Delete(p => idlist.Contains(p.ID));
                                json.Status = "y";
                            }
                        }
                    }

                }
                else
                {
                    json.Msg = "未找到要删除的系统记录";
                }
                WriteLog(Common.Enums.enumOperator.Remove, "删除系统：" + json.Msg, Common.Enums.enumLog4net.WARN);
            }
            catch (Exception e)
            {
                json.Msg = "删除系统发生内部错误！";
                WriteLog(Common.Enums.enumOperator.Remove, "删除系统：", e);
            }
            return Json(json);
        }
        #endregion

        #region 帮助方法及其他控制器调用
        /// <summary>
        /// 查询分页文章列表
        /// </summary>
        private Common.PageInfo BindList(string status)
        {

            //基本数据
            var query = this.SystemManage.LoadAll(p => CurrentUser.System_Id.Any(e => e == p.ID));

            //是否锁定
            if (!string.IsNullOrEmpty(status))
            {
                bool islogin = status == "True";
                query = query.Where(p => p.IS_LOGIN == islogin);
            }
            //关键词
            if (!string.IsNullOrEmpty(keywords))
            {
                keywords = keywords.ToLower();
                query = query.Where(p => p.NAME.Contains(keywords));
            }
            //排序
            query = query.OrderByDescending(p => p.CREATEDATE);
            //分页
            var result = this.SystemManage.Query(query, page, pagesize);
            var list = result.List.Select((p, i) => new
            {
                p.ID,
                p.NAME,
                p.SITEURL,
                ISLOGIN = p.IS_LOGIN ? "<i class=\"fa fa-circle text-navy\"></i>" : "<i class=\"fa fa-circle text-danger\"></i>",
                p.CREATEDATE
            });
            return new PageInfo(result.Index, result.PageSize, result.Count, Common.JsonConverter.JsonClass(list));
        }        
        #endregion
    }
}