using ParkingZone.Data;
using ParkingZone.Entities;

namespace ParkingZone.Repositories
{
    public class ReservationRepository : Repository<Reservation>, IReservationRepository
    {
        public ReservationRepository(ApplicationDbContext context) : base(context) { }  
    }
}
