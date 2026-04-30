using FluentValidation;
using HelpDesk.Application.Commands.UserCommand;

namespace HelpDesk.Application.Validators
{
    public class CreateUserValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain a digit.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain a special character.");

            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Invalid role value.");
        }
    }
}
