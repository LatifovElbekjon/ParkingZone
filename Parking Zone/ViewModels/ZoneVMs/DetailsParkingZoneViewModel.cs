namespace ParkingZone.ViewModels.ZoneVMs;

public class DetailsParkingZoneViewModel
{
    public DetailsParkingZoneViewModel() { }

    public DetailsParkingZoneViewModel(ParkingZone.Entities.ParkingZone zone)
    {
        Id = zone.Id;
        Name = zone.Name;
        Address = zone.Address;
        EstablishedDate = zone.EstablishedDate;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public DateTime EstablishedDate { get; set; }

}
