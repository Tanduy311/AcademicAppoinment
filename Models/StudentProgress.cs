using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AcademicAppoinment.Models
{
    public class StudentProgress
    {
        public int StudentProgressId { get; set; }
        public int StudentId { get; set; }

        [Required]
        [MaxLength(260)]
        public string FileName { get; set; }

        [Required]
        [MaxLength(1024)]
        public string FilePath { get; set; }

        [MaxLength(100)]
        public string? ContentType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonIgnore]
        public Student? Student { get; set; }
    }
}