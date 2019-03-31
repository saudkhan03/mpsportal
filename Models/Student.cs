using System;
using System.ComponentModel.DataAnnotations;
using portal.mps.Data;

namespace portal.mps.Models
{
    public class Student
    {       
        [Required]
        [Key]
        public string StudentId { get; set; }
        [StringLength(16)]
        public string AadhaarNumber { get; set; }
        public ImgDoc AadhaarCard { get; set; }
        public int? AadhaarCardId { get; set; }
        [Required]
        public DateTime AdmissionDate { get; set; }
        [Required]
        public string StudentUserId { get; set; }
        public mpsUser StudentUser { get; set; }
        [Required]
        public bool isActive { get; set; }
        public string RollNumber { get; set; }
        [StringLength(255)]
        public string StudentPic { get; set; }
        public ImgDoc TransferCert { get; set; }
        public int? TransferCertId { get; set; }

        [StringLength(64)]
        public string FatherName { get; set; }
        [StringLength(16)]
        public string FatherAadhaarNumber { get; set; }
        public ImgDoc FatherAadhaarCard { get; set; }
        public int? FatherAadhaarCardId { get; set; }
        [StringLength(255)]
        public string FatherOccupation { get; set; }
        [StringLength(64)]
        public string MotherName { get; set; }
        [StringLength(16)]
        public string MotherAadhaarNumber { get; set; }
        public ImgDoc MotherAadhaarCard { get; set; }
        public int? MotherAadhaarCardId { get; set; }
        
        [StringLength(255)]
        public string MotherOccupation { get; set; }
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