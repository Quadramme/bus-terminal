using System;

namespace Interfaces.DTO
{
    public class Seat
    {

        public int Number { get; set; }

        public int RouteId { get; set; }

        public int Status { get; set; }
		
		public DateTime StatusChangedOn { get; set; }

    }

}
