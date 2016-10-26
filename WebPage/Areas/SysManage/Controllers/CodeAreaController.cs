using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Service.IService;
using WebPage.Controllers;

namespace WebPage.Areas.SysManage.Controllers
{
        /// <summary>
        /// 省市县镇级联控制器
        /// </summary>
        public class CodeAreaController : BaseController
        {
            ICodeAreaManage CodeAreaManage { get; set; }
            /// <summary>
            /// 获取省份
            /// </summary>
            public ActionResult Prov()
            {
                var json = new JsonHelper() { Status = "y", Msg = "Success" };
                json.Data = JsonConverter.Serialize(this.CodeAreaManage.LoadListAll(p => p.LEVELS == 1));
                return Json(json);
            }
            /// <summary>
            /// 根据省份获取城市信息
            /// </summary>
            /// <param name="id">省份ID</param>
            /// <returns></returns>
            public ActionResult City(string id)
            {
                var json = new JsonHelper() { Status = "y", Msg = "Success" };
                if (string.IsNullOrEmpty(id))
                {
                    json.Msg = "Error";
                    json.Status = "n";
                }
                else
                {
                    json.Data = JsonConverter.Serialize(this.CodeAreaManage.LoadListAll(p => p.LEVELS == 2 && p.PID == id));
                }
                return Json(json);
            }
            /// <summary>
            /// 根据城市获取县级市信息
            /// </summary>
            /// <param name="id">城市ID</param>
            /// <returns></returns>
            public ActionResult Country(string id)
            {
                var json = new JsonHelper() { Status = "y", Msg = "Success" };
                if (string.IsNullOrEmpty(id))
                {
                    json.Msg = "Error";
                    json.Status = "n";
                }
                else
                {
                    json.Data = JsonConverter.Serialize(this.CodeAreaManage.LoadListAll(p => p.LEVELS == 3 && p.PID == id));
                }
                return Json(json);
            }
        }
}