export type UserRole = 'Admin' | 'Agent' | 'User';

export interface UserModel {
    id: string;
    fullName: string;
    email: string;
    role: UserRole;
    isActive: boolean;
    createdAt: string;
}

export interface CreateUserRequest {
    fullName: string;
    email: string;
    password: string;
    role: UserRole;
}

export interface UpdateRoleRequest { newRole: UserRole; }
export interface LoginRequest { email: string; password: string; }