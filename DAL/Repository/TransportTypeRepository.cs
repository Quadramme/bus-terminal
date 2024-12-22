using DomainModel;
using Interfaces.Repository;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL.Repository
{

    public class TransportTypeRepository : IRepository<TransportType>
    {

        public TransportTypeRepository(BusTerminalContext dbcontext)
        {
            dbc = dbcontext;
        }

        public void Create(TransportType item)
        {
            dbc.TransportType.Add(item);
        }
        
        public TransportType Get(params object[] keyValues)
        {
            return dbc.TransportType.Find(keyValues);
        }

        public IEnumerable<TransportType> GetAll()
        {
            return dbc.TransportType.ToList();
        }

        public void Update(TransportType item)
        {
            dbc.Entry(item).State = EntityState.Modified;
        }

        public void Delete(params object[] keyValues)
        {
            var bc = dbc.TransportType.Find(keyValues);

            if (bc != null)
                dbc.TransportType.Remove(bc);

        }

        private BusTerminalContext dbc;

    }

}