using System.Collections.Generic;

namespace DomainModel
{

    public class Driver
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public virtual ICollection<Transport> Transport { get; set; }
    }

}