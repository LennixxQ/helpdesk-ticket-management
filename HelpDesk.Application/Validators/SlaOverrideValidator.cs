using FluentValidation;
using HelpDesk.Application.DTOs.Sla;

namespace HelpDesk.Application.Validators
{
    public class SlaOverrideValidator : AbstractValidator<SlaOverrideRequest>
    {
        public SlaOverrideValidator()
        {
            RuleFor(x => x.TicketId).NotEmpty();
            RuleFor(x => x.NewDeadline)
                .GreaterThan(DateTime.UtcNow).WithMessage("New deadline must be in the future.");
            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Override reason is required.")
                .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.");
        }
    }
}
