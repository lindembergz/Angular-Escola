export const environment = {
  production: false,
  apiUrl: 'http://localhost:5001',
  appName: 'Sistema de Gestão Escolar',
  version: '1.0.0',
  auth: {
    tokenStorageKey: 'auth_token',
    refreshTokenStorageKey: 'refresh_token',
    userStorageKey: 'current_user',
    tokenExpirationBuffer: 5 * 60 * 1000, // 5 minutes in milliseconds
    maxLoginAttempts: 5,
    lockoutDuration: 30 * 60 * 1000, // 30 minutes in milliseconds
    autoRefreshEnabled: true,
    sessionTimeout: 60 * 60 * 1000 // 1 hour in milliseconds
  },
  features: {
    enablePasswordStrengthMeter: true,
    enableRememberMe: true,
    enableEmailConfirmation: true,
    enablePasswordReset: true,
    enableMultipleSchools: true,
    enableRoleBasedAccess: true
  },
  ui: {
    defaultTheme: 'lara-light-blue',
    enableDarkMode: true,
    enableAnimations: true,
    toastDuration: 5000,
    loadingTimeout: 30000
  }
};