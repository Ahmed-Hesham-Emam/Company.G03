namespace Company.G03.PL.Services
    {
    public class TransientServices : ITransientServices
        {
        public TransientServices()
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
