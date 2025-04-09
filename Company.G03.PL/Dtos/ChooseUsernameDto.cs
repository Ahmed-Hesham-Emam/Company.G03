using System.ComponentModel.DataAnnotations;

namespace Company.G03.PL.Dtos
    {
    public class ChooseUsernameDto
        {

        public string UserId { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        }
    }
