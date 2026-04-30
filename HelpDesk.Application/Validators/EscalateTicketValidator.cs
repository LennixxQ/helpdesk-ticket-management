using FluentValidation;
using HelpDesk.Application.Commands.EscalationCommand;

namespace HelpDesk.Application.Validators;

public class EscalateTicketValidator : AbstractValidator<EscalateTicketCommand>
{
    public EscalateTicketValidator()
    {
        RuleFor(x => x.TicketId).NotEmpty().WithMessage("Ticket ID is required.");
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Escalation reason is required.")
            .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.");
    }
}