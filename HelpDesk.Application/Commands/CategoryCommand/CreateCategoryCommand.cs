namespace HelpDesk.Application.Commands.CategoryCommand
{
    public class CreateCategoryCommand
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
