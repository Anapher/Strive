/// <reference types="Cypress" />

describe("WebRTC", () => {
  it("Activate microphone", () => {
    cy.createAndJoinOpenedConference("Vincent");
    cy.get("#media-controls-troubleshooting").click();
    cy.get("#troubleshooting-connection-badge").contains("new");

    cy.get("body").trigger("keydown", { key: "Escape" });
    cy.wait(200);
    cy.get("body").trigger("keyup", { key: "Escape" });

    cy.get("#media-controls-screen").click();

    cy.get("#media-controls-troubleshooting").click();
    cy.get("#troubleshooting-connection-badge")
      .contains("new")
      .should("not.exist");
  });
});
