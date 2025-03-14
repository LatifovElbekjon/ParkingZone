using ParkingZone.Entities;
using ParkingZone.Enums;

namespace ParkingZone.ViewModels.SlotVMs
{
    public class SlotsItemViewModel
    {
        public SlotsItemViewModel() { } 
        public SlotsItemViewModel(Slot slot)
        {
            Id = slot.Id;
            Number = slot.Number;
            IsAvailableForBooking = slot.IsAvailableForBooking;
            Tariff = slot.Tariff;
            CanEditAndDelete = !slot.Reservations.Any(c => c.Status == Enums.ReservationStatus.InProgress || c.Status == Enums.ReservationStatus.Pending);
        }

        public Guid Id { get; set; }
        public int Number {  get; set; }
        public bool IsAvailableForBooking { get; set; }
        public ESlotCategory Tariff { get; set; } 
        public bool CanEditAndDelete { get; set; }
    }
}
