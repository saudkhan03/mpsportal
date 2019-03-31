
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using portal.mps.Data;

namespace portal.mps.Models
{
    public class StaffPayment
    {
        public int Id { get; set; }
        [Required]
        public Decimal AmountPaid { get; set; }
        public DateTime PaidDate { get; set; }
        [Required]
        public string StaffId { get; set; }
        public Staff Staff { get; set; }
        [Required]
        public int StaffSalaryId{ get; set; }
        public StaffSalary StaffSalary{ get; set; }
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