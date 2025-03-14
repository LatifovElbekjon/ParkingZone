using Microsoft.AspNetCore.Mvc;
using Moq;
using ParkingZone.Areas.Admin.Controllers;
using ParkingZone.ViewModels;
using ParkingZone.Services;
using System.Text.Json;
using ParkingZone.ViewModels.ZoneVMs;

namespace ParkingZone.Tests.Controllers.Test.Admin
{
    public class ParkingZoneControllerTests
    {
        private readonly Guid testParkingZoneId = new Guid("6B29FC40-CA47-1067-B31D-00DD010662DA");
        private readonly Entities.ParkingZone testParkingZone = new Entities.ParkingZone()
        {
            Id = new Guid("6B29FC40-CA47-1067-B31D-00DD010662DA"),
            Name = "elbek",
            Address = "Samarqand"
        };

        #region Index
        [Fact]
        public void GivenNothing_WhenGetIndexIsCalled_ThenServiceIsCalledOnceAndReturnedExistingZoneList()
        {
            //Arrange
            var existingZoneList = new List<Entities.ParkingZone>()
            {
                new Entities.ParkingZone()
                {
                    Id = new Guid("6B29FC40-CA47-1067-B31D-00DD010662DA"),
                    Name = "name1",
                    Address = "address1",
                    Slots = new List<Entities.Slot>()
                    {
                        new()
                        {
                            Reservations = new List<Entities.Reservation>()
                        }
                    }
                },
                new Entities.ParkingZone()
                {
                    Id = new Guid("6B29FC40-CA47-1067-B31D-00DD010662DB"),
                    Name = "name2",
                    Address = "address2",
                    Slots = new List<Entities.Slot>()
                    {
                        new()
                        {
                            Reservations = new List<Entities.Reservation>()
                        }
                    }
                },
                new Entities.ParkingZone()
                {
                    Id = new Guid("6B29FC40-CA47-1067-B31D-00DD010662DC"),
                    Name = "name3",
                    Address = "address3",
                    Slots = new List<Entities.Slot>()
                    {
                        new()
                        {
                            Reservations = new List<Entities.Reservation>()
                        }
                    }
                }
            };

            var existingZoneListItemVMs = new List<ParkingZonesListItemVM>()
            {
                new ParkingZonesListItemVM()
                {
                    Id = new Guid("6B29FC40-CA47-1067-B31D-00DD010662DA"),
                    Name = "name1",
                    Address = "address1",
                    CurrentUsingSlotsCount = 0,
                    AllSlots = 1,
                    FreeSlotsCount = 0
                },
                new ParkingZonesListItemVM()
                {
                     Id = new Guid("6B29FC40-CA47-1067-B31D-00DD010662DB"),
                    Name = "name2",
                    Address = "address2",
                    CurrentUsingSlotsCount = 0,
                    AllSlots = 1,
                    FreeSlotsCount = 0
                },
                new ParkingZonesListItemVM() 
                {
                    Id = new Guid("6B29FC40-CA47-1067-B31D-00DD010662DC"),
                    Name = "name3",
                    Address = "address3",
                    CurrentUsingSlotsCount = 0,
                    AllSlots = 1,
                    FreeSlotsCount = 0
                }
            };

            Mock<IParkingZoneService> mockParkingZoneService = new Mock<IParkingZoneService>();
            mockParkingZoneService.Setup(m => m.GetAll()).Returns(existingZoneList);

            //Act
            var parkingZoneController = new ParkingZonesController(mockParkingZoneService.Object);
            var result = parkingZoneController.Index();

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.NotNull(result);
            Assert.Equal(JsonSerializer.Serialize((result as ViewResult).Model), JsonSerializer.Serialize(existingZoneListItemVMs));
            mockParkingZoneService.Verify(m => m.GetAll(), Times.Once());
        }
        #endregion

        #region Details
        [Fact]
        public void GivenIdOfNotExistingZone_WhenGetByIdIsCalled_ThenServiceCalledOnceAndReturnedNotFoundResult()
        {
            //Arrange
            Mock<IParkingZoneService> mockParkingZoneService = new Mock<IParkingZoneService>();
            var parkingZoneController = new ParkingZonesController(mockParkingZoneService.Object);

            //Act
            var result = parkingZoneController.Details(testParkingZoneId);

            //Assert
            Assert.IsType<NotFoundResult>(result);
            mockParkingZoneService.Verify(b => b.GetById(testParkingZoneId), Times.Once());
            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public void GivenIdOfExistingZone_WhenGetByIdIsCalled_ThenServiceCalledOnceAndReturnedDetailsZoneVM()
        {
            //Arrange
            var detailsZoneVM = new DetailsParkingZoneViewModel()
            {
                Id = testParkingZoneId,
                Name = "elbek",
                Address = "Samarqand"
            };

            Mock<IParkingZoneService> mockParkingZoneService = new Mock<IParkingZoneService>();
            mockParkingZoneService.Setup(m => m.GetById(testParkingZoneId)).Returns(testParkingZone);
            var parkingZoneController = new ParkingZonesController(mockParkingZoneService.Object);

            //Act
            var result = parkingZoneController.Details(testParkingZoneId);

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.IsType<DetailsParkingZoneViewModel>((result as ViewResult).Model);
            Assert.Equal(JsonSerializer.Serialize((result as ViewResult).Model), JsonSerializer.Serialize(detailsZoneVM));
            mockParkingZoneService.Verify(n => n.GetById(testParkingZoneId), Times.Once());
        }
        #endregion

        #region Create
        [Fact]
        public void GivenNothing_WhenCalledGetCreate_ThenReturnsEmptyViewResult()
        {
            //Arrange
            var parkingZoneController = new ParkingZonesController(null);

            //Act
            var result = parkingZoneController.Create();

            //Assert
            Assert.IsType<ViewResult>(result);
            Assert.NotNull(result);
            Assert.IsAssignableFrom<CreateParkingZoneViewModel>((result as ViewResult).Model);
        }

        [Fact]
        public void GivenCreateZoneVM_WhenCalledPostCreate_ThenZoneServiceCalledOnceAndRedirectToIndex()
        {
            //Arrange
            var createParkingZoneVM = new CreateParkingZoneViewModel()
            {
                Name = testParkingZone.Name,
                Address = testParkingZone.Address
            };
            Mock<IParkingZoneService> mockParkingZoneService = new Mock<IParkingZoneService>();
            mockParkingZoneService.Setup(m => m.Add(It.IsAny<Entities.ParkingZone>()));
            var parkingZoneController = new ParkingZonesController(mockParkingZoneService.Object);

            //Act
            var result = parkingZoneController.Create(createParkingZoneVM);

            //Assert
            Assert.IsType<RedirectToActionResult>(result);
            mockParkingZoneService.Verify(p => p.Add(It.IsAny<Entities.ParkingZone>()), Times.Once);
        }

        [Fact]
        public void GivenInvalidCreateZoneVM_WhenCalledPostCreate_ThenReturnsModelError()
        {
            //Arrange
            var CreateZoneVM = new CreateParkingZoneViewModel()
            {
                Name = "name",
                Address = null
            };
            Mock<IParkingZoneService> mockParkingZoneService = new Mock<IParkingZoneService>();
            var parkingZoneController = new ParkingZonesController(mockParkingZoneService.Object);
            parkingZoneController.ModelState.AddModelError("Address", "Address invalid");

            //Act
            var result = parkingZoneController.Create(CreateZoneVM);

            //Assert
            Assert.IsType<CreateParkingZoneViewModel>((result as ViewResult)!.Model);
            Assert.IsType<ViewResult>(result);
            Assert.False(parkingZoneController.ModelState.IsValid);
        }
        #endregion

        #region Edit
        [Fact]
        public void GivenNotExistingZoneId_WhenCalledGetEdit_ThenParkingZoneServiceCalledOnceAndReturnsNotFound()
        {
            //Arrange
            Mock<IParkingZoneService> mockParkingZoneService = new Mock<IParkingZoneService>();
            mockParkingZoneService.Setup(m => m.GetById(testParkingZoneId));
            var parkingZoneController = new ParkingZonesController(mockParkingZoneService.Object);

            //Act
            var result = parkingZoneController.Edit(testParkingZoneId);

            //Assert
            Assert.True(result is NotFoundResult);
            mockParkingZoneService.Verify(i => i.GetById(testParkingZoneId));
        }

        [Fact]
        public void GivenExistingZoneId_WhenCalledGetEdit_ThenParkingZoneServiceCalledOnceAndReturnsEditZoneVM()
        {
            //Arrange
            var editZoneVM = new EditParkingZoneViewModel()
            {
                Id = testParkingZoneId,
                Name = testParkingZone.Name,
                Address = testParkingZone.Address
            };
            Mock<IParkingZoneService> mockParkingZoneService = new Mock<IParkingZoneService>();
            mockParkingZoneService.Setup(m => m.GetById(testParkingZoneId)).Returns(testParkingZone);
            var parkingZoneController = new ParkingZonesController(mockParkingZoneService.Object);

            //Act
            var result = parkingZoneController.Edit(testParkingZoneId);

            //Assert
            var model = Assert.IsAssignableFrom<EditParkingZoneViewModel>((result as ViewResult).Model);
            Assert.NotNull(result);
            Assert.Equal(JsonSerializer.Serialize(editZoneVM), JsonSerializer.Serialize(model));
            mockParkingZoneService.Verify(v => v.GetById(testParkingZoneId));
        }

        [Fact]
        public void GivenZoneIdAndEditZoneVMWithIncompatibleIds_WhenCalledPostEdit_ThenReturnsNotFound()
        {
            //Arrange
            var editZoneVM = new EditParkingZoneViewModel()
            {
                Id = Guid.NewGuid(),
                Name = testParkingZone.Name,
                Address = testParkingZone.Address
            };
            Mock<IParkingZoneService> mockParkingZoneService = new Mock<IParkingZoneService>();
            var parkingZoneController = new ParkingZonesController(mockParkingZoneService.Object);

            //Act
            var result = parkingZoneController.Edit(testParkingZoneId, editZoneVM);

            //Assert
            Assert.True(result is NotFoundResult);
        }

        [Theory]
        [InlineData(null, "address")]
        [InlineData("name", null)]
        public void GivenExistingZoneIdAndInvalidEditZoneVM_WhenCalledPostEdit_ReturnsModelError(string? name, string? address)
        {
            //Arrange
            var editZoneVM = new EditParkingZoneViewModel()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Address = address
            };
            Mock<IParkingZoneService> mockParkingZoneService = new Mock<IParkingZoneService>();
            var parkingZoneController = new ParkingZonesController(mockParkingZoneService.Object);
            parkingZoneController.ModelState.AddModelError("Field", "Name and address required");

            //Act
            var result = parkingZoneController.Edit(editZoneVM.Id, editZoneVM);

            //Assert
            Assert.IsAssignableFrom<EditParkingZoneViewModel>((result as ViewResult).Model);
            Assert.IsType<ViewResult>(result);
            Assert.False(parkingZoneController.ModelState.IsValid);
        }

        [Fact]
        public void GivenNotExistingZoneIdAndCorrectEditZoneVM_WhenCalledPostEdit_ThenParkingZoneServiceCalledOnceAndReturnsNotFound()
        {
            //Arrange
            var editZoneVM = new EditParkingZoneViewModel()
            {
                Id = testParkingZoneId,
                Name = testParkingZone.Name,
                Address = testParkingZone.Address
            };
            Mock<IParkingZoneService> mockParkingZoneService = new Mock<IParkingZoneService>();
            mockParkingZoneService.Setup(m => m.GetById(testParkingZoneId));
            var parkingZoneController = new ParkingZonesController(mockParkingZoneService.Object);

            //Act
            var result = parkingZoneController.Edit(testParkingZoneId, editZoneVM);

            //Assert
            Assert.True(parkingZoneController.ModelState.IsValid);
            Assert.True(result is NotFoundResult);
            mockParkingZoneService.Verify(v => v.GetById(testParkingZoneId));
        }

        [Fact]
        public void GivenExistingZoneIdAndCorrectEditZoneVM_WhenCalledPostEdit_ThenParkingZoneServiceCalledTwiceAndRedirectToIndex()
        {
            //Arrange
            var editParkingZoneVM = new EditParkingZoneViewModel()
            {
                Id = testParkingZoneId,
                Name = testParkingZone.Name,
                Address = testParkingZone.Address
            };

            Mock<IParkingZoneService> mockParkingZoneService = new Mock<IParkingZoneService>();
            mockParkingZoneService.Setup(m => m.GetById(testParkingZoneId)).Returns(testParkingZone);
            mockParkingZoneService.Setup(m => m.Update(testParkingZone));

            //Act
            var parkingZoneController = new ParkingZonesController(mockParkingZoneService.Object);
            var result = parkingZoneController.Edit(testParkingZoneId, editParkingZoneVM);

            //Assert
            Assert.IsType<RedirectToActionResult>(result);
            mockParkingZoneService.Verify(m => m.Update(testParkingZone), Times.Once);
            mockParkingZoneService.Verify(m => m.GetById(testParkingZoneId), Times.Once);
        }
        #endregion

        #region Delete
        [Fact]
        public void GivenExsitingZoneId_WhenCalledDeleteConfirmed_ThenParkingZoneServiceCalledTwiceAndRedirectToIndex()
        {
            //Arrange
            Mock<IParkingZoneService> mockParkingZoneService = new Mock<IParkingZoneService>();
            mockParkingZoneService.Setup(n => n.GetById(testParkingZoneId)).Returns(testParkingZone);
            mockParkingZoneService.Setup(m => m.Delete(It.IsAny<Entities.ParkingZone>()));

            //Act
            var parkingZoneController = new ParkingZonesController(mockParkingZoneService.Object);
            var result = parkingZoneController.DeleteConfirmed(testParkingZoneId);

            //Assert
            mockParkingZoneService.Verify(m => m.GetById(testParkingZoneId), Times.Once);
            mockParkingZoneService.Verify(m => m.Delete(It.IsAny<Entities.ParkingZone>()), Times.Once);
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Fact]
        public void GivenNotExistingZoneId_WhenCalledDeleteConfirmed_ThenParkingZoneServiceCalledOnceAndReturnNotFound()
        {
            //Arrange
            Mock<IParkingZoneService> mockParkingZoneService = new Mock<IParkingZoneService>();
            mockParkingZoneService.Setup(n => n.GetById(testParkingZoneId));

            //Act
            var parkingZoneController = new ParkingZonesController(mockParkingZoneService.Object);
            var result = parkingZoneController.DeleteConfirmed(testParkingZoneId);

            //Assert
            mockParkingZoneService.Verify(m => m.GetById(testParkingZoneId), Times.Once);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GivenNotExistingZoneId_WhenCalledDelete_ThenParkingZoneServiceCalledOnceAndReturnsNotFound()
        {
            //Arrange
            Mock<IParkingZoneService> mockParkingZoneService = new Mock<IParkingZoneService>();
            mockParkingZoneService.Setup(n => n.GetById(testParkingZoneId));

            //Act
            var parkingZoneController = new ParkingZonesController(mockParkingZoneService.Object);
            var result = parkingZoneController.Delete(testParkingZoneId);

            //Assert
            mockParkingZoneService.Verify(m => m.GetById(testParkingZoneId), Times.Once);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GivenExistingZoneId_WhenCalledDelete_ThenParkingZoneServiceCalledOnceAndReturnsDetailsZoneVM()
        {
            //Arrange
            Mock<IParkingZoneService> mockParkingZoneService = new Mock<IParkingZoneService>();
            mockParkingZoneService.Setup(n => n.GetById(testParkingZoneId)).Returns(testParkingZone);

            //Act
            var parkingZoneController = new ParkingZonesController(mockParkingZoneService.Object);
            var result = parkingZoneController.Delete(testParkingZoneId);

            //Assert
            mockParkingZoneService.Verify(m => m.GetById(testParkingZoneId), Times.Once);
            Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<DetailsParkingZoneViewModel>((result as ViewResult)!.Model);
        }
        #endregion
    }
}
