using System.ComponentModel.DataAnnotations;

namespace TasteTrack_RMS.Models
{
    public class ItemMaster
    {
        public int ItemID { get; set; }

        [Required]
        [StringLength(100)]
        public string ItemName { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 9999.99)]
        public decimal ItemPrice { get; set; }
    }
}
