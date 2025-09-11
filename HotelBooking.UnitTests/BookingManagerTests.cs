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

        #endregion

        #region GetFullyOccupiedDates
        
        #endregion
    }
}
