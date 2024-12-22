using DomainModel;
using Interfaces.Repository;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL.Repository
{

    public class EmployeeRepository : IRepository<Employee>
    {

        public EmployeeRepository(BusTerminalContext dbcontext)
        {
            dbc = dbcontext;
        }

        public void Create(Employee item)
        {
            dbc.Employee.Add(item);
        }

        public Employee Get(params object[] keyValues)
        {
            return dbc.Employee.Find(keyValues);
        }

        public IEnumerable<Employee> GetAll()
        {
            return dbc.Employee.ToList();
        }

        public void Update(Employee item)
        {
            dbc.Entry(item).State = EntityState.Modified;
        }

        public void Delete(params object[] keyValues)
        {
            var bc = dbc.Employee.Find(keyValues);

            if (bc != null)
                dbc.Employee.Remove(bc);
        }

        private BusTerminalContext dbc;

    }

}