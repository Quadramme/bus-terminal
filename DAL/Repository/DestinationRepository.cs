using DomainModel;
using Interfaces.Repository;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL.Repository
{

    public class DestinationRepository : IRepository<Destination>
    {

        public DestinationRepository(BusTerminalContext dbcontext)
        {
            dbc = dbcontext;
        }

        public void Create(Destination item)
        {
            dbc.Destination.Add(item);
        }

        public Destination Get(params object[] keyValues)
        {
            return dbc.Destination.Find(keyValues);
        }

        public IEnumerable<Destination> GetAll()
        {
            return dbc.Destination.ToList();
        }

        public void Update(Destination item)
        {
            dbc.Entry(item).State = EntityState.Modified;
        }

        public void Delete(params object[] keyValues)
        {
            var bc = dbc.Destination.Find(keyValues);

            if (bc != null)
                dbc.Destination.Remove(bc);
        }
        
        private BusTerminalContext dbc;

    }

}