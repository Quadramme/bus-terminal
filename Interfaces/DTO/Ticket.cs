using System;

namespace Interfaces.DTO
{
    public class Ticket
    {

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public bool Gender { get; set; }
        public int Seat { get; set; }
        public DateTime DOB { get; set; }
        public int Luggage { get; set; }
        public int OrderId { get; set; }
        public decimal Price { get; set; }

    }

}
