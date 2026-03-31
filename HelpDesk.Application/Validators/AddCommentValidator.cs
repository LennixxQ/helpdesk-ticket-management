using FluentValidation;
using HelpDesk.Application.Commands.CommentCommand;

namespace HelpDesk.Application.Validators
{
    public class AddCommentValidator : AbstractValidator<AddCommentCommand>
    {
        public AddCommentValidator()
        {
            RuleFor(x => x.TicketId)
                .NotEmpty().WithMessage("TicketId is required.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Comment content is required.")
                .MaximumLength(2000).WithMessage("Comment must not exceed 2000 characters.");
        }
    }
}
