using System;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using Xunit;
using System.Linq;
using System.Threading.Tasks;


namespace HotelBooking.UnitTests
{
    public class BookingManagerTests
    {
        private IBookingManager bookingManager;
        IRepository<Booking> bookingRepository;

        public BookingManagerTests(){
            DateTime start = DateTime.Today.AddDays(10);
            DateTime end = DateTime.Today.AddDays(20);
            bookingRepository = new FakeBookingRepository(start, end);
            IRepository<Room> roomRepository = new FakeRoomRepository();
            bookingManager = new BookingManager(bookingRepository, roomRepository);
        }

        #region CreateBooking
        [Fact]
        public async Task CreateBooking_StartDateAfterEndDate_ThrowsArgumentException()
        {
            // Arrange
            Booking booking = new Booking
            {
                StartDate = DateTime.Today.AddDays(2),
                EndDate = DateTime.Today.AddDays(1)
            };

            // Act
            Task result() => bookingManager.CreateBooking(booking);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(result);
        }
        
        [Fact]
        public async Task CreateBooking_StartDateNotInTheFutureEndDateIs_ThrowsArgumentException()
        {
            // Arrange
            Booking booking = new Booking
            {
                StartDate = DateTime.Today.AddDays(-1),
                EndDate = DateTime.Today.AddDays(1)
            };

            // Act
            Task result() => bookingManager.CreateBooking(booking);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(result);
        }

        [Fact]
        public async Task CreateBooking_StartDateAndEndDateBeforeOccupied_ReturnsTrue()
        {
            // Arrange
            Booking booking = new Booking
            {
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(2)
            };
            
            // Act
            bool result = await bookingManager.CreateBooking(booking);
            
            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CreateBooking_StartDateAndEndDateOnEitherSideOfAllOccupied_ReturnsFalse()
        {
            // Arrange
            Booking booking = new Booking
            {
                StartDate = DateTime.Today.AddDays(5),
                EndDate = DateTime.Today.AddDays(25)
            };

            // Act
            bool result = await bookingManager.CreateBooking(booking);
            
            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CreateBooking_StartDateAndEndDateAfterOccupied_ReturnTrue()
        {
            // Arrange
            Booking booking = new Booking
            {
                StartDate = DateTime.Today.AddDays(25),
                EndDate = DateTime.Today.AddDays(30)
            };
            
            // Act
            bool result = await bookingManager.CreateBooking(booking);
            
            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CreateBooking_StartDateBeforeEndDateInOccupied_ReturnFalse()
        {
            // Arrange
            Booking booking = new Booking
            {
                StartDate = DateTime.Today.AddDays(5),
                EndDate = DateTime.Today.AddDays(15)
            };
            
            // Act
            bool result = await bookingManager.CreateBooking(booking);
            
            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CreateBooking_StartDateAndEndDateInOccupied_ReturnFalse()
        {
            // Arrange
            Booking booking = new Booking
            {
                StartDate = DateTime.Today.AddDays(12),
                EndDate = DateTime.Today.AddDays(15)
            };
            
            // Act
            bool result = await bookingManager.CreateBooking(booking);
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public async Task CreateBooking_StartDateInEndDateAfterOccupied_ReturnFalse()
        {
            // Arrange
            Booking booking = new Booking
            {
                StartDate = DateTime.Today.AddDays(15),
                EndDate = DateTime.Today.AddDays(25)
            };

            // Act
            bool result = await bookingManager.CreateBooking(booking);

            // Assert
            Assert.False(result);
        }
        #endregion
        
        #region FindAvailableRoom
        [Fact]
        public async Task FindAvailableRoom_StartDateNotInTheFuture_ThrowsArgumentException()
        {
            // Arrange
            DateTime date = DateTime.Today;

            // Act
            Task result() => bookingManager.FindAvailableRoom(date, date);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(result);
        }

        [Fact]
        public async Task FindAvailableRoom_StartDateNotInTheFutureEndDateIs_ThrowsArgumentException()
        {
            // Arrange
            DateTime start = DateTime.Today.AddDays(-1);
            DateTime end = DateTime.Today.AddDays(1);

            // Act
            Task result() => bookingManager.FindAvailableRoom(start, end);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(result);
        }
        
        [Fact]
        public async Task FindAvailableRoom_RoomAvailable_RoomIdNotMinusOne()
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            // Act
            int roomId = await bookingManager.FindAvailableRoom(date, date);
            // Assert
            Assert.NotEqual(-1, roomId);
        }
        
        [Fact]
        public async Task FindAvailableRoom_StartDateAndEndDateOnEitherSideOfAllOccupied_ReturnMinusOne()
        {
            // Arrange
            DateTime start = DateTime.Today.AddDays(5);
            DateTime end = DateTime.Today.AddDays(25);

            // Act
            int roomId = await bookingManager.FindAvailableRoom(start, end);

            // Assert
            Assert.Equal(-1, roomId);
        }

        [Fact]
        public async Task FindAvailableRoom_RoomAvailable_ReturnsAvailableRoom()
        {
            // This test was added to satisfy the following test design
            // principle: "Tests should have strong assertions".

            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            
            // Act
            int roomId = await bookingManager.FindAvailableRoom(date, date);

            var bookingForReturnedRoomId = (await bookingRepository.GetAllAsync()).
                Where(b => b.RoomId == roomId
                           && b.StartDate <= date
                           && b.EndDate >= date
                           && b.IsActive);
            
            // Assert
            Assert.Empty(bookingForReturnedRoomId);
        }
        
        [Fact]
        public async Task FindAvailableRoom_StartDateBeforeEndDateInOccupied_ReturnMinusOne()
        {
            // Arrange
            DateTime start = DateTime.Today.AddDays(5);
            DateTime end = DateTime.Today.AddDays(15);

            // Act
            int roomId = await bookingManager.FindAvailableRoom(start, end);

            // Assert
            Assert.Equal(-1, roomId);
        }

        [Fact]
        public async Task FindAvailableRoom_StartDateAndEndDateInOccupied_ReturnMinusOne()
        {
            // Arrange
            DateTime start = DateTime.Today.AddDays(12);
            DateTime end = DateTime.Today.AddDays(15);

            // Act
            int roomId = await bookingManager.FindAvailableRoom(start, end);

            // Assert
            Assert.Equal(-1, roomId);
        }

        [Fact]
        public async Task FindAvailableRoom_StartDateInEndDateAfterOccupied_ReturnMinusOne()
        {
            // Arrange
            DateTime start = DateTime.Today.AddDays(15);
            DateTime end = DateTime.Today.AddDays(25);

            // Act
            int roomId = await bookingManager.FindAvailableRoom(start, end);

            // Assert
            Assert.Equal(-1, roomId);
        }

        [Fact]
        public async Task FindAvailableRoom_StartDateAndEndDateAfterOccupied_RoomIdNotMinus()
        {
            // Arrange
            DateTime start = DateTime.Today.AddDays(25);
            DateTime end = DateTime.Today.AddDays(30);

            // Act
            int roomId = await bookingManager.FindAvailableRoom(start, end);

            // Assert
            Assert.NotEqual(-1, roomId);
        }

        #endregion
        #region GetFullyOccupiedDates
        
        [Fact]
        public async Task GetFullyOccupiedDates_StartDateAfterEndDate_ThrowsArgumentException()
        {
            // Arrange
            DateTime start = DateTime.Today.AddDays(5);
            DateTime end = DateTime.Today.AddDays(1);

            // Act
            Task result() => bookingManager.GetFullyOccupiedDates(start, end);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(result);
        }

        [Fact]
        public async Task GetFullyOccupiedDates_StartDateAndEndDateBeforeOccupied_EmptyList()
        {
            // Arrange
            DateTime start = DateTime.Today.AddDays(1);
            DateTime end = DateTime.Today.AddDays(5);

            // Act
            var result = await bookingManager.GetFullyOccupiedDates(start, end);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetFullyOccupiedDates_StartDateAndEndDateOnEitherSideOfAllOccupied_AllDatesOccupied()
        {
            // Arrange
            DateTime start = DateTime.Today.AddDays(5);
            DateTime end = DateTime.Today.AddDays(25);

            // Act
            var result = await bookingManager.GetFullyOccupiedDates(start, end);

            // Assert
            var expectedDates = Enumerable.Range(0, 11)
                .Select(d => DateTime.Today.AddDays(10 + d)); // occupied from day 10 to 20
            Assert.Equal(expectedDates, result);
        }

        [Fact]
        public async Task GetFullyOccupiedDates_StartDateBeforeEndDateInOccupied_LaterHalfOccupied()
        {
            // Arrange
            DateTime start = DateTime.Today.AddDays(5);
            DateTime end = DateTime.Today.AddDays(15);

            // Act
            var result = await bookingManager.GetFullyOccupiedDates(start, end);

            // Assert
            var expectedDates = Enumerable.Range(0, 6)
                .Select(d => DateTime.Today.AddDays(10 + d)); // overlap 10–15
            Assert.Equal(expectedDates, result);
        }

        [Fact]
        public async Task GetFullyOccupiedDates_StartDateAndEndDateInOccupied_AllOccupied()
        {
            // Arrange
            DateTime start = DateTime.Today.AddDays(12);
            DateTime end = DateTime.Today.AddDays(15);

            // Act
            var result = await bookingManager.GetFullyOccupiedDates(start, end);

            // Assert
            var expectedDates = Enumerable.Range(0, 4)
                .Select(d => DateTime.Today.AddDays(12 + d)); // fully inside 10–20
            Assert.Equal(expectedDates, result);
        }

        [Fact]
        public async Task GetFullyOccupiedDates_StartDateInEndDateAfterOccupied_FirstHalfOccupied()
        {
            // Arrange
            DateTime start = DateTime.Today.AddDays(15);
            DateTime end = DateTime.Today.AddDays(25);

            // Act
            var result = await bookingManager.GetFullyOccupiedDates(start, end);

            // Assert
            var expectedDates = Enumerable.Range(0, 6)
                .Select(d => DateTime.Today.AddDays(15 + d)); // overlap 15–20
            Assert.Equal(expectedDates, result);
        }

        [Fact]
        public async Task GetFullyOccupiedDates_StartDateAndEndDateAfterOccupied_EmptyList()
        {
            // Arrange
            DateTime start = DateTime.Today.AddDays(25);
            DateTime end = DateTime.Today.AddDays(30);

            // Act
            var result = await bookingManager.GetFullyOccupiedDates(start, end);

            // Assert
            Assert.Empty(result);
        }
        #endregion
        
        
    }
}
