using System;
using System.Collections.Generic;

namespace Interfaces.DTO
{
    public class Route
    {
        public int Id { get; set; }
        public int RouteInfoId { get; set; }
        public int LuggageSpace { get; set; }
        public DateTime Date { get; set; }
        public int RouteStatusId { get; set; }
        public List<int> SeatIds { get; set; }
    }

}