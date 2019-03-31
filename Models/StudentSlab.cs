using System;
using System.ComponentModel.DataAnnotations;
using portal.mps.Data;

namespace portal.mps.Models
{
    public class StudentSlab
    {
       public int Id { get; set; }
       public string StudentId { get; set; }
       public Student Student { get; set; }
       public int SlabId { get; set; }
       public Slab Slab { get; set; }
       [Required]
       [StringLength(32)]
       public string AcademicYear { get; set; }
       public DateTime FeesStartDate { get; set; }
       public DateTime FeesEndDate { get; set; }
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