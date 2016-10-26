using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Models
{
    public class CommonHelper
    {
        #region 声明容器
        /// <summary>
        /// 省市区级联管理
        /// </summary>
        public ICodeAreaManage CodeAreaManage = Spring.Context.Support.ContextRegistry.GetContext().GetObject("Service.CodeArea") as ICodeAreaManage;
        /// <summary>
        /// 编码管理
        /// </summary>
        public ICodeManage CodeManage = Spring.Context.Support.ContextRegistry.GetContext().GetObject("Service.Code") as ICodeManage;
        /// <summary>
        /// 用户管理
        /// </summary>
        public IUserManage UserManage = Spring.Context.Support.ContextRegistry.GetContext().GetObject("Service.User") as IUserManage;
        /// <summary>
        /// 大数据字段管理
        /// </summary>
        public IContentManage ContentManage = Spring.Context.Support.ContextRegistry.GetContext().GetObject("Service.Com.Content") as IContentManage;
        #endregion

        #region 获取右侧导航
        public  MvcHtmlString GetModuleMenu(Domain.SYS_MODULE module,List<Domain.SYS_MODULE> moduleList)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder(15000);
            var SecondModule = moduleList.FindAll(p => p.PARENTID == module.ID && p.LEVELS == 1).OrderBy(p => p.SHOWORDER).ToList();

            if (SecondModule != null && SecondModule.Count > 0)
            {                
                foreach (var item in SecondModule)
                {
                    str.Append("<li data-id=\"" + module.ALIAS + "\" class=\"FirstModule\" >");
                    str.Append("<a class=\"" + (ChildModuleList(item, moduleList, str, false) ? "" : "J_menuItem") + "\" href=\"" + (string.IsNullOrEmpty(item.MODULEPATH) ? "javascript:void(0)" : item.MODULEPATH) + "\" ><i class=\"" + item.ICON + "\"></i> <span class=\"nav-label\">" + item.NAME + "</span>" + (ChildModuleList(item, moduleList, str, false) ? "<span class=\"fa arrow\"></span>" : "") + "</a>");
                    ChildModuleList(item, moduleList, str, true);
                    str.Append("</li>");
                }                
            }

            return new MvcHtmlString(str.ToString());
           
        }
        private bool ChildModuleList(Domain.SYS_MODULE module, List<Domain.SYS_MODULE> moduleList, System.Text.StringBuilder str,bool IsAppend)
        {           
            var ChildModule = moduleList.FindAll(p => p.PARENTID == module.ID).OrderBy(p => p.SHOWORDER).ToList();

            if (ChildModule != null && ChildModule.Count > 0)
            {
                if (IsAppend)
                {
                    str.Append("<ul class=\"nav nav-second-level\">");
                    foreach (var item in ChildModule)
                    {
                        str.Append("<li>");
                        str.Append("<a class=\"" + (ChildModuleList(item, moduleList, str, false) ? "" : "J_menuItem") + "\" href=\"" + (string.IsNullOrEmpty(item.MODULEPATH) ? "javascript:void(0)" : item.MODULEPATH) + "\" ><i class=\"" + item.ICON + "\"></i> <span class=\"nav-label\">" + item.NAME + "</span>" + (ChildModuleList(item, moduleList, str, false) ? "<span class=\"fa arrow\"></span>" : "") + "</a>");
                        ChildModuleList(item, moduleList, str, true);
                        str.Append("</li>");
                    }
                    str.Append("</ul>");
                }
                return true;
            }
            return false;
        }
        #endregion

        /// <summary>
        /// 获取用户职务
        /// </summary>
        /// <param name="levels"></param>
        /// <returns></returns>
        public string GetUserZW(string levels)
        {
            return CodeManage.Get(m => m.CODEVALUE == levels && m.CODETYPE == "ZW").NAMETEXT;
        }

        /// <summary>
        /// 根据代码获取省区市名称
        /// </summary>
        /// <param name="codearealist">省,市,区</param>
        /// <returns></returns>
        public  string GetCodeAreaName(string codearealist)
        {
            if(!string.IsNullOrEmpty(codearealist))
            {
                var arealist = codearealist.Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(p => p).ToList();
                if(arealist!=null && arealist.Count>0)
                {
                    var strArea = string.Empty;
                    foreach(var item in arealist)
                    {
                        strArea += CodeAreaManage.Get(p => p.ID == item).NAME + "&nbsp;";
                    }
                    return strArea;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return  string.Empty;
            }
        }
       
        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Domain.SYS_USER GetUserById(int id)
        {
            return UserManage.Get(p => p.ID == id);
        }
        /// <summary>
        /// 根据用户账号获取用户姓名
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetUserNameByAccount(string account)
        {
            return UserManage.Get(p => p.ACCOUNT == account) == null ? "" : UserManage.Get(p => p.ACCOUNT == account).NAME;
        }
        /// <summary>
        /// 根据用户账号获取用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Domain.SYS_USER GetUserByAccount(string account)
        {
            return UserManage.Get(p => p.ACCOUNT == account);
        }
       
        /// <summary>
        /// 获取大数据字段文本值
        /// </summary>
        /// <param name="FK_RELATIONID"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public string GetContentText(string FK_RELATIONID,string TableName)
        {
            return ContentManage.Get(p => p.FK_RELATIONID == FK_RELATIONID && p.FK_TABLE == TableName) == null ? "" : Common.Utils.DropHTML(ContentManage.Get(p => p.FK_RELATIONID == FK_RELATIONID && p.FK_TABLE == TableName).CONTENT);
        }
       

    }
}