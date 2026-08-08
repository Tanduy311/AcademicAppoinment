using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AcademicAppoinment.DTOs
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterRequest : ControllerBase

    {
        [Required(ErrorMessage = "User Name can't be empty")]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "Password can't be empty")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Email is not right format")]  
        public string Email { get; set; }

        public string FullName { get; set; }
    }
}
