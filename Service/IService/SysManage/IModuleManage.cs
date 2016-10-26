using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.IService
{
    /// <summary>
    /// Service模型处理接口
    /// add yuangang by 2015-05-22
    /// </summary>
    public interface IModuleManage : IRepository<Domain.SYS_MODULE>
    {
        /// <summary>
        /// 获取用户权限模块集合
        /// add yuangang by 2015-05-30
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="permission">用户授权集合</param>
        /// <param name="siteId">站点ID</param>
        /// <returns></returns>
        List<Domain.SYS_MODULE> GetModule(int userId, List<Domain.SYS_PERMISSION> permission, List<string> systemid);
        /// <summary>
        /// 递归模块列表，返回按级别排序
        /// add yuangang by 2015-06-03
        /// </summary>
        List<Domain.SYS_MODULE> RecursiveModule(List<Domain.SYS_MODULE> list);

        /// <summary>
        /// 批量变更当前模块下其他模块的级别
        /// </summary>
        bool MoreModifyModule(int moduleId, int levels);
         /// <summary>
        /// 删除模块，同时删除下级模块，相关权限、用户权限、角色权限
        /// add yuangang by 2016-05-18
        /// </summary>
        /// <param name="idlist">模块ID集合</param>
        /// <returns>删除成功失败</returns>
        bool Remove(List<string> moduleId);
        /// <summary>
        /// 根据当前模块集合获取当前级与下级
        /// add yuangang by 2016-05-18
        /// </summary>
        /// <param name="list">当前模块集合</param>
        /// <returns>递归后的当前级与下级的模块集合</returns>
        List<Domain.SYS_MODULE> RecursiveChildModule(List<Domain.SYS_MODULE> list);
    }
}
