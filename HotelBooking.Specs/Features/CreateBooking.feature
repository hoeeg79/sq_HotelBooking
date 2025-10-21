Feature: CreateBooking
    Allow customers to create a hotel booking
    
@mytag
Scenario Outline: Create booking for an available room
    Given the hotel has an room available from <StartDate> to <EndDate>
    When the customer attempts to create the booking
    Then the booking should be successfully created
    
    Examples:
        | StartDate | EndDate |
        | 1         | 1       |
        | 2         | 3       |