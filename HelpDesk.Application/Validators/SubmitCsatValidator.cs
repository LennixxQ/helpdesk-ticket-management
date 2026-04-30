using FluentValidation;
using HelpDesk.Application.Commands.CsatCommand;

namespace HelpDesk.Application.Validators;

public class SubmitCsatValidator : AbstractValidator<SubmitCsatCommand>
{
    public SubmitCsatValidator()
    {
        RuleFor(x => x.TicketId).NotEmpty().WithMessage("Ticket ID is required.");
        RuleFor(x => x.Score).InclusiveBetween(1, 5).WithMessage("Score must be between 1 and 5.");
        RuleFor(x => x.Comments).MaximumLength(500).When(x => x.Comments != null);
    }
}