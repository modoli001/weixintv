using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.ServiceImp
{
    /// <summary>
    /// 系统管理业务曾实现类
    /// add yuangang by 2015-09-09
    /// </summary>
    public class SystemManage : RepositoryBase<Domain.SYS_SYSTEM>, IService.ISystemManage
    {
        /// <summary>
        /// 获取系统ID、NAME
        /// </summary>
        /// <param name="systems">用户拥有操作权限的系统</param>
        /// <returns></returns>
        public dynamic LoadSystemInfo(List<string> systems)
        {
            return Common.JsonConverter.JsonClass(this.LoadAll(p => systems.Any(e => e == p.ID)).OrderBy(p => p.CREATEDATE).Select(p => new { p.ID, p.NAME }).ToList());
        }
    }
}
