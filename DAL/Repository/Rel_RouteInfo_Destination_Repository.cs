using DomainModel;
using Interfaces.Repository;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL.Repository
{

    public class Rel_RouteInfo_Destination_Repository : IRepository<Rel_RouteInfo_Destination>
    {

        public Rel_RouteInfo_Destination_Repository(BusTerminalContext dbcontext)
        {
            dbc = dbcontext;
        }

        public void Create(Rel_RouteInfo_Destination item)
        {
            dbc.Rel_RouteInfo_Destination.Add(item);
        }
        
        public Rel_RouteInfo_Destination Get(params object[] keyValues)
        {
            return dbc.Rel_RouteInfo_Destination.Find(keyValues);
        }

        public IEnumerable<Rel_RouteInfo_Destination> GetAll()
        {
            return dbc.Rel_RouteInfo_Destination.ToList();
        }

        public void Update(Rel_RouteInfo_Destination item)
        {
            dbc.Entry(item).State = EntityState.Modified;
        }

        public void Delete(params object[] keyValues)
        {
            var bc = dbc.Rel_RouteInfo_Destination.Find(keyValues);

            if (bc != null)
                dbc.Rel_RouteInfo_Destination.Remove(bc);
        }

        private BusTerminalContext dbc;

    }

}