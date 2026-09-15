Feature: Corporate Wellness Form Validation

  @Selenium
  Scenario: Trigger warning alert by submitting invalid contact details
    Given the user navigates to the Corporate Wellness page on Practo
    When the user fills the wellness form with invalid details
      | Name       | Organization | ContactNumber | Email          |
      | Test User  | Demo Corp    | invalid_phone | not-an-email   |
    And the user clicks the submit button
    Then a warning alert should be displayed
    And the warning message should be captured and logged