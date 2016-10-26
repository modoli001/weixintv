using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.IService
{
    /// <summary>
    /// 系统管理业务接口
    /// add yuangang by 2015-09-09
    /// </summary>
    public interface ISystemManage : IRepository<Domain.SYS_SYSTEM>
    {
       /// <summary>
        /// 获取系统ID、NAME
       /// </summary>
       /// <param name="systems">用户拥有操作权限的系统</param>
       /// <returns></returns>
        dynamic LoadSystemInfo(List<string> systems);
    }
}
