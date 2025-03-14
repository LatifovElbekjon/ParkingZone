using ParkingZone.Entities;
using ParkingZone.Services;

namespace ParkingZone.Services
{
    public interface ISlotService : IService<Slot>
    {
        List<Slot> GetByZoneId(Guid zoneId);
        bool SlotWithThisNumberExists(Guid zoneId, int number);
        List<Slot> GetFreeSlotsByZoneId(Guid zoneId, DateTime startTime, int duration);
        bool IsSlotFree(Slot slot, DateTime startTime, int duration);
    }
}
