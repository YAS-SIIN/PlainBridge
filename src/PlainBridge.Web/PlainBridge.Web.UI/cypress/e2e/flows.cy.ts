describe('Critical flows', () => {
  beforeEach(() => {
    cy.mockAuth();
  });

  it('Loads host applications and toggles IsActive (mock backend)', () => {
    cy.intercept('GET', '**/api/HostApplication', {
      statusCode: 200,
      body: { resultCode: 0, data: [{ id: 1, name: 'A', appId: 'x', domain: 'd', internalUrl: 'u', isActive: 1 }] }
    }).as('getHost');
    cy.visit('/host-applications');
    cy.wait('@getHost');

    cy.intercept('PATCH', '**/api/HostApplication/UpdateState/1/*', {
      statusCode: 200,
      body: { resultCode: 0, data: { id: 1, state: 0 } }
    }).as('toggle');

    // simulate toggle via UI control if data-testid present
    // fallback: directly call API via action button if present
    cy.wait('@toggle');
  });

  it('View /profile and fetch current user', () => {
    cy.intercept('GET', '**/api/User/GetCurrentUser', { fixture: 'currentUser.json' }).as('me');
    cy.visit('/profile');
    cy.wait('@me');
    cy.contains('John');
  });

  it('Change password success and validation error', () => {
    cy.intercept('GET', '**/api/User/GetCurrentUser', { fixture: 'currentUser.json' }).as('me');
    cy.visit('/profile');
    cy.wait('@me');

    cy.intercept('PATCH', '**/api/User/ChangePassword', {
      statusCode: 400,
      body: { resultCode: 1, errors: { newPassword: ['too weak'] } }
    }).as('changeFail');

    cy.get('input[name="currentPassword"]').type('OldPass1!');
    cy.get('input[name="newPassword"]').type('weak');
    cy.get('input[name="confirmNewPassword"]').type('weak');
    cy.contains(/change password/i).click();
    cy.wait('@changeFail');

    cy.intercept('PATCH', '**/api/User/ChangePassword', { statusCode: 200, body: { resultCode: 0, data: {} } }).as('changeOk');
    cy.get('input[name="newPassword"]').clear().type('Strong1!');
    cy.get('input[name="confirmNewPassword"]').clear().type('Strong1!');
    cy.contains(/change password/i).click();
    cy.wait('@changeOk');
  });
});

