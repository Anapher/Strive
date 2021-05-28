// ***********************************************
// This example commands.js shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************
//
//
// -- This is a parent command --
// Cypress.Commands.add('login', (email, password) => { ... })
//
//
// -- This is a child command --
// Cypress.Commands.add('drag', { prevSubject: 'element'}, (subject, options) => { ... })
//
//
// -- This is a dual command --
// Cypress.Commands.add('dismiss', { prevSubject: 'optional'}, (subject, options) => { ... })
//
//
// -- This will overwrite an existing command --
// Cypress.Commands.overwrite('visit', (originalFn, url, options) => { ... })
import "cypress-wait-until";

Cypress.Commands.add("identityServerLogin", (username) => {
  cy.visit("https://identity.localhost/Account/Login");
  cy.get("input[name=Username]").type(username);
  cy.get("input[name=Password]").type("test");
  cy.get("button[value=login]").click();
});

Cypress.Commands.add("forceVisit", (url) => {
  cy.window().then((win) => {
    return win.open(url, "_self");
  });
});

Cypress.Commands.add("waitUntilSiteAvailable", (url) => {
  cy.waitUntil(
    () =>
      cy
        .request({ failOnStatusCode: false, url, log: false })
        .then((x) => x.isOkStatusCode),
    { interval: 500, timeout: 60000 }
  );
});

Cypress.Commands.add("loginAndLoadMainSite", (username) => {
  cy.waitUntilSiteAvailable("/");
  cy.waitUntilSiteAvailable("https://api.localhost/swagger/");

  cy.identityServerLogin(username);

  cy.forceVisit("https://localhost");
});

Cypress.Commands.add("createAndJoinConference", (username) => {
  cy.loginAndLoadMainSite(username);
  cy.get("#create-conference-button").click();

  cy.get("button[type=submit]").click();
  cy.get("#join-conference-button").click();
});

Cypress.Commands.add("createAndJoinOpenedConference", (username) => {
  cy.createAndJoinConference(username);
  cy.get("#moderator-open-conference-button").click();
});
