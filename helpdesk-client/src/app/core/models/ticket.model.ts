import { CommentModel } from './comment.model';

export enum TicketStatus {
    Open = 1,
    InProgress = 2,
    OnHold = 3,
    Resolved = 4,
    Closed = 5,
    Reopened = 6
}

export enum TicketPriority {
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

export interface TicketModel {
    id: string;
    title: string;
    description: string;
    categoryId: string;
    categoryName: string;
    priority: TicketPriority;
    status: TicketStatus;
    raisedByUserId: string;
    raisedByUserName: string;
    assignedAgentId: string | null;
    assignedAgentName: string | null;
    createdAt: string;
    lastModifiedAt: string | null;
    comments: CommentModel[];
}

export interface CreateTicketRequest {
    title: string;
    description: string;
    categoryId: string;
    priority: TicketPriority;
    raisedByUserId?: string;
}

// ✅ Ab yeh body mein jayega (POST /GetAllTicket)
export interface TicketFilterParams {
    page?: number;
    pageSize?: number;
    status?: TicketStatus | '';
    priority?: TicketPriority | '';
    categoryId?: string;
    agentId?: string;
}

export interface AssignTicketRequest {
    ticketId: string;
    agentId: string;
}

export interface UpdateStatusRequest {
    ticketId: string;
    newStatus: TicketStatus;
}

export interface UpdatePriorityRequest {
    ticketId: string;
    priority: TicketPriority;
}

export interface CreateTicketResponse {
    id: string;
    status: TicketStatus;
}