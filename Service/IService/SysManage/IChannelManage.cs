using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.IService
{
    public interface IChannelManage:IRepository<Domain.SYS_CHANNEL>
    {
        List<Domain.SYS_CHANNEL> RecursiveModule(List<Domain.SYS_CHANNEL> list);

        /// <summary>
        /// 批量变更下级模块的级别
        /// </summary>
        bool MoreModifyModule(int id, int levels);
    }
}
