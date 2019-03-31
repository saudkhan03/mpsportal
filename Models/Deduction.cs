using System.ComponentModel.DataAnnotations;

namespace portal.mps.Models
{
    public class Deduction
    {
        public int Id { get; set; }
        [StringLength(32)]
        [Required]
        public string DeductionName { get; set; }
        public string DeductionDesc { get; set; }
        [StringLength(8)]
        [Required]
        public string DeductionType { get; set; }
        public decimal DeductionValue { get; set; }
    }
}