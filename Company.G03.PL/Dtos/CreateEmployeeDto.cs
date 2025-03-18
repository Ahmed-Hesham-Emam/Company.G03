using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Company.G03.PL.Dtos
    {
    public class CreateEmployeeDto
        {
        [Required(ErrorMessage = "This Field Is Required !")]
        public string Name { get; set; }
        [Range(22, 60, ErrorMessage = "Age Must Be Between 22 And 60")]
        public int? Age { get; set; }
        [RegularExpression(@"[0-9]{1,3}-[a-zA-Z]{4,10}-[a-zA-Z]{4,10}-[a-zA-Z]{4,10}$",
            ErrorMessage = "Example: 123-Street-City-Country")]
        public string Address { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage = "Must be a valid E-mail")]
        public string Email { get; set; }
        [Phone(ErrorMessage = "Must be a valid Phone Number")]
        public string Phone { get; set; }
        [DataType(DataType.Currency, ErrorMessage = "Must be a valid Salary")]
        public decimal Salary { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        [DisplayName("Hiring Date")]
        public DateTime HiringDate { get; set; }
        [DisplayName("Created At")]
        public DateTime CreatedAt { get; set; }
        }
    }
