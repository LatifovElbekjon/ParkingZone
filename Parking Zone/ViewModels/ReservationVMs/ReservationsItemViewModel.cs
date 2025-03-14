using ParkingZone.Entities;
using ParkingZone.Enums;

namespace ParkingZone.ViewModels.ReservationVMs
{
    public class ReservationsItemViewModel
    {
        public ReservationsItemViewModel() { }
        public ReservationsItemViewModel(Reservation reservation) 
        {
            Id = reservation.Id;
            StartTime = reservation.StartTime.ToString("g");
            Duration = reservation.Duration;
            SlotNumber = reservation.Slot.Number;
            ZoneAdress = reservation.Slot.Zone.Name + ", " + reservation.Slot.Zone.Address;
            Status = reservation.Status;
        }

        public Guid Id { get; set; }
        public string StartTime { get; set; }
        public int Duration { get; set; }
        public int SlotNumber { get; set; }
        public string ZoneAdress { get; set; }
        public ReservationStatus Status { get; set; }
    }
}
