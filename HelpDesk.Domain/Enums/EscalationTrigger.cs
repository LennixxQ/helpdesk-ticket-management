namespace HelpDesk.Domain.Enums
{
    public enum EscalationTrigger
    {
        Manual = 1,
        CriticalNotAssigned = 2,
        SlaBreached = 3,
        OnHoldTooLong = 4,
        UserReopenedThreeTimes = 5
    }
}
