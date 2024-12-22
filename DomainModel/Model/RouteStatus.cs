using System.Collections.Generic;

namespace DomainModel
{

    public class RouteStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Route> Route { get; set; }
    }

}