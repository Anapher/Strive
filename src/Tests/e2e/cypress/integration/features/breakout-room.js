/// <reference types="Cypress" />

describe("Breakout Room", () => {
  it("Open breakout rooms", () => {
    cy.createAndJoinOpenedConference("Vincent");

    cy.get("#scene-management-actions").click();

    cy.get("#scene-management-actions-breakoutrooms").click();

    cy.get("div[role=dialog]").get("input[name=amount]").type("{backspace}8");
    cy.get("div[role=dialog]").get("input[name=deadline]").type("{backspace}9");
    cy.get("div[role=dialog]")
      .get("input[name=description]")
      .type("Let's see if breakout rooms are working");
    cy.get("div[role=dialog]").get("button[type=submit]").click();

    cy.get("div[role=dialog]").should("not.exist");

    cy.get("#appbar-status-chip-breakoutrooms").contains(
      "Let's see if breakout rooms are working"
    );

    cy.get("#room-list").children().should("have.length", 9);

    cy.get("#room-list").children().first("button").click();

    cy.get("#chat-tabs")
      .get("div[role=tablist]")
      .children()
      .should("have.length", 2);
  });
});
