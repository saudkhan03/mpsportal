using System;
using System.ComponentModel.DataAnnotations;
using portal.mps.Data;

namespace portal.mps.Models
{
    public class Slab{
        public int Id { get; set; }
        [Required]
        [StringLength(32)]
        public string SlabName { get; set; }
        [Required]
        [StringLength(32)]
        public string Grade { get; set; }
        public decimal TotalFee { get; set; }
    }
}