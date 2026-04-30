using FluentValidation;
using HelpDesk.Application.Commands.DepartmentCommand;

namespace HelpDesk.Application.Validators;

public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Department name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
    }
}