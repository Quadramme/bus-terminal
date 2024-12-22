using DomainModel;
using Interfaces.Repository;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL.Repository
{

    public class RegionRepository : IRepository<Region>
    {

        public RegionRepository(BusTerminalContext dbcontext)
        {
            dbc = dbcontext;
        }

        public void Create(Region item)
        {
            dbc.Region.Add(item);
        }
        
        public Region Get(params object[] keyValues)
        {
            return dbc.Region.Find(keyValues);
        }

        public IEnumerable<Region> GetAll()
        {
            return dbc.Region.ToList();
        }

        public void Update(Region item)
        {
            dbc.Entry(item).State = EntityState.Modified;
        }

        public void Delete(params object[] keyValues)
        {
            var bc = dbc.Region.Find(keyValues);

            if (bc != null)
                dbc.Region.Remove(bc);
        }

        private BusTerminalContext dbc;

    }

}