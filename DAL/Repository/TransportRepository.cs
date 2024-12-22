using DomainModel;
using Interfaces.Repository;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL.Repository
{

    public class TransportRepository : IRepository<Transport>
    {

        public TransportRepository(BusTerminalContext dbcontext)
        {
            dbc = dbcontext;
        }

        public void Create(Transport item)
        {
            dbc.Transport.Add(item);
        }
        
        public Transport Get(params object[] keyValues)
        {
            return dbc.Transport.Find(keyValues);
        }

        public IEnumerable<Transport> GetAll()
        {
            return dbc.Transport.ToList();
        }

        public void Update(Transport item)
        {
            dbc.Entry(item).State = EntityState.Modified;
        }

        public void Delete(params object[] keyValues)
        {
            var bc = dbc.Transport.Find(keyValues);

            if (bc != null)
                dbc.Transport.Remove(bc);
        }

        private BusTerminalContext dbc;

    }

}