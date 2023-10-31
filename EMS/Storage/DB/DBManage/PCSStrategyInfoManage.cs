using EMS.Model;
using EMS.Storage.DB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Storage.DB.DBManage
{
    public class PCSStrategyInfoManage : IManage<PCSStrategyInfoModel>
    {
        public bool Insert(PCSStrategyInfoModel Entity)
        {
            throw new NotImplementedException();
        }
        
        public bool Delete(PCSStrategyInfoModel Entity)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAll()
        {
            throw new NotImplementedException();

        }

        public bool Update(PCSStrategyInfoModel Entity)
        {
            throw new NotSupportedException();
        }

        public List<PCSStrategyInfoModel> Get()
        {
            try
            {
                using (var db = new ORMContext())
                {
                    var result = db.PCSStrategyInfos.ToList();
                    return result;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
