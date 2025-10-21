Feature: CreateBooking
    Allow customers to create a hotel booking
    
@mytag
Scenario: Create booking for an available room
    Given the hotel has a room available
    And a customer wants to book a room from <StartDate> to <EndDate>
    When the customer attempts to create the booking
    Then the booking should be successfully created
    
        Examples:
        | Id | StartDate        | EndDate          | IsActive | CustomerId | RoomId |
        | 1  | Today.AddDays(1) | Today.AddDays(1) | true     | 1          | 1      |