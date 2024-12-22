using DomainModel;
using Interfaces.Repository;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL.Repository
{

    public class SeatRepository : IRepository<Seat>
    {

        public SeatRepository(BusTerminalContext dbcontext)
        {
            dbc = dbcontext;
        }

        public void Create(Seat item)
        {
            dbc.Seat.Add(item);
        }
        
        public Seat Get(params object[] keyValues)
        {
            return dbc.Seat.Find(keyValues);
        }

        public IEnumerable<Seat> GetAll()
        {
            return dbc.Seat.ToList();
        }

        public void Update(Seat item)
        {
            dbc.Entry(item).State = EntityState.Modified;
        }

        public void Delete(params object[] keyValues)
        {
            var bc = dbc.Seat.Find(keyValues);

            if (bc != null)
                dbc.Seat.Remove(bc);

        }

        private BusTerminalContext dbc;

    }

}