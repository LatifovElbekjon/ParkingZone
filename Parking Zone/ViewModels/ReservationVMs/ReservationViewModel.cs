using ParkingZone.Entities;
using System.ComponentModel.DataAnnotations;

namespace ParkingZone.ViewModels.ReservationVMs
{
    public class ReservationViewModel
    {
        public ReservationViewModel() { }
        public ReservationViewModel(Slot slot, string startTime, int duration)
        {
            StartTime = startTime;
            Duration = duration;
            SlotId = slot.Id;
            SlotNumber = slot.Number;
            ZoneName = slot.Zone.Name;
        }

        [DateGreaterThanOrEqualToNow(ErrorMessage = "Start time must be greater than or equal to the current time.")]
        public string StartTime { get; set; } 
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than 0")]
        public int Duration { get; set; }
        public Guid SlotId { get; set; }
        public int? SlotNumber { get; set; }
        public string? ZoneName { get; set; }

        public Reservation MapToModel()
        {
            return new Reservation()
            {
                SlotId = SlotId,
                Duration = Duration,
                StartTime = DateTime.Parse(StartTime)
            };
        }
    }
}
