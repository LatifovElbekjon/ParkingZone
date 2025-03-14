using ParkingZone.Repositories;

namespace ParkingZone.Services
{
    public class Service<T> : IService<T> where T : class
    {
        private readonly IRepository<T> _repository;

        public Service(IRepository<T> repository)
        {
            _repository = repository;
        }

        public virtual void Add(T model)
        {
            _repository.Add(model);
            _repository.Save();
        }

        public virtual void Delete(T model)
        {
            _repository.Delete(model);
            _repository.Save();
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _repository.GetAll();
        }

        public virtual T? GetById(Guid id)
        {
            return _repository.GetById(id);
        }

        public virtual void Update(T model)
        {
            _repository.Update(model);
            _repository.Save();
        }
    }
}
