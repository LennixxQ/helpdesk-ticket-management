using FluentValidation;
using HelpDesk.Application.DTOs.Import;

namespace HelpDesk.Application.Validators
{
    public class BulkImportRowValidator : AbstractValidator<BulkImportRowDto>
    {
        public BulkImportRowValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Role).NotEmpty()
                .Must(r => new[] { "Admin", "Agent", "User" }.Contains(r))
                .WithMessage("Role must be Admin, Agent, or User.");
        }
    }
}
