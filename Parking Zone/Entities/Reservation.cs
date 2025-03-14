using ParkingZone.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingZone.Entities
{
    public class Reservation
    {
        [Key]
        public Guid Id { get; set; }
        public virtual Slot Slot { get; set; }
        public Guid SlotId { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string UserId { get; set; }
        [NotMapped]
        public ReservationStatus Status
        {
            get
            {
                if(StartTime <= DateTime.Now && DateTime.Now < StartTime.AddHours(Duration))
                {
                    return ReservationStatus.InProgress;
                }
                else if(StartTime < DateTime.Now && DateTime.Now >= StartTime.AddHours(Duration))
                {
                    return ReservationStatus.Finished;
                }
                else
                {
                    return ReservationStatus.Pending;
                }
            }
        }
    }
}
