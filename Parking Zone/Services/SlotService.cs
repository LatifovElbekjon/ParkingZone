using ParkingZone.Entities;
using ParkingZone.Repositories;

namespace ParkingZone.Services
{
    public class SlotService : Service<Slot>, ISlotService
    {
        private readonly ISlotRepository _slotRepository;
        public SlotService(ISlotRepository repo) : base(repo) 
        {
            _slotRepository = repo;
        }

        public List<Slot> GetByZoneId(Guid zoneId)
        {
            var slots = _slotRepository.GetAll().Where(x => x.ZoneId == zoneId).OrderBy(b => b.Number).ToList();
            return slots;
        }

        public bool SlotWithThisNumberExists(Guid zoneId, int number)
        {
            return _slotRepository.GetAll().Where(b => b.ZoneId == zoneId).Any(x => x.Number == number);
        }

        public List<Slot> GetFreeSlotsByZoneId(Guid zoneId, DateTime startTime, int duration)
        {
            return _slotRepository.GetAll().Where(x => x.ZoneId == zoneId)
                .Where(b => b.IsAvailableForBooking && IsSlotFree(b, startTime, duration)).ToList();
        }
        //
        public bool IsSlotFree(Slot slot, DateTime startTime, int duration)
        {
            return !slot.Reservations.Any(x=>(x.StartTime <= startTime 
            && x.StartTime.AddHours(x.Duration) > startTime) 
            || (x.StartTime >= startTime && startTime.AddHours(duration) > x.StartTime));
        }
    }
}
