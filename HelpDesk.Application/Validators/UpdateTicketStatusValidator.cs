using FluentValidation;
using HelpDesk.Application.Commands.TicketCommand;

namespace HelpDesk.Application.Validators
{
    public class UpdateTicketStatusValidator : AbstractValidator<UpdateTicketStatusCommand>
    {
        public UpdateTicketStatusValidator()
        {
            RuleFor(x => x.TicketId)
                .NotEmpty().WithMessage("TicketId is required.");

            RuleFor(x => x.NewStatus)
                .IsInEnum().WithMessage("Invalid status value.");
        }
    }
}
