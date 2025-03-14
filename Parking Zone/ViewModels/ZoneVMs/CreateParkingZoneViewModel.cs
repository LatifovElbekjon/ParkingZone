using System.ComponentModel.DataAnnotations;

namespace ParkingZone.ViewModels.ZoneVMs;

public class CreateParkingZoneViewModel
{
    [MaxLength(20)]
    public required string Name { get; set; }
    public required string Address { get; set; }

    public ParkingZone.Entities.ParkingZone MapToModel()
    {
        return new ParkingZone.Entities.ParkingZone()
        {
            Address = Address,
            Name = Name
        };
    }
}
