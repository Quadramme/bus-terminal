using DomainModel;
using Interfaces.Repository;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL.Repository
{

    public class TicketRepository : IRepository<Ticket>
    {

        public TicketRepository(BusTerminalContext dbcontext)
        {
            dbc = dbcontext;
        }

        public void Create(Ticket item)
        {
            dbc.Ticket.Add(item);
        }
        
        public Ticket Get(params object[] keyValues)
        {
            return dbc.Ticket.Find(keyValues);
        }

        public IEnumerable<Ticket> GetAll()
        {
            return dbc.Ticket.ToList();
        }

        public void Update(Ticket item)
        {
            dbc.Entry(item).State = EntityState.Modified;
        }

        public void Delete(params object[] keyValues)
        {
            var bc = dbc.Ticket.Find(keyValues);

            if (bc != null)
                dbc.Ticket.Remove(bc);

        }

        private BusTerminalContext dbc;

    }

}