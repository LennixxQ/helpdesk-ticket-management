export interface DashboardModel {
    totalTickets: number;
    ticketsByStatus: Record<string, number>;
    ticketsByPriority: Record<string, number>;
    topAgentsThisMonth: TopAgentModel[];
    ticketsThisMonth: number;
    ticketsLastMonth: number;
}
export interface TopAgentModel {
    agentId: string;
    agentName: string;
    resolvedCount: number;
}