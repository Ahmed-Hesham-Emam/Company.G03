using System.Xml.Serialization;

namespace Company.G03.PL.Dtos
    {
    [XmlRoot("AspNetUsers")]
    public class AdminUserWrapper
        {
        [XmlElement("row")]
        public AdminUserDto UserDto { get; set; }
        }

    public class AdminUserDto
        {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool TermsAndConditions { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        }
    }
