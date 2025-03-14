using Moq;
using ParkingZone.Entities;
using ParkingZone.Repositories;
using ParkingZone.Services;
using System.Text.Json;

namespace ParkingZone.Tests.Services.Test
{
    public class SlotServiceTests
    {
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

        #region GetByZoneId
        [Fact]
        public void GivenZoneId_WhenCalledGetAll_ThenSlotRepositoryCalledOnceAndReturnsExpectedSlots()
        {
            var expectedSlots = new List<Slot>()
            {
                new Slot 
                {
                    Id = Guid.NewGuid(),
                    Number = 1,
                    ZoneId = testZoneId
                },
                new Slot 
                {
                    Id = Guid.NewGuid(),
                    Number = 2,
                    ZoneId = testZoneId
                },
                new Slot 
                {
                    Id = Guid.NewGuid(), 
                    Number = 3, 
                    ZoneId = testZoneId
                }
            };
            var mockSlotRepository = new Mock<ISlotRepository>();
            mockSlotRepository.Setup(m=>m.GetAll()).Returns(expectedSlots);
            var slotService = new SlotService(mockSlotRepository.Object);

            //Act
            var result = slotService.GetByZoneId(testZoneId);  

            Assert.Equal(expectedSlots, result);
            mockSlotRepository.Verify(n=>n.GetAll(), Times.Once);
        }
        #endregion

        #region Add
        [Fact]
        public void GivenTestSlotModel_WhenCalledAdd_ThenSlotRepositoryCalledTwice()
        {
            //Arrange
            Mock<ISlotRepository> mockSlotRepo = new Mock<ISlotRepository>();
            mockSlotRepo.Setup(x => x.Add(testSlot));
            mockSlotRepo.Setup(x => x.Save());
            var parkingZoneService = new SlotService(mockSlotRepo.Object);

            //Act
            parkingZoneService.Add(testSlot);

            //Assert
            mockSlotRepo.Verify(v => v.Add(testSlot), times: Times.Once);
            mockSlotRepo.Verify(c => c.Save(), Times.Once);
        }
        #endregion

        #region Update
        [Fact]
        public void GivenTestSlotModel_WhenCalledEdit_ThenSlotRepositoryCalledTwice()
        {
            //Arrange
            Mock<ISlotRepository> mockSlotRepo = new Mock<ISlotRepository>();
            mockSlotRepo.Setup(x => x.Update(testSlot));
            mockSlotRepo.Setup(x => x.Save());
            var parkingZoneService = new SlotService(mockSlotRepo.Object);

            //Act
            parkingZoneService.Update(testSlot);

            //Assert
            mockSlotRepo.Verify(v => v.Update(testSlot), times: Times.Once);
            mockSlotRepo.Verify(c => c.Save(), Times.Once);
        }
        #endregion

        #region FreeSlots
        [Theory]
        [MemberData(nameof(GetData), parameters: 5)]
        public void GivenSlotsAndTimePeriods_WhenCalledIsSlotFree_ThenReturnedTrueForFreeSlots(Slot slot, DateTime startTime, int duration, bool expectedResult)
        {
            //Append
            Mock<ISlotRepository> mockSlotRepo = new Mock<ISlotRepository>();
            var slotService = new SlotService(mockSlotRepo.Object);
            //Act
            var result = slotService.IsSlotFree(slot, startTime, duration);

            //Assert
            Assert.Equal(result, expectedResult);
        }

        public static IEnumerable<object[]> GetData(int count)
        {
            var slot = new Slot()
            {
                Reservations = new ()
                {
                    new ()
                    {
                        StartTime = new DateTime(2024, 01, 27, 8, 00, 00),
                        Duration = 2
                    } 
                }
            };
            var datas = new List<object[]>()
            {
                new object[] { slot, new DateTime(2024, 01, 27, 8, 00, 00), 1, false },
                new object[] { slot, new DateTime(2024, 01, 27, 5, 00, 00), 4, false },
                new object[] { slot, new DateTime(2024, 01, 27, 7, 00, 00), 2, false },
                new object[] { slot, new DateTime(2024, 01, 27, 5, 00, 00), 3, true },
                new object[] { slot, new DateTime(2024, 01, 27, 10, 00, 00), 1, true }
            };
            return datas;
        }

        [Fact]
        public void GivenZoneIdAndTimePeriod_WhenCalledGetFreeSlotsByZoneId_ThenSlotRepositoryCalledOnceReturnsExistedSlots()
        {
            //Append
            var _testStartTime = new DateTime(2024, 07, 12, 11, 00, 00);
            int _testDuration = 2;
            var freeSlots = new List<Slot>()
            {
                new ()
                {
                    Id = new Guid("d1247492-2852-4b39-a2bb-9b655f562569"),
                    ZoneId = testZoneId,
                    IsAvailableForBooking = true,
                    Reservations = new()
                    {
                        new()
                        {
                            StartTime = new DateTime(2024, 07, 12, 8, 00, 00),
                            Duration = 3
                        }
                    }
                },
                new ()
                {
                    Id = new Guid("d1247492-2852-4b39-a2bb-9b655f562579"),
                    ZoneId = testZoneId,
                    IsAvailableForBooking = true,
                    Reservations = new()
                    {
                        new()
                        {
                            StartTime = new DateTime(2024, 07, 12, 14, 00, 00),
                            Duration = 3
                        }
                    }
                }
            };
            var bookedSlots = new List<Slot>()
            {
                new ()
                {
                    Id = new Guid("d1247492-2852-4b39-b2bb-9b655f562569"),
                    ZoneId = testZoneId,
                    IsAvailableForBooking = true,
                    Reservations = new()
                    {
                        new()
                        {
                            StartTime = new DateTime(2024, 07, 12, 10, 00, 00),
                            Duration = 2
                        }
                    }
                },
                new ()
                {
                    Id = new Guid("d1247492-2852-4b39-a3bb-9b655f562569"),
                    ZoneId = testZoneId,
                    IsAvailableForBooking = true,
                    Reservations = new()
                    {
                        new()
                        {
                            StartTime = new DateTime(2024, 07, 12, 12, 00, 00),
                            Duration = 1
                        }
                    }
                }
            }; 
            var allSlots = freeSlots.Concat(bookedSlots);
            Mock<ISlotRepository> mockRepo = new Mock<ISlotRepository>();
            mockRepo.Setup(v=>v.GetAll()).Returns(allSlots);  
            var slotService = new SlotService(mockRepo.Object);

            //Act
            var result = slotService.GetFreeSlotsByZoneId(testZoneId, _testStartTime, _testDuration);

            //Assert
            mockRepo.Verify(c=>c.GetAll(), Times.Once());
            Assert.Equal(JsonSerializer.Serialize(freeSlots), JsonSerializer.Serialize(result));
        }
        #endregion

    }
}
