using Microsoft.AspNetCore.Mvc;
using Moq;
using ParkingZone.Areas.Admin.Controllers;
using ParkingZone.Services;
using System.Text.Json;
using ParkingZone.Entities;
using ParkingZone.ViewModels.SlotVMs;
using ParkingZone.Enums;

namespace ParkingZone.Tests.Controllers.Test.Admin
{
    public class SlotControllerTests
    {
        private readonly Guid testSlotId = new Guid("d1247492-2852-4b39-a2bb-9b655f562559");
        private readonly Guid testZoneId = new Guid("d1247492-2852-4b39-a2bb-9b655f562558");
        private readonly Entities.ParkingZone testParkingZone = new Entities.ParkingZone()
        {
            Id = new Guid("d1247492-2852-4b39-a2bb-9b655f562558"),
            Name = "elbek",
            Address = "Samarqand"
        };
        private readonly Slot testSlot = new Slot()
        {
            Id = new Guid("d1247492-2852-4b39-a2bb-9b655f562559"),
            Number = 1,
            Tariff = Enums.ESlotCategory.Econom,
            ZoneId = new Guid("d1247492-2852-4b39-a2bb-9b655f562558"),
            IsAvailableForBooking = true
        };

        #region Index
        [Fact]
        public void GivenIdNotExistingZone_WhenGetIndexIsCalled_ThenZoneServiceIsCalledOnceAndReturnedBadRequest()
        {
            //Arrange
            Mock<IParkingZoneService> mockZoneService = new Mock<IParkingZoneService>();
            mockZoneService.Setup(n => n.GetById(testZoneId));

            //Act
            var slotsController = new SlotsController(Mock.Of<ISlotService>(), mockZoneService.Object);
            var result = slotsController.Index(testZoneId);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
            mockZoneService.Verify(n => n.GetById(testZoneId), Times.Once());
        }

        [Fact]
        public void GivenZoneId_WhenGetIndexIsCalled_ThenSlotAndZoneServicesIsCalledOnceAndReturnedExceptedSlotVMs()
        {
            //Arrange
            var expectedSlots = new List<Slot>()
            {
                new Slot
                {
                    Id = new Guid("d1247492-2852-4b39-a2bb-9b655f562501"),
                    Number = 1,
                    IsAvailableForBooking = true,
                    Reservations = new List<Reservation>()
                    {
                        new()
                        {
                            StartTime = new DateTime(2024, 01, 27, 12, 00, 00),
                            Duration = 1
                        }
                    }
                },
                new Slot
                {
                    Id = new Guid("d1247492-2852-4b39-a2bb-9b655f562502"),
                    Number = 2,
                    IsAvailableForBooking = true,
                    Reservations = new List<Reservation>()
                    {
                        new()
                        {
                            StartTime = new DateTime(2024, 01, 28, 12, 00, 00),
                            Duration = 1
                        }
                    }
                },
                new Slot
                {
                    Id = new Guid("d1247492-2852-4b39-a2bb-9b655f562503"),
                    Number = 3,
                    IsAvailableForBooking = true,
                    Reservations = new List<Reservation>()
                    {
                        new()
                        {
                            StartTime = new DateTime(2024, 01, 29, 12, 00, 00),
                            Duration = 1
                        }
                    }
                }
            };

            var expectedSlotsItemVMs = new List<SlotsItemViewModel>()
            {
                new SlotsItemViewModel()
                {
                    Id = new Guid("d1247492-2852-4b39-a2bb-9b655f562501"),
                    Number = 1,
                    IsAvailableForBooking = true,
                    CanEditAndDelete = true
                },
                new SlotsItemViewModel()
                {
                    Id = new Guid("d1247492-2852-4b39-a2bb-9b655f562502"),
                    Number = 2,
                    IsAvailableForBooking = true,
                    CanEditAndDelete = true
                },
                new SlotsItemViewModel()
                {
                    Id = new Guid("d1247492-2852-4b39-a2bb-9b655f562503"),
                    Number = 3,
                    IsAvailableForBooking = true,
                    CanEditAndDelete = true
                }
            };


            Mock<ISlotService> mockSlotService = new Mock<ISlotService>();
            Mock<IParkingZoneService> mockZoneService = new Mock<IParkingZoneService>();
            mockSlotService.Setup(m => m.GetByZoneId(testZoneId)).Returns(expectedSlots);
            mockZoneService.Setup(n => n.GetById(testZoneId)).Returns(testParkingZone);

            //Act
            var slotsController = new SlotsController(mockSlotService.Object, mockZoneService.Object);
            var result = slotsController.Index(testZoneId);

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.Equal(JsonSerializer.Serialize((result as ViewResult)!.Model), JsonSerializer.Serialize(expectedSlotsItemVMs));
            mockSlotService.Verify(m => m.GetByZoneId(testZoneId), Times.Once());
            mockZoneService.Verify(n => n.GetById(testZoneId), Times.Once());
        }
        #endregion

        #region Create
        [Fact]
        public void GivenIdNotExistingZone_WhenGetCreateIsCalled_ThenZoneServiceIsCalledOnceAndReturnedNotFound()
        {
            //Arrange
            Mock<IParkingZoneService> mockZoneService = new Mock<IParkingZoneService>();
            mockZoneService.Setup(m => m.GetById(testZoneId));
            var slotController = new SlotsController(Mock.Of<ISlotService>(), mockZoneService.Object);

            //Act
            var result = slotController.Create(testZoneId);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
            mockZoneService.Verify(n=>n.GetById(testZoneId), Times.Once());
        }

        [Fact]
        public void GivenIdExistingZone_WhenCalledGetCreate_ThenReturnsEmptyViewResult()
        {
            //Arrange
            Mock<IParkingZoneService> mockZoneService = new Mock<IParkingZoneService>();
            mockZoneService.Setup(m => m.GetById(testZoneId)).Returns(testParkingZone);
            var slotController = new SlotsController(Mock.Of<ISlotService>(), mockZoneService.Object);

            //Act
            var result = slotController.Create(testZoneId);

            //Assert
            Assert.IsType<ViewResult>(result);
            mockZoneService.Verify(n => n.GetById(testZoneId), Times.Once());
        }

        [Theory]
        [InlineData(null, ESlotCategory.Super)]
        [InlineData(false, null)]
        public void GivenInvalidCreateSlotVM_WhenPostCreateIsCalled_ThenReturnedModelError(bool status, ESlotCategory tariff)
        {
            //Arrange
            var createSlotVM = new CreateSlotViewModel()
            {
                Number = 1,
                IsAvailableForBooking = status,
                Tariff = tariff,
                ZoneId = testZoneId
            };
            Mock<IParkingZoneService> mockZoneService = new Mock<IParkingZoneService>();
            mockZoneService.Setup(m => m.GetById(testZoneId)).Returns(testParkingZone);
            var slotController = new SlotsController(Mock.Of<ISlotService>(), mockZoneService.Object);
            slotController.ModelState.AddModelError("field", "Status or tariff required!");

            //Act
            var result = slotController.Create(createSlotVM);

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.False(slotController.ModelState.IsValid);
            Assert.IsType<CreateSlotViewModel>((result as ViewResult)!.Model);
            mockZoneService.Verify(n=>n.GetById(testZoneId), Times.Once);
        }

        [Fact]
        public void GivenIdNotExistingZone_WhenPostCreateIsCalled_ThenZoneServiceIsCalledOnceAndReturnedNotFound()
        {
            //Arrange
            var createSlotVM = new CreateSlotViewModel()
            {
                Number = 1,
                IsAvailableForBooking = true,
                Tariff = ESlotCategory.Econom,
                ZoneId = testZoneId
            };
            Mock<IParkingZoneService> mockZoneService = new Mock<IParkingZoneService>();
            mockZoneService.Setup(m => m.GetById(testZoneId));
            var slotController = new SlotsController(Mock.Of<ISlotService>(), mockZoneService.Object);

            //Act
            var result = slotController.Create(createSlotVM);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
            mockZoneService.Verify(n => n.GetById(testZoneId), Times.Once());
        }

        [Fact]
        public void GivenRepeatedSlotNumber_WhenPostCreateIsCalled_ThenSlotServiceCalledOnceAndReturnedModelError()
        {
            //Arrange
            var createSlotVM = new CreateSlotViewModel()
            {
                Number = 1,
                IsAvailableForBooking = true,
                Tariff = ESlotCategory.Econom,
                ZoneId = testZoneId
            };
            Mock<ISlotService> mockSlotService = new Mock<ISlotService>();
            Mock<IParkingZoneService> mockZoneService = new Mock<IParkingZoneService>();
            mockZoneService.Setup(m => m.GetById(testZoneId)).Returns(testParkingZone);
            mockSlotService.Setup(v=>v.SlotWithThisNumberExists(testZoneId, 1)).Returns(true);
            var slotController = new SlotsController(mockSlotService.Object, mockZoneService.Object);
            slotController.ModelState.AddModelError("Number", "Number already exist");

            //Act
            var result = slotController.Create(createSlotVM);

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.False(slotController.ModelState.IsValid);
            Assert.IsType<CreateSlotViewModel>((result as ViewResult)!.Model);
            mockSlotService.Verify(v=>v.SlotWithThisNumberExists(testZoneId, 1), Times.Once);
            mockZoneService.Verify(v=>v.GetById(testZoneId), Times.Once);
        }

        [Fact]
        public void GivenCreateSlotVM_WhenCalledPostCreate_ThenZoneAndSlotServicesCalledOnceAndRedirectToIndex()
        {
            //Arrange
            var createSlotVM = new CreateSlotViewModel()
            {
                IsAvailableForBooking = true,
                Tariff = ESlotCategory.Econom,
                ZoneId = testZoneId,
                Number = 1
            };
            Mock<IParkingZoneService> mockZoneService = new Mock<IParkingZoneService>();
            Mock<ISlotService> mockSlotService = new Mock<ISlotService>();
            mockZoneService.Setup(v => v.GetById(testZoneId)).Returns(testParkingZone);
            mockSlotService.Setup(m => m.Add(It.IsAny<Slot>()));
            var slotController = new SlotsController(mockSlotService.Object, mockZoneService.Object);

            //Act
            var result = slotController.Create(createSlotVM);

            //Assert
            Assert.IsType<RedirectToActionResult>(result);
            mockSlotService.Verify(p => p.Add(It.IsAny<Slot>()), Times.Once);
            mockZoneService.Verify(m=>m.GetById(testZoneId), Times.Once()); 
        }
        #endregion

        #region Edit
        [Fact]
        public void GivenIdNotExistingSlot_WhenCalledGetEdit_ThenSlotServiceCalledOnceAndReturnedBadRequest()
        {
            //Arrange
            Mock<ISlotService> mockSlotService = new Mock<ISlotService>();  
            mockSlotService.Setup(b => b.GetById(testSlotId));
            var slotController = new SlotsController(mockSlotService.Object, Mock.Of<IParkingZoneService>());

            //Act
            var result = slotController.Edit(testSlotId);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
            mockSlotService.Verify(b=>b.GetById(testSlotId), Times.Once);
        }

        [Fact]
        public void GivenIdExistingSlot_WhenCalledGetEdit_ThenSlotServiceCalledOnceAndReturnedExpectedEditSlotVM()
        {
            //Arrange
            testSlot.Reservations = new List<Reservation>()
            {
                new()
                {
                    StartTime = new DateTime(2024, 01, 27, 12, 00, 00),
                    Duration = 1
                }
            };
            var expectedEditSlotVM = new EditSlotViewModel()
            {
                Id = testSlotId,
                Number = testSlot.Number,
                IsAvailableForBooking = testSlot.IsAvailableForBooking,
                ZoneId = testSlot.ZoneId,
                Tariff = testSlot.Tariff
            };
            Mock<ISlotService> mockSlotService = new Mock<ISlotService>();
            mockSlotService.Setup(b => b.GetById(testSlotId)).Returns(testSlot);
            var slotController = new SlotsController(mockSlotService.Object, Mock.Of<IParkingZoneService>());

            //Act
            var result = slotController.Edit(testSlotId);

            //Assert
            Assert.IsType<ViewResult>(result);
            mockSlotService.Verify(b => b.GetById(testSlotId), Times.Once);
            Assert.IsAssignableFrom<EditSlotViewModel>((result as ViewResult)!.Model);
            Assert.Equal(JsonSerializer.Serialize((result as ViewResult)!.Model), JsonSerializer.Serialize(expectedEditSlotVM));
        }

        [Theory]
        [InlineData(null)]
        public void GivenInvalidEditSlotVM_WhenCalledPostEdit_ThenReturnedModelError(int number)
        {
            //Arrange
            testSlot.Reservations = new List<Reservation>()
            {
                new()
                {
                    StartTime = new DateTime(2024, 01, 27, 12, 00, 00),
                    Duration = 1
                }
            };
            var expectedEditSlotVM = new EditSlotViewModel()
            {
                Id = testSlotId,
                Number = number,
                IsAvailableForBooking = testSlot.IsAvailableForBooking,
                ZoneId = testSlot.ZoneId,
                Tariff = testSlot.Tariff
            };
            Mock<IParkingZoneService> mockZoneService = new Mock<IParkingZoneService>();
            Mock<ISlotService> mockSlotService = new Mock<ISlotService>();
            mockZoneService.Setup(n => n.GetById(testZoneId)).Returns(testParkingZone);
            mockSlotService.Setup(n => n.GetById(testSlotId)).Returns(testSlot);
            var slotController = new SlotsController(mockSlotService.Object, mockZoneService.Object);
            slotController.ModelState.AddModelError("Number", "Number is required");

            //Act
            var result = slotController.Edit(expectedEditSlotVM);

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsType<EditSlotViewModel>((result as ViewResult)!.Model);
            Assert.False(slotController.ModelState.IsValid);    
        }

        [Theory]
        [InlineData(0)]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        public void GivenNumberLessThan0_WhenCalledPostEdit_ThenReturnedModelError(int number)
        {
            //Arrange
            testSlot.Reservations = new List<Reservation>()
            {
                new()
                {
                    StartTime = new DateTime(2024, 01, 27, 12, 00, 00),
                    Duration = 1
                }
            };
            var expectedEditSlotVM = new EditSlotViewModel()
            {
                Id = testSlotId,
                Number = number,
                IsAvailableForBooking = testSlot.IsAvailableForBooking,
                ZoneId = testSlot.ZoneId,
                Tariff = testSlot.Tariff
            };
            Mock<IParkingZoneService> mockZoneService = new Mock<IParkingZoneService>();
            Mock<ISlotService> mockSlotService = new Mock<ISlotService>();
            mockZoneService.Setup(n => n.GetById(testZoneId)).Returns(testParkingZone);
            mockSlotService.Setup(n=>n.GetById(testSlotId)).Returns(testSlot);
            var slotController = new SlotsController(mockSlotService.Object, mockZoneService.Object);
            slotController.ModelState.AddModelError("Number", "Number must be greater than 0");

            //Act
            var result = slotController.Edit(expectedEditSlotVM);

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsType<EditSlotViewModel>((result as ViewResult)!.Model);
            Assert.Equal(JsonSerializer.Serialize((result as ViewResult)!.Model), JsonSerializer.Serialize(expectedEditSlotVM));
            Assert.False(slotController.ModelState.IsValid);
        }

        [Fact]
        public void GivenIdNotExisitngZone_WhenCalledPostEdit_ThenZoneServiceCalledOnceAndReturnedNotFound()
        {
            //Arrange
            var expectedEditSlotVM = new EditSlotViewModel()
            {
                Id = testSlotId,
                Number = testSlot.Number,
                IsAvailableForBooking = testSlot.IsAvailableForBooking,
                ZoneId = testSlot.ZoneId,
                Tariff = testSlot.Tariff
            };
            Mock<IParkingZoneService> mockZoneService = new Mock<IParkingZoneService>();
            mockZoneService.Setup(n => n.GetById(testZoneId));
            var slotsController = new SlotsController(Mock.Of<ISlotService>(), mockZoneService.Object);

            //Act
            var result = slotsController.Edit(expectedEditSlotVM);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
            mockZoneService.Verify(n => n.GetById(testZoneId), Times.Once());
        }

        [Fact]
        public void GivenIdNotExisitngSlot_WhenCalledPostEdit_ThenZoneAndSlotServicesCalledOnceAndReturnedNotFound()
        {
            //Arrange
            var expectedEditSlotVM = new EditSlotViewModel()
            {
                Id = testSlotId,
                Number = testSlot.Number,
                IsAvailableForBooking = testSlot.IsAvailableForBooking,
                ZoneId = testSlot.ZoneId,
                Tariff = testSlot.Tariff
            };
            Mock<IParkingZoneService> mockZoneService = new Mock<IParkingZoneService>();
            Mock<ISlotService> mockSlotService = new Mock<ISlotService>();
            mockZoneService.Setup(n => n.GetById(testZoneId)).Returns(testParkingZone);
            mockSlotService.Setup(t => t.GetById(testSlotId));
            var slotsController = new SlotsController(mockSlotService.Object, mockZoneService.Object);

            //Act
            var result = slotsController.Edit(expectedEditSlotVM);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
            mockZoneService.Verify(n => n.GetById(testZoneId), Times.Once());
            mockSlotService.Verify(v=>v.GetById(testSlotId), Times.Once());
        }

        [Fact]
        public void GivenEditSloVM_WhenCalledPostEdit_ThenSlotServiceCalledTwiceAndRedirectToIndex()
        {
            //Arrange
            testSlot.Reservations = new List<Reservation>()
            {
                new()
                {
                    StartTime = new DateTime(2024, 01, 27, 12, 00, 00),
                    Duration = 1
                }
            };
            var expectedEditSlotVM = new EditSlotViewModel()
            {
                Id = testSlotId,
                Number = testSlot.Number,
                IsAvailableForBooking = testSlot.IsAvailableForBooking,
                ZoneId = testSlot.ZoneId,
                Tariff = testSlot.Tariff
            };
            Mock<IParkingZoneService> mockZoneService = new Mock<IParkingZoneService>();
            Mock<ISlotService> mockSlotService = new Mock<ISlotService>();
            mockZoneService.Setup(n => n.GetById(testZoneId)).Returns(testParkingZone);
            mockSlotService.Setup(t => t.GetById(testSlotId)).Returns(testSlot);
            mockSlotService.Setup(y => y.Update(It.IsAny<Slot>()));
            var slotsController = new SlotsController(mockSlotService.Object, mockZoneService.Object);

            //Act
            var result = slotsController.Edit(expectedEditSlotVM);

            //Assert
            Assert.IsType<RedirectToActionResult>(result);
            mockZoneService.Verify(n => n.GetById(testZoneId), Times.Once());
            mockSlotService.Verify(n => n.GetById(testSlotId), Times.Once());
            mockSlotService.Verify(n => n.Update(It.IsAny<Slot>()), Times.Once());
        }

        [Fact]
        public void GivenSlotWithUsingReservations_WhenPostEditIsCalled_ThenSlotServiceCalledOnceAndReturnedModelError()
        {
            //Arrange
            testSlot.Reservations = new List<Reservation>()
            {
                new()
                {
                    StartTime = new DateTime(2024, 01, 27, 12, 00, 00),
                    Duration = 1
                },
                new()
                {
                    StartTime = DateTime.Now,
                    Duration = 1
                }
            };
            var expectedEditSlotVM = new EditSlotViewModel()
            {
                Id = testSlotId,
                Number = 45,
                IsAvailableForBooking = testSlot.IsAvailableForBooking,
                ZoneId = testSlot.ZoneId,
                Tariff = testSlot.Tariff
            };
            Mock<IParkingZoneService> mockZoneService = new Mock<IParkingZoneService>();
            Mock<ISlotService> mockSlotService = new Mock<ISlotService>();
            mockZoneService.Setup(n => n.GetById(testZoneId)).Returns(testParkingZone);
            mockSlotService.Setup(n => n.GetById(testSlotId)).Returns(testSlot);
            var slotController = new SlotsController(mockSlotService.Object, mockZoneService.Object);
            slotController.ModelState.AddModelError("", "This slot has using reservations, so you can't change it");

            //Act
            var result = slotController.Edit(expectedEditSlotVM);

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.False(slotController.ModelState.IsValid);
            Assert.IsType<EditSlotViewModel>((result as ViewResult)!.Model);
            mockZoneService.Verify(b => b.GetById(testZoneId), Times.Once);
            mockSlotService.Verify(k => k.GetById(testSlotId), Times.Once);
        }

        [Fact]
        public void GivenRepeatedSlotNumber_WhenPostEditIsCalled_ThenSlotServiceCalledOnceAndReturnedModelError()
        {
            //Arrange
            testSlot.Reservations = new List<Reservation>()
            {
                new()
                {
                    StartTime = new DateTime(2024, 01, 27, 12, 00, 00),
                    Duration = 1
                }
            };
            var expectedEditSlotVM = new EditSlotViewModel()
            {
                Id = testSlotId,
                Number = 45,
                IsAvailableForBooking = testSlot.IsAvailableForBooking,
                ZoneId = testSlot.ZoneId,
                Tariff = testSlot.Tariff
            };
            Mock<IParkingZoneService> mockZoneService = new Mock<IParkingZoneService>();
            Mock<ISlotService> mockSlotService = new Mock<ISlotService>();
            mockZoneService.Setup(n => n.GetById(testZoneId)).Returns(testParkingZone);
            mockSlotService.Setup(n => n.GetById(testSlotId)).Returns(testSlot);
            mockSlotService.Setup(v => v.SlotWithThisNumberExists(testZoneId, 45)).Returns(true);
            var slotController = new SlotsController(mockSlotService.Object, mockZoneService.Object);
            slotController.ModelState.AddModelError("Number", "Number already exist");

            //Act
            var result = slotController.Edit(expectedEditSlotVM);

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.False(slotController.ModelState.IsValid);
            Assert.IsType<EditSlotViewModel>((result as ViewResult)!.Model);
            mockSlotService.Verify(v => v.SlotWithThisNumberExists(testZoneId, 45), Times.Once);
            mockZoneService.Verify(b=>b.GetById(testZoneId), Times.Once);
            mockSlotService.Verify(k=>k.GetById(testSlotId), Times.Once);
        }
        #endregion

        #region Details
        [Fact]
        public void GivenIdNotExistingSlot_WhenDetailsIsCalled_ThenSlotServiceIsCalledOnceAndReturnedNotFound()
        {
            //Arrange
            Mock<ISlotService> mockSlotService = new Mock<ISlotService>();
            mockSlotService.Setup(n => n.GetById(testSlotId));

            //Act
            var slotsController = new SlotsController(mockSlotService.Object, Mock.Of<IParkingZoneService>());
            var result = slotsController.Details(testSlotId);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
            mockSlotService.Verify(n => n.GetById(testSlotId), Times.Once());
        }

        [Fact]
        public void GivenIdNotExistingZone_WhenDetailsIsCalled_ThenZoneServiceIsCalledOnceAndReturnedNotFound()
        {
            //Arrange
            Mock<ISlotService> mockSlotService = new Mock<ISlotService>();
            Mock<IParkingZoneService> mockZoneService = new Mock<IParkingZoneService>();
            mockZoneService.Setup(n => n.GetById(testZoneId));
            mockSlotService.Setup(v => v.GetById(testSlotId)).Returns(testSlot);

            //Act
            var slotsController = new SlotsController(mockSlotService.Object, mockZoneService.Object);
            var result = slotsController.Details(testSlotId);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
            mockZoneService.Verify(n => n.GetById(testZoneId), Times.Once());
            mockSlotService.Verify(n => n.GetById(testSlotId), Times.Once());
        }

        [Fact]
        public void GivenIdExistingSlot_WhenDetailsIsCalled_ThenSlotAndZoneServicesIsCalledOnceAndReturnedExpectedDetailsSlotVM()
        {
            //Arrange
            testSlot.Reservations = new List<Reservation>()
            {
                new()
                {
                    StartTime = new DateTime(2024, 01, 27, 12, 00, 00),
                    Duration = 1
                }
            };
            var expectedDetailsSlotVM = new DetailsSlotViewModel()
            {
                SlotId = testSlotId,
                SlotNumber = testSlot.Number,
                IsAvailableForBooking = testSlot.IsAvailableForBooking,
                Tariff = testSlot.Tariff,
                ZoneId = testSlot.ZoneId
            };
            Mock<ISlotService> mockSlotService = new Mock<ISlotService>();
            Mock<IParkingZoneService> mockZoneService = new Mock<IParkingZoneService>();
            mockZoneService.Setup(n => n.GetById(testZoneId)).Returns(testParkingZone);
            mockSlotService.Setup(v => v.GetById(testSlotId)).Returns(testSlot);

            //Act
            var slotsController = new SlotsController(mockSlotService.Object, mockZoneService.Object);
            var result = slotsController.Details(testSlotId);

            //Assert
            Assert.IsType<ViewResult>(result);
            mockZoneService.Verify(n => n.GetById(testZoneId), Times.Once());
            mockSlotService.Verify(n => n.GetById(testSlotId), Times.Once());
            Assert.IsAssignableFrom<DetailsSlotViewModel>((result as ViewResult)!.Model);
            Assert.Equal(JsonSerializer.Serialize((result as ViewResult)!.Model), JsonSerializer.Serialize(expectedDetailsSlotVM));
        }
        #endregion

        #region Delete
        [Fact]
        public void GivenIdNotExistingSlot_WhenGetDeleteIsCalled_ThenSlotServiceIsCalledOnceAndReturnedNotFound()
        {
            //Arrange
            Mock<ISlotService> mockSlotService = new Mock<ISlotService>();
            mockSlotService.Setup(n => n.GetById(testSlotId));
            var slotsController = new SlotsController(mockSlotService.Object, Mock.Of<IParkingZoneService>());

            //Act
            var result = slotsController.Delete(testSlotId);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
            mockSlotService.Verify(n => n.GetById(testSlotId), Times.Once());
        }

        [Fact]
        public void GivenIdNotExistingZone_WhenGetDeleteIsCalled_ThenServicesAreCalledTwiceAndReturnedNotFound()
        {
            //Arrange
            Mock<ISlotService> mockSlotService = new Mock<ISlotService>();
            Mock<IParkingZoneService> mockZoneService = new Mock<IParkingZoneService>();
            mockZoneService.Setup(n => n.GetById(testZoneId));
            mockSlotService.Setup(v => v.GetById(testSlotId)).Returns(testSlot);
            var slotsController = new SlotsController(mockSlotService.Object, mockZoneService.Object);

            //Act
            var result = slotsController.Delete(testSlotId);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
            mockZoneService.Verify(n => n.GetById(testZoneId), Times.Once());
            mockSlotService.Verify(n => n.GetById(testSlotId), Times.Once());
        }

        [Fact]
        public void GivenIdExistingSlot_WhenGetDeleteIsCalled_ThenServicesAreCalledOnceAndReturnedExpectedDetailsSlotVM()
        {
            //Arrange
            testSlot.Reservations = new List<Reservation>()
            {
                new()
                {
                    StartTime = new DateTime(2024, 01, 27, 12, 00, 00),
                    Duration = 1
                }
            };
            var expectedDetailsSlotVM = new DetailsSlotViewModel()
            {
                SlotId = testSlotId,
                SlotNumber = testSlot.Number,
                IsAvailableForBooking = testSlot.IsAvailableForBooking,
                Tariff = testSlot.Tariff,
                ZoneId = testSlot.ZoneId
            };
            Mock<ISlotService> mockSlotService = new Mock<ISlotService>();
            Mock<IParkingZoneService> mockZoneService = new Mock<IParkingZoneService>();
            mockZoneService.Setup(n => n.GetById(testZoneId)).Returns(testParkingZone);
            mockSlotService.Setup(v => v.GetById(testSlotId)).Returns(testSlot);
            var slotsController = new SlotsController(mockSlotService.Object, mockZoneService.Object);

            //Act
            var result = slotsController.Delete(testSlotId);

            //Assert
            Assert.IsType<ViewResult>(result);
            mockZoneService.Verify(n => n.GetById(testZoneId), Times.Once());
            mockSlotService.Verify(n => n.GetById(testSlotId), Times.Once());
            Assert.IsAssignableFrom<DetailsSlotViewModel>((result as ViewResult)!.Model);
            Assert.Equal(JsonSerializer.Serialize((result as ViewResult)!.Model), JsonSerializer.Serialize(expectedDetailsSlotVM));
        }

        [Fact]
        public void GivenIdNotExistingSlot_WhenDeleteConfirmIsCalled_ThenSlotServiceIsCalledOnceAndReturnedNotFound()
        {
            //Arrange
            Mock<ISlotService> mockSlotService = new Mock<ISlotService>();
            mockSlotService.Setup(n => n.GetById(testSlotId));

            //Act
            var slotsController = new SlotsController(mockSlotService.Object, Mock.Of<IParkingZoneService>());
            var result = slotsController.DeleteConfirm(testSlotId);

            //Assert
            Assert.IsType<NotFoundObjectResult>(result);
            mockSlotService.Verify(n => n.GetById(testSlotId), Times.Once());
        }

        [Fact]
        public void GivenIdExistingSlot_WhenDeleteConfirmIsCalled_ThenSlotServiceIsCalledTwiceAndRedirectToIndex()
        {
            //Arrange
            testSlot.Reservations = new List<Reservation>()
            {
                new()
                {
                    StartTime = new DateTime(2024, 01, 27, 12, 00, 00),
                    Duration = 1
                }
            };
            Mock<ISlotService> mockSlotService = new Mock<ISlotService>();
            mockSlotService.Setup(n => n.GetById(testSlotId)).Returns(testSlot);
            mockSlotService.Setup(c => c.Delete(testSlot));

            //Act
            var slotsController = new SlotsController(mockSlotService.Object, Mock.Of<IParkingZoneService>());
            var result = slotsController.DeleteConfirm(testSlotId);

            //Assert
            Assert.IsType<RedirectToActionResult>(result);
            mockSlotService.Verify(n => n.GetById(testSlotId), Times.Once());
            mockSlotService.Verify(n => n.Delete(testSlot), Times.Once());
        }

        [Fact]
        public void GivenIdExistingSlotWithUsingReservations_WhenDeleteConfirmIsCalled_ThenSlotServiceIsCalledTwiceAndReturnedRedirectToActionResult()
        {
            //Arrange
            testSlot.Reservations = new List<Reservation>()
            {
                new()
                {
                    StartTime = DateTime.Now,
                    Duration = 1
                }
            };
            Mock<ISlotService> mockSlotService = new Mock<ISlotService>();
            mockSlotService.Setup(n => n.GetById(testSlotId)).Returns(testSlot);

            //Act
            var slotsController = new SlotsController(mockSlotService.Object, Mock.Of<IParkingZoneService>());
            var result = slotsController.DeleteConfirm(testSlotId);

            //Assert
            Assert.IsType<RedirectToActionResult>(result);
            mockSlotService.Verify(n => n.GetById(testSlotId), Times.Once());
        }
        #endregion
    }
}
