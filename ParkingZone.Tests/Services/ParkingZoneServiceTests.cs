using Moq;
using ParkingZone.Repositories;
using ParkingZone.Services;

namespace ParkingZone.Test.Services.Test
{
    public class ParkingZoneServiceTests
    {
        private readonly Guid testZoneId = new Guid("d1247492-2852-4b39-a2bb-9b655f562558");
        private readonly Entities.ParkingZone testParkingZone = new Entities.ParkingZone()
        {
            Id = new Guid("d1247492-2852-4b39-a2bb-9b655f562558"),
            Name = "Yama",
            Address = "Mars"
        };

        #region GetAll
        [Fact]
        public void GivenNothing_WhenCalledGetAll_ThenZoneRepositoryCalledOnceAndReturnsExpectedZones()
        {
            //Arrange
            var expectedZones = new List<Entities.ParkingZone>()
            {
                new Entities.ParkingZone()
                {
                    Id= new Guid("d1247492-2852-4b39-a2bb-9b655f562551"),
                    Name = "Elbek1",
                    Address = "Sam1"
                },
                new Entities.ParkingZone()
                {
                    Id= new Guid("d1247492-2852-4b39-a2bb-9b655f562552"),
                    Name = "Elbek2",
                    Address = "Sam2"        
                },
                new Entities.ParkingZone()
                {
                    Id= new Guid("d1247492-2852-4b39-a2bb-9b655f562553"),
                    Name = "Elbek3",
                    Address = "Sam3"
                }
            };

            Mock<IParkingZoneRepository> mockParkingZoneRepo = new Mock<IParkingZoneRepository>();
            mockParkingZoneRepo.Setup(p => p.GetAll()).Returns(expectedZones);
            var parkingZoneService = new ParkingZoneService(mockParkingZoneRepo.Object);

            //Act
            var result = parkingZoneService.GetAll();   

            //Assert
            Assert.Equal(expectedZones, result);
            Assert.NotNull(result);
            mockParkingZoneRepo.Verify(p=>p.GetAll(), Times.Once);
        }
        #endregion

        #region GetById
        [Fact]
        public void GivenExistingZoneId_WhenCalledGetById_ThenZoneRepositoryCalledOnceAndReturnsExpectedZone()
        {
            Mock<IParkingZoneRepository> mockParkingZoneRepo = new Mock<IParkingZoneRepository>();
            mockParkingZoneRepo.Setup(x=>x.GetById(testZoneId)).Returns(testParkingZone);

            var parkingZoneService = new ParkingZoneService(mockParkingZoneRepo.Object);
            var result = parkingZoneService.GetById(testZoneId);

            Assert.Equal(result,  testParkingZone);
            Assert.NotNull(result);
        }
        #endregion

        #region Add
        [Fact]
        public void GivenTestZoneModel_WhenCalledAdd_ThenZoneRepositoryCalledTwice()
        {
            Mock<IParkingZoneRepository> mockParkingZoneRepo = new Mock<IParkingZoneRepository>();
            mockParkingZoneRepo.Setup(x => x.Add(testParkingZone));
            mockParkingZoneRepo.Setup(x => x.Save());

            var parkingZoneService = new ParkingZoneService(mockParkingZoneRepo.Object);
            parkingZoneService.Add(testParkingZone);

            mockParkingZoneRepo.Verify(v=>v.Add(testParkingZone), times: Times.Once);
            mockParkingZoneRepo.Verify(c=>c.Save(), Times.Once);
        }
        #endregion

        #region Delete
        [Fact]
        public void GivenTestZoneModel_WhenCalledDelete_ThenZoneRepositoryCalledTwice()
        {
            Mock<IParkingZoneRepository> mockParkingZoneRepo = new Mock<IParkingZoneRepository>();
            mockParkingZoneRepo.Setup(x => x.Delete(testParkingZone));
            mockParkingZoneRepo.Setup(x => x.Save());

            var parkingZoneService = new ParkingZoneService(mockParkingZoneRepo.Object);
            parkingZoneService.Delete(testParkingZone);

            mockParkingZoneRepo.Verify(v => v.Delete(testParkingZone), times: Times.Once);
            mockParkingZoneRepo.Verify(c => c.Save(), Times.Once);
        }
        #endregion

        #region Update
        [Fact]
        public void GivenTestZoneModel_WhenCalledUpdate_ThenZoneRepositoryCalledTwice()
        {
            Mock<IParkingZoneRepository> mockParkingZoneRepo = new Mock<IParkingZoneRepository>();
            mockParkingZoneRepo.Setup(x => x.Update(testParkingZone));
            mockParkingZoneRepo.Setup(x => x.Save());

            var parkingZoneService = new ParkingZoneService(mockParkingZoneRepo.Object);
            parkingZoneService.Update(testParkingZone);

            mockParkingZoneRepo.Verify(v => v.Update(testParkingZone), times: Times.Once);
            mockParkingZoneRepo.Verify(c => c.Save(), Times.Once);
        }
        #endregion
    }
}
