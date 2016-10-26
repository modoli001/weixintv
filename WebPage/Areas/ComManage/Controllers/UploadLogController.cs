using Common;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPage.Controllers;

namespace WebPage.Areas.ComManage.Controllers
{
    public class UploadLogController : BaseController
    {

        #region 声明容器
        /// <summary>
        /// 文件上传管理
        /// </summary>
        IUploadManage UploadManage { get; set; }

        #endregion

        #region 基本视图
        /// <summary>
        /// 上传记录首页
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "UploadLog", OperaAction = "View")]
        public ActionResult Index()
        {
            try
            {
                #region 处理查询参数
                string fileExt = Request.QueryString["fileExt"];
                ViewBag.Search = base.keywords;
                ViewData["fileExt"] = fileExt;
                #endregion

                return View(BindList(fileExt));
            }
            catch (Exception e)
            {
                WriteLog(Common.Enums.enumOperator.Select, "文件上传记录加载主页：", e);
                throw e.InnerException;
            }
        }
        /// <summary>
        /// 上传记录详情
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "UploadLog", OperaAction = "View")]
        public ActionResult Detail(string logId)
        {
            return View(UploadManage.Get(p => p.ID == logId) ?? new Domain.COM_UPLOAD());
        }
        #endregion

        #region 帮助方法及其它控制器调用
        /// <summary>
        /// 分页查询上传日志
        /// </summary>
        private Common.PageInfo BindList(string fileExt)
        {
            //基础数据
            var query = this.UploadManage.LoadAll(null);            

            //查询关键字
            if (!string.IsNullOrEmpty(keywords))
            {
                keywords = keywords.ToLower();
                query = query.Where(p => p.UPNEWNAME.Contains(keywords) || p.UPOPEATOR.Contains(keywords) || p.UPOLDNAME.Contains(keywords));
            }

            //除超级管理员，其他人默认看自己的文件
            if (!this.CurrentUser.IsAdmin)
            {
                var userid = CurrentUser.Id.ToString();
                query = query.Where(p => p.FK_USERID == userid);
            }

            //排序
            query = query.OrderByDescending(p => p.UPTIME);

            //分页
            var result = this.UploadManage.Query(query, page, 28);

            var list = result.List.Select(p => new
            {
                p.ID,
                p.UPOPEATOR,
                p.UPNEWNAME,
                p.UPTIME,
                SIZE=p.UPFILESIZE+p.UPFILEUNIT,
                ICON=GetFileIcon(p.UPFILESUFFIX)

            }).ToList();
            
            //文件类型
            if (!string.IsNullOrEmpty(fileExt))
            {
                switch (fileExt)
                {
                    case "images":
                        list = list.Where(p => p.ICON == "fa fa-image").ToList();
                        break;
                    case "videos":
                        list = list.Where(p => p.ICON == "fa fa-film").ToList();
                        break;
                    case "musics":
                        list = list.Where(p => p.ICON == "fa fa-music").ToList();
                        break;
                    case "docements":
                        list = list.Where(p => p.ICON == "fa fa-file-word-o" || p.ICON == "fa fa-file-excel-o" || p.ICON == "fa fa-file-powerpoint-o" || p.ICON == "fa fa-file-pdf-o" || p.ICON == "fa fa-file-text-o").ToList();
                        break;
                    case "others":
                        list = list.Where(p => p.ICON == "fa fa-file" || p.ICON == "fa fa-file-zip-o").ToList();
                        break;
                }
            }

            return new Common.PageInfo(result.Index, result.PageSize, result.Count, Common.JsonConverter.JsonClass(list));
        }
        private string GetFileIcon(string _fileExt)
        {
            var images = ConfigurationManager.AppSettings["Image"].Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => p).ToList();
            var videos = ConfigurationManager.AppSettings["Video"].Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => p).ToList();
            var musics = ConfigurationManager.AppSettings["Music"].Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => p).ToList();
            //图片文件
            if (images.Contains(_fileExt.ToLower()))
            {
                return "fa fa-image";
            }
            //视频文件
            else if (videos.Contains(_fileExt.ToLower()))
            {
                return "fa fa-film";
            }
            //音频文件
            else if (musics.Contains(_fileExt.ToLower()))
            {
                return "fa fa-music";
            }
            else
            {
                switch (_fileExt.ToLower())
                {
                    case "doc":
                    case "docx":
                        return "fa fa-file-word-o";
                    case "xls":
                    case "xlsx":
                        return "fa fa-file-excel-o";
                    case "ppt":
                    case "pptx":
                        return "fa fa-file-powerpoint-o";
                    case "pdf":
                        return "fa fa-file-pdf-o";
                    case "txt":
                        return "fa fa-file-text-o";
                    case "zip":
                    case "rar":
                        return "fa fa-file-zip-o";
                    default:
                        return "fa fa-file";
                }
            }
        }
        #endregion
    }
}