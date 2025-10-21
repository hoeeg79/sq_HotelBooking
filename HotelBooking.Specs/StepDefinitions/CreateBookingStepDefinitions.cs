using System;
using HotelBooking.Core;
using Reqnroll;

[Binding]
public sealed class CreateBookingStepDefinitions
{
    private Booking _booking;
    
    [Given("the hotel has an room available from {int} to {int}")]
    public void GivenTheHotelHasAnRoomAvailableFromTo(int startOffset, int endOffset)
    {
        Booking booking = new Booking
        {
            StartDate = DateTime.Today.AddDays(startOffset),
            EndDate = DateTime.Today.AddDays(endOffset)
        };
        
        // TODO: Make the booking manager with mock objects. See BookingManagerTests.cs for reference.
    }

    [When("the customer attempts to create the booking")]
    public void WhenTheCustomerAttemptsToCreateTheBooking()
    {
        // TODO: Implement act step. See BookingManagerTests.cs for reference.
        ScenarioContext.StepIsPending();
    }

    [Then("the booking should be successfully created")]
    public void ThenTheBookingShouldBeSuccessfullyCreated()
    {
        // TODO: Implement assert step. See BookingManagerTests.cs for reference.
        ScenarioContext.StepIsPending();
    }


}