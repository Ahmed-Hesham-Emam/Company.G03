namespace Company.G03.PL.Services
    {
    public interface ISingletonServices
        {
        public Guid Guid { get; set; }
        string GetGuid();
        }
    }
