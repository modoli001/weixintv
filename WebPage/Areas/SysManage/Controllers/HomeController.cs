using Common;
using Service.IService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebPage.Controllers;

namespace WebPage.Areas.SysManage.Controllers
{
    public class HomeController : BaseController
    {
        #region 声明容器
        /// <summary>
        /// 模块管理
        /// </summary>
        IModuleManage ModuleManage { get; set; }
        /// <summary>
        /// 部门管理
        /// </summary>
        IDepartmentManage DepartmentManage { get; set; }
        /// <summary>
        /// 用户在线管理
        /// </summary>
        IUserOnlineManage UserOnlineManage { get; set; }
        #endregion

        #region 基本视图
        public ActionResult Index()
        {
            //获取系统模块列表（如果用BUI可以写个方法输出Json给BUI）
            ViewData["Module"] = ModuleManage.GetModule(this.CurrentUser.Id, this.CurrentUser.Permissions, this.CurrentUser.System_Id);          

            //通讯录
            ViewData["Contacts"] = Contacts();

            return View(this.CurrentUser);
        }

        public ActionResult Default()
        {           
            return View(this.CurrentUser);
        }

        #endregion


        #region 帮助方法
        /// <summary>
        /// 通讯录
        /// </summary>
        /// <returns></returns>
        private object Contacts()
        {

            var Departs = DepartmentManage.LoadAll(m => m.BUSINESSLEVEL == 1).OrderBy(m => m.SHOWORDER).ToList().Select(m => new
            {
                m.ID,
                DepartName = m.NAME,
                UserList = GetDepartUsers(m.ID)
            });

            return JsonConverter.JsonClass(Departs);
        }
        private object GetDepartUsers(string departId)
        {
            var departs = DepartmentManage.LoadAll(p => p.PARENTID == departId).OrderBy(p => p.SHOWORDER).Select(p => p.ID).ToList();
            var UsersList = UserManage.LoadListAll(p => p.ID != CurrentUser.Id && departs.Any(e => e == p.DPTID)).OrderBy(p => p.LEVELS).OrderBy(p => p.CREATEDATE).Select(p => new
            {
                p.ID,
                FaceImg = string.IsNullOrEmpty(p.FACE_IMG) ? "/Sys/User/User_Default_Avatat?name=" + p.NAME.Substring(0, 1) : p.FACE_IMG,
                p.NAME,
                InsideEmail = p.ACCOUNT + EmailDomain,
                p.LEVELS,
                ConnectId = UserOnlineManage.LoadAll(m => m.FK_UserId == p.ID).FirstOrDefault() == null ? "" : UserOnlineManage.LoadAll(m => m.FK_UserId == p.ID).FirstOrDefault().ConnectId,
                IsOnline = UserOnlineManage.LoadAll(m => m.FK_UserId == p.ID).FirstOrDefault() == null ? false : UserOnlineManage.LoadAll(m => m.FK_UserId == p.ID).FirstOrDefault().IsOnline
            }).ToList();

            return UsersList.OrderBy(p => p.IsOnline).ToList();
        }
        #endregion

    }
}