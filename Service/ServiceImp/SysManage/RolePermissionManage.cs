using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Service.IService;

namespace Service.ServiceImp
{
    /// <summary>
    /// Service层角色授权关系接口
    /// add yuangang by 2015-05-22
    /// </summary>
    public class RolePermissionManage : RepositoryBase<Domain.SYS_ROLE_PERMISSION>, IService.IRolePermissionManage
    {
       /// <summary>
       /// 权限管理
       /// </summary>
        IPermissionManage PermissionManage { get; set; }
        /// <summary>
        /// 保存角色权限
        /// </summary>
        public bool SetRolePermission(int roleId, string newper)
        {
            try
            {
                //1、获取角色权限，是否存在，存在即删除
                if (this.IsExist(p => p.ROLEID == roleId))
                {
                    //2、删除角色权限
                    this.Delete(p => p.ROLEID == roleId);
                }
                //3、添加角色权限
                if (string.IsNullOrEmpty(newper)) return true;
                //Trim 保证数据安全
                var str = newper.Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (str.Count() > 0)
                {
                    foreach (var per in str.Select(t => new Domain.SYS_ROLE_PERMISSION()
                    {
                        PERMISSIONID = int.Parse(t),
                        ROLEID = roleId
                    }))
                    {
                        this._Context.Set<Domain.SYS_ROLE_PERMISSION>().Add(per);
                    }
                }
                else
                {
                    return true;
                }
                //4、Save
                return _Context.SaveChanges() > 0;
            }
            catch (Exception e) { throw e.InnerException; }
        }
    }
}
