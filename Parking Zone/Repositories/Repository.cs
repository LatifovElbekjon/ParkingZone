using Microsoft.EntityFrameworkCore;
using ParkingZone.Data;

namespace ParkingZone.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> table;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            table = _context.Set<T>();
        }
        public T Add(T model)
        {
            table.Add(model);
            return model;
        }

        public void Delete(T model)
        {
            table.Remove(model);
        }

        public IEnumerable<T> GetAll()
        {
            return table.ToList();
            }

        public T GetById(Guid id)
        {
            return table.Find(id)!;
        }

        public void Update(T model)
        {
            table.Update(model);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
