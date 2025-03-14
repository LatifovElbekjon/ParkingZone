using Microsoft.EntityFrameworkCore;
using ParkingZone.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingZone.Entities
{
    public class Slot
    {
        [Key]
        public Guid Id { get; set; }
        public int Number { get; set; }
        public bool IsAvailableForBooking { get; set; }
        public virtual ParkingZone Zone { get; set; }
        public Guid ZoneId { get; set; }
        public ESlotCategory Tariff { get; set; }
        public virtual List<Reservation> Reservations { get; set; } 
    }
}
