using System.ComponentModel.DataAnnotations;

namespace AcademicAppointment.Domain.Entities
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
        public string EmailAddress { get; set; } = null!;

        [MaxLength(50)]
        public string FullName { get; set; } = null!;

        public int RoleId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Role? Role { get; set; }

        public Student? Student { get; set; }

        public Lecturer? Lecturer { get; set; }

        public ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();
    }
}

