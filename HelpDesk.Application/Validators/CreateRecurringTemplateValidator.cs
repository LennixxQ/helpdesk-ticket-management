using FluentValidation;
using HelpDesk.Application.Commands.RecurringTemplateCommand;
using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Validators;

public class CreateRecurringTemplateValidator : AbstractValidator<CreateRecurringTemplateCommand>
{
    public CreateRecurringTemplateValidator()
    {
        RuleFor(x => x.TemplateName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.TicketTitle).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Priority).IsInEnum();
        RuleFor(x => x.RecurrencePattern).IsInEnum();
        RuleFor(x => x.RaiseOnBehalfOfId).NotEmpty();
        RuleFor(x => x.CronExpression)
            .NotEmpty().WithMessage("Cron expression required for Custom pattern.")
            .When(x => x.RecurrencePattern == RecurrencePattern.Custom);
        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .WithMessage("Start date must be today or future.");
    }
}