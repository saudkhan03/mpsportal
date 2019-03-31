using System;
using System.ComponentModel.DataAnnotations;
using portal.mps.Data;

namespace portal.mps.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public int ExpenseHeaderId { get; set; }
        public ExpenseHeader ExpenseHeader { get; set; }
        public decimal ExpenseAmount { get; set; }
        public string ExpenseDesc { get; set; }
        public DateTime ExpenseDate { get; set; }
        public int? attachment1Id { get; set; }
        public ImgDoc attachment1 { get; set; }
        public int? attachment2Id { get; set; }
        public ImgDoc attachment2 { get; set; }
        //public ImgDoc attachment3 { get; set; }
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
        public int StudentSlabLinkedId { get; internal set; } //slabId of student
    }
}