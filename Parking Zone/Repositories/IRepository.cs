namespace ParkingZone.Repositories
{
    public interface IRepository<T> where T : class
    {
        T Add(T model);
        void Update(T model);
        void Delete(T model);
        T GetById(Guid id);
        IEnumerable<T> GetAll();
        void Save();
    }
}
