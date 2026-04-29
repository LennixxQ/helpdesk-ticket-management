using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Commands.UserCommand
{
    public class CreateUserCommand
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public Guid? DepartmentId { get; set; }
    }
}
