namespace ParkingZone.Entities
{
    public class ParkingZone
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime EstablishedDate { get; set; }
        public virtual List<Slot> Slots { get; set; }
    }
}
