using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeApp.Model
{
    public class Trip
    {
        public DateTime Date { get; set; }
        public int? Platform { get; set; }
        public int AvailableSeats { get; set; }
        public int AvailableLuggageSpace { get; set; }
    }
}
