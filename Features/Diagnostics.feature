Feature: Diagnostics Page Extraction

Scenario: Extract and display top cities from the Diagnostics page
Given the user navigates to "https://www.practo.com"
    When the user navigates to the Diagnostics page
    Then the user extracts all top cities names and stores them in a list
    And displays the top cities in the console