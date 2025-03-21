namespace Company.G03.PL.Services
    {
    public interface ITransientServices
        {
        public Guid Guid { get; set; }
        string GetGuid();
        }
    }
