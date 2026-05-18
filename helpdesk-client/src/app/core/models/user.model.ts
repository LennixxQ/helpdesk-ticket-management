export enum UserRole {
    Admin = 1,
    Agent = 2,
    User = 3,
    DepartmentHead = 4
}

export interface UserModel {
    id: string;
    fullName: string;
    email: string;
    role: UserRole;
    isActive: boolean;
    createdAt: string;
    departmentId?: string;
    departmentName?: string;
}

export interface CreateUserRequest {
    fullName: string;
    email: string;
    password: string;
    role: UserRole;
}

export interface UpdateRoleRequest { newRole: UserRole; }
export interface LoginRequest { email: string; password: string; }