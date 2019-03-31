using System.ComponentModel.DataAnnotations;

namespace portal.mps.Models
{
    public class AcademicEntity
    {
        public int Id { get; set; }
        [Required]
        [StringLength(128)]
        public string AcademicEntityName { get; set; }
        [StringLength(32)]
        public string AcademicEntityType {get; set;}
        [StringLength(12)]
        public string AcademicEntityValueType { get; set; }
        [StringLength(32)]
        public string AcademicEntityGrade { get; set; }
        public bool Active { get; set; }
    }
    public enum AcademicEntityValueType
    {
        Marks,
        Grade,
        Attendance,
        Text,
        Value
    }

    public enum AcademicEntityType
    {
        Subject,
        Attendance,
        Text,
        Value,
        Character_Social,
        Character_Residential
    }

}