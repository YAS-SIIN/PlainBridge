export const TEST_IDS = {
  profile: {
    name: 'profile-name-input',
    family: 'profile-family-input',
    username: 'profile-username-input',
    phone: 'profile-phone-input',
    saveBtn: 'profile-save-btn',
    currentPassword: 'profile-current-password',
    newPassword: 'profile-new-password',
    confirmNewPassword: 'profile-confirm-new-password',
    changePasswordBtn: 'profile-change-password-btn'
  },
  hostApps: {
    table: 'host-apps-table',
    filter: 'host-apps-filter',
    toggle: (id: number) => `host-apps-toggle-${id}`
  },
  serverApps: {
    table: 'server-apps-table',
    filter: 'server-apps-filter',
    toggle: (id: number) => `server-apps-toggle-${id}`
  }
};

