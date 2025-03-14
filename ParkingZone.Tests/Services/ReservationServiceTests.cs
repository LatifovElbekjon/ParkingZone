using Moq;
using ParkingZone.Entities;
using ParkingZone.Repositories;
using ParkingZone.Services;
using System.Text.Json;

namespace ParkingZone.Tests.Services
{
    public class ReservationServiceTests
    {
        private readonly Reservation _testReservation = new Reservation()
        {
            Id = new Guid("9135b607-5b3c-4c41-4b26-08dca2396615"),
            SlotId = new Guid("d1247492-2852-4b39-b2bb-9b655f562569"),
            StartTime = new DateTime(2024, 07, 12, 14, 00, 00),
            Duration = 3,
            UserId = "21714886-d382-4a5c-98d1-34ebfc2f53d5"
        };
        private readonly Guid testSlotId = new Guid("d1247492-2852-4b39-a2bb-9b655f562559");
        private readonly Guid testZoneId = new Guid("d1247492-2852-4b39-a2bb-9b655f562558");
        private readonly Slot testSlot = new Slot()
        {
            Id = new Guid("d1247492-2852-4b39-a2bb-9b655f562559"),
            Number = 1,
            Tariff = Enums.ESlotCategory.Econom,
            ZoneId = new Guid("d1247492-2852-4b39-a2bb-9b655f562558"),
            IsAvailableForBooking = true,
            Reservations = new List<Reservation>()
        };
        private readonly Entities.ParkingZone testParkingZone = new Entities.ParkingZone()
        {
            Id = new Guid("d1247492-2852-4b39-a2bb-9b655f562558"),
            Name = "Yama",
            Address = "Mars",
            Slots = new List<Slot>()
        };
        private readonly string _testUserId = new Guid("21714886-d382-4a5c-98d1-34ebfc2f53d5").ToString();

        #region Add
        [Fact]
        public void GivenTestReservation_WhenCalledReserve_ThenReservationRepositoryCalledOnce()
        {
            //Arrange
            Mock<IReservationRepository> reservationRepo = new Mock<IReservationRepository>();
            reservationRepo.Setup(c => c.Add(_testReservation));
            reservationRepo.Setup(c => c.Save());
            var reservationService = new ReservationService(reservationRepo.Object);

            //Act
            reservationService.Add(_testReservation);

            //Assert
            reservationRepo.Verify(c=>c.Add(_testReservation), Times.Once);
            reservationRepo.Verify(c=>c.Save(), Times.Once);
        }
        #endregion

        #region GetByUserId
        [Fact]
        public void GivenUserID_WhenCalledGetReservationsByUserId_ThenReturnedExpectedReservations()
        {
            //Arrange
            var allReservations = new List<Reservation>()
            {
                new()
                {
                    UserId = Guid.NewGuid().ToString()
                },
                new()
                {
                    UserId = _testUserId
                },
                new()
                {
                    UserId = Guid.NewGuid().ToString()  
                },
                new()
                {
                    UserId = _testUserId
                }
            };
            var expectedReservations = new List<Reservation>()
            {
                new()
                {
                    UserId = _testUserId
                },
                new()
                {
                    UserId = _testUserId
                }
            };
            var reserveRepo = new Mock<IReservationRepository>();
            reserveRepo.Setup(d=>d.GetAll()).Returns(allReservations);  
            var reservationService = new ReservationService(reserveRepo.Object);

            //Act
            var result = reservationService.GetReservationsByUserId(_testUserId);

            //Assert
            Assert.Equal(JsonSerializer.Serialize(expectedReservations), JsonSerializer.Serialize(result));
            reserveRepo.Verify(s=>s.GetAll(), Times.Once);  
        }
        #endregion
    }
}
