using System;
using System.ComponentModel.DataAnnotations;
using portal.mps.Data;

namespace portal.mps.Models
{
    public class StudentMatrix
    {
        public int Id { get; set; }
        public Student Student { get; set; }
        [Required]
        public string StudentId { get; set; }
        [Required]
        [StringLength(12)]        
        public string AcademicYear { get; set; }
        public AcademicEntity Subject { get; set; }
        public int SubjectId { get; set; }
        [StringLength(255)]
        public string Exam1 { get; set; }
        [StringLength(255)]
        public string Exam2 { get; set; }
        [StringLength(255)]
        public string Exam3 { get; set; }
        [StringLength(255)]
        public string Exam4 { get; set; }
        [StringLength(255)]
        public string Exam5 { get; set; }
        [StringLength(255)]
        public string Exam6 { get; set; }
        [StringLength(255)]
        public string Median { get; set; }
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