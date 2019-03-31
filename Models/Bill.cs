using System;

namespace portal.mps.Models
{
    public class Bill
    {
        public int Id { get; set; }
        public long BillNumber { get; set; }
        public string BillType { get; set; }
        public string GeneratedFor { get; set; }
        public Decimal BillAmount { get; set; }
        public DateTime GeneratedDate { get; set; }
        public string PaymentType { get; set; }
        public int BillFileId { get; set; }
        public ImgDoc BillFile { get; set; }
        public int PaymentId { get; set; }
    }
}