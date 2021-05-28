/// <reference types="Cypress" />

describe("Create conference", () => {
  it("Create conference", () => {
    cy.loginAndLoadMainSite("Vincent");
    cy.get("#create-conference-button").click();

    cy.get("input[name=configuration\\.name]").type("Test conference");

    cy.get("#create-conference-tab-1").click();
    cy.get("#create-conference-tabpanel-1").contains("Vincent");

    cy.get("#create-conference-tab-2").click();
    cy.get("input[value=moderator]").click();

    cy.get("button[type=submit]").click();

    cy.get("#created-conference-url").should((input) => {
      const val = input.val();
      expect(val).to.match(/^https.+\/c\/.+$/);
    });
  });
});
