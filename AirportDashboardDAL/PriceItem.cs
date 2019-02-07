using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Table("PriceList")]
    public class PriceItem
    {
        [Key]
        public Guid PriceItemID { get; set; }

        [Required]
        public Guid FlightID { get; set; }

        [StringLength(1)]
        public string FlightClass { get; set; }

        [Range(0, float.MaxValue)]
        public float Price { get; set; }

        [ForeignKey("FlightID")]
        public virtual Flight Flight { get; set; }

        public PriceItem()
        {
            PriceItemID = Guid.NewGuid();
        }
    }
}
