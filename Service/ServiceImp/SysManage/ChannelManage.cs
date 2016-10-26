using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ServiceImp
{
    public class ChannelManage:RepositoryBase<Domain.SYS_CHANNEL>,IService.IChannelManage
    {
        public List<Domain.SYS_CHANNEL> RecursiveModule(List<Domain.SYS_CHANNEL> list)
        {
            List<Domain.SYS_CHANNEL> result = new List<Domain.SYS_CHANNEL>();

            if(list!=null && list.Count>0)
            {
                ChileModule(list, result, 0);
            }

            return result;
        }

        private void ChileModule(List<Domain.SYS_CHANNEL> list,List<Domain.SYS_CHANNEL>  newList,int parentId)
        {
            var result = list.Where(p => p.ParentID == parentId && p.IsDisplay).OrderBy(p => p.DisplayOrder).ToList();

            if (result != null && result.Count > 0)
            {
                foreach(var item in result)
                {
                    newList.Add(item);

                    ChileModule(list, newList, item.ID);
                }
            }
        }


        /// <summary>
        /// 批量变更下级模块的级别
        /// </summary>
        public bool MoreModifyModule(int id, int levels)
        {
            //根据当前模块ID获取下级模块的集合
            var ChildModule = this.LoadAll(p => p.ParentID == id).ToList();

            if (ChildModule != null && ChildModule.Count > 0)
            {
                foreach (var item in ChildModule)
                {
                    item.Levels = levels + 1;
                    this.Update(item);
                    MoreModifyModule(item.ID, item.Levels);
                }
            }
            return true;
        }
    }
}
