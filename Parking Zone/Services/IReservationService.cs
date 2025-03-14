using ParkingZone.Entities;

namespace ParkingZone.Services
{
    public interface IReservationService : IService<Reservation>
    {
        IEnumerable<Reservation> GetReservationsByUserId(string userId);
        void Prolong(Reservation reservation, int addedHours);
    }
}
