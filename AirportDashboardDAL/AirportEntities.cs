namespace Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class AirportEntities : DbContext
    {
        public AirportEntities()
            : base("name=AirportEntities") {}

        public virtual DbSet<Flight> Flights { get; set; }
        public virtual DbSet<PriceItem> PriceLists { get; set; }
        public virtual DbSet<Passenger> PassengerLists { get; set; }
    }
}