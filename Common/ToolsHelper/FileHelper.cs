using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Configuration;

namespace Common
{
    /// <summary>
    /// 文件帮助类
    /// add yuangang by 2015-06-11
    /// </summary>
    public class FileHelper
    {
        #region 获取文件到集合中
        /// <summary>
        /// 读取指定位置文件列表到集合中
        /// </summary>
        /// <param name="Path">指定路径</param>
        /// <returns></returns>
        public static DataTable GetFileTable(string Path)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("name", typeof(string));
            dt.Columns.Add("ext", typeof(string));
            dt.Columns.Add("size", typeof(string));
            dt.Columns.Add("icon", typeof(string));
            dt.Columns.Add("isfolder", typeof(bool));
            dt.Columns.Add("isImage", typeof(bool));
            dt.Columns.Add("fullname", typeof(string));
            dt.Columns.Add("path", typeof(string));
            dt.Columns.Add("time", typeof(DateTime));

            DirectoryInfo dirinfo = new DirectoryInfo(Path);
            FileInfo fi;
            DirectoryInfo dir;
            string FileName = string.Empty, FileExt = string.Empty, FileSize = string.Empty, FileIcon = string.Empty, FileFullName = string.Empty, FilePath = string.Empty;
            bool IsFloder = false, IsImage = false;
            DateTime FileModify;
            try
            {
                foreach (FileSystemInfo fsi in dirinfo.GetFileSystemInfos())
                {
                    if (fsi is FileInfo)
                    {
                        fi = (FileInfo)fsi;
                        //获取文件名称
                        FileName = fi.Name.Substring(0, fi.Name.LastIndexOf('.'));
                        FileFullName = fi.Name;
                        //获取文件扩展名
                        FileExt = fi.Extension;
                        //获取文件大小
                        FileSize = GetDiyFileSize(fi);
                        //获取文件最后修改时间
                        FileModify = fi.LastWriteTime;
                        //文件图标
                        FileIcon = GetFileIcon(FileExt);
                        //是否为图片
                        IsImage = IsImageFile(FileExt.Substring(1, FileExt.Length - 1));
                        //文件路径
                        FilePath = urlconvertor(fi.FullName);
                    }
                    else
                    {
                        dir = (DirectoryInfo)fsi;
                        //获取目录名
                        FileName = dir.Name;
                        //获取目录最后修改时间
                        FileModify = dir.LastWriteTime;
                        //设置目录文件为文件夹
                        FileExt = "folder";
                        //文件夹图标
                        FileIcon = "fa fa-folder";
                        IsFloder = true;
                        //文件路径
                        FilePath = urlconvertor(dir.FullName);

                    }
                    DataRow dr = dt.NewRow();
                    dr["name"] = FileName;
                    dr["fullname"] = FileFullName;
                    dr["ext"] = FileExt;
                    dr["size"] = FileSize;
                    dr["time"] = FileModify;
                    dr["icon"] = FileIcon;
                    dr["isfolder"] = IsFloder;
                    dr["isImage"] = IsImage;
                    dr["path"] = FilePath;
                    dt.Rows.Add(dr);
                }
            }
            catch (Exception e)
            {

                throw e;
            }
            return dt;
        }
        /// <summary>
        /// 获取目录下所有文件（包含子目录）
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>

        public static DataTable GetAllFileTable(string Path)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("name", typeof(string));
            dt.Columns.Add("ext", typeof(string));
            dt.Columns.Add("size", typeof(string));
            dt.Columns.Add("icon", typeof(string));
            dt.Columns.Add("isfolder", typeof(bool));
            dt.Columns.Add("isImage", typeof(bool));
            dt.Columns.Add("fullname", typeof(string));
            dt.Columns.Add("path", typeof(string));
            dt.Columns.Add("time", typeof(DateTime));

            string[] folders = Directory.GetDirectories(Path, "*", SearchOption.AllDirectories);

            List<string> Listfloders = new List<string>() { Path };

            if (folders != null && folders.Count() > 0)
            {
                foreach (var folder in folders)
                {
                    Listfloders.Add(folder);
                }
            }

            foreach (var f in Listfloders)
            {
                DirectoryInfo dirinfo = new DirectoryInfo(f);
                FileInfo fi;
                string FileName = string.Empty, FileExt = string.Empty, FileSize = string.Empty, FileIcon = string.Empty, FileFullName = string.Empty, FilePath = string.Empty;
                bool IsFloder = false, IsImage = false;
                DateTime FileModify;
                try
                {
                    foreach (FileSystemInfo fsi in dirinfo.GetFiles())
                    {

                        fi = (FileInfo)fsi;
                        //获取文件名称
                        FileName = fi.Name.Substring(0, fi.Name.LastIndexOf('.'));
                        FileFullName = fi.Name;
                        //获取文件扩展名
                        FileExt = fi.Extension.ToLower();
                        //获取文件大小
                        FileSize = GetDiyFileSize(fi);
                        //获取文件最后修改时间
                        FileModify = fi.LastWriteTime;
                        //文件图标
                        FileIcon = GetFileIcon(FileExt);
                        //是否为图片
                        IsImage = IsImageFile(FileExt.Substring(1, FileExt.Length - 1));
                        //文件路径
                        FilePath = urlconvertor(fi.FullName);

                        DataRow dr = dt.NewRow();
                        dr["name"] = FileName;
                        dr["fullname"] = FileFullName;
                        dr["ext"] = FileExt;
                        dr["size"] = FileSize;
                        dr["time"] = FileModify;
                        dr["icon"] = FileIcon;
                        dr["isfolder"] = IsFloder;
                        dr["isImage"] = IsImage;
                        dr["path"] = FilePath;
                        dt.Rows.Add(dr);
                    }
                }
                catch (Exception e)
                {

                    throw e;
                }
            }

            return dt;
        }

        #endregion

        #region 检测指定路径是否存在
        /// <summary>
        /// 检测指定路径是否存在
        /// </summary>
        /// <param name="Path">目录的绝对路径</param> 
        public static bool IsExistDirectory(string Path)
        {
            return Directory.Exists(Path);
        }
        #endregion

        #region 检测指定文件是否存在,如果存在则返回true
        /// <summary>
        /// 检测指定文件是否存在,如果存在则返回true
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>  
        public static bool IsExistFile(string filePath)
        {
            return File.Exists(filePath);
        }
        #endregion

        #region 创建文件夹
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="FolderPath">文件夹的绝对路径</param>
        public static void CreateFolder(string FolderPath)
        {
            if (!IsExistDirectory(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
        }
        #endregion

        #region 判断上传文件后缀名
        /// <summary>
        /// 判断上传文件后缀名
        /// </summary>
        /// <param name="strExtension">后缀名</param>
        public static bool IsCanEdit(string strExtension)
        {
            strExtension = strExtension.ToLower();
            if (strExtension.LastIndexOf(".") >= 0)
            {
                strExtension = strExtension.Substring(strExtension.LastIndexOf("."));
            }
            else
            {
                strExtension = ".txt";
            }
            string[] strArray = new string[] { ".htm", ".html", ".txt", ".js", ".css", ".xml", ".sitemap" };
            for (int i = 0; i < strArray.Length; i++)
            {
                if (strExtension.Equals(strArray[i]))
                {
                    return true;
                }
            }
            return false;
        }


        public static bool IsSafeName(string strExtension)
        {
            strExtension = strExtension.ToLower();
            if (strExtension.LastIndexOf(".") >= 0)
            {
                strExtension = strExtension.Substring(strExtension.LastIndexOf("."));
            }
            else
            {
                strExtension = ".txt";
            }
            string[] strArray = new string[] { ".jpg", ".gif", ".png" };
            for (int i = 0; i < strArray.Length; i++)
            {
                if (strExtension.Equals(strArray[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsZipName(string strExtension)
        {
            strExtension = strExtension.ToLower();
            if (strExtension.LastIndexOf(".") >= 0)
            {
                strExtension = strExtension.Substring(strExtension.LastIndexOf("."));
            }
            else
            {
                strExtension = ".txt";
            }
            string[] strArray = new string[] { ".zip", ".rar" };
            for (int i = 0; i < strArray.Length; i++)
            {
                if (strExtension.Equals(strArray[i]))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 创建文件夹
        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="fileName">文件的绝对路径</param>
        public static void CreateSuffic(string filename)
        {
            try
            {
                if (!Directory.Exists(filename))
                {
                    Directory.CreateDirectory(filename);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="fileName">文件的绝对路径</param>
        public static void CreateFiles(string fileName)
        {
            try
            {
                //判断文件是否存在，不存在创建该文件
                if (!IsExistFile(fileName))
                {
                    FileInfo file = new FileInfo(fileName);
                    FileStream fs = file.Create();
                    fs.Close();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// 创建一个文件,并将字节流写入文件。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="buffer">二进制流数据</param>
        public static void CreateFile(string filePath, byte[] buffer)
        {
            try
            {
                //判断文件是否存在，不存在创建该文件
                if (!IsExistFile(filePath))
                {
                    FileInfo file = new FileInfo(filePath);
                    FileStream fs = file.Create();
                    fs.Write(buffer, 0, buffer.Length);
                    fs.Close();
                }
                else
                {
                    File.WriteAllBytes(filePath, buffer);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region 将文件移动到指定目录
        /// <summary>
        /// 将文件移动到指定目录
        /// </summary>
        /// <param name="sourceFilePath">需要移动的源文件的绝对路径</param>
        /// <param name="descDirectoryPath">移动到的目录的绝对路径</param>
        public static void Move(string sourceFilePath, string descDirectoryPath)
        {
            string sourceName = GetFileName(sourceFilePath);
            if (IsExistDirectory(descDirectoryPath))
            {
                //如果目标中存在同名文件,则删除
                if (IsExistFile(descDirectoryPath + "\\" + sourceName))
                {
                    DeleteFile(descDirectoryPath + "\\" + sourceName);
                }
                else
                {
                    //将文件移动到指定目录
                    File.Move(sourceFilePath, descDirectoryPath + "\\" + sourceName);
                }
            }
        }
        #endregion

        /// <summary>
        /// 将源文件的内容复制到目标文件中
        /// </summary>
        /// <param name="sourceFilePath">源文件的绝对路径</param>
        /// <param name="descDirectoryPath">目标文件的绝对路径</param>
        public static void Copy(string sourceFilePath, string descDirectoryPath)
        {
            File.Copy(sourceFilePath, descDirectoryPath, true);
        }

        /// <summary>
        /// 从文件的绝对路径中获取文件名
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param> 
        public static string GetFileName(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            return file.Name;
        }

        /// <summary>
        /// 获取文件的后缀名
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static string GetExtension(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            return file.Extension;
        }


        /// <summary>
        /// 删除指定文件
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static void DeleteFile(string filePath)
        {
            if (IsExistFile(filePath))
            {
                File.Delete(filePath);
            }
        }
        /// <summary>
        /// 删除指定目录及其所有子目录
        /// </summary>
        /// <param name="directoryPath">文件的绝对路径</param>
        public static void DeleteDirectory(string directoryPath)
        {
            if (IsExistDirectory(directoryPath))
            {
                Directory.Delete(directoryPath);
            }
        }

        /// <summary>
        /// 清空指定目录下所有文件及子目录,但该目录依然保存.
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        public static void ClearDirectory(string directoryPath)
        {
            if (IsExistDirectory(directoryPath))
            {
                //删除目录中所有的文件
                string[] fileNames = GetFileNames(directoryPath);
                for (int i = 0; i < fileNames.Length; i++)
                {
                    DeleteFile(fileNames[i]);
                }
                //删除目录中所有的子目录
                string[] directoryNames = GetDirectories(directoryPath);
                for (int i = 0; i < directoryNames.Length; i++)
                {
                    DeleteDirectory(directoryNames[i]);
                }
            }
        }

        #region  剪切  粘贴
        /// <summary>
        /// 剪切文件
        /// </summary>
        /// <param name="source">原路径</param> 
        /// <param name="destination">新路径</param> 
        public bool FileMove(string source, string destination)
        {
            bool ret = false;
            FileInfo file_s = new FileInfo(source);
            FileInfo file_d = new FileInfo(destination);
            if (file_s.Exists)
            {
                if (!file_d.Exists)
                {
                    file_s.MoveTo(destination);
                    ret = true;
                }
            }
            if (ret == true)
            {
                //Response.Write("<script>alert('剪切文件成功！');</script>");
            }
            else
            {
                //Response.Write("<script>alert('剪切文件失败！');</script>");
            }
            return ret;
        }
        #endregion

        /// <summary>
        /// 检测指定目录是否为空
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>  
        public static bool IsEmptyDirectory(string directoryPath)
        {
            try
            {
                //判断文件是否存在
                string[] fileNames = GetFileNames(directoryPath);
                if (fileNames.Length > 0)
                {
                    return false;
                }
                //判断是否存在文件夹
                string[] directoryNames = GetDirectories(directoryPath);
                if (directoryNames.Length > 0)
                {
                    return false;
                }
                return true;
            }
            catch //(Exception ex)
            {

                return true;
            }
        }

        #region 获取指定目录中所有文件列表
        /// <summary>
        /// 获取指定目录中所有文件列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>  
        public static string[] GetFileNames(string directoryPath)
        {
            if (!IsExistDirectory(directoryPath))
            {
                throw new FileNotFoundException();
            }
            return Directory.GetFiles(directoryPath);
        }
        #endregion

        #region 获取指定目录中的子目录列表
        /// <summary>
        /// 获取指定目录中所有子目录列表,若要搜索嵌套的子目录列表,请使用重载方法
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        public static string[] GetDirectories(string directoryPath)
        {
            try
            {
                return Directory.GetDirectories(directoryPath);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// 获取指定目录及子目录中所有子目录列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param>
        /// <param name="isSearchChild">是否搜索子目录</param>
        public static string[] GetDirectories(string directoryPath, string searchPattern, bool isSearchChild)
        {
            try
            {
                if (isSearchChild)
                {
                    return Directory.GetDirectories(directoryPath, searchPattern, SearchOption.AllDirectories);
                }
                else
                {
                    return Directory.GetDirectories(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region 获取一个文件的长度
        /// <summary>
        /// 获取文件长度，返回字符串 byte kb mb gb
        /// </summary>
        public static string GetDiyFileSize(FileInfo fi)
        {
            string FileSize = string.Empty;
            //获取文件大小
            if (fi.Length < 1024)//小于1KB
            {
                FileSize = fi.Length.ToString() + "byte";
            }
            else if (fi.Length > 1024 && fi.Length < 1024 * 1024)//大于1KB小于1MB
            {
                FileSize = Math.Round(GetFileSizeByKB(fi.Length), 2).ToString() + "KB";
            }
            else if (fi.Length > 1024 * 1024 && fi.Length < 1024 * 1024 * 1024)//大于1MB小于1GB
            {
                FileSize = Math.Round(GetFileSizeByMB(fi.Length), 2).ToString() + "MB";
            }
            else //GB
            {
                FileSize = Math.Round(GetFileSizeByGB(fi.Length), 2).ToString() + "GB";
            }
            return FileSize;
        }
        public static decimal GetDiyFileSize(long Length, out string unit)
        {
            decimal FileSize = 0m;
            unit = string.Empty;
            //获取文件大小
            if (Length < 1024)//小于1KB
            {
                FileSize = Length;
                unit = "byte";
            }
            else if (Length > 1024 && Length < 1024 * 1024)//大于1KB小于1MB
            {
                FileSize = Math.Round(GetFileSizeByKB(Length), 2);
                unit = "KB";
            }
            else if (Length > 1024 * 1024 && Length < 1024 * 1024 * 1024)//大于1MB小于1GB
            {
                FileSize = Math.Round(GetFileSizeByMB(Length), 2);
                unit = "MB";
            }
            else //GB
            {
                FileSize = Math.Round(GetFileSizeByGB(Length), 2);
                unit = "GB";
            }
            return FileSize;
        }
        /// <summary> 
        /// 获取一个文件的长度,单位为Byte 
        /// </summary> 
        /// <param name="filePath">文件的绝对路径</param>         
        public static long GetFileSize(string filePath)
        {
            //创建一个文件对象 
            FileInfo fi = new FileInfo(filePath);
            //获取文件的大小 
            return fi.Length;
        }
        /// <summary> 
        /// 获取一个文件的长度,单位为KB 
        /// </summary> 
        /// <param name="filelength">byte</param>         
        public static decimal GetFileSizeByKB(long filelength)
        {
            return Convert.ToDecimal(filelength) / 1024;
        }
        /// <summary> 
        /// 获取一个文件的长度,单位为MB 
        /// </summary> 
        /// <param name="filePath">byte</param>         
        public static decimal GetFileSizeByMB(long filelength)
        {
            //获取文件的大小 
            return Convert.ToDecimal(Convert.ToDecimal(filelength / 1024) / 1024);
        }
        /// <summary> 
        /// 获取一个文件的长度,单位为GB
        /// </summary> 
        /// <param name="filePath">byte</param>        
        public static decimal GetFileSizeByGB(long filelength)
        {
            //获取文件的大小 
            return Convert.ToDecimal(Convert.ToDecimal(Convert.ToDecimal(filelength / 1024) / 1024) / 1024);
        }
        #endregion

        //本地路径转换成URL相对路径
        private static string urlconvertor(string Url)
        {
            string tmpRootDir = System.Web.HttpContext.Current.Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath.ToString());//获取程序根目录
            string converUrl = Url.Replace(tmpRootDir, ""); //转换成相对路径
            converUrl = converUrl.Replace(@"\", @"/");
            return "/" + converUrl;
        }

        #region 文件图标
        public static string GetFileIcon(string _fileExt)
        {
            var images = ConfigurationManager.AppSettings["Image"].Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => p).ToList();
            var videos = ConfigurationManager.AppSettings["Video"].Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => p).ToList();
            var musics = ConfigurationManager.AppSettings["Music"].Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => p).ToList();
            //图片文件
            if (images.Contains(_fileExt.ToLower().Remove(0, 1)))
            {
                return "fa fa-image";
            }
            //视频文件
            else if (videos.Contains(_fileExt.ToLower().Remove(0, 1)))
            {
                return "fa fa-film";
            }
            //音频文件
            else if (musics.Contains(_fileExt.ToLower().Remove(0, 1)))
            {
                return "fa fa-music";
            }
            else
            {
                switch (_fileExt.ToLower())
                {
                    case ".doc":
                    case ".docx":
                        return "fa fa-file-word-o";
                    case ".xls":
                    case ".xlsx":
                        return "fa fa-file-excel-o";
                    case ".ppt":
                    case ".pptx":
                        return "fa fa-file-powerpoint-o";
                    case ".pdf":
                        return "fa fa-file-pdf-o";
                    case ".txt":
                        return "fa fa-file-text-o";
                    case ".zip":
                    case ".rar":
                        return "fa fa-file-zip-o";
                    default:
                        return "fa fa-file";
                }
            }
        }
        #endregion

        #region 文件格式
        /// <summary>
        /// 是否为图片
        /// </summary>
        /// <param name="_fileExt">文件扩展名，不含“.”</param>
        /// <returns></returns>
        private static bool IsImageFile(string _fileExt)
        {
            var images = ConfigurationManager.AppSettings["Image"].Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => p).ToList();
            if (images.Contains(_fileExt.ToLower())) return true;
            return false;
        }
        #endregion
    }
}
