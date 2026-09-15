Feature: Hospital Search and Filtering

  @Playwright
  Scenario: Search for 24/7 hospitals with parking and high ratings in Bangalore
    Given the user navigates to "https://www.practo.com"
    When the user selects location as "Bangalore"
    And the user searches for "Hospital"
    And the user applies the filter for "24*7"
    And the user applies the filter for "Has Parking"
    Then the user extracts hospitals with a rating greater than 3.5
    And displays the hospital names in the console