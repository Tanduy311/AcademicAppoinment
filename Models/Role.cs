using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AcademicAppoinment.Models
{
    public class Role
    {
        public int RoleId { get; set; }

        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; } = null!;

        // Một role có thể thuộc nhiều user
        [JsonIgnore]
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
