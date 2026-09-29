Feature: Corporate Wellness Form Validation

  @Playwright
  Scenario: Trigger warning alert by submitting invalid contact details
    Given the user navigates to "https://www.practo.com"
    When the user navigates to the Corporate Wellness page on Practo
    And the user fills the wellness form with invalid details from "corporatewellnessnegative.json"
    Then the schedule button should be disabled
    And a warning alert should be displayed
    And the warning message should be captured and logged