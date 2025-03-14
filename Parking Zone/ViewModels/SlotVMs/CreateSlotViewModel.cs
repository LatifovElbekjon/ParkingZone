using ParkingZone.Entities;
using ParkingZone.Enums;
using System.ComponentModel.DataAnnotations;

namespace ParkingZone.ViewModels.SlotVMs
{
    public class CreateSlotViewModel
    {
        public CreateSlotViewModel() { }
        [EnumDataType(typeof(ESlotCategory))]
        public ESlotCategory Tariff { get; set; }
        public bool IsAvailableForBooking { get; set; }
        [Required]
        public int Number {  get; set; }
        public Guid ZoneId { get; set; }

        public Slot MapToModel()
        {
            return new Slot()
            {
                Tariff = this.Tariff,
                IsAvailableForBooking = this.IsAvailableForBooking,
                ZoneId = ZoneId,
                Number = this.Number
            };
        }
    }
}
