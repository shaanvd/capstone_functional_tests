# Practo Test Automation Suite

This repository contains the functional test automation for my Capstone project for the **Practo** web application. The framework combines Behavior-Driven Development (BDD) for functional UI testing.

## Technology Stack
* **Core:** .NET 8, C#
* **UI Automation & Browser Routing:** Playwright for .NET
* **BDD Engine:** Reqnroll (Community successor to SpecFlow)
* **Test Runner & Assertions:** NUnit
* **Logging & Reporting:** Serilog, Allure

---

## Repository Structure
```text
 ZapSecurityAutomation
 ┣  Features/          # Reqnroll .feature files (Gherkin BDD scenarios)
 ┣  StepDefinitions/   # C# step bindings bridging Gherkin to Playwright
 ┣  Pages/             # Page Object Model (POM) classes for UI interactions
 ┣  TestData           # Data files (JSON) for data inputs
 ┗  Reports/           # Generated output (Allure)
  
