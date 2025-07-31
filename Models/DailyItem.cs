namespace TasteTrack_RMS.Models
{
    public class DailyItem
    {
        public int ID { get; set; }
        public DateTime? Date { get; set; }
        public int? ItemId { get; set; }
        public string? Status { get; set; }
        public string? ItemName { get; set; }
        public decimal? ItemPrice { get; set; }
    }
}
