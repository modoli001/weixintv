using Common;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebPage.Controllers;

namespace WebPage.Areas.SysManage.Controllers
{
    public class CodeController : BaseController
    {
        #region 声明容器
        /// <summary>
        /// 编码管理
        /// </summary>
        ICodeManage CodeManage { get; set; }
        #endregion

        #region 基本视图
        /// <summary>
        /// 加载主页
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "Code", OperaAction = "View")]
        public async Task<ActionResult> Index()
        {
            try
            {
                #region 处理查询参数
                string codetype = Request.QueryString["codetype"];
                #endregion

                #region 加载列表
                var resultTask =  BindList(codetype);
                ViewBag.Search = base.keywords;
                ViewData["codeType"] = Common.Enums.ClsDic.DicCodeType;
                ViewData["codet"] = codetype;
                var result = await resultTask;
                #endregion

                return View(result);
            }
            catch (Exception e)
            {
                WriteLog(Common.Enums.enumOperator.Select, "数据字典管理加载主页：", e);
                throw e.InnerException;
            }

        }
        /// <summary>
        /// 加载详情
        /// </summary>
        /// <param name="id">编码ID</param>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Code", OperaAction = "Detail")]
        public ActionResult Detail(int? id)
        {
            var entity = this.CodeManage.Get(p => p.ID == id)??new Domain.SYS_CODE();
            ViewData["codeType"] = Common.Enums.ClsDic.DicCodeType;
            return View(entity);
        }
        /// <summary>
        /// 保存编码
        /// </summary>
        [UserAuthorizeAttribute(ModuleAlias = "Code", OperaAction = "Add,Edit")]
        public ActionResult Save(Domain.SYS_CODE entity)
        {
            bool isEdit = false;
            JsonHelper json = new JsonHelper() { Msg = "保存编码成功", Status = "n" };
            try
            {
                if (entity != null)
                {                   
                    //添加
                    if (entity.ID <= 0)
                    {
                        entity.CREATEDATE = DateTime.Now;
                        entity.CREATEUSER = this.CurrentUser.Name;
                        entity.UPDATEDATE = DateTime.Now;
                        entity.UPDATEUSER = this.CurrentUser.Name;
                    }
                    else //修改
                    {                       
                        entity.UPDATEDATE = DateTime.Now;
                        entity.UPDATEUSER = this.CurrentUser.Name;
                        isEdit = true;
                    }
                    //判断岗位是否重名 
                    if (!this.CodeManage.IsExist(p => p.NAMETEXT == entity.NAMETEXT && p.CODETYPE == entity.CODETYPE && p.ID != entity.ID))
                    {
                        if (CodeManage.SaveOrUpdate(entity, isEdit))
                        {
                            json.Status = "y";
                        }
                        else
                        {
                            json.Msg = "保存失败";
                        }
                    }
                    else
                    {
                        json.Msg = "编码" + entity.NAMETEXT + "已存在，不能重复添加";
                    }
                }
                else
                {
                    json.Msg = "未找到需要保存的编码记录";
                }
                if (isEdit)
                {
                    WriteLog(Common.Enums.enumOperator.Edit, "修改编码，结果：" + json.Msg, Common.Enums.enumLog4net.INFO);
                }
                else
                {
                    WriteLog(Common.Enums.enumOperator.Add, "添加编码，结果：" + json.Msg, Common.Enums.enumLog4net.INFO);
                }
            }
            catch (Exception e)
            {
                json.Msg = "保存编码发生内部错误！";
                WriteLog(Common.Enums.enumOperator.None, "保存编码：", e);
            }
            return Json(json);
        }
        /// <summary>
        /// 删除编码
        /// </summary>
        /// <param name="idList">编码ID字符串</param>
        [UserAuthorizeAttribute(ModuleAlias = "Code", OperaAction = "Remove")]
        public ActionResult Delete(string idList)
        {
            var json = new JsonHelper() { Msg = "删除编码完毕", Status = "n" };
            try
            {
                if (!string.IsNullOrEmpty(idList))
                {
                    var idList1 = idList.Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => int.Parse(p)).ToList();
                    this.CodeManage.Delete(p => idList1.Any(e => e == p.ID));
                    json.Status = "y";
                }
                else
                {
                    json.Msg = "未找到要删除的记录";
                }
                WriteLog(Common.Enums.enumOperator.Remove, "删除编码，结果：" + json.Msg, Common.Enums.enumLog4net.WARN);
            }
            catch (Exception e) { WriteLog(Common.Enums.enumOperator.Remove, "删除编码出现异常：", e); }
            return Json(json);
        }
        #endregion

        #region 帮助方法及其它控制器调用
        /// <summary>
        /// 加载列表
        /// </summary>
        private async Task<object> BindList(string codetype)
        {
            var predicate = PredicateBuilder.True<Domain.SYS_CODE>();
            //关键词
            if (!string.IsNullOrEmpty(keywords))
            {
                predicate = predicate.And(p => p.NAMETEXT.Contains(keywords));
            }
            //编码类型
            if (!string.IsNullOrEmpty(codetype))
            {
                predicate = predicate.And(p => p.CODETYPE == codetype);
            }
            var query = this.CodeManage.LoadAll(predicate).
                OrderByDescending(p => p.UPDATEDATE).OrderBy(p => p.SHOWORDER).OrderBy(p => p.CODETYPE);
            return await Task.Run(() => this.CodeManage.Query(query, base.page, base.pagesize));
        }
        /// <summary>
        /// 根据类别获取上下级关系
        /// </summary>
        public ActionResult GetParentCode()
        {
            var json = new JsonHelper() { Status = "n", Data = "" };
            string codetype = Request.Form["type"];
            if (!string.IsNullOrEmpty(codetype))
            {
                var result = this.CodeManage.LoadListAll(p => p.CODETYPE == codetype);
                if (result != null && result.Count > 0)
                {
                    json.Data = result;
                    json.Status = "y";
                }
            }
            return Json(json);
        }
        #endregion
    }
}