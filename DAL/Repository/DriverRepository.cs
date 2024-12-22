using DomainModel;
using Interfaces.Repository;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL.Repository
{

    public class DriverRepository : IRepository<Driver>
    {

        public DriverRepository(BusTerminalContext dbcontext)
        {
            dbc = dbcontext;
        }

        public void Create(Driver item)
        {
            dbc.Driver.Add(item);
        }
        
        public Driver Get(params object[] keyValues)
        {
            return dbc.Driver.Find(keyValues);
        }

        public IEnumerable<Driver> GetAll()
        {
            return dbc.Driver.ToList();
        }

        public void Update(Driver item)
        {
            dbc.Entry(item).State = EntityState.Modified;
        }

        public void Delete(params object[] keyValues)
        {
            var bc = dbc.Driver.Find(keyValues);

            if (bc != null)
                dbc.Driver.Remove(bc);
        }

        private BusTerminalContext dbc;

    }

}