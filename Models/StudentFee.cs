using System;
using System.ComponentModel.DataAnnotations;
using portal.mps.Data;

namespace portal.mps.Models
{
    public class StudentFee
    {
        public int Id { get; set; }
        [Required]
        [StringLength(32)]
        public string AcademicYear { get; set; }
        public Decimal PaidFees { get; set; }
        public DateTime? PaidDate { get; set; }
        [Required]
        public int StudentSlabId { get; set; }
        public StudentSlab StudentSlab { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public DateTime ModifiedDate { get; set; }
        [Required]
        public string CreatedById { get; set; }
        public mpsUser CreatedBy { get; set; }
        [Required]
        public string ModifiedById { get; set; }
        public mpsUser ModifiedBy { get; set; }
    }
}