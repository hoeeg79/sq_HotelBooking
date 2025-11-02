using System;
using System.Threading.Tasks;
using HotelBooking.Core;
using HotelBooking.TestUtils.Fakes;
using Reqnroll;
using Xunit;

[Binding]
public sealed class CreateBookingStepDefinitions
{
    private BookingManager bookingManager;
    private readonly BookingFixture _fixture;
    private Booking _booking;
    private bool _result;
    
    public CreateBookingStepDefinitions(BookingFixture fixture)
    {
        _fixture = fixture;
        bookingManager = new BookingManager(_fixture.BookingRepositoryMock.Object, _fixture.RoomRepositoryMock.Object);
    }

    [Given("the hotel has an room available from {int} to {int}")]
    public void GivenTheHotelHasAnRoomAvailableFromTo(int startOffset, int endOffset)
    {
        _booking = new Booking
        {
            StartDate = DateTime.Today.AddDays(startOffset),
            EndDate = DateTime.Today.AddDays(endOffset)
        };
    }

    [When("the customer attempts to create the booking")]
    public async Task WhenTheCustomerAttemptsToCreateTheBooking()
    {
        _result = await bookingManager.CreateBooking(_booking);
    }

    [Then("the booking should be successfully created")]
    public void ThenTheBookingShouldBeSuccessfullyCreated()
    {
        Assert.True(_result);
    }
}