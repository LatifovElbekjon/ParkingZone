namespace ParkingZone.ViewModels.ZoneVMs;

public class EditParkingZoneViewModel
{
    public EditParkingZoneViewModel() { }
    public EditParkingZoneViewModel(ParkingZone.Entities.ParkingZone zone)
    {
        Id = zone.Id;
        Name = zone.Name;
        Address = zone.Address;
    }

    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }

    public void MapToModel(ParkingZone.Entities.ParkingZone zone)
    {
        zone.Name = Name;
        zone.Address = Address;
    }
}
