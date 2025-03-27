using System.ComponentModel.DataAnnotations;

namespace Company.G03.PL.Dtos
    {
    public class ForgotPasswordDto
        {
        [Required(ErrorMessage = "Email is a required field")]
        public required string Email { get; set; }
        }
    }
