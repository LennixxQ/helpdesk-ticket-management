namespace HelpDesk.Application.DTOs.Department
{
    public class UpdateDepartmentDto
    {
        public string Name { get; set; } = string.Empty;
        public Guid? DepartmentHeadId { get; set; }
    }
}
