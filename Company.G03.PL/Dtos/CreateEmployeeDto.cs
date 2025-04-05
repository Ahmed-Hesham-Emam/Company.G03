using Company.G03.DAL.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Company.G03.PL.Dtos
    {
    public class CreateEmployeeDto
        {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }
        //////////////////////////////////////////////////////////
        [Range(22, 60, ErrorMessage = "Age Must Be Between 22 And 60")]
        public int? Age { get; set; }
        //////////////////////////////////////////////////////////

        [RegularExpression(@"\d+-[\D]{4,10}-[\D]{4,10}-[\D]{4,10}$",
            ErrorMessage = "Example: 123-Street-City-Country")]
        public required string Address { get; set; }
        //////////////////////////////////////////////////////////

        [DataType(DataType.EmailAddress, ErrorMessage = "Must be a valid E-mail")]
        public required string Email { get; set; }
        //////////////////////////////////////////////////////////

        [Phone(ErrorMessage = "Must be a valid Phone Number")]
        //////////////////////////////////////////////////////////
        public required string Phone { get; set; }

        [DataType(DataType.Currency, ErrorMessage = "Must be a valid Salary")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Salary must be a positive number")]
        public decimal Salary { get; set; }
        //////////////////////////////////////////////////////////

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        [DisplayName("Hiring Date")]
        public DateTime HiringDate { get; set; }
        [DisplayName("Created At")]
        public DateTime CreatedAt { get; set; }

        [DisplayName("Department")]

        public int? DepartmentId { get; set; }

        public Department? Department { get; set; }

        public IFormFile? Image { get; set; }

        public string? ImageName { get; set; }

        }
    }
