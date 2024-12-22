using DomainModel;
using Interfaces.Repository;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL.Repository
{

    public class RouteStatusRepository : IRepository<RouteStatus>
    {

        public RouteStatusRepository(BusTerminalContext dbcontext)
        {
            dbc = dbcontext;
        }

        public void Create(RouteStatus item)
        {
            dbc.RouteStatus.Add(item);
        }
        
        public RouteStatus Get(params object[] keyValues)
        {
            return dbc.RouteStatus.Find(keyValues);
        }

        public IEnumerable<RouteStatus> GetAll()
        {
            return dbc.RouteStatus.ToList();
        }

        public void Update(RouteStatus item)
        {
            dbc.Entry(item).State = EntityState.Modified;
        }

        public void Delete(params object[] keyValues)
        {
            var bc = dbc.RouteStatus.Find(keyValues);

            if (bc != null)
                dbc.RouteStatus.Remove(bc);
        }

        private BusTerminalContext dbc;

    }

}