using Reqnroll;

[Binding]
public sealed class CreateBookingStepDefinitions
{
    /**
     * Har lavet en feature og steps. De er dog ikke implementeret endnu.
     * Dette skal vi så lige regne ud hvordan, samt få defineret nogle flere scenarier i feature.
     * Specielt det omkring datoer, da vi kun kan bruge strings.
     * Der skal også regnes ud hvordan man håndterer dependencies i testene - eg. mocking af repo.
     */
    
    [Given("the hotel has a room available")]
    public void GivenTheHotelHasARoomAvailable()
    {
        // TODO: implement arrange (precondition) logic
        ScenarioContext.StepIsPending();
    }

    [Given("a customer wants to book a room from {string} to {string}")]
    public void GivenACustomerWantsToBookARoomFromTo(string p0, string p1)
    {
        // TODO: implement arrange (precondition) logic
        ScenarioContext.StepIsPending();
    }

    [When("the customer attempts to create the booking")]
    public void WhenTheCustomerAttemptsToCreateTheBooking()
    {
        // TODO: implement act (action) logic
        ScenarioContext.StepIsPending();
    }

    [Then("the booking should be successfully created")]
    public void ThenTheBookingShouldBeSuccessfullyCreated()
    {
        // TODO: implement assert (verification) logic
        ScenarioContext.StepIsPending();
    }

    [Then("the booking details should include:")]
    public void ThenTheBookingDetailsShouldInclude(Reqnroll.Table table)
    {
        // TODO: implement assert (verification) logic
        ScenarioContext.StepIsPending();
    }
}