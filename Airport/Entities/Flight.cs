using System;

namespace Airport.Entities
{
    public class Flight
    {
        public Guid Id { get; set; }

        public string Number { get; set; }

        public DateTime ArrivalTime { get; set; }

        public DateTime DepartureTime { get; set; }

        public TimeSpan Delay { get; set; }

        public string Departure { get; set; }

        public string Destination { get; set; }
    }
}
