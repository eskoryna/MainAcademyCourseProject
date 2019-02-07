using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Table("Flight")]
    public class Flight
    {
        [Key]
        public Guid FlightID { get; set; }

        [StringLength(255)]
        public string Destination { get; set; }

        [StringLength(255)]
        public string Departure { get; set; }

        [StringLength(20)]
        public string FlightNumber { get; set; }

        [StringLength(255)]
        public string Carrier { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DepartureDateTime { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ArrivalDateTime { get; set; }

        [StringLength(20)]
        public string FlightStatus { get; set; }

        [StringLength(3)]
        public string Terminal { get; set; }

        [StringLength(1)]
        public string ArriveDepart { get; set; }

        public virtual ICollection<PriceItem> PriceLists { get; set; } = new HashSet<PriceItem>();
        public virtual ICollection<Passenger> PassengerLists { get; set; } = new HashSet<Passenger>();

        public Flight()
        {
            FlightID = Guid.NewGuid();
        }
    }
}
