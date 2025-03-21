
namespace Company.G03.PL.Services
    {
    public class ScopedServices : IScopedServices
        {
        public ScopedServices()
            {
            Guid = Guid.NewGuid();
            }
        public Guid Guid { get; set; }

        public string GetGuid()
            {
            return Guid.ToString();
            }
        }
    }
