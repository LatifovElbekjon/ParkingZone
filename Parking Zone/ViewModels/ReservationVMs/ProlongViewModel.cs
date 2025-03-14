using ParkingZone.Entities;
using System.ComponentModel.DataAnnotations;

namespace ParkingZone.ViewModels.ReservationVMs
{
    public class ProlongViewModel
    {
        public ProlongViewModel() { }
        public ProlongViewModel(Reservation reservation) 
        {
            ReservationId = reservation.Id;
            SlotNumber = reservation.Slot.Number;
            ZoneAddress = reservation.Slot.Zone.Address;
            EndDate = reservation.StartTime.AddHours(reservation.Duration).ToString("g");
        }
        public Guid ReservationId { get; set; }
        public int? SlotNumber { get; set; }
        public string? ZoneAddress { get; set; }
        public string? EndDate { get; set; }
        [Display(Name = "Add hours")]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than 0")]
        public int AdditionalHours { get; set; }
    }
}
