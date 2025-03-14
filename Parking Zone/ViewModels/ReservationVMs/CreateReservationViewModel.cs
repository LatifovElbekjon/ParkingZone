using ParkingZone.Entities;
using System.ComponentModel.DataAnnotations;

namespace ParkingZone.ViewModels.ReservationVMs
{
    public class CreateReservationViewModel
    {
        [Required(ErrorMessage = "Zone is required")]
        public Guid ZoneId { get; set; }
        [Required(ErrorMessage = "Start time is required")]
        //[DateGreaterThanOrEqualToNow(ErrorMessage = "Start time must be greater than or equal to the current time.")]
        public DateTime StartTime { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than 0")]
        public int Duration { get; set; }
    }
}
