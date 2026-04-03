import { CommentModel } from './comment.model';

export type TicketStatus = 'Open' | 'InProgress' | 'OnHold' | 'Resolved' | 'Closed' | 'Reopened';
export type TicketPriority = 'Low' | 'Medium' | 'High' | 'Critical';

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

export interface TicketFilterParams {
    page?: number;
    pageSize?: number;
    status?: TicketStatus | '';
    priority?: TicketPriority | '';
    categoryId?: string;
    agentId?: string;
}