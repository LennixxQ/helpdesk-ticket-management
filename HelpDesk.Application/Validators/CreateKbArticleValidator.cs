using FluentValidation;
using HelpDesk.Application.Commands.KbCommand;

namespace HelpDesk.Application.Validators;

public class CreateKbArticleValidator : AbstractValidator<CreateKbArticleCommand>
{
    public CreateKbArticleValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.").MaximumLength(300);

        RuleFor(x => x.Content).NotEmpty().WithMessage("Content is required.");

        RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Category is required.");
    }
}