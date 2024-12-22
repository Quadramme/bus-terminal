using DomainModel;
using Interfaces.Repository;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL.Repository
{

    public class PaymentInfoRepository : IRepository<PaymentInfo>
    {

        public PaymentInfoRepository(BusTerminalContext dbcontext)
        {
            dbc = dbcontext;
        }

        public void Create(PaymentInfo item)
        {
            dbc.PaymentInfo.Add(item);
        }
        
        public PaymentInfo Get(params object[] keyValues)
        {
            return dbc.PaymentInfo.Find(keyValues);
        }

        public IEnumerable<PaymentInfo> GetAll()
        {
            return dbc.PaymentInfo.ToList();
        }

        public void Update(PaymentInfo item)
        {
            dbc.Entry(item).State = EntityState.Modified;
        }

        public void Delete(params object[] keyValues)
        {
            var bc = dbc.PaymentInfo.Find(keyValues);

            if (bc != null)
                dbc.PaymentInfo.Remove(bc);
        }

        private BusTerminalContext dbc;

    }

}