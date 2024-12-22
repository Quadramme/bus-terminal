using System.Collections.Generic;

namespace DomainModel
{

    public class Region
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Destination> Destination { get; set; }
    }

}