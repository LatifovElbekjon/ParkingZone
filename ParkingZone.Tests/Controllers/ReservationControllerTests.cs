using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
using Moq;
using ParkingZone.Areas.Admin.Controllers;
using ParkingZone.Controllers;
using ParkingZone.Entities;
using ParkingZone.Repositories;
using ParkingZone.Services;
using ParkingZone.ViewModels.ReservationVMs;
using ParkingZone.ViewModels.SlotVMs;
using ParkingZone.ViewModels.ZoneVMs;
using System.Globalization;
using System.Security.Claims;
using System.Security.Policy;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ParkingZone.Tests.Controllers
{
    public class ReservationControllerTests
    {
        private readonly Guid _testZoneGUID = new Guid("02a3f2c6-25d6-4d9d-9f0c-43b2d96eecd1");
        private readonly Guid _testSlotGUID = new Guid("02a3f2c6-25d6-4d9d-9f0c-43b2d96eecd2");
        private readonly string _testUserId = new Guid("21714886-d382-4a5c-98d1-34ebfc2f53d5").ToString();
        private string _testStartTime = new DateTime(2024, 01, 27, 12, 00, 00).ToString();
        private int _testDuration = 4;

        private readonly Mock<IParkingZoneService> mockZoneService;
        private readonly Mock<ISlotService> mockSlotService;
        private readonly Mock<IReservationService> mockReservationService;
        private readonly ReservationsController controller;

        private readonly Slot _testSlotRecord = new Slot()
        {
            Id = new Guid("02a3f2c6-25d6-4d9d-9f0c-43b2d96eecd2"),
            Number = 1,
            IsAvailableForBooking = true,
            Tariff = Enums.ESlotCategory.Econom,
            ZoneId = new Guid("02a3f2c6-25d6-4d9d-9f0c-43b2d96eecd1"),
            Zone = new Entities.ParkingZone()
            {
                Name = "Mustaqillik"
            }
        };

        private readonly Entities.ParkingZone _testZoneRecord = new Entities.ParkingZone()
        {
            Id = new Guid("02a3f2c6-25d6-4d9d-9f0c-43b2d96eecd1"),
            Name = "Mustaqillik",
            Address = "Buxoro"
        };

        public ReservationControllerTests()
        {
            mockZoneService = new Mock<IParkingZoneService>();
            mockSlotService = new Mock<ISlotService>();
            mockReservationService = new Mock<IReservationService>();
            controller = new ReservationsController(mockSlotService.Object, mockZoneService.Object, mockReservationService.Object);
        }

        #region FreeSlots
        [Fact]
        public void GivenNothing_WhenCalledFreeSlots_ThenReturnedViewResultWithViewModel()
        {
            // Arrange
            var zones = new List<Entities.ParkingZone>()
            {
                _testZoneRecord
            };

            var expectedFreeSlotsVM = new CreateReservationViewModel();
            expectedFreeSlotsVM.ZoneId = _testZoneGUID;

            mockZoneService
                .Setup(service => service.GetAll())
                .Returns(zones);

            // Act
            var result = controller.FreeSlots();

            // Assert
            Assert.IsAssignableFrom<CreateReservationViewModel>((result as ViewResult)!.Model);   
            mockZoneService.Verify(d => d.GetAll(), Times.Once);
        }
        #endregion

        #region ShowFreeSlots
        [Fact]
        public void GivenCreateReservionVMWithIdOfNotExistingZone_WhenCalledShowFreeSlots_ThenZoneServiceCallesOnceAndReturnedBadRequest()
        {
            //Append
            var _testStartTime = new DateTime(2024, 07, 17, 02, 00, 00);
            var createVM = new CreateReservationViewModel()
            {
                Duration = _testDuration,
                StartTime = _testStartTime,
                ZoneId = _testZoneGUID
            };
            mockZoneService.Setup(c => c.GetById(_testZoneGUID));

            //Act
            var result = controller.ShowFreeSlots(createVM);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
            mockZoneService.Verify(x => x.GetById(_testZoneGUID), Times.Once);
        }

        [Fact] 
        public void GivencCeateReservationVM_WhenCalledShowFreeSlots_ThenZoneAndSlotServicesCallesOnceReturnedViewResult()
        {
            //Arrange
            _testSlotRecord.Reservations = new List<Reservation>()
            {
                new()
                {
                    StartTime = new DateTime(2024, 01, 27, 12, 00, 00),
                    Duration = 1
                }
            };
            var _testStartTime = new DateTime(2024, 07, 17, 02, 00, 00);
            var createVM = new CreateReservationViewModel()
            {
                Duration = _testDuration,
                StartTime = _testStartTime,
                ZoneId = _testZoneGUID
            };
            var freeSlots = new List<Slot>()
            {
                new()
                {
                    ZoneId = _testZoneGUID,
                    Number = _testSlotRecord.Number,
                    Id = _testSlotRecord.Id,
                    IsAvailableForBooking = _testSlotRecord.IsAvailableForBooking,  
                    Tariff = _testSlotRecord.Tariff,
                    Reservations = _testSlotRecord.Reservations
                }
            };

            var freeslotsVM = new FreeSlotsViewModel();
            freeslotsVM.FreeSlots = new List<SlotsItemViewModel>()
            {
                new()
                {
                    Id = _testSlotGUID,
                    IsAvailableForBooking = _testSlotRecord.IsAvailableForBooking,
                    Number = _testSlotRecord.Number,
                    Tariff = _testSlotRecord.Tariff,
                    CanEditAndDelete = true
                }
            };
            freeslotsVM.StartTime = _testStartTime;
            freeslotsVM.Duration = _testDuration;
            freeslotsVM.ZoneName = _testZoneRecord.Name;
            mockZoneService.Setup(c => c.GetById(_testZoneGUID)).Returns(_testZoneRecord);
            mockSlotService.Setup(v=>v.GetFreeSlotsByZoneId(_testZoneGUID, _testStartTime, _testDuration)).Returns(freeSlots);

            //Act
            var result = controller.ShowFreeSlots(createVM);

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.NotNull(result as ViewResult);
            Assert.IsAssignableFrom<FreeSlotsViewModel>((result as ViewResult)!.Model);
            Assert.Equal(JsonSerializer.Serialize(freeslotsVM), JsonSerializer.Serialize((result as ViewResult)!.Model));
            mockSlotService.Verify(d => d.GetFreeSlotsByZoneId(_testZoneGUID, _testStartTime, _testDuration), Times.Once);
            mockZoneService.Verify(x=>x.GetById(_testZoneGUID), Times.Once);
        }
        #endregion

        #region Reserve
        [Fact]
        public void GivenNotExistingSlotId_WhenCalledGetReserve_ThenSlotServiceCalledOnceAndReturnedNotFound()
        {
            //Append
            mockSlotService.Setup(x => x.GetById(_testSlotGUID));

            //Act
            var result = controller.Reserve(_testSlotGUID, _testStartTime, _testDuration);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
            mockSlotService.Verify(c=>c.GetById(_testSlotGUID), Times.Once);
        }

        [Fact]
        public void GivenSlotIdWithIdOfNotExistingZone_WhenCalledGetReserve_ThenZoneServiceCalledOnceAndReturnedNotFound()
        {
            //Append
            mockSlotService.Setup(x => x.GetById(_testSlotGUID)).Returns(_testSlotRecord);
            mockZoneService.Setup(x => x.GetById(_testZoneGUID));

            //Act
            var result = controller.Reserve(_testSlotGUID, _testStartTime, _testDuration);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
            mockSlotService.Verify(c => c.GetById(_testSlotGUID), Times.Once);
            mockZoneService.Verify(c => c.GetById(_testZoneGUID), Times.Once);
        }

        [Fact]
        public void GivenSlotIdAndTimePeriod_WhenCalledGetReserve_ThenSlotAndZoneServicesCalledOnceAndReturnedViewResult()
        {
            //Append
            var _testReservationVM = new ReservationViewModel()
            {
                SlotId = _testSlotGUID,
                ZoneName = _testZoneRecord.Name,
                SlotNumber = _testSlotRecord.Number,
                StartTime = _testStartTime,
                Duration = _testDuration
            };
            mockSlotService.Setup(x => x.GetById(_testSlotGUID)).Returns(_testSlotRecord);
            mockZoneService.Setup(x => x.GetById(_testZoneGUID)).Returns(_testZoneRecord);

            //Act
            var result = controller.Reserve(_testSlotGUID, _testStartTime, _testDuration);

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ReservationViewModel>((result as ViewResult)!.Model);
            mockSlotService.Verify(c => c.GetById(_testSlotGUID), Times.Once);
            mockZoneService.Verify(c => c.GetById(_testZoneGUID), Times.Once);
        }

        [Fact]
        public void GivenSlotIdWithIdOfNotExistingSlot_WhenCalledPostReserve_ThenSlotServiceCalledOnceAndReturnedNotFound()
        {
            //Append
            var _testReservationVM = new ReservationViewModel()
            {
                SlotId = _testSlotGUID,
                ZoneName = _testZoneRecord.Name,
                SlotNumber = _testSlotRecord.Number,
                StartTime = _testStartTime,
                Duration = _testDuration
            };
            mockSlotService.Setup(x => x.GetById(_testSlotGUID));

            //Act
            var result = controller.Reserve(_testReservationVM);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
            mockSlotService.Verify(c => c.GetById(_testSlotGUID), Times.Once);
        }

        [Fact]
        public void GivenReservationVMWithSlotIdThatIsNotFreeStartTimeAndDuration_WhenCalledPostReserve_ThenReturnedModelError()
        {
            //Arrange
            var _testReservationVM = new ReservationViewModel()
            {
                SlotId = _testSlotGUID,
                ZoneName = _testZoneRecord.Name,
                SlotNumber = _testSlotRecord.Number,
                StartTime = _testStartTime,
                Duration = _testDuration
            };
            mockSlotService.Setup(x => x.GetById(_testSlotGUID)).Returns(_testSlotRecord);
            mockSlotService.Setup(f=>f.IsSlotFree(_testSlotRecord, DateTime.Parse(_testStartTime), _testDuration)).Returns(false);
            controller.ModelState.AddModelError("Duration", "Slot is not free with this duration");
            controller.ModelState.AddModelError("StartTime", "Slot is not free with this time");

            //Act
            var result = controller.Reserve(_testReservationVM);

            //Assert
            Assert.IsType<ReservationViewModel>((result as ViewResult)!.Model);
            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public void GivenCorrectReservationVM_WhenCalledPostReserve_ThenSlotServiceCalledThriceAndReturnedViewResult()
        {
            //Arrange
            var _testReservationVM = new ReservationViewModel()
            {
                SlotId = _testSlotGUID,
                StartTime = _testStartTime,
                Duration = _testDuration
            };
            var mockHttpContext = new Mock<HttpContext>();

            var mockClaimsPrincipal = CreateMockClaimsPrincipal();

            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = mockClaimsPrincipal }
            };
            controller.ControllerContext = controllerContext;
            mockSlotService.Setup(x => x.GetById(_testSlotGUID)).Returns(_testSlotRecord);
            mockSlotService.Setup(f => f.IsSlotFree(_testSlotRecord, DateTime.Parse(_testStartTime), _testDuration)).Returns(true);
            mockReservationService.Setup(c => c.Add(It.IsAny<Reservation>()));

            //Act
            var result = controller.Reserve(_testReservationVM);

            //Assert
            mockSlotService.Verify(f => f.IsSlotFree(_testSlotRecord, DateTime.Parse(_testStartTime), _testDuration), Times.Once);
            mockSlotService.Verify(c => c.GetById(_testSlotGUID), Times.Once);
            Assert.True(controller.ModelState.IsValid);
            Assert.IsType<RedirectToActionResult>(result);
            mockReservationService.Verify(s => s.Add(It.IsAny<Reservation>()), Times.Once);
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
    }
}
