using ParkingZone.Entities;
using ParkingZone.Enums;

namespace ParkingZone.ViewModels.SlotVMs
{
    public class EditSlotViewModel
    {
        public EditSlotViewModel() { }  
        public EditSlotViewModel(Slot slot) 
        {
            Id = slot.Id;
            Number = slot.Number;
            IsAvailableForBooking = slot.IsAvailableForBooking;
            Tariff = slot.Tariff;
            ZoneId = slot.ZoneId;
        }
        public Guid Id { get; set; }
        public int Number { get; set; }
        public bool IsAvailableForBooking { get; set; }
        public ESlotCategory Tariff {  get; set; }
        public Guid ZoneId { get; set; }

        public void MapToModel(Slot slot)
        {
            slot.Number = Number;
            slot.IsAvailableForBooking = IsAvailableForBooking;
            slot.Tariff = Tariff;   
        }
    }
}
