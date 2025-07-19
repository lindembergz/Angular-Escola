/**
 * Modelos de dados para o módulo de autenticação
 */

export interface User {
  id: string;
  primeiroNome: string;
  ultimoNome: string;
  nomeCompleto: string;
  email: string;
  codigoPerfil: string;
  nomePerfil: string;
  nivelPerfil: number;
  iniciais: string;
  ativo: boolean;
  emailConfirmado: boolean;
  ultimoLoginEm?: string;
  escolaId?: string;
  permissoes?: string[];
}

export interface LoginCredentials {
  email: string;
  senha: string;
  lembrarMe?: boolean;
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  tipoToken: string;
  expiraEm: number;
  expiraEm_DateTime: string;
  usuario: User;
  requerMudancaSenha: boolean;
  requerConfirmacaoEmail: boolean;
  permissoes: string[];
  escolas: SchoolAccess[];
}

export interface SchoolAccess {
  id: string;
  nome: string;
  ehPrincipal: boolean;
  permissoes: string[];
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface ChangePasswordRequest {
  senhaAtual: string;
  novaSenha: string;
  confirmarSenha: string;
  invalidarTodasSessoes?: boolean;
}

export interface ForgotPasswordRequest {
  email: string;
  urlCallback?: string;
}

export interface ResetPasswordRequest {
  email: string;
  token: string;
  novaSenha: string;
  confirmarSenha: string;
}

export interface PasswordStrength {
  forca: number;
  ehValida: boolean;
  erros: string[];
  sugestoes: string[];
  nivel: PasswordStrengthLevel;
}

export type PasswordStrengthLevel = 'MuitoFraca' | 'Fraca' | 'Razoavel' | 'Boa' | 'Forte' | 'MuitoForte';

export interface EmailConfirmationRequest {
  email: string;
  token: string;
}

export interface ResendConfirmationRequest {
  email: string;
}

// Enums para papéis de usuário
export enum UserRole {
  SUPER_ADMIN = 'SuperAdmin',
  ADMIN = 'Admin',
  DIRECTOR = 'Director',
  COORDINATOR = 'Coordinator',
  SECRETARY = 'Secretary',
  FINANCIAL_MANAGER = 'FinancialManager',
  TEACHER = 'Teacher',
  PARENT = 'Parent',
  STUDENT = 'Student',
  GUEST = 'Guest'
}

// Mapeamento de papéis para nomes amigáveis
export const USER_ROLE_NAMES: Record<UserRole, string> = {
  [UserRole.SUPER_ADMIN]: 'Super Administrador',
  [UserRole.ADMIN]: 'Administrador',
  [UserRole.DIRECTOR]: 'Diretor',
  [UserRole.COORDINATOR]: 'Coordenador Pedagógico',
  [UserRole.SECRETARY]: 'Secretário',
  [UserRole.FINANCIAL_MANAGER]: 'Responsável Financeiro',
  [UserRole.TEACHER]: 'Professor',
  [UserRole.PARENT]: 'Responsável',
  [UserRole.STUDENT]: 'Aluno',
  [UserRole.GUEST]: 'Convidado'
};

// Permissões do sistema
export enum Permission {
  // Acadêmico
  ACADEMIC_READ = 'academic.read',
  ACADEMIC_MANAGE = 'academic.manage',
  
  // Financeiro
  FINANCIAL_READ = 'financial.read',
  FINANCIAL_MANAGE = 'financial.manage',
  
  // Relatórios
  REPORTS_GENERATE = 'reports.generate',
  REPORTS_VIEW = 'reports.view',
  
  // Sistema
  SYSTEM_CONFIGURE = 'system.configure',
  SYSTEM_ADMIN = 'system.admin',
  
  // Usuários
  USERS_MANAGE = 'users.manage',
  USERS_READ = 'users.read',
  
  // Escolas
  SCHOOLS_MANAGE = 'schools.manage',
  SCHOOLS_READ = 'schools.read',
  
  // Alunos
  STUDENTS_READ = 'students.read',
  STUDENTS_MANAGE = 'students.manage',
  
  // Notas
  GRADES_MANAGE = 'grades.manage',
  GRADES_READ = 'grades.read',
  
  // Frequência
  ATTENDANCE_MANAGE = 'attendance.manage',
  ATTENDANCE_READ = 'attendance.read',
  
  // Filhos (para responsáveis)
  CHILDREN_READ = 'children.read'
}

// Interface para estado de loading
export interface LoadingState {
  login: boolean;
  logout: boolean;
  refreshToken: boolean;
  changePassword: boolean;
  forgotPassword: boolean;
  resetPassword: boolean;
  confirmEmail: boolean;
  resendConfirmation: boolean;
}

// Interface para erros de validação
export interface ValidationErrors {
  [key: string]: string[];
}

// Interface para resposta de erro da API
export interface ApiError {
  title: string;
  detail: string;
  status: number;
  errors?: ValidationErrors;
}

// Additional interfaces for auth service
export interface EmailAvailabilityResponse {
  available: boolean;
}

export interface PasswordStrengthResponse {
  forca: number;
  ehValida: boolean;
  erros: string[];
  sugestoes: string[];
  nivel: PasswordStrengthLevel;
}

export interface TokenInfo {
  token: string;
  expiresAt: Date;
  isExpired: boolean;
  expiresInMinutes: number;
}

export interface AuthConfig {
  apiBaseUrl: string;
  tokenStorageKey: string;
  refreshTokenStorageKey: string;
  userStorageKey: string;
  tokenExpirationBuffer: number;
  maxLoginAttempts: number;
  lockoutDuration: number;
  passwordValidation: {
    minLength: number;
    requireUppercase: boolean;
    requireLowercase: boolean;
    requireNumbers: boolean;
    requireSpecialChars: boolean;
    maxLength: number;
  };
}

export interface AuthEvent {
  type: AuthEventType;
  timestamp: Date;
  userId?: string;
  email?: string;
  ipAddress: string;
  userAgent: string;
  success: boolean;
  error?: string;
  metadata?: {
    url: string;
    referrer: string;
  };
}

export enum AuthEventType {
  Login = 'Login',
  Logout = 'Logout',
  TokenRefresh = 'TokenRefresh',
  PasswordChange = 'PasswordChange',
  PasswordReset = 'PasswordReset',
  EmailConfirmation = 'EmailConfirmation'
}

// Extended login request with client info
export interface LoginRequest extends LoginCredentials {
  agenteUsuario?: string;
  enderecoIp?: string;
}

// Extended refresh token request
export interface ExtendedRefreshTokenRequest extends RefreshTokenRequest {
  accessToken?: string | null;
  agenteUsuario?: string;
  enderecoIp?: string;
}