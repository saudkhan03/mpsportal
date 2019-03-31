using System.ComponentModel.DataAnnotations;

namespace portal.mps.Models
{
    public class ExpenseHeader
    {
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string ExpenseHeaderName { get; set; }
        [StringLength(255)]
        public string ExpenseHeaderDesc { get; set; }
    }
}