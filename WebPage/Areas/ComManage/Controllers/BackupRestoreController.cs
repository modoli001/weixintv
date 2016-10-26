using Common;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebPage.Controllers;

namespace WebPage.Areas.ComManage.Controllers
{
    public class BackupRestoreController : BaseController
    {
       
        #region 基本视图 - 备份
        /// <summary>
        /// 系统备份加载列表
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Backup", OperaAction = "View")]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 程序备份
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Backup", OperaAction = "BackUpApplication")]
        public ActionResult BackUpApplication()
        {
            return View();
        }
        /// <summary>
        /// 数据库备份
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Backup", OperaAction = "BackUpDataBase")]
        public ActionResult BackUpDataBase()
        {
            return View();
        }

        /// <summary>
        /// 删除文件或文件夹
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Backup", OperaAction = "Remove")]
        public ActionResult DeleteBy()
        {
            var jsonM = new JsonHelper() { Status = "y", Msg = "success" };
            try
            {
                var path = Request.Form["path"].Trim(';').Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(p => p).ToList();

                foreach (var file in path)
                {
                    //删除文件
                    FileHelper.DeleteFile(Server.MapPath(file));
                }

                WriteLog(Common.Enums.enumOperator.Remove, "删除文件：" + path, Common.Enums.enumLog4net.WARN);
            }
            catch (Exception ex)
            {
                jsonM.Status = "err";
                jsonM.Msg = "删除过程中发生错误！";
                WriteLog(Common.Enums.enumOperator.Remove, "删除文件发生错误：", ex);
            }
            return Json(jsonM);
        }
        #endregion

        #region 基本视图 - 还原
        [UserAuthorizeAttribute(ModuleAlias = "Restore", OperaAction = "View")]
        public ActionResult Restore()
        {
            return View();
        }
        #endregion


        #region 帮助方法及其它控制器调用
        /// <summary>
        /// 获取备份文件信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBackUpData()
        {
            string fileExt = Request.Form["fileExt"];
            string path = "/App_Data/BackUp/";
            var jsonM = new JsonHelper() { Status = "y", Msg = "success" };
            try
            {                
                if (!FileHelper.IsExistDirectory(Server.MapPath(path)))
                {
                    jsonM.Status = "n";
                    jsonM.Msg = "目录不存在！";
                }
                else if (FileHelper.IsEmptyDirectory(Server.MapPath(path)))
                {
                    jsonM.Status = "empty";
                }
                else
                {
                    if (fileExt == "*" || string.IsNullOrEmpty(fileExt))
                    {
                        jsonM.Data = Common.Utils.DataTableToList<FileModel>(FileHelper.GetAllFileTable(Server.MapPath(path))).OrderByDescending(p => p.time).ToList();
                    }
                    else
                    {
                        jsonM.Data = Common.Utils.DataTableToList<FileModel>(FileHelper.GetAllFileTable(Server.MapPath(path))).OrderByDescending(p => p.time).Where(p => p.ext == fileExt).ToList();
                    }
                   
                }

            }
            catch (Exception)
            {
                jsonM.Status = "err";
                jsonM.Msg = "获取文件失败！";
            }
            return Content(JsonConverter.Serialize(jsonM, true));
        }
        /// <summary>
        /// 备份程序文件
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Backup", OperaAction = "BackUpApplication")]
        public ActionResult BackUpFiles()
        {
            var json = new JsonHelper() { Msg = "程序备份完成", Status = "n" };

            try
            {
                //检查上传的物理路径是否存在，不存在则创建
                if (!Directory.Exists(Server.MapPath("/App_Data/BackUp/ApplicationBackUp/")))
                {
                    Directory.CreateDirectory(Server.MapPath("/App_Data/BackUp/ApplicationBackUp/"));
                }

                ZipHelper.ZipDirectory(Server.MapPath("/"), Server.MapPath("/App_Data/BackUp/ApplicationBackUp/"), "App_" + this.CurrentUser.PinYin + "_" + DateTime.Now.ToString("yyyyMMddHHmmss"), true, new List<string>() { Server.MapPath("/App_Data/") });
                WriteLog(Common.Enums.enumOperator.None, "程序备份：" + json.Msg, Common.Enums.enumLog4net.WARN);
                json.Status = "y";
            }
            catch (Exception e)
            {
                json.Msg = "程序备份失败！";
                WriteLog(Common.Enums.enumOperator.None, "程序备份：", e);
            }

            return Json(json);
        }
        /// <summary>
        /// 备份数据
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Backup", OperaAction = "BackUpDataBase")]
        public ActionResult BackUpData()
        {
            var json = new JsonHelper() { Msg = "数据备份完成", Status = "n" };

            try
            {
                //检查上传的物理路径是否存在，不存在则创建
                if (!Directory.Exists(Server.MapPath("/App_Data/BackUp/DataBaseBackUp/")))
                {
                    Directory.CreateDirectory(Server.MapPath("/App_Data/BackUp/DataBaseBackUp/"));
                }
                //备份数据库                
                using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
                {
                    var bakPath = Server.MapPath("/App_Data/BackUp/DataBaseBackUp/");
                    using (SqlCommand cmd = new SqlCommand("backup database wkmvc_db to disk='" + bakPath + "Data_" + this.CurrentUser.PinYin + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".bak'", conn))
                    {
                        try
                        {
                            conn.Open();
                            cmd.CommandTimeout = 0;
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                        finally
                        {
                            conn.Close();
                            cmd.Dispose();
                        }
                    }
                }

                WriteLog(Common.Enums.enumOperator.None, "数据备份：" + json.Msg, Common.Enums.enumLog4net.WARN);
                json.Status = "y";
            }
            catch (Exception e)
            {
                json.Msg = "数据备份失败！";
                WriteLog(Common.Enums.enumOperator.None, "数据备份：", e);
            }

            return Json(json);
        }
        /// <summary>
        /// 还原数据
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Restore", OperaAction = "RestoreData")]
        public ActionResult RestoreData()
        {
            var json = new JsonHelper() { Msg = "数据还原完成", Status = "n" };

            var path = Request.Form["path"];

            try
            {
                //检查还原备份的物理路径是否存在
                if (!System.IO.File.Exists(Server.MapPath(path)))
                {
                    json.Msg = "还原数据失败，备份文件不存在或已损坏！";
                    return Json(json);
                }
                //还原数据库                
                //using (SqlConnection Con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString))
                //{

                //    try
                //    {
                //        Con.Open();
                //        SqlCommand Com = new SqlCommand("use master restore database wkmvc_db  from disk='" + Server.MapPath(path) + "'", Con);
                //        Com.ExecuteNonQuery();
                //    }
                //    catch (Exception e)
                //    {
                //        throw e;
                //    }
                //}

                WriteLog(Common.Enums.enumOperator.None, "数据还原：" + json.Msg, Common.Enums.enumLog4net.WARN);
                json.Status = "y";
            }
            catch (Exception e)
            {
                json.Msg = "数据还原失败！";
                WriteLog(Common.Enums.enumOperator.None, "数据还原：", e);
            }

            return Json(json);
        }
        #endregion
    }
}