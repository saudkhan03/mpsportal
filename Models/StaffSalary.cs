
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using portal.mps.Data;

namespace portal.mps.Models
{
    public class StaffSalary
    {
        public int Id { get; set; }
        [Required]
        public Decimal Salary { get; set; } //monthly salary
        public DateTime SalarySetDate{ get; set; }
        public string Dedutions { get; set; }
        [Required]
        [StringLength(32)]
        public string StaffId { get; set; }
        public Staff Staff { get; set; }
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