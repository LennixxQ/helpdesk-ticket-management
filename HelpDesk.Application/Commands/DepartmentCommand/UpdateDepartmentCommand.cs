namespace HelpDesk.Application.Commands.DepartmentCommand
{
    public class UpdateDepartmentCommand
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? DepartmentHeadId { get; set; }
    }
}
