namespace TasteTrack_RMS.Models
{
    public class SalesReport
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalValue { get; set; }
    }

    public class ItemWiseReport
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
    }

    public class DailySalesReport
    {
        public DateTime OrderDate { get; set; }
        public decimal TotalValue { get; set; } 
        public int TotalOrders { get; set; }
    }

    public class ItemWiseReportData
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Value { get; set; }
    }
}
