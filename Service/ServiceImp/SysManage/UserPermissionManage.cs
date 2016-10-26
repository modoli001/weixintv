using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Service.IService;

namespace Service.ServiceImp
{
    /// <summary>
    /// Service层用户授权接口
    /// add yuangang by 2016-05-19
    /// </summary>
    public class UserPermissionManage : RepositoryBase<Domain.SYS_USER_PERMISSION>,IService.IUserPermissionManage
    {
        IPermissionManage PermissionManage { get; set; }
        /// <summary>
        /// 保存用户权限
        /// </summary>
        public bool SetUserPermission(int userId, string newper)
        {
            try
            {
                //1、获取用户权限，是否存在，存在即删除
                if (this.IsExist(p => p.FK_USERID == userId))
                {
                    //2、删除用户权限
                    this.Delete(p => p.FK_USERID == userId);
                }
                //3、添加用户权限
                var str = newper.Trim(',').Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (str.Count() > 0)
                {
                    foreach (var per in str.Select(t => new Domain.SYS_USER_PERMISSION()
                    {
                        FK_USERID = userId,
                        FK_PERMISSIONID = int.Parse(t)
                    }))
                    {
                        _Context.Set<Domain.SYS_USER_PERMISSION>().Add(per);
                    }
                }
                else
                {
                    return true;
                }
                //4、Save
                return _Context.SaveChanges() > 0;
            }
            catch (Exception e) { throw e; }
        }
    }
}
