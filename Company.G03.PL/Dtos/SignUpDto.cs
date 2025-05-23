﻿using System.ComponentModel.DataAnnotations;

namespace Company.G03.PL.Dtos
    {
    public class SignUpDto
        {
        [Required(ErrorMessage = "UserName is a required field")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "First Name is a required field")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is a required field")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Email is a required field")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is a required field")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\+20(10|11|12|15)[0-9]{8}$", ErrorMessage = "Phone number must be a valid  Egyptian phone number that starts with '+20' Example:'+201012345678' ")]
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is a required field")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z\s]).{8,20}$",
            ErrorMessage = "Password must be between 8 and 20 characters and contain at least one lowercase letter, one uppercase letter, one numeric digit, and one special character ' Empty sapces aren't allowed '.")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is a required field")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        public required string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "You must agree to the Terms and Conditions")]
        public required bool TermsAndConditions { get; set; }
        }
    }
