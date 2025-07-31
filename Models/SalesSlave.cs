namespace TasteTrack_RMS.Models
{
    public class SalesSlave
    {
        public int ID { get; set; }
        public int? OrderID { get; set; }
        public int? ItemID { get; set; }
        public int? Quantity { get; set; }
        public decimal? Value { get; set; }
        public string? ItemName { get; set; }
    }
}
