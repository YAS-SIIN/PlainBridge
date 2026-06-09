/// <reference types="cypress" />

Cypress.Commands.add('mockAuth', (token = 'test-token') => {
  window.sessionStorage.setItem('auth_token', token);
});

declare global {
  namespace Cypress {
    interface Chainable {
      mockAuth(token?: string): Chainable<void>;
    }
  }
}

