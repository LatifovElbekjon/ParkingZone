using Microsoft.AspNetCore.Identity;

namespace ParkingZone.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
