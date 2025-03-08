using System.ComponentModel.DataAnnotations;

namespace Company.G03.PL.Dtos
    {
    public class CreateDepartmentDto
        {
        [Required(ErrorMessage = "This Field Is Required !")]
        public string Code { get; set; }

        [Required(ErrorMessage = "This Field Is Required !")]
        public string Name { get; set; }

        [Required(ErrorMessage = "This Field Is Required !")]
        public DateTime CreatedAt { get; set; }
        }
    }
