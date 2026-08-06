using System.ComponentModel.DataAnnotations;

namespace AcademicAppoinment.Models
{
    public class Admin
    {
        public int AdminId { get; set; }
        [Required]
        public string PasswordHash { get; set; } = null!;
    }
}
