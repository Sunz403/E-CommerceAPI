using System.ComponentModel.DataAnnotations;

namespace E_CommAPI.Models
{
        public class User
        {
            [Key]
            public int Id { get; set; }

            [Required]
            [StringLength(50)]
            public string Username { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string Role { get; set; } = "Customer"; // Admin, Customer

            [Required]
            public string ApiKey { get; set; } = string.Empty;

            public DateTime ApiKeyExpiry { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public bool IsActive { get; set; } = true;
           
        }

}
