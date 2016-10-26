using Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.ServiceImp
{
    /// <summary>
    /// Service模型处理类
    /// add yuangang by 2015-05-22
    /// </summary>
    public class ModuleManage : RepositoryBase<Domain.SYS_MODULE>, IService.IModuleManage
    {
        #region 声明容器
        /// <summary>
        /// 权限
        /// </summary>
        IPermissionManage PermissionManage { get; set; }
        /// <summary>
        /// 用户权限
        /// </summary>
        IUserPermissionManage UserPermissionManage { get; set; }
        /// <summary>
        /// 角色权限
        /// </summary>
        IRolePermissionManage RolePermissionManage { get; set; }
        #endregion

        /// <summary>
        /// 获取用户权限模块集合
        /// add yuangang by 2015-05-30
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="permission">用户授权集合</param>
        /// <param name="siteId">站点ID</param>
        /// <returns></returns>
        public List<Domain.SYS_MODULE> GetModule(int userId, List<Domain.SYS_PERMISSION> permission,List<string> systemid)
        {
            //返回模块
            var retmodule = new List<Domain.SYS_MODULE>();
            var permodule = new List<Domain.SYS_MODULE>();
            //权限转模块
            if (permission != null)
            {
                permodule.AddRange(permission.Select(p => p.SYS_MODULE));
                //去重
                permodule = permodule.Distinct(new ModuleDistinct()).ToList();
            }
            //检索显示与系统
            permodule = permodule.Where(p => p.ISSHOW && systemid.Any(e => e == p.FK_BELONGSYSTEM)).ToList();
            //构造上级导航模块
            var prevModule = this.LoadListAll(p => systemid.Any(e => e == p.FK_BELONGSYSTEM));
            //反向递归算法构造模块带上级上上级模块
            if (permodule.Count > 0)
            {
                foreach (var item in permodule)
                {
                    RecursiveModule(prevModule, retmodule, item.PARENTID);
                    retmodule.Add(item);
                }
            }
            //去重
            retmodule = retmodule.Distinct(new ModuleDistinct()).ToList();
            //返回模块集合
            return retmodule.OrderBy(p => p.LEVELS).ThenBy(p => p.SHOWORDER).ToList();
        }

        /// <summary>
        /// 反向递归模块集合，可重复模块数据，最后去重
        /// </summary>
        /// <param name="PrevModule">总模块</param>
        /// <param name="retmodule">返回模块</param>
        /// <param name="parentId">上级ID</param>
        private void RecursiveModule(List<Domain.SYS_MODULE> PrevModule, List<Domain.SYS_MODULE> retmodule, int? parentId)
        {
           var result = PrevModule.Where(p => p.ID == parentId);
            if (result != null)
            {
                foreach (var item in result)
                {
                    retmodule.Add(item);
                    RecursiveModule(PrevModule, retmodule, item.PARENTID);
                }
            }
        }

        /// <summary>
        /// 递归模块列表，返回按级别排序
        /// add yuangang by 2015-06-03
        /// </summary>
        public List<Domain.SYS_MODULE> RecursiveModule(List<Domain.SYS_MODULE> list)
        {
            List<Domain.SYS_MODULE> result = new List<Domain.SYS_MODULE>();
            if (list!=null && list.Count>0)
            {
                ChildModule(list, result, 0);
            }
            return result;
        }
        /// <summary>
        /// 递归模块列表
        /// add yuangang by 2015-06-03
        /// </summary>
        private void ChildModule(List<Domain.SYS_MODULE> list, List<Domain.SYS_MODULE> newlist, int parentId)
        {
            var result = list.Where(p => p.PARENTID == parentId).OrderBy(p => p.LEVELS).OrderBy(p => p.SHOWORDER).ToList();
            if (result.Count() > 0)
            {
                for (int i = 0; i < result.Count(); i++)
                {
                    newlist.Add(result[i]);
                    ChildModule(list, newlist, result[i].ID);
                }
            }
        }

        /// <summary>
        /// 批量变更下级模块的级别
        /// </summary>
        public bool MoreModifyModule(int moduleId, int levels)
        {
           //根据当前模块ID获取下级模块的集合
            var ChildModule = this.LoadAll(p => p.PARENTID == moduleId).ToList();
            if (ChildModule.Any())
            {
                foreach (var item in ChildModule)
                {
                    item.LEVELS = levels + 1;
                    this.Update(item);
                    MoreModifyModule(item.ID, item.LEVELS);
                }
            }
            return true;
        }

        /// <summary>
        /// 获取模板列表
        /// </summary>
        public dynamic LoadModuleInfo(int id)
        {
            return Common.JsonConverter.JsonClass(this.LoadAll(p=>p.PARENTID==id).OrderBy(p => p.ID).Select(p => new { p.ID, p.NAME }).ToList());
        }

        /// <summary>
        /// 删除模块，同时删除下级模块，相关权限、用户权限、角色权限
        /// add huafg by 2016-05-18
        /// </summary>
        /// <param name="idlist">模块ID集合</param>
        /// <returns>删除成功失败</returns>
        public bool Remove(List<string> moduleId)
        {
            bool bl = false;
            try
            {
                //1、查询要删除的模块集合
                var temp_Module = this.LoadAll(p => moduleId.Contains(p.ID.ToString())).ToList();
                //2、递归当前级与下级
                var moduleIDList = RecursiveChildModule(temp_Module).Select(p => p.ID).ToList();
                //3、通过要删除的模块ID获取要删除的权限ID集合
                var permissionIDList = this.PermissionManage.LoadAll(p => moduleIDList.Contains(p.MODULEID)).Select(p => p.ID).ToList();
                //4、通过要删除的权限集合ID获取 角色权限关系
                var rolePerIdList = this.RolePermissionManage.LoadAll(p => permissionIDList.Contains(p.PERMISSIONID)).Select(p => p.ID).ToList();
                //5、通过要删除的权限集合ID获取 用户权限关系
                var userPerIdList = this.UserPermissionManage.LoadAll(p => permissionIDList.Contains(p.FK_PERMISSIONID)).Select(p => p.ID).ToList();

                //6、开启事务进行顺序删除
                using (var transaction = new System.Transactions.TransactionScope())
                {
                    //1、删除角色权限关系
                    this.RolePermissionManage.Delete(p => rolePerIdList.Contains(p.ID));
                    //2、删除用户权限关系
                    this.UserPermissionManage.Delete(p => userPerIdList.Contains(p.ID));
                    //3、删除权限
                    this.PermissionManage.Delete(p => permissionIDList.Contains(p.ID));
                    //4、删除模块及子模块，N级递归
                    this.Delete(p => moduleIDList.Contains(p.ID));

                    bl = true;
                    transaction.Complete();
                }
            }
            catch (Exception e) { throw e; }
            return bl;
        }

        /// <summary>
        /// 根据当前模块集合获取当前级与下级
        /// add yuangang by 2016-05-18
        /// </summary>
        /// <param name="list">当前模块集合</param>
        /// <returns>递归后的当前级与下级的模块集合</returns>
        public List<Domain.SYS_MODULE> RecursiveChildModule(List<Domain.SYS_MODULE> list)
        {
            try
            {
                var retModule = list;
                var listId = list.Select(p => p.ID).ToList();
                //查找下级模块
                var child = this.LoadAll(p => listId.Contains(p.PARENTID)).ToList();
                if (child.Count > 0)
                {
                    retModule.AddRange(RecursiveChildModule(child));
                }
                return retModule;
            }
            catch
            {
                return null;
            }
        }
    }
  /// <summary>
    /// 模型去重，非常重要
    /// add yuangang by 2015-08-03
    /// </summary>
    public class ModuleDistinct : IEqualityComparer<Domain.SYS_MODULE> 
    {
        public bool Equals(Domain.SYS_MODULE x, Domain.SYS_MODULE y)
        {
            return x.ID == y.ID; 
        }

        public int GetHashCode(Domain.SYS_MODULE obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}
