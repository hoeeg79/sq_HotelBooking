using System;
using HotelBooking.Core;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using HotelBooking.UnitTests.Fixtures;


namespace HotelBooking.UnitTests
{
    public class BookingManagerTests : IClassFixture<BookingFixture>
    {
        private readonly BookingFixture _fixture;

        public BookingManagerTests(BookingFixture fixture)
        {
            _fixture = fixture;
        }

        #region CreateBooking
        [Theory]
        [InlineData(2,1)]
        [InlineData(-1,1)]
        public async Task CreateBooking_StartDateAfterEndDate_ThrowsArgumentException(
            int startOffset,
            int endOffset)
        {
            // Arrange
            Booking booking = new Booking
            {
                StartDate = DateTime.Today.AddDays(startOffset),
                EndDate = DateTime.Today.AddDays(endOffset)
            };
            var bookingManager = new BookingManager(_fixture.BookingRepositoryMock.Object, _fixture.RoomRepositoryMock.Object);

            // Act
            Task result() => bookingManager.CreateBooking(booking);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(result);
        }

        [Theory]
        [InlineData(1,2)]
        [InlineData(25,30)]
        public async Task CreateBooking_StartDateAndEndDateBeforeOccupied_ReturnsTrue(
            int startOffset,
            int endOffset)
        {
            // Arrange
            Booking booking = new Booking
            {
                StartDate = DateTime.Today.AddDays(startOffset),
                EndDate = DateTime.Today.AddDays(endOffset)
            };
            var bookingManager = new BookingManager(_fixture.BookingRepositoryMock.Object, _fixture.RoomRepositoryMock.Object);

            // Act
            bool result = await bookingManager.CreateBooking(booking);
            
            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData(5,25)]
        [InlineData(5,15)]
        [InlineData(12,15)]
        [InlineData(15,25)]
        public async Task CreateBooking_StartDateAndEndDateOnEitherSideOfAllOccupied_ReturnsFalse(
            int startOffset,
            int endOffset)
        {
            // Arrange
            Booking booking = new Booking
            {
                StartDate = DateTime.Today.AddDays(startOffset),
                EndDate = DateTime.Today.AddDays(endOffset)
            };
            
            var bookingManager = new BookingManager(_fixture.BookingRepositoryMock.Object, _fixture.RoomRepositoryMock.Object);
            
            // Act
            bool result = await bookingManager.CreateBooking(booking);
            
            // Assert
            Assert.False(result);
        }
        
        #endregion
        
        #region FindAvailableRoom
        [Theory]
        [InlineData(0,0)]
        [InlineData(-1,1)]
        public async Task FindAvailableRoom_DateInvalid_ThrowsArgumentException(
            int startOffset, int endOffset
        )
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(startOffset);
            DateTime endDate = DateTime.Today.AddDays(endOffset);
            
            var bookingManager = new BookingManager(_fixture.BookingRepositoryMock.Object, _fixture.RoomRepositoryMock.Object);

            // Act
            Task result() => bookingManager.FindAvailableRoom(startDate, endDate);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(result);
        }
        
        [Theory]
        [InlineData(1,1)]
        [InlineData(25,30)]
        public async Task FindAvailableRoom_RoomAvailable_RoomIdNotMinusOne(
            int startOffset, int endOffset
        )
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(startOffset);
            DateTime endDate = DateTime.Today.AddDays(endOffset);

            var bookingManager = new BookingManager(_fixture.BookingRepositoryMock.Object, _fixture.RoomRepositoryMock.Object);
            
            // Act
            int roomId = await bookingManager.FindAvailableRoom(startDate, endDate);
            // Assert
            Assert.NotEqual(-1, roomId);
        }
        
        [Theory]
        [InlineData(5, 25)]
        [InlineData(5, 15)]
        [InlineData(12,15)]
        [InlineData(15,25)]
        public async Task FindAvailableRoom_RoomNotAvailable_ReturnMinusOne(
            int startOffset, int endOffset
        )
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(startOffset);
            DateTime endDate = DateTime.Today.AddDays(endOffset);

            var bookingManager = new BookingManager(_fixture.BookingRepositoryMock.Object, _fixture.RoomRepositoryMock.Object);

            // Act
            int roomId = await bookingManager.FindAvailableRoom(startDate, endDate);

            // Assert
            Assert.Equal(-1, roomId);
        }

        [Theory]
        [InlineData(1,1)]
        public async Task FindAvailableRoom_RoomAvailable_ReturnsAvailableRoom(
            int startOffset,
            int endOffset)
        {
            // This test was added to satisfy the following test design
            // principle: "Tests should have strong assertions".

            // Arrange
            DateTime startDate = DateTime.Today.AddDays(startOffset);
            DateTime endDate = DateTime.Today.AddDays(endOffset);

            var bookingManager = new BookingManager(_fixture.BookingRepositoryMock.Object, _fixture.RoomRepositoryMock.Object);

            // Act
            int roomId = await bookingManager.FindAvailableRoom(startDate, endDate);

            var bookingForReturnedRoomId = (await _fixture.BookingRepositoryMock.Object.GetAllAsync()).
                Where(b => b.RoomId == roomId
                           && b.StartDate <= startDate
                           && b.EndDate >= endDate
                           && b.IsActive);
            
            // Assert
            Assert.Empty(bookingForReturnedRoomId);
        }

        #endregion
        
        #region GetFullyOccupiedDates
        
        [Fact]
        public async Task GetFullyOccupiedDates_StartDateAfterEndDate_ThrowsArgumentException()
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(5);
            DateTime endDate = DateTime.Today.AddDays(1);

            var bookingManager = new BookingManager(_fixture.BookingRepositoryMock.Object, _fixture.RoomRepositoryMock.Object);

            // Act
            Task result() => bookingManager.GetFullyOccupiedDates(startDate, endDate);

            // Assert
            await Assert.ThrowsAsync<ArgumentException>(result);
        }

        [Theory]
        [InlineData(1,5)]
        [InlineData(25,30)]
        public async Task GetFullyOccupiedDates_StartDateAndEndDateBeforeOrAfterOccupied_EmptyList(
            int startOffset,
            int endOffset)
        {
            // This test was added to satisfy the following test design
            // principle: "Tests should have strong assertions".

            // Arrange
            DateTime startDate = DateTime.Today.AddDays(startOffset);
            DateTime endDate = DateTime.Today.AddDays(endOffset);

            var bookingManager = new BookingManager(_fixture.BookingRepositoryMock.Object, _fixture.RoomRepositoryMock.Object);

            // Act
            var result = await bookingManager.GetFullyOccupiedDates(startDate, endDate);

            // Assert
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(5,25)]
        public async Task GetFullyOccupiedDates_StartDateAndEndDateOnEitherSideOfAllOccupied_AllDatesOccupied(
            int startOffset,
            int endOffset)
        {
            // This test was added to satisfy the following test design
            // principle: "Tests should have strong assertions".

            // Arrange
            DateTime startDate = DateTime.Today.AddDays(startOffset);
            DateTime endDate = DateTime.Today.AddDays(endOffset);

            var bookingManager = new BookingManager(_fixture.BookingRepositoryMock.Object, _fixture.RoomRepositoryMock.Object);

            // Act
            var result = await bookingManager.GetFullyOccupiedDates(startDate, endDate);

            // Assert
            var expectedDates = Enumerable.Range(0, 11)
                .Select(d => DateTime.Today.AddDays(10 + d)); // occupied from day 10 to 20
            Assert.Equal(expectedDates, result);
        }

        [Fact]
        public async Task GetFullyOccupiedDates_StartDateBeforeEndDateInOccupied_LaterHalfOccupied()
        {
            // This test was added to satisfy the following test design
            // principle: "Tests should have strong assertions".

            // Arrange
            DateTime startDate = DateTime.Today.AddDays(5);
            DateTime endDate = DateTime.Today.AddDays(15);

            var bookingManager = new BookingManager(_fixture.BookingRepositoryMock.Object, _fixture.RoomRepositoryMock.Object);

            // Act
            var result = await bookingManager.GetFullyOccupiedDates(startDate, endDate);

            // Assert
            var expectedDates = Enumerable.Range(0, 6)
                .Select(d => DateTime.Today.AddDays(10 + d)); // overlap 10–15
            Assert.Equal(expectedDates, result);
        }

        [Fact]
        public async Task GetFullyOccupiedDates_StartDateAndEndDateInOccupied_AllOccupied()
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(12);
            DateTime endDate = DateTime.Today.AddDays(15);
            
            
            var bookingManager = new BookingManager(_fixture.BookingRepositoryMock.Object, _fixture.RoomRepositoryMock.Object);

            // Act
            // Act
            var result = await bookingManager.GetFullyOccupiedDates(startDate, endDate);

            // Assert
            var expectedDates = Enumerable.Range(0, 4)
                .Select(d => DateTime.Today.AddDays(12 + d)); // fully inside 10–20
            Assert.Equal(expectedDates, result);
        }

        [Fact]
        public async Task GetFullyOccupiedDates_StartDateInEndDateAfterOccupied_FirstHalfOccupied()
        {
            // Arrange
            DateTime startDate = DateTime.Today.AddDays(15);
            DateTime endDate = DateTime.Today.AddDays(25);
            
            
            var bookingManager = new BookingManager(_fixture.BookingRepositoryMock.Object, _fixture.RoomRepositoryMock.Object);

            // Act
            var result = await bookingManager.GetFullyOccupiedDates(startDate, endDate);

            // Assert
            var expectedDates = Enumerable.Range(0, 6)
                .Select(d => DateTime.Today.AddDays(15 + d)); // overlap 15–20
            Assert.Equal(expectedDates, result);
        }
        
        #endregion
        
        
    }
}
