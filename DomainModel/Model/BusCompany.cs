using System.Collections.Generic;

namespace DomainModel
{

    public class BusCompany
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Transport> Transport { get; set; }
    }

}
