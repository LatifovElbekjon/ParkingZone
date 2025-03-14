using Microsoft.EntityFrameworkCore;
using ParkingZone.Data;
using ParkingZone.Entities;

namespace ParkingZone.Repositories
{
    public class SlotRepository : Repository<Slot>, ISlotRepository
    {
        public SlotRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }
}
