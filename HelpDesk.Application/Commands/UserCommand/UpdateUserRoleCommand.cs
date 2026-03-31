using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Commands.UserCommand
{
    public class UpdateUserRoleCommand
    {
        public Guid UserId { get; set; }
        public UserRole NewRole { get; set; }
    }
}
