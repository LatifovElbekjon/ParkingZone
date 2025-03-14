using ParkingZone.Entities;
using ParkingZone.Enums;

namespace ParkingZone.ViewModels.SlotVMs
{
    public class DetailsSlotViewModel
    {
        public DetailsSlotViewModel() { }
        public DetailsSlotViewModel(Slot slot)
        {
            SlotId = slot.Id;
            SlotNumber = slot.Number;
            ZoneId = slot.ZoneId;   
            IsAvailableForBooking = slot.IsAvailableForBooking;
            Tariff = slot.Tariff;
        }
        public Guid SlotId { get; set; }
        public int SlotNumber { get; set; }
        public Guid ZoneId { get; set; }
        public bool IsAvailableForBooking { get; set; }
        public ESlotCategory Tariff { get; set; }
    }
}
