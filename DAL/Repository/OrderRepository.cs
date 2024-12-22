using DomainModel;
using Interfaces.Repository;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL.Repository
{

    public class OrderRepository : IRepository<Order>
    {

        public OrderRepository(BusTerminalContext dbcontext)
        {
            dbc = dbcontext;
        }

        public void Create(Order item)
        {
            dbc.Order.Add(item);
            
        }
        
        public Order Get(params object[] keyValues)
        {
            return dbc.Order.Find(keyValues);
        }

        public IEnumerable<Order> GetAll()
        {
            return dbc.Order.ToList();
        }

        public void Update(Order item)
        {
            dbc.Entry(item).State = EntityState.Modified;
        }

        public void Delete(params object[] keyValues)
        {
            var bc = dbc.Order.Find(keyValues);

            if (bc != null)
                dbc.Order.Remove(bc);
        }

        private BusTerminalContext dbc;

    }

}