using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Table("PassengerList")]
    public class Passenger
    {
        [Key]
        public Guid PassengerID { get; set; }

        [Required]
        public Guid FlightID { get; set; }

        [StringLength(1)]
        public string FlightClass { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(30)]
        public string Nationality { get; set; }

        [StringLength(20)]
        public string Passport { get; set; }

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [StringLength(1)]
        public string Sex { get; set; }

        [ForeignKey("FlightID")]
        public virtual Flight Flight { get; set; }

        public Passenger()
        {
            PassengerID = Guid.NewGuid();
        }
    }
}

