using Common;
using Service.IService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebPage.Controllers;

namespace WebPage.Areas.ComManage.Controllers
{
    /// <summary>
    /// 文件上传控制器
    /// add yuangang by 2015-10-14
    /// </summary>
    public class UploadController : BaseController
    {
        #region 声明容器
        /// <summary>
        /// 文件上传管理
        /// </summary>
        IUploadManage UploadManage { get; set; }

        #endregion

        #region 基本视图
        /// <summary>
        /// 文件管理默认页面
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Files", OperaAction = "View")]
        public ActionResult Home()
        {
            var fileExt = Request.QueryString["fileExt"] ?? "";
            ViewData["fileExt"] = fileExt;
            return View();
        }
        /// <summary>
        /// 通过路径获取所有文件
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllFileData()
        {
            string fileExt = Request.Form["fileExt"];
            var jsonM = new JsonHelper() { Status = "y", Msg = "success" };
            try
            {
                var images = ConfigurationManager.AppSettings["Image"].Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => "." + p).ToList();
                var videos = ConfigurationManager.AppSettings["Video"].Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => "." + p).ToList();
                var musics = ConfigurationManager.AppSettings["Music"].Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => "." + p).ToList();
                var documents = ConfigurationManager.AppSettings["Document"].Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => "." + p).ToList();

                switch(fileExt)
                {
                    case "images":   
                    
                        jsonM.Data = Common.Utils.DataTableToList<FileModel>(FileHelper.GetAllFileTable(Server.MapPath(ConfigurationManager.AppSettings["uppath"]))).OrderByDescending(p => p.name).Where(p=>images.Any(e=>e==p.ext)).ToList();
                        break;
                    case "videos":
                       
                        jsonM.Data = Common.Utils.DataTableToList<FileModel>(FileHelper.GetAllFileTable(Server.MapPath(ConfigurationManager.AppSettings["uppath"]))).OrderByDescending(p => p.name).Where(p => videos.Any(e => e == p.ext)).ToList();
                        break;
                    case "musics":
                       
                        jsonM.Data = Common.Utils.DataTableToList<FileModel>(FileHelper.GetAllFileTable(Server.MapPath(ConfigurationManager.AppSettings["uppath"]))).OrderByDescending(p => p.name).Where(p => musics.Any(e => e == p.ext)).ToList();
                        break;
                    case "files":
                       
                        jsonM.Data = Common.Utils.DataTableToList<FileModel>(FileHelper.GetAllFileTable(Server.MapPath(ConfigurationManager.AppSettings["uppath"]))).OrderByDescending(p => p.name).Where(p => documents.Any(e => e == p.ext)).ToList();
                        break;
                    case "others":

                        jsonM.Data = Common.Utils.DataTableToList<FileModel>(FileHelper.GetAllFileTable(Server.MapPath(ConfigurationManager.AppSettings["uppath"]))).OrderByDescending(p => p.name).Where(p => !images.Contains(p.ext) && !videos.Contains(p.ext) && !musics.Contains(p.ext) && !documents.Contains(p.ext)).ToList();
                        break;
                    default:
                        jsonM.Data = Common.Utils.DataTableToList<FileModel>(FileHelper.GetAllFileTable(Server.MapPath(ConfigurationManager.AppSettings["uppath"]))).OrderByDescending(p => p.name).ToList();
                        break;
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
        /// 单文件上传视图
        /// </summary>
        /// <returns></returns>
        public ActionResult FileMain()
        {
            ViewData["spath"] = ConfigurationManager.AppSettings["uppath"];
            return View();
        }
        /// <summary>
        /// 通过路径获取文件
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFileData()
        {
            string path = Request.Form["path"];
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
                    jsonM.Data = Common.Utils.DataTableToList<FileModel>(FileHelper.GetFileTable(Server.MapPath(path))).OrderByDescending(p => p.name).ToList();
                }
            }
            catch (Exception)
            {
                jsonM.Status = "err";
                jsonM.Msg = "获取文件失败！";
            }
            return Content(JsonConverter.Serialize(jsonM, true));
        }
        #endregion

        #region 文件上传
        /// <summary>
        /// 单个文件上传
        /// </summary>
        [HttpPost]
        public ActionResult SignUpFile()
        {
            var jsonM = new JsonHelper() { Status = "n", Msg = "success" };
            try
            {
                //取得上传文件
                HttpPostedFileBase upfile = Request.Files["fileUp"];

                //原始文件路径
                string delpath = Request.QueryString["delpath"];

                //缩略图
                bool isThumbnail = string.IsNullOrEmpty(Request.QueryString["isThumbnail"]) ? false : Request.QueryString["isThumbnail"].ToLower() == "true" ? true : false;
                //水印
                bool isWater = string.IsNullOrEmpty(Request.QueryString["isWater"]) ? false : Request.QueryString["isWater"].ToLower() == "true" ? true : false;


                if (upfile == null)
                {
                    jsonM.Msg = "请选择要上传文件！";
                    return Json(jsonM);
                }
                jsonM = FileSaveAs(upfile, isThumbnail, isWater);

                #region 移除原始文件
                if (jsonM.Status == "y" && !string.IsNullOrEmpty(delpath))
                {
                    if (System.IO.File.Exists(Utils.GetMapPath(delpath)))
                    {
                        System.IO.File.Delete(Utils.GetMapPath(delpath));
                    }
                }
                #endregion

                if (jsonM.Status == "y")
                {
                    #region 记录上传数据
                    string unit = string.Empty;
                    var jsonValue = Common.JsonConverter.ConvertJson(jsonM.Data.ToString());
                    var entity = new Domain.COM_UPLOAD()
                    {
                        ID = Guid.NewGuid().ToString(),
                        FK_USERID = this.CurrentUser.Id.ToString(),
                        UPOPEATOR = this.CurrentUser.Name,
                        UPTIME = DateTime.Now,
                        UPOLDNAME = jsonValue.oldname,
                        UPNEWNAME = jsonValue.newname,
                        UPFILESIZE = FileHelper.GetDiyFileSize(long.Parse(jsonValue.size), out unit),
                        UPFILEUNIT = unit,
                        UPFILEPATH = jsonValue.path,
                        UPFILESUFFIX = jsonValue.ext,
                        UPFILETHUMBNAIL = jsonValue.thumbpath,
                        UPFILEIP = Utils.GetIP(),
                        UPFILEURL = "http://" + Request.Url.AbsoluteUri.Replace("http://", "").Substring(0, Request.Url.AbsoluteUri.Replace("http://", "").IndexOf('/'))
                    };
                    this.UploadManage.Save(entity);
                    #endregion

                    #region 返回文件信息
                    jsonM.Data = "{\"oldname\": \"" + jsonValue.oldname + "\","; //原始名称
                    jsonM.Data += " \"newname\":\"" + jsonValue.newname + "\","; //新名称
                    jsonM.Data += " \"path\": \"" + jsonValue.path + "\", ";  //路径
                    jsonM.Data += " \"thumbpath\":\"" + jsonValue.thumbpath + "\","; //缩略图
                    jsonM.Data += " \"size\": \"" + jsonValue.size + "\",";   //大小
                    jsonM.Data += " \"id\": \"" + entity.ID + "\",";   //上传文件ID
                    jsonM.Data += " \"uptime\": \"" + entity.UPTIME + "\",";   //上传时间
                    jsonM.Data += " \"operator\": \"" + entity.UPOPEATOR + "\",";   //上传人
                    jsonM.Data += " \"unitsize\": \"" + entity.UPFILESIZE + unit + "\",";   //带单位大小
                    jsonM.Data += "\"ext\":\"" + jsonValue.ext + "\"}";//后缀名
                    #endregion
                }

            }
            catch (Exception ex)
            {
                jsonM.Msg = "上传过程中发生错误，消息：" + ex.Message;
                jsonM.Status = "n";
            }
            return Json(jsonM);
        }

       /// <summary>
       /// 文件上传方法
       /// </summary>
       /// <param name="postedFile">文件流</param>
       /// <param name="isThumbnail">是否生成缩略图</param>
       /// <param name="isWater">是否生成水印</param>
       /// <returns>上传后文件信息</returns>
        private JsonHelper FileSaveAs(HttpPostedFileBase postedFile, bool isThumbnail, bool isWater)
        {
            var jsons = new JsonHelper { Status = "n" };
            try
            {
                string fileExt = Utils.GetFileExt(postedFile.FileName); //文件扩展名，不含“.”
                int fileSize = postedFile.ContentLength; //获得文件大小，以字节为单位
                string fileName = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf(@"\") + 1); //取得原文件名
                string newFileName = Utils.GetRamCode() + "." + fileExt; //随机生成新的文件名
                string upLoadPath = GetUpLoadPath(fileExt); //上传目录相对路径
                string fullUpLoadPath = Utils.GetMapPath(upLoadPath); //上传目录的物理路径
                string newFilePath = upLoadPath + newFileName; //上传后的路径
                string newThumbnailFileName = "thumb_" + newFileName; //随机生成缩略图文件名

                //检查文件扩展名是否合法
                if (!CheckFileExt(fileExt))
                {
                    jsons.Msg = "不允许上传" + fileExt + "类型的文件！";
                    return jsons;
                }

                //检查文件大小是否合法
                if (!CheckFileSize(fileExt, fileSize))
                {
                    jsons.Msg = "文件超过限制的大小啦！";
                    return jsons;
                }

                //检查上传的物理路径是否存在，不存在则创建
                if (!Directory.Exists(fullUpLoadPath))
                {
                    Directory.CreateDirectory(fullUpLoadPath);
                }

                //检查文件是否真实合法
                //如果文件真实合法 则 保存文件 关闭文件流
                //if (!CheckFileTrue(postedFile, fullUpLoadPath + newFileName))
                //{
                //    jsons.Msg = "不允许上传不可识别的文件!";
                //    return jsons;
                //}

                //保存文件
                postedFile.SaveAs(fullUpLoadPath + newFileName);

                string thumbnail = string.Empty;

                //如果是图片，检查是否需要生成缩略图，是则生成
                if (IsImage(fileExt) && isThumbnail && ConfigurationManager.AppSettings["ThumbnailWidth"].ToString() != "0" && ConfigurationManager.AppSettings["ThumbnailHeight"].ToString() != "0")
                {
                    Thumbnail.MakeThumbnailImage(fullUpLoadPath + newFileName, fullUpLoadPath + newThumbnailFileName,
                       int.Parse(ConfigurationManager.AppSettings["ThumbnailWidth"]), int.Parse(ConfigurationManager.AppSettings["ThumbnailHeight"]), "W");
                    thumbnail = upLoadPath + newThumbnailFileName;
                }
                //如果是图片，检查是否需要打水印
                if (IsImage(fileExt) && isWater)
                {
                    switch (ConfigurationManager.AppSettings["WatermarkType"].ToString())
                    {
                        case "1":
                            WaterMark.AddImageSignText(newFilePath, newFilePath,
                                ConfigurationManager.AppSettings["WatermarkText"], int.Parse(ConfigurationManager.AppSettings["WatermarkPosition"]),
                                int.Parse(ConfigurationManager.AppSettings["WatermarkImgQuality"]), ConfigurationManager.AppSettings["WatermarkFont"], int.Parse(ConfigurationManager.AppSettings["WatermarkFontsize"]));
                            break;
                        case "2":
                            WaterMark.AddImageSignPic(newFilePath, newFilePath,
                                ConfigurationManager.AppSettings["WatermarkPic"], int.Parse(ConfigurationManager.AppSettings["WatermarkPosition"]),
                                int.Parse(ConfigurationManager.AppSettings["WatermarkImgQuality"]), int.Parse(ConfigurationManager.AppSettings["WatermarkTransparency"]));
                            break;
                    }
                }

                string unit = string.Empty;

                //处理完毕，返回JOSN格式的文件信息
                jsons.Data = "{\"oldname\": \"" + fileName + "\",";     //原始文件名
                jsons.Data += " \"newname\":\"" + newFileName + "\",";  //文件新名称
                jsons.Data += " \"path\": \"" + newFilePath + "\", ";   //文件路径
                jsons.Data += " \"thumbpath\":\"" + thumbnail + "\",";  //缩略图路径
                jsons.Data += " \"size\": \"" + fileSize + "\",";       //文件大小
                jsons.Data += "\"ext\":\"" + fileExt + "\"}";           //文件格式
                jsons.Status = "y";
                return jsons;
            }
            catch
            {
                jsons.Msg = "上传过程中发生意外错误！";
                return jsons;
            }
        }
        
        /// <summary>
        /// 返回上传目录相对路径
        /// </summary>
        /// <param name="_fileExt">文件类型</param>
        /// <returns></returns>
        private string GetUpLoadPath(string _fileExt)
        {
            string path = ConfigurationManager.AppSettings["uppath"];

            //主流图片格式 psd、swf、tiff等格式放入others
            if (IsImage(_fileExt))
            {
                path += "images/";
            }
            //主流视频格式
            else if (IsVideos(_fileExt))
            {
                path += "videos/";
            }
            //主流文档格式
            else if (IsDocument(_fileExt))
            {
                path += "files/";
            }
            //主流音频格式
            else if (IsMusics(_fileExt))
            {
                path += "musics/";
            }
            else if(_fileExt=="bak")
            {
                path = "/App_Data/BackUp/DataBaseBackUp/";
            }
            //其它文件格式
            else
            {
                path += "others/";
            }
                     
            #region 用户控制

            //判读是否为超级管理员，超级管理员可查看所有的文件
            if (!base.CurrentUser.IsAdmin)
            {
                path += base.CurrentUser.PinYin + "/";
            }
            #endregion

            path += DateTime.Now.ToString("yyyy") + "/" + DateTime.Now.ToString("MM") + "/";

            return path;
        }       
        #endregion

        #region 文件格式
        /// <summary>
        /// 是否为图片
        /// </summary>
        /// <param name="_fileExt">文件扩展名，不含“.”</param>
        /// <returns></returns>
        private bool IsImage(string _fileExt)
        {
            var images = ConfigurationManager.AppSettings["Image"].Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => p).ToList();
            if (images.Contains(_fileExt.ToLower())) return true;
            return false;
        }
        /// <summary>
        /// 是否为视频
        /// </summary>
        /// <param name="_fileExt">文件扩展名，不含“.”</param>
        /// <returns></returns>
        private bool IsVideos(string _fileExt)
        {
            var videos = ConfigurationManager.AppSettings["Video"].Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => p).ToList();
            if (videos.Contains(_fileExt.ToLower())) return true;
            return false;
        }
        /// <summary>
        /// 是否为音频
        /// </summary>
        /// <param name="_fileExt">文件扩展名，不含“.”</param>
        /// <returns></returns>
        private bool IsMusics(string _fileExt)
        {
            var musics = ConfigurationManager.AppSettings["Music"].Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => p).ToList();
            if (musics.Contains(_fileExt.ToLower())) return true;
            return false;
        }
        /// <summary>
        /// 是否为文档
        /// </summary>
        /// <param name="_fileExt">文件扩展名，不含“.”</param>
        /// <returns></returns>
        private bool IsDocument(string _fileExt)
        {
            var documents = ConfigurationManager.AppSettings["Document"].Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => p).ToList();
            if (documents.Contains(_fileExt.ToLower())) return true;
            return false;
        }
        #endregion

        #region 文件检测
        /// <summary>
        /// 检查是否为合法的上传文件
        /// </summary>
        /// <param name="_fileExt">文件扩展名，不含“.”</param>
        private bool CheckFileExt(string _fileExt)
        {
            //检查危险文件
            string[] excExt = { "asp", "aspx", "php", "jsp", "htm", "html" };
            for (int i = 0; i < excExt.Length; i++)
            {
                if (excExt[i].ToLower() == _fileExt.ToLower())
                {
                    return false;
                }
            }
            //检查合法文件
            var allowExts = ConfigurationManager.AppSettings["AttachExtension"].Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => p).ToList();
            if (!allowExts.Contains(_fileExt.ToLower()))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 检查文件大小是否合法
        /// </summary>
        /// <param name="_fileExt">文件扩展名，不含“.”</param>
        /// <param name="_fileSize">文件大小(B)</param>
        private bool CheckFileSize(string _fileExt, int _fileSize)
        {
            //图片文件
            if (IsImage(_fileExt))
            {
                if (_fileSize / 1024 > int.Parse(ConfigurationManager.AppSettings["AttachImagesize"].ToString()))
                    return false;
            }
            //视频文件
            else if(IsVideos(_fileExt))
            {
                if (_fileSize / 1024 > int.Parse(ConfigurationManager.AppSettings["AttachVideosize"].ToString()))
                    return false;
            }
            //文档文件
            else if (IsDocument(_fileExt))
            {
                if (_fileSize / 1024 > int.Parse(ConfigurationManager.AppSettings["AttachDocmentsize"].ToString()))
                    return false;
            }
            //其它文件
            else
            {
                if (_fileSize / 1024 > int.Parse(ConfigurationManager.AppSettings["AttachFilesize"].ToString()))
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 是否真实，入侵验证
        /// </summary>
        /// <param name="_fileBinary">文件post</param>
        /// <param name="newfilePath">新文件路径+文件名</param>
        /// <returns></returns>
        private bool CheckFileTrue(HttpPostedFileBase _fileBinary, string newfilePath)
        {
            Stream fs = _fileBinary.InputStream;
            BinaryReader br = new BinaryReader(fs);
            byte but;
            string str = "";
            try
            {
                but = br.ReadByte();
                str = but.ToString();
                but = br.ReadByte();
                str += but.ToString();
            }
            catch
            {
                return false;
            }
            /*文件扩展名说明
             * 4946/104116 txt
             * 7173        gif 
             * 255216      jpg
             * 13780       png
             * 6677        bmp
             * 239187      txt,aspx,asp,sql
             * 208207      xls.doc.ppt
             * 6063        xml
             * 6033        htm,html
             * 4742        js
             * 8075        xlsx,zip,pptx,mmap,zip
             * 8297        rar   
             * 01          accdb,mdb
             * 7790        exe,dll           
             * 5666        psd 
             * 255254      rdp 
             * 10056       bt种子 
             * 64101       bat 
             * 4059        sgf
             * 3780        pdf
             */
            bool bl = false;
            string[] binary = { "4946", "104116", "7173", "255216", "13780", "6677", "239187", "208207", "6063", "6033", "4742", "8075", "8297", "01", "5666", "255254", "10056" };
            for (int i = 0; i < binary.Length; i++)
            {
                if (binary[i].ToLower() == str.ToLower())
                {
                    bl = true;
                }
            }
            if (bl)
            {
                _fileBinary.SaveAs(newfilePath);
            }
            br.Close();
            fs.Close();
            return bl;
        }
        #endregion

        #region 文件操作
        /// <summary>
        /// 删除文件或文件夹
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Files", OperaAction = "Remove")]
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
        /// <summary>
        /// 复制文件到文件夹
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Files", OperaAction = "Copy")]
        public ActionResult Copy(string files)
        {
            ViewData["Files"] = files;
            ViewData["spath"] = ConfigurationManager.AppSettings["uppath"];
            return View();
        }
        [UserAuthorizeAttribute(ModuleAlias = "Files", OperaAction = "Copy")]
        public ActionResult CopyFiles()
        {
            var json = new JsonHelper() { Msg = "复制文件完成", Status = "n" };

            try
            {
                var files = Request.Form["files"].Trim(';').Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(p => p).ToList();
                var path = Request.Form["path"];
                foreach (var file in files)
                {
                    FileHelper.Copy(Server.MapPath(file), Server.MapPath(path) + FileHelper.GetFileName(Server.MapPath(file)));
                }
                WriteLog(Common.Enums.enumOperator.None, "复制文件：" + Request.Form["files"].Trim(';') + "，结果：" + json.Msg, Common.Enums.enumLog4net.WARN);
                json.Status = "y";
            }
            catch(Exception e)
            {
                json.Msg = "复制文件失败！";
                WriteLog(Common.Enums.enumOperator.None, "复制文件：", e);
            }

            return Json(json);
        }
        /// <summary>
        /// 移动文件到文件夹
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Files", OperaAction = "Cut")]
        public ActionResult Cut(string files)
        {
            ViewData["Files"] = files;
            ViewData["spath"] = ConfigurationManager.AppSettings["uppath"];
            return View();
        }
        [UserAuthorizeAttribute(ModuleAlias = "Files", OperaAction = "Cut")]
        public ActionResult CutFiles()
        {
            var json = new JsonHelper() { Msg = "移动文件完成", Status = "n" };

            try
            {
                var files = Request.Form["files"].Trim(';').Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(p => p).ToList();
                var path = Request.Form["path"];
                foreach (var file in files)
                {
                    FileHelper.Move(Server.MapPath(file), Server.MapPath(path));
                }
                WriteLog(Common.Enums.enumOperator.None, "移动文件：" + Request.Form["files"].Trim(';') + "，结果：" + json.Msg, Common.Enums.enumLog4net.WARN);
                json.Status = "y";
            }
            catch (Exception e)
            {
                json.Msg = "移动文件失败！";
                WriteLog(Common.Enums.enumOperator.None, "移动文件：", e);
            }

            return Json(json);
        }
        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Files", OperaAction = "Compress")]
        public ActionResult Compress(string files)
        {
            ViewData["Files"] = files;
            ViewData["spath"] = ConfigurationManager.AppSettings["uppath"];
            return View();
        }
        [UserAuthorizeAttribute(ModuleAlias = "Files", OperaAction = "Compress")]
        public ActionResult CompressFiles()
        {
            var json = new JsonHelper() { Msg = "压缩文件完成", Status = "n" };

            try
            {
                var files = Request.Form["files"].Trim(';').Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(p => p).ToList();
                var path = Request.Form["path"];
                foreach (var file in files)
                {
                    ZipHelper.ZipFile(Server.MapPath(file), Server.MapPath(path));
                }
                //ZipHelper.ZipDirectory(Server.MapPath("/upload/files/"), Server.MapPath(path));
                WriteLog(Common.Enums.enumOperator.None, "压缩文件：" + Request.Form["files"].Trim(';') + "，结果：" + json.Msg, Common.Enums.enumLog4net.WARN);
                json.Status = "y";
            }
            catch (Exception e)
            {
                json.Msg = "压缩文件失败！";
                WriteLog(Common.Enums.enumOperator.None, "压缩文件：", e);
            }

            return Json(json);
        }
        /// <summary>
        /// 解压文件
        /// </summary>
        /// <returns></returns>
        [UserAuthorizeAttribute(ModuleAlias = "Files", OperaAction = "Expand")]
        public ActionResult Expand(string files)
        {
            ViewData["Files"] = files;
            ViewData["spath"] = ConfigurationManager.AppSettings["uppath"];
            return View();
        }
        [UserAuthorizeAttribute(ModuleAlias = "Files", OperaAction = "Expand")]
        public ActionResult ExpandFiles()
        {
            var json = new JsonHelper() { Msg = "解压文件完成", Status = "n" };

            try
            {
                var files = Request.Form["files"];
                var path = Request.Form["path"];
                var password = Request.Form["password"];

                if (string.IsNullOrEmpty(password))
                {
                    json.Msg = "请输入解压密码！";
                    return Json(json);
                }

                string CurrentPassword=ConfigurationManager.AppSettings["ZipPassword"] != null ? new Common.CryptHelper.AESCrypt().Decrypt(ConfigurationManager.AppSettings["ZipPassword"].ToString()) : "guodongbudingxizhilang";

                if (password != CurrentPassword)
                {
                    json.Msg = "解压密码无效！";
                    return Json(json);
                }
               
                ZipHelper.UnZip(Server.MapPath(files), Server.MapPath(path),password);
                
                WriteLog(Common.Enums.enumOperator.None, "解压文件：" + Request.Form["files"].Trim(';') + "，结果：" + json.Msg, Common.Enums.enumLog4net.WARN);
                json.Status = "y";
            }
            catch (Exception e)
            {
                json.Msg = "解压文件失败！";
                WriteLog(Common.Enums.enumOperator.None, "解压文件：", e);
            }

            return Json(json);
        }
        #endregion
    }

    #region 自定义帮助类
    /// <summary>
    /// 文件模型
    /// </summary>
    public class FileModel
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 文件全称
        /// </summary>
        public string fullname { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string path { get; set; }
        /// <summary>
        /// 文件格式
        /// </summary>
        public string ext { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public string size { get; set; }
        /// <summary>
        /// 文件图标
        /// </summary>
        public string icon { get; set; }
        /// <summary>
        /// 是否为文件夹
        /// </summary>
        public bool isfolder { get; set; }
        /// <summary>
        /// 是否为图片
        /// </summary>
        public bool isImage { get; set; }
        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime time { get; set; }

    }
    /// <summary>
    /// 缩略图构造类
    /// </summary>
    public class Thumbnail
    {
        private Image srcImage;
        private string srcFileName;

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="FileName">原始图片路径</param>
        public bool SetImage(string FileName)
        {
            srcFileName = Utils.GetMapPath(FileName);
            try
            {
                srcImage = Image.FromFile(srcFileName);
            }
            catch
            {
                return false;
            }
            return true;

        }

        /// <summary>
        /// 回调
        /// </summary>
        /// <returns></returns>
        public bool ThumbnailCallback()
        {
            return false;
        }

        /// <summary>
        /// 生成缩略图,返回缩略图的Image对象
        /// </summary>
        /// <param name="Width">缩略图宽度</param>
        /// <param name="Height">缩略图高度</param>
        /// <returns>缩略图的Image对象</returns>
        public Image GetImage(int Width, int Height)
        {
            Image img;
            Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(ThumbnailCallback);
            img = srcImage.GetThumbnailImage(Width, Height, callb, IntPtr.Zero);
            return img;
        }

        /// <summary>
        /// 保存缩略图
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        public void SaveThumbnailImage(int Width, int Height)
        {
            switch (Path.GetExtension(srcFileName).ToLower())
            {
                case ".png":
                    SaveImage(Width, Height, ImageFormat.Png);
                    break;
                case ".gif":
                    SaveImage(Width, Height, ImageFormat.Gif);
                    break;
                case ".bmp":
                    SaveImage(Width, Height, ImageFormat.Bmp);
                    break;
                default:
                    SaveImage(Width, Height, ImageFormat.Jpeg);
                    break;
            }
        }

        /// <summary>
        /// 生成缩略图并保存
        /// </summary>
        /// <param name="Width">缩略图的宽度</param>
        /// <param name="Height">缩略图的高度</param>
        /// <param name="imgformat">保存的图像格式</param>
        /// <returns>缩略图的Image对象</returns>
        public void SaveImage(int Width, int Height, ImageFormat imgformat)
        {
            if (imgformat != ImageFormat.Gif && (srcImage.Width > Width) || (srcImage.Height > Height))
            {
                Image img;
                Image.GetThumbnailImageAbort callb = new Image.GetThumbnailImageAbort(ThumbnailCallback);
                img = srcImage.GetThumbnailImage(Width, Height, callb, IntPtr.Zero);
                srcImage.Dispose();
                img.Save(srcFileName, imgformat);
                img.Dispose();
            }
        }

        #region Helper

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="image">Image 对象</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="ici">指定格式的编解码参数</param>
        private static void SaveImage(Image image, string savePath, ImageCodecInfo ici)
        {
            //设置 原图片 对象的 EncoderParameters 对象
            EncoderParameters parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(Encoder.Quality, ((long)100));
            image.Save(savePath, ici, parameters);
            parameters.Dispose();
        }

        /// <summary>
        /// 获取图像编码解码器的所有相关信息
        /// </summary>
        /// <param name="mimeType">包含编码解码器的多用途网际邮件扩充协议 (MIME) 类型的字符串</param>
        /// <returns>返回图像编码解码器的所有相关信息</returns>
        private static ImageCodecInfo GetCodecInfo(string mimeType)
        {
            ImageCodecInfo[] CodecInfo = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in CodecInfo)
            {
                if (ici.MimeType == mimeType)
                    return ici;
            }
            return null;
        }

        /// <summary>
        /// 计算新尺寸
        /// </summary>
        /// <param name="width">原始宽度</param>
        /// <param name="height">原始高度</param>
        /// <param name="maxWidth">最大新宽度</param>
        /// <param name="maxHeight">最大新高度</param>
        /// <returns></returns>
        private static Size ResizeImage(int width, int height, int maxWidth, int maxHeight)
        {
            //此次2012-02-05修改过=================
            if (maxWidth <= 0)
                maxWidth = width;
            if (maxHeight <= 0)
                maxHeight = height;
            //以上2012-02-05修改过=================
            decimal MAX_WIDTH = (decimal)maxWidth;
            decimal MAX_HEIGHT = (decimal)maxHeight;
            decimal ASPECT_RATIO = MAX_WIDTH / MAX_HEIGHT;

            int newWidth, newHeight;
            decimal originalWidth = (decimal)width;
            decimal originalHeight = (decimal)height;

            if (originalWidth > MAX_WIDTH || originalHeight > MAX_HEIGHT)
            {
                decimal factor;
                // determine the largest factor 
                if (originalWidth / originalHeight > ASPECT_RATIO)
                {
                    factor = originalWidth / MAX_WIDTH;
                    newWidth = Convert.ToInt32(originalWidth / factor);
                    newHeight = Convert.ToInt32(originalHeight / factor);
                }
                else
                {
                    factor = originalHeight / MAX_HEIGHT;
                    newWidth = Convert.ToInt32(originalWidth / factor);
                    newHeight = Convert.ToInt32(originalHeight / factor);
                }
            }
            else
            {
                newWidth = width;
                newHeight = height;
            }
            return new Size(newWidth, newHeight);
        }

        /// <summary>
        /// 得到图片格式
        /// </summary>
        /// <param name="name">文件名称</param>
        /// <returns></returns>
        public static ImageFormat GetFormat(string name)
        {
            string ext = name.Substring(name.LastIndexOf(".") + 1);
            switch (ext.ToLower())
            {
                case "jpg":
                case "jpeg":
                    return ImageFormat.Jpeg;
                case "bmp":
                    return ImageFormat.Bmp;
                case "png":
                    return ImageFormat.Png;
                case "gif":
                    return ImageFormat.Gif;
                default:
                    return ImageFormat.Jpeg;
            }
        }
        #endregion

        /// <summary>
        /// 制作小正方形
        /// </summary>
        /// <param name="image">图片对象</param>
        /// <param name="newFileName">新地址</param>
        /// <param name="newSize">长度或宽度</param>
        public static void MakeSquareImage(Image image, string newFileName, int newSize)
        {
            int i = 0;
            int width = image.Width;
            int height = image.Height;
            if (width > height)
                i = height;
            else
                i = width;

            Bitmap b = new Bitmap(newSize, newSize);

            try
            {
                Graphics g = Graphics.FromImage(b);
                //设置高质量插值法
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //设置高质量,低速度呈现平滑程度
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                //清除整个绘图面并以透明背景色填充
                g.Clear(Color.Transparent);
                if (width < height)
                    g.DrawImage(image, new Rectangle(0, 0, newSize, newSize), new Rectangle(0, (height - width) / 2, width, width), GraphicsUnit.Pixel);
                else
                    g.DrawImage(image, new Rectangle(0, 0, newSize, newSize), new Rectangle((width - height) / 2, 0, height, height), GraphicsUnit.Pixel);

                SaveImage(b, newFileName, GetCodecInfo("image/" + GetFormat(newFileName).ToString().ToLower()));
            }
            finally
            {
                image.Dispose();
                b.Dispose();
            }
        }

        /// <summary>
        /// 制作小正方形
        /// </summary>
        /// <param name="fileName">图片文件名</param>
        /// <param name="newFileName">新地址</param>
        /// <param name="newSize">长度或宽度</param>
        public static void MakeSquareImage(string fileName, string newFileName, int newSize)
        {
            MakeSquareImage(Image.FromFile(fileName), newFileName, newSize);
        }

        /// <summary>
        /// 制作远程小正方形
        /// </summary>
        /// <param name="url">图片url</param>
        /// <param name="newFileName">新地址</param>
        /// <param name="newSize">长度或宽度</param>
        public static void MakeRemoteSquareImage(string url, string newFileName, int newSize)
        {
            Stream stream = GetRemoteImage(url);
            if (stream == null)
                return;
            Image original = Image.FromStream(stream);
            stream.Close();
            MakeSquareImage(original, newFileName, newSize);
        }

        /// <summary>
        /// 制作缩略图
        /// </summary>
        /// <param name="original">图片对象</param>
        /// <param name="newFileName">新图路径</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="maxHeight">最大高度</param>
        public static void MakeThumbnailImage(Image original, string newFileName, int maxWidth, int maxHeight)
        {
            Size _newSize = ResizeImage(original.Width, original.Height, maxWidth, maxHeight);

            using (Image displayImage = new Bitmap(original, _newSize))
            {
                try
                {
                    if (original.Width > maxWidth)
                    {
                        displayImage.Save(newFileName, original.RawFormat);
                    }
                }
                finally
                {
                    original.Dispose();
                }
            }
        }

        /// <summary>
        /// 制作缩略图
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="newFileName">新图路径</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="maxHeight">最大高度</param>
        public static void MakeThumbnailImage(string fileName, string newFileName, int maxWidth, int maxHeight)
        {
            //2012-02-05修改过，支持替换
            byte[] imageBytes = File.ReadAllBytes(fileName);
            Image img = Image.FromStream(new System.IO.MemoryStream(imageBytes));
            if (img.Width > maxWidth)
            {
                MakeThumbnailImage(img, newFileName, maxWidth, maxHeight);
            }
            //原文
            //MakeThumbnailImage(Image.FromFile(fileName), newFileName, maxWidth, maxHeight);
        }

        #region 2012-2-19 新增生成图片缩略图方法
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="fileName">源图路径（绝对路径）</param>
        /// <param name="newFileName">缩略图路径（绝对路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>    
        public static bool MakeThumbnailImage(string fileName, string newFileName, int width, int height, string mode)
        {
            Image originalImage = Image.FromFile(fileName);
            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            if (ow > towidth)
            {
                switch (mode)
                {
                    case "HW"://指定高宽缩放（可能变形）                
                        break;
                    case "W"://指定宽，高按比例                    
                        toheight = originalImage.Height * width / originalImage.Width;
                        break;
                    case "H"://指定高，宽按比例
                        towidth = originalImage.Width * height / originalImage.Height;
                        break;
                    case "Cut"://指定高宽裁减（不变形）                
                        if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                        {
                            oh = originalImage.Height;
                            ow = originalImage.Height * towidth / toheight;
                            y = 0;
                            x = (originalImage.Width - ow) / 2;
                        }
                        else
                        {
                            ow = originalImage.Width;
                            oh = originalImage.Width * height / towidth;
                            x = 0;
                            y = (originalImage.Height - oh) / 2;
                        }
                        break;
                    default:
                        break;
                }

                //新建一个bmp图片
                Bitmap b = new Bitmap(towidth, toheight);
                try
                {
                    //新建一个画板
                    Graphics g = Graphics.FromImage(b);
                    //设置高质量插值法
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    //设置高质量,低速度呈现平滑程度
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    //清空画布并以透明背景色填充
                    g.Clear(Color.Transparent);
                    //在指定位置并且按指定大小绘制原图片的指定部分
                    g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight), new Rectangle(x, y, ow, oh), GraphicsUnit.Pixel);

                    SaveImage(b, newFileName, GetCodecInfo("image/" + GetFormat(newFileName).ToString().ToLower()));
                }
                catch (System.Exception e)
                {
                    throw e;
                }
                finally
                {
                    originalImage.Dispose();
                    b.Dispose();
                }
                return true;
            }
            else
            {
                originalImage.Dispose();
                return false;
            }
        }
        #endregion

        #region 2012-10-30 新增图片裁剪方法
        /// <summary>
        /// 裁剪图片并保存
        /// </summary>
        /// <param name="fileName">源图路径（绝对路径）</param>
        /// <param name="newFileName">缩略图路径（绝对路径）</param>
        /// <param name="maxWidth">缩略图宽度</param>
        /// <param name="maxHeight">缩略图高度</param>
        /// <param name="cropWidth">裁剪宽度</param>
        /// <param name="cropHeight">裁剪高度</param>
        /// <param name="X">X轴</param>
        /// <param name="Y">Y轴</param>
        public static bool MakeThumbnailImage(string fileName, string newFileName, int maxWidth, int maxHeight, int cropWidth, int cropHeight, int X, int Y)
        {
            byte[] imageBytes = File.ReadAllBytes(fileName);
            Image originalImage = Image.FromStream(new System.IO.MemoryStream(imageBytes));
            Bitmap b = new Bitmap(cropWidth, cropHeight);
            try
            {
                using (Graphics g = Graphics.FromImage(b))
                {
                    //设置高质量插值法
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    //设置高质量,低速度呈现平滑程度
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    //清空画布并以透明背景色填充
                    g.Clear(Color.Transparent);
                    //在指定位置并且按指定大小绘制原图片的指定部分
                    g.DrawImage(originalImage, new Rectangle(0, 0, cropWidth, cropWidth), X, Y, cropWidth, cropHeight, GraphicsUnit.Pixel);
                    Image displayImage = new Bitmap(b, maxWidth, maxHeight);
                    SaveImage(displayImage, newFileName, GetCodecInfo("image/" + GetFormat(newFileName).ToString().ToLower()));
                    return true;
                }
            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                b.Dispose();
            }
        }
        #endregion

        /// <summary>
        /// 制作远程缩略图
        /// </summary>
        /// <param name="url">图片URL</param>
        /// <param name="newFileName">新图路径</param>
        /// <param name="maxWidth">最大宽度</param>
        /// <param name="maxHeight">最大高度</param>
        public static void MakeRemoteThumbnailImage(string url, string newFileName, int maxWidth, int maxHeight)
        {
            Stream stream = GetRemoteImage(url);
            if (stream == null)
                return;
            Image original = Image.FromStream(stream);
            stream.Close();
            MakeThumbnailImage(original, newFileName, maxWidth, maxHeight);
        }

        /// <summary>
        /// 获取图片流
        /// </summary>
        /// <param name="url">图片URL</param>
        /// <returns></returns>
        private static Stream GetRemoteImage(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "GET";
            request.ContentLength = 0;
            request.Timeout = 20000;
            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                return response.GetResponseStream();
            }
            catch
            {
                return null;
            }
        }
    }
    /// <summary>
    /// 水印构造类
    /// </summary>
    public class WaterMark
    {
        /// <summary>
        /// 图片水印
        /// </summary>
        /// <param name="imgPath">服务器图片相对路径</param>
        /// <param name="filename">保存文件名</param>
        /// <param name="watermarkFilename">水印文件相对路径</param>
        /// <param name="watermarkStatus">图片水印位置 0=不使用 1=左上 2=中上 3=右上 4=左中  9=右下</param>
        /// <param name="quality">附加水印图片质量,0-100</param>
        /// <param name="watermarkTransparency">水印的透明度 1--10 10为不透明</param>
        public static void AddImageSignPic(string imgPath, string filename, string watermarkFilename, int watermarkStatus, int quality, int watermarkTransparency)
        {
            if (!File.Exists(Utils.GetMapPath(imgPath)))
                return;
            byte[] _ImageBytes = File.ReadAllBytes(Utils.GetMapPath(imgPath));
            Image img = Image.FromStream(new System.IO.MemoryStream(_ImageBytes));
            filename = Utils.GetMapPath(filename);

            if (watermarkFilename.StartsWith("/") == false)
                watermarkFilename = "/" + watermarkFilename;
            watermarkFilename = Utils.GetMapPath(watermarkFilename);
            if (!File.Exists(watermarkFilename))
                return;
            Graphics g = Graphics.FromImage(img);
            //设置高质量插值法
            //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            Image watermark = new Bitmap(watermarkFilename);

            if (watermark.Height >= img.Height || watermark.Width >= img.Width)
                return;

            ImageAttributes imageAttributes = new ImageAttributes();
            ColorMap colorMap = new ColorMap();

            colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
            colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
            ColorMap[] remapTable = { colorMap };

            imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

            float transparency = 0.5F;
            if (watermarkTransparency >= 1 && watermarkTransparency <= 10)
                transparency = (watermarkTransparency / 10.0F);


            float[][] colorMatrixElements = {
												new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
												new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
												new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
												new float[] {0.0f,  0.0f,  0.0f,  transparency, 0.0f},
												new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
											};

            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            int xpos = 0;
            int ypos = 0;

            switch (watermarkStatus)
            {
                case 1:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 2:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 3:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)(img.Height * (float).01);
                    break;
                case 4:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 5:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 6:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)((img.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 7:
                    xpos = (int)(img.Width * (float).01);
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
                case 8:
                    xpos = (int)((img.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
                case 9:
                    xpos = (int)((img.Width * (float).99) - (watermark.Width));
                    ypos = (int)((img.Height * (float).99) - watermark.Height);
                    break;
            }

            g.DrawImage(watermark, new Rectangle(xpos, ypos, watermark.Width, watermark.Height), 0, 0, watermark.Width, watermark.Height, GraphicsUnit.Pixel, imageAttributes);

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType.IndexOf("jpeg") > -1)
                    ici = codec;
            }
            EncoderParameters encoderParams = new EncoderParameters();
            long[] qualityParam = new long[1];
            if (quality < 0 || quality > 100)
                quality = 80;

            qualityParam[0] = quality;

            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityParam);
            encoderParams.Param[0] = encoderParam;

            if (ici != null)
                img.Save(filename, ici, encoderParams);
            else
                img.Save(filename);

            g.Dispose();
            img.Dispose();
            watermark.Dispose();
            imageAttributes.Dispose();
        }

        /// <summary>
        /// 文字水印
        /// </summary>
        /// <param name="imgPath">服务器图片相对路径</param>
        /// <param name="filename">保存文件名</param>
        /// <param name="watermarkText">水印文字</param>
        /// <param name="watermarkStatus">图片水印位置 0=不使用 1=左上 2=中上 3=右上 4=左中  9=右下</param>
        /// <param name="quality">附加水印图片质量,0-100</param>
        /// <param name="fontname">字体</param>
        /// <param name="fontsize">字体大小</param>
        public static void AddImageSignText(string imgPath, string filename, string watermarkText, int watermarkStatus, int quality, string fontname, int fontsize)
        {
            byte[] _ImageBytes = File.ReadAllBytes(Utils.GetMapPath(imgPath));
            Image img = Image.FromStream(new System.IO.MemoryStream(_ImageBytes));
            filename = Utils.GetMapPath(filename);

            Graphics g = Graphics.FromImage(img);
            Font drawFont = new Font(fontname, fontsize, FontStyle.Regular, GraphicsUnit.Pixel);
            SizeF crSize;
            crSize = g.MeasureString(watermarkText, drawFont);

            float xpos = 0;
            float ypos = 0;

            switch (watermarkStatus)
            {
                case 1:
                    xpos = (float)img.Width * (float).01;
                    ypos = (float)img.Height * (float).01;
                    break;
                case 2:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = (float)img.Height * (float).01;
                    break;
                case 3:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = (float)img.Height * (float).01;
                    break;
                case 4:
                    xpos = (float)img.Width * (float).01;
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 5:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 6:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 7:
                    xpos = (float)img.Width * (float).01;
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
                case 8:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
                case 9:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
            }

            g.DrawString(watermarkText, drawFont, new SolidBrush(Color.White), xpos + 1, ypos + 1);
            g.DrawString(watermarkText, drawFont, new SolidBrush(Color.Yellow), xpos, ypos);

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo ici = null;
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.MimeType.IndexOf("jpeg") > -1)
                    ici = codec;
            }
            EncoderParameters encoderParams = new EncoderParameters();
            long[] qualityParam = new long[1];
            if (quality < 0 || quality > 100)
                quality = 80;

            qualityParam[0] = quality;

            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityParam);
            encoderParams.Param[0] = encoderParam;

            if (ici != null)
                img.Save(filename, ici, encoderParams);
            else
                img.Save(filename);

            g.Dispose();
            img.Dispose();
        }
    }
    #endregion
}