namespace ParkingZone.Services
{
    public interface IService<T> where T : class
    {
        void Add(T model);
        void Update(T model);
        void Delete(T model);
        T? GetById(Guid id);
        IEnumerable<T> GetAll();
    }
}
