using ParkingZone.Data;
using ParkingZone.Entities;
using ParkingZone.Repositories;

namespace ParkingZone.Services
{
    public class ReservationService : Service<Reservation>, IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        public ReservationService(IReservationRepository repo) : base(repo)
        {
            _reservationRepository = repo;
        }

        public virtual IEnumerable<Reservation> GetReservationsByUserId(string userId)
        {
            return _reservationRepository.GetAll().Where(v => v.UserId == userId).OrderByDescending(c => c.StartTime);
        }

        public void Prolong(Reservation reservation, int addedHours)
        {
            reservation.Duration += addedHours;
            Update(reservation);
        }
    }
}
