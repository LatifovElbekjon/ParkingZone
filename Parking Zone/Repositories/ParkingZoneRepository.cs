using ParkingZone.Data;
using ParkingZone.Entities;

namespace ParkingZone.Repositories
{
    public class ParkingZoneRepository : Repository<ParkingZone.Entities.ParkingZone>, IParkingZoneRepository
    {
        public ParkingZoneRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }
}
