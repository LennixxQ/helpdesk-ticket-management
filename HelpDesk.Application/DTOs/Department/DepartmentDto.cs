namespace HelpDesk.Application.DTOs.Department
{
    public class DepartmentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public Guid? DepartmentHeadId { get; set; }
        public string? DepartmentHeadName { get; set; }
    }
}
