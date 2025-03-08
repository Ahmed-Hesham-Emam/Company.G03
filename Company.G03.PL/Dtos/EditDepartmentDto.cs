using System.ComponentModel.DataAnnotations;

namespace Company.G03.PL.Dtos
    {
    public class EditDepartmentDto
        {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        }
    }
