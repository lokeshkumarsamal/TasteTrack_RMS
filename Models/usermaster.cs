using System.ComponentModel.DataAnnotations;

namespace TasteTrack_RMS.Models
{
    public class usermaster
    {
        [Required]
        [StringLength(50)]
        public string userid { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string passwd { get; set; } = string.Empty;

        [StringLength(50)]
        public string? usertype { get; set; }
    }

    public class LoginRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string? UserType { get; set; }
        public string? UserId { get; set; }
    }
}
