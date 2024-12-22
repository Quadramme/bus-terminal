using DomainModel;
using Interfaces.Repository;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL.Repository
{

    public class PassengerRepository : IRepository<Passenger>
    {

        public PassengerRepository(BusTerminalContext dbcontext)
        {
            dbc = dbcontext;
        }

        public void Create(Passenger item)
        {
            dbc.Passenger.Add(item);
        }
        
        public Passenger Get(params object[] keyValues)
        {
            return dbc.Passenger.Find(keyValues);
        }

        public IEnumerable<Passenger> GetAll()
        {
            return dbc.Passenger.ToList();
        }

        public void Update(Passenger item)
        {
            dbc.Entry(item).State = EntityState.Modified;
        }

        public void Delete(params object[] keyValues)
        {
            var bc = dbc.Passenger.Find(keyValues);

            if (bc != null)
                dbc.Passenger.Remove(bc);
        }

        private BusTerminalContext dbc;

    }

}