using System;
using System.Collections.Generic;

namespace Interfaces.DTO
{

    public class Order
    {

        public int Id { get; set; }

        public int RouteId { get; set; }

        public int DeparturePoint { get; set; }

        public int ArrivalPoint { get; set; }

        public int? PassengerId { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public DateTime Date { get; set; }

    }

}