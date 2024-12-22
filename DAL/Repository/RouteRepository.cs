using DomainModel;
using Interfaces.Repository;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL.Repository
{

    public class RouteRepository : IRepository<Route>
    {

        public RouteRepository(BusTerminalContext dbcontext)
        {
            dbc = dbcontext;
        }

        public void Create(Route item)
        {
            dbc.Route.Add(item);
        }
        
        public Route Get(params object[] keyValues)
        {
            return dbc.Route.Find(keyValues);
        }

        public IEnumerable<Route> GetAll()
        {
            return dbc.Route.ToList();
        }

        public void Update(Route item)
        {
            dbc.Entry(item).State = EntityState.Modified;
        }

        public void Delete(params object[] keyValues)
        {
            var bc = dbc.Route.Find(keyValues);

            if (bc != null)
                dbc.Route.Remove(bc);
        }

        private BusTerminalContext dbc;

    }

}