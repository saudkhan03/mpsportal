using System;
using System.ComponentModel.DataAnnotations;
using portal.mps.Data;

namespace portal.mps.Models
{
    public class Staff
    {
        [Required]
        [Key]
        public string StaffId { get; set; }
        [StringLength(16)]
        public string AadhaarNumber { get; set; }
        public ImgDoc AadhaarCard { get; set; }
        public int? AadhaarCardId { get; set; }
        [Required]
        public DateTime JoiningDate { get; set; }
        [Required]
        public string StaffUserId { get; set; }
        public mpsUser StaffUser { get; set; }
        [Required]
        public bool isActive { get; set; }
        public bool isTeacher { get; set; }
        [StringLength(255)]
        public string StaffPic { get; set; }
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