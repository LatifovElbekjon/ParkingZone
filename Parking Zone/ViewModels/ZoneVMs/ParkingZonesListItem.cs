using ParkingZone.Entities;

namespace ParkingZone.ViewModels.ZoneVMs;

public class ParkingZonesListItemVM
{
    public ParkingZonesListItemVM() { }
    public ParkingZonesListItemVM(ParkingZone.Entities.ParkingZone parkingZone)
    {
        Id = parkingZone.Id;
        Name = parkingZone.Name;
        Address = parkingZone.Address;
        EstablishedDate = parkingZone.EstablishedDate;
        FreeSlotsCount = parkingZone.Slots.Count(d => d.IsAvailableForBooking == true && !d.Reservations.Any(r =>
            r.Status == Enums.ReservationStatus.InProgress
            || r.Status == Enums.ReservationStatus.Pending));
        CurrentUsingSlotsCount = parkingZone.Slots.Count(d => d.IsAvailableForBooking == true && d.Reservations.Any(r =>
            r.Status == Enums.ReservationStatus.InProgress
            || r.Status == Enums.ReservationStatus.Pending));
        AllSlots = parkingZone.Slots.Count();
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public DateTime EstablishedDate { get; set; }
    public int AllSlots { get; set; }
    public int FreeSlotsCount { get; set; }
    public int CurrentUsingSlotsCount { get; set; }
}
