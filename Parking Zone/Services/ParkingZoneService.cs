using ParkingZone.Entities;
using ParkingZone.Repositories;

namespace ParkingZone.Services
{
    public class ParkingZoneService : Service<ParkingZone.Entities.ParkingZone>, IParkingZoneService
    {
        public ParkingZoneService(IParkingZoneRepository repository) : base(repository)
        {
        }

        public override void Add(ParkingZone.Entities.ParkingZone model)
        {
            model.Id = Guid.NewGuid();
            model.EstablishedDate = DateTime.Now;
            base.Add(model);
        }
    }
}
