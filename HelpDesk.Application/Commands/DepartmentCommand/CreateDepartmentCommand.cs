namespace HelpDesk.Application.Commands.DepartmentCommand
{
    public class CreateDepartmentCommand
    {
        public string Name { get; set; } = string.Empty;
        public Guid? DepartmentHeadId { get; set; }
    }
}
