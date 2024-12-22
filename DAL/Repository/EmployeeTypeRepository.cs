using DomainModel;
using Interfaces.Repository;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL.Repository
{

    public class EmployeeTypeRepository : IRepository<EmployeeType>
    {

        public EmployeeTypeRepository(BusTerminalContext dbcontext)
        {
            dbc = dbcontext;
        }

        public void Create(EmployeeType item)
        {
            dbc.EmployeeType.Add(item);
        }
        
        public EmployeeType Get(params object[] keyValues)
        {
            return dbc.EmployeeType.Find(keyValues);
        }

        public IEnumerable<EmployeeType> GetAll()
        {
            return dbc.EmployeeType.ToList();
        }

        public void Update(EmployeeType item)
        {
            dbc.Entry(item).State = EntityState.Modified;
        }

        public void Delete(params object[] keyValues)
        {
            var bc = dbc.EmployeeType.Find(keyValues);

            if (bc != null)
                dbc.EmployeeType.Remove(bc);
        }

        private BusTerminalContext dbc;

    }

}