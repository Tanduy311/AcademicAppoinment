using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AcademicAppoinment.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string AccountName { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        [Required]
        public required string EmailAddress { get; set; }

        [MaxLength(50)]
        public required string FullName { get; set; }

        // Khóa ngoại tới Role
        public int RoleId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties

        [JsonIgnore]
        public Role? Role { get; set; }

        [JsonIgnore]
        public Student? Student { get; set; }

        [JsonIgnore]
        public Lecturer? Lecturer { get; set; }

        [JsonIgnore]
        public ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();
    }
}
