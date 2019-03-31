using System;
using System.ComponentModel.DataAnnotations;

namespace portal.mps.Models
{
    public class ImgDoc
    {
        public int Id { get; set; }
        [Required]
        [StringLength(32)]
        public string name { get; set; }
        [Required]
        [StringLength(6)]
        public string fileExtension { get; set; }
        [Required]
        public string content { get; set; }
        [StringLength(32)]
        public string contentType { get; set; }
        public byte[] byteInfo{ get; set; }
    }
}