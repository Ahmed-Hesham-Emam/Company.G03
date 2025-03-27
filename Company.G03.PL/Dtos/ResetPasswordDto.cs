using System.ComponentModel.DataAnnotations;

namespace Company.G03.PL.Dtos
    {
    public class ResetPasswordDto
        {
        [Required(ErrorMessage = "Password is a required field")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z\s]).{8,20}$",
    ErrorMessage = "Password must be between 8 and 20 characters and contain at least one lowercase letter, one uppercase letter, one numeric digit, and one special character ' Empty sapces aren't allowed '.")]
        public required string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password is a required field")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
        public required string ConfirmPassword { get; set; }
        }
    }
