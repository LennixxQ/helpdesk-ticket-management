using FluentValidation;
using HelpDesk.Application.DTOs.Priority;

namespace HelpDesk.Application.Validators
{
    public class UpdatePriorityValidator : AbstractValidator<UpdatePriorityRequest>
    {
        public UpdatePriorityValidator()
        {
            RuleFor(x => x.TicketId).NotEmpty();
            RuleFor(x => x.Priority).IsInEnum().WithMessage("Invalid priority value.");
        }
    }
}
