namespace HelpDesk.Domain.Enums
{
    public enum NotificationEventType
    {
        TicketCreated = 1,
        TicketAssigned = 2,
        TicketStatusChanged = 3,
        CommentAdded = 4,
        TicketClosed = 5,
        TicketReopened = 6,
        TicketEscalated = 7,
        SlaWarning = 8,
        SlaBreached = 9,
        AgentReassigned = 10,
        AccountCreated = 11,
        AccountDeactivated = 12,
        SurveyRequest = 13
    }
}
