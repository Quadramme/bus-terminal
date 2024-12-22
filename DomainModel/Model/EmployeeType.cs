using System.Collections.Generic;

namespace DomainModel
{
    public class EmployeeType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Employee> Employee{ get; set; }
    }

}
