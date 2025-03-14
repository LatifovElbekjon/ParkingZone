using Microsoft.AspNetCore.Mvc.Rendering;
using ParkingZone.Entities;
using ParkingZone.ViewModels.SlotVMs;

namespace ParkingZone.ViewModels.ReservationVMs
{
    public class FreeSlotsViewModel
    {
        public IEnumerable<SlotsItemViewModel>? FreeSlots { get; set; }
        public string ZoneName { get; set; }
        public Guid ZoneId { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
    }
}
