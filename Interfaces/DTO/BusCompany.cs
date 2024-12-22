using System.Collections.Generic;

namespace Interfaces.DTO
{
    
    public class BusCompany
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int> TransportIds { get; set; }
    }

}
