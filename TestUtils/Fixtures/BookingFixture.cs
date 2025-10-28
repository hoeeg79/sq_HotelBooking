using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotelBooking.Core;
using Moq;

namespace HotelBooking.TestUtils.Fakes;

public class BookingFixture
{    
    public Mock<IRepository<Booking>> BookingRepositoryMock { get; }
    public Mock<IRepository<Room>> RoomRepositoryMock { get; }
    public DateTime Today { get; } = DateTime.Today;

    public BookingFixture()
    {
        BookingRepositoryMock = new Mock<IRepository<Booking>>(MockBehavior.Strict);
        RoomRepositoryMock    = new Mock<IRepository<Room>>(MockBehavior.Strict);

        BookingRepositoryMock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(new List<Booking>
            {
                new Booking { Id=1, StartDate=Today.AddDays(1),  EndDate=Today.AddDays(1),  IsActive=true, CustomerId=1, RoomId=1 },
                new Booking { Id=2, StartDate=Today.AddDays(10), EndDate=Today.AddDays(20), IsActive=true, CustomerId=1, RoomId=1 },
                new Booking { Id=3, StartDate=Today.AddDays(10), EndDate=Today.AddDays(20), IsActive=true, CustomerId=2, RoomId=2 },
            });
        
        BookingRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Booking>()))
            .Returns(Task.CompletedTask);

        RoomRepositoryMock.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(new List<Room>
            {
                new Room { Id=1, Description="A" },
                new Room { Id=2, Description="B" },
            });
    }
}