/// <reference types="Cypress" />

describe("Poll", () => {
  it("Create single choice and vote", () => {
    cy.createAndJoinOpenedConference("Vincent");

    cy.get("#scene-management-actions").click();

    cy.get("#scene-management-actions-poll").click();

    cy.get("div[role=dialog]")
      .get("input[name=config\\.question]")
      .type("What is love?");

    cy.get("div[role=dialog]")
      .get("textarea[name=instruction\\.options]")
      .type("Baby don't hurt me{enter}I don't know");

    cy.get("div[role=dialog]").get("button[type=submit]").click();

    cy.contains("Baby don't hurt me").closest("button").click();

    cy.contains("Baby don't hurt me")
      .closest("button")
      .should("not.be.disabled");
  });

  it("Create multiple choice answer not final and vote multiple", () => {
    cy.createAndJoinOpenedConference("Vincent");

    cy.get("#scene-management-actions").click();

    cy.get("#scene-management-actions-poll").click();

    cy.get("div[role=dialog]")
      .get("input[name=config\\.question]")
      .type("Who is your favorite artist?");

    cy.get("#poll-dialog-select-mode").click();

    cy.get("li[data-value=multipleChoice]").click();

    cy.get("div[role=dialog]")
      .get("textarea[name=instruction\\.options]")
      .type(
        "Kollegah der Boss{enter}SSIO{enter}Haftbefehl{enter}187 Strassenbande"
      );

    cy.get("div[role=dialog]").get(".MuiSwitch-root").click();

    cy.get("div[role=dialog]")
      .get("input[name=config\\.isAnswerFinal]")
      .click();

    cy.get("div[role=dialog]").get("button[type=submit]").click();

    cy.contains("Kollegah der Boss").closest("button").click();
    cy.contains("SSIO").closest("button").click();
    cy.contains("187 Strassenbande").closest("button").click();

    cy.contains("Kollegah der Boss")
      .closest("button")
      .should("have.class", "MuiChip-colorPrimary");
    cy.contains("SSIO")
      .closest("button")
      .should("have.class", "MuiChip-colorPrimary");
    cy.contains("187 Strassenbande")
      .closest("button")
      .should("have.class", "MuiChip-colorPrimary");
  });
});
