/// <reference types="Cypress" />

describe("Core tests", () => {
  it("Authenticate and load main site", () => {
    cy.loginAndLoadMainSite("Vincent");
    cy.get("#create-conference-button");
  });
});
