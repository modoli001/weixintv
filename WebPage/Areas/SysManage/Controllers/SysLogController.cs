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
    public class SysLogController : BaseController
    {
        #region 声明容器
        /// <summary>
        /// 系统日志管理
        /// </summary>
        ISyslogManage SyslogManage { get; set; }

        #endregion

        #region 基本视图
        /// <summary>
        /// 加载首页
        /// 日志特殊规则：
        /// 1、非管理员只能查看自己的操作内容
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "Syslog", OperaAction = "View")]
        public ActionResult Index()
        {
            //获取查询条件
            var level = Request.QueryString["level"];
            var action = Request.QueryString["actions"];
            var result = BindList(level, action);
            ViewData["logaction"] = Tools.BindEnums(typeof(Common.Enums.enumOperator));
            ViewData["sellog"] = action;
            ViewData["lev"] = level;
            ViewBag.Search = base.keywords;
            return View(result);
        }
        /// <summary>
        /// 加载详情
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "Syslog", OperaAction = "Detail")]
        public ActionResult Detail(int? id)
        {
            var entity = this.SyslogManage.Get(p => p.ID == id) ?? new Domain.SYS_LOG();
            return View(entity);
        }
        /// <summary>
        /// 删除日志
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "Syslog", OperaAction = "Remove")]
        public ActionResult Delete(string idList)
        {
            var json = new JsonHelper() { Msg = "删除日志完毕", Status = "n" };
            var id = idList.Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
            try
            {
                SyslogManage.Delete(p => id.Contains(p.ID));
                json.Status = "y";
                WriteLog(Common.Enums.enumOperator.Remove, "删除系统日志：" + json.Msg, Common.Enums.enumLog4net.WARN);
            }
            catch (Exception e)
            {
                json.Msg = "删除系统日志发生内部错误！";
                WriteLog(Common.Enums.enumOperator.Remove, "删除系统日志：", e);
            }
            return Json(json);
        }
        #endregion

        #region 帮助方法及其它控制器调用
        /// <summary>
        /// 加载列表
        /// </summary>
        private object BindList(string level, string action)
        {
            var predicate = PredicateBuilder.True<Domain.SYS_LOG>();
            //日志级别
            if (!string.IsNullOrEmpty(level))
            {
                predicate = predicate.And(p => p.LEVELS == level);
            }
            //日志动作
            if (!string.IsNullOrEmpty(action))
            {
                predicate = predicate.And(p => p.ACTION == action);
            }
            //关键词
            if (!string.IsNullOrEmpty(base.keywords))
            {
                predicate = predicate.And(p => p.MESSAGE.Contains(keywords));
            }
            //人员
            if (!this.CurrentUser.IsAdmin)
            {
                predicate = predicate.And(p => p.CLIENTUSER == CurrentUser.Name);
            }
            return this.SyslogManage.Query(this.SyslogManage.LoadAll(predicate).OrderByDescending(p => p.DATES), base.page, base.pagesize);
        }
        #endregion
    }
}