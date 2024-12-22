using DomainModel;
using Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL.Repository
{

    public class BusCompanyRepository : IRepository<BusCompany>
    {

        public BusCompanyRepository(BusTerminalContext dbcontext) 
        {
            dbc = dbcontext;
        }

        public void Create(BusCompany item)
        {
            dbc.BusCompany.Add(item);
        }

        public BusCompany Get(params object[] keyValues)
        {
            return dbc.BusCompany.Find(keyValues);
        }

        public IEnumerable<BusCompany> GetAll()
        {
            return dbc.BusCompany.ToList();
        }

        public void Update(BusCompany item)
        {
            dbc.Entry(item).State = EntityState.Modified;
        }

        public void Delete(params object[] keyValues)
        {
            var bc = dbc.BusCompany.Find(keyValues);
            
            if (bc != null)
                dbc.BusCompany.Remove(bc);
        }

        private BusTerminalContext dbc;

    }

}