using FluentValidation;
using HelpDesk.Application.Commands.TicketCommand;

namespace HelpDesk.Application.Validators
{
    public class AssignTicketValidator : AbstractValidator<AssignTicketCommand>
    {
        public AssignTicketValidator()
        {
            RuleFor(x => x.TicketId)
                .NotEmpty().WithMessage("TicketId is required.");

            RuleFor(x => x.AgentId)
                .NotEmpty().WithMessage("AgentId is required.");
        }
    }
}
