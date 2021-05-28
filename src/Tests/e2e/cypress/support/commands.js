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

Cypress.Commands.add("IdentityServerAPILogin", (username) => {
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

Cypress.Commands.add("loginAndLoadMainSite", (username) => {
  cy.waitUntil(
    () =>
      cy
        .request({ failOnStatusCode: false, url: "/", log: false })
        .then((x) => x.isOkStatusCode),
    { interval: 500, timeout: 60000 }
  );
  cy.IdentityServerAPILogin(username);

  cy.forceVisit("https://localhost");
});
