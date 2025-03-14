using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ParkingZone.Areas.User.Controllers;
using ParkingZone.Entities;
using ParkingZone.Services;
using ParkingZone.ViewModels.ReservationVMs;
using System.Security.Claims;
using System.Text.Json;

namespace ParkingZone.Tests.Controllers.User
{
    public class ReservationsControllerTests
    {
        private readonly string _testUserId = new Guid("02a3f2c6-25d6-4d9d-9f0c-43b2d96eecd2").ToString();
        private readonly Guid _testReservationGUID = new Guid("12a3f2c6-25d6-4d9d-9f0c-43b2d96eecd2");
        private readonly Mock<IReservationService> mockReservationService;
        private readonly Mock<ISlotService> mockSlotService;
        private readonly ReservationsController controller;
        private string _testStartTime = new DateTime(2024, 01, 27, 12, 00, 00).ToString();
        private int _testDuration = 4;

        private readonly Slot _testSlotRecord = new Slot()
        {
            Id = new Guid("02a3f2c6-25d6-4d9d-9f0c-53b2d96eecd2"),
            Number = 1,
            IsAvailableForBooking = true,
            Tariff = Enums.ESlotCategory.Econom,
            ZoneId = new Guid("02a3f2c6-25d6-4d9d-9f0c-43b2d96eecd1"),
            Zone = new Entities.ParkingZone()
            {
                Name = "Mustaqillik"
            }
        };

        public ReservationsControllerTests()
        {
            mockReservationService = new Mock<IReservationService>();
            mockSlotService = new Mock<ISlotService>();
            controller = new ReservationsController(mockSlotService.Object, mockReservationService.Object);
        }

        #region Index
        [Fact]
        public void GivenNothing_WhenGetIndexIsCalled_ThenExpectedReservationsItemVMsReturned()
        {
            //Arrange
            var reservations = new List<Reservation>()
            {
                new()
                {
                    StartTime = DateTime.Now.AddHours(-1),
                    Duration = 3,
                    SlotId = _testSlotRecord.Id,
                    UserId = _testUserId,
                    Id = new Guid("02a3f2c6-25d6-4d9d-9f0c-43b2d96eecd1"),
                    Slot = _testSlotRecord
                },
                new()
                {
                    StartTime = DateTime.Now.AddHours(-3),
                    Duration = 1,
                    SlotId = _testSlotRecord.Id,
                    UserId = _testUserId,
                    Id = _testReservationGUID,
                    Slot = _testSlotRecord
                }
            };

            var expectedReservationsItemVMs = new List<ReservationsItemViewModel>()
            {
                new()
                {
                    Id = new Guid("02a3f2c6-25d6-4d9d-9f0c-43b2d96eecd1"),
                    StartTime = DateTime.Now.AddHours(-1).ToString("g"),
                    Duration = 3,
                    SlotNumber = 1,
                    ZoneAdress = "Mustaqillik, ",
                    Status = Enums.ReservationStatus.InProgress
                },
                new()
                {
                    Id = _testReservationGUID,  
                    StartTime = DateTime.Now.AddHours(-3).ToString("g"),
                    Duration = 1,
                    SlotNumber = 1,
                    ZoneAdress = "Mustaqillik, ",
                    Status= Enums.ReservationStatus.Finished
                }
            };

            mockReservationService
                .Setup(s => s.GetReservationsByUserId(It.IsAny<string>()))
                .Returns(reservations);

            var HttpContextMock = new Mock<HttpContext>();

            var mockClaimsPrincipal = CreateMockClaimsPrincipal();

            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = mockClaimsPrincipal }
            };
            controller.ControllerContext = controllerContext;

            //Act
            var result = controller.Index();

            //Assert
            Assert.IsType<ViewResult>(result);
            mockReservationService.Verify(d => d.GetReservationsByUserId(It.IsAny<string>()), Times.Once);
            Assert.Equal(JsonSerializer.Serialize(expectedReservationsItemVMs), JsonSerializer.Serialize((result as ViewResult)!.Model));
        }

        private ClaimsPrincipal CreateMockClaimsPrincipal()
        {
            var claims = new List<Claim>()
            {
                new(ClaimTypes.NameIdentifier, _testUserId)
            };
            var identity = new ClaimsIdentity(claims);

            return new ClaimsPrincipal(identity);
        }
        #endregion

        #region Prolong
        [Fact]
        public void GivenNotExistingReservationId_WhenCalledGetProlong_ThenCalledReservationServiceAndReturnedNotFound()
        {
            //Arrange
            mockReservationService.Setup(s => s.GetById(_testReservationGUID));

            //Act
            var result = controller.Prolong(_testReservationGUID);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
            mockReservationService.Verify(d => d.GetById(_testReservationGUID), Times.Once);
        }

        [Fact]
        public void GivenExistingReservationId_WhenCalledGetProlong_ThenCalledReservationServiceAndReturnedViewResult()
        {
            //Arrange
            var reservation = new Reservation()
            {
                Id = _testReservationGUID,
                StartTime = DateTime.Now.AddHours(1),
                Duration = 2,
                UserId = _testUserId,
                SlotId = _testSlotRecord.Id,
                Slot = _testSlotRecord
            };
            var prolongVM = new ProlongViewModel()
            {
                ReservationId = reservation.Id,
                SlotNumber = _testSlotRecord.Number,
                EndDate = reservation.StartTime.AddHours(reservation.Duration).ToString("g"),
                ZoneAddress = null
            };
            mockReservationService.Setup(s => s.GetById(_testReservationGUID)).Returns(reservation);

            //Act
            var result = controller.Prolong(_testReservationGUID);

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal(JsonSerializer.Serialize(prolongVM), JsonSerializer.Serialize((result as ViewResult)!.Model));
            mockReservationService.Verify(d => d.GetById(_testReservationGUID), Times.Once);
        }

        [Fact]
        public void GivenProlongVmWithNotExistingReservationId_WhenCalledPostProlong_ThenCalledReservationServiceAndReturnedNotFound()
        {
            //Arrange
            var prolongVM = new ProlongViewModel()
            {
                ReservationId = _testReservationGUID
            };
            mockReservationService.Setup(s => s.GetById(_testReservationGUID));

            //Act
            var result = controller.Prolong(prolongVM);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
            mockReservationService.Verify(d => d.GetById(_testReservationGUID), Times.Once);
        }

        [Fact]
        public void GivenProlongVMWithIdOfNotExistingSlot_WhenCalledPostProlong_ThenCalledSlotServiceAndReturnedNotFound()
        {
            //Arrange
            var reservation = new Reservation()
            {
                Id = _testReservationGUID,
                StartTime = DateTime.Now.AddHours(1),
                Duration = 2,
                UserId = _testUserId,
                SlotId = _testSlotRecord.Id,
                Slot = _testSlotRecord
            };
            var prolongVM = new ProlongViewModel()
            {
                ReservationId = reservation.Id,
                SlotNumber = _testSlotRecord.Number,
                EndDate = reservation.StartTime.AddHours(reservation.Duration).ToString("g"),
                ZoneAddress = null
            };
            mockReservationService.Setup(s => s.GetById(_testReservationGUID)).Returns(reservation);
            mockSlotService.Setup(h => h.GetById(_testSlotRecord.Id));

            //Act
            var result = controller.Prolong(prolongVM);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
            mockReservationService.Verify(d => d.GetById(_testReservationGUID), Times.Once);
            mockSlotService.Verify(c => c.GetById(_testSlotRecord.Id), Times.Once);
        }

        [Fact]
        public void GivenProlongVMWithSlotIdThatIsNotFreeStartTimeAndDuration_WhenCalledPostProlong_ThenReturnedModelError()
        {
            //Arrange
            _testSlotRecord.Reservations = new List<Reservation>();
            var reservation = new Reservation()
            {
                Id = _testReservationGUID,
                StartTime = DateTime.Now,
                Duration = 2,
                UserId = _testUserId,
                SlotId = _testSlotRecord.Id,
                Slot = _testSlotRecord
            };
            var prolongVM = new ProlongViewModel()
            {
                ReservationId = reservation.Id,
                SlotNumber = _testSlotRecord.Number,
                EndDate = reservation.StartTime.AddHours(reservation.Duration).ToString("g"),
                ZoneAddress = null,
                AdditionalHours = 1
            };
            mockReservationService.Setup(s => s.GetById(_testReservationGUID)).Returns(reservation);
            mockSlotService.Setup(h => h.GetById(_testSlotRecord.Id)).Returns(_testSlotRecord);
            mockSlotService.Setup(f => f.IsSlotFree(_testSlotRecord, reservation.StartTime.AddHours(reservation.Duration), prolongVM.AdditionalHours)).Returns(false);
            controller.ModelState.AddModelError("AdditionalHours", "Slot not free this time");

            //Act
            var result = controller.Prolong(prolongVM);

            //Assert
            mockReservationService.Verify(d => d.GetById(_testReservationGUID), Times.Once);
            mockSlotService.Verify(d => d.IsSlotFree(_testSlotRecord, reservation.StartTime.AddHours(reservation.Duration), prolongVM.AdditionalHours), Times.Once);
            mockSlotService.Verify(c => c.GetById(_testSlotRecord.Id), Times.Once);
            Assert.IsType<ProlongViewModel>((result as ViewResult)!.Model);
            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public void GivenProlongVM_WhenCalledPostProlong_ThenReturnedSuccesfullyViewResult()
        {
            //Arrange
            _testSlotRecord.Reservations = new List<Reservation>();
            var reservation = new Reservation()
            {
                Id = _testReservationGUID,
                StartTime = DateTime.Now,
                Duration = 2,
                UserId = _testUserId,
                SlotId = _testSlotRecord.Id,
                Slot = _testSlotRecord
            };
            var prolongVM = new ProlongViewModel()
            {
                ReservationId = reservation.Id,
                SlotNumber = _testSlotRecord.Number,
                EndDate = reservation.StartTime.AddHours(reservation.Duration).ToString("g"),
                ZoneAddress = null,
                AdditionalHours = 1
            };
            mockReservationService.Setup(s => s.GetById(_testReservationGUID)).Returns(reservation);
            mockSlotService.Setup(h => h.GetById(_testSlotRecord.Id)).Returns(_testSlotRecord);
            mockSlotService.Setup(f => f.IsSlotFree(_testSlotRecord, reservation.StartTime.AddHours(reservation.Duration), prolongVM.AdditionalHours)).Returns(true);
            mockReservationService.Setup(p => p.Prolong(reservation, prolongVM.AdditionalHours));

            //Act
            var result = controller.Prolong(prolongVM);

            //Assert
            mockReservationService.Verify(d => d.GetById(_testReservationGUID), Times.Once);
            mockSlotService.Verify(c => c.GetById(_testSlotRecord.Id), Times.Once);
            mockSlotService.Verify(d => d.IsSlotFree(_testSlotRecord, reservation.StartTime.AddHours(reservation.Duration), prolongVM.AdditionalHours), Times.Once);
            mockReservationService.Verify(d => d.Prolong(reservation, prolongVM.AdditionalHours), Times.Once);
            Assert.IsType<RedirectToActionResult>(result);
            Assert.True(controller.ModelState.IsValid);
        }
        #endregion
    }
}