using DomainModel;
using Interfaces.Repository;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL.Repository
{

    public class RouteInfoRepository : IRepository<RouteInfo>
    {

        public RouteInfoRepository(BusTerminalContext dbcontext)
        {
            dbc = dbcontext;
        }

        public void Create(RouteInfo item)
        {
            dbc.RouteInfo.Add(item);
        }
        
        public RouteInfo Get(params object[] keyValues)
        {
            return dbc.RouteInfo.Find(keyValues);
        }

        public IEnumerable<RouteInfo> GetAll()
        {
            return dbc.RouteInfo.ToList();
        }

        public void Update(RouteInfo item)
        {
            dbc.Entry(item).State = EntityState.Modified;
        }

        public void Delete(params object[] keyValues)
        {
            var bc = dbc.RouteInfo.Find(keyValues);

            if (bc != null)
                dbc.RouteInfo.Remove(bc);

        }

        private BusTerminalContext dbc;

    }

}