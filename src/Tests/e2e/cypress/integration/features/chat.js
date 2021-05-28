/// <reference types="Cypress" />

describe("Chat", () => {
  it("Send chat message and verify", () => {
    cy.createAndJoinOpenedConference("Vincent");

    cy.get("textarea[name=message]").type("Hello world!{enter}");

    cy.get("textarea[name=message]").invoke("val").should("be.empty");

    cy.get("#chat-message-list").contains("Hello world!");
  });

  it("Send emoji and verify", () => {
    cy.createAndJoinOpenedConference("Vincent");

    cy.get("#chat-open-emojis").click();
    cy.contains("🔥").closest("button").click();
    cy.get("textarea[name=message]").type("{enter}");

    cy.get("#chat-message-list").contains("🔥");
  });
});
