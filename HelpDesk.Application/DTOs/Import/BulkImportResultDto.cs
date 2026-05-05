namespace HelpDesk.Application.DTOs.Import
{
    public class BulkImportResultDto
    {
        public int AccountsCreated { get; set; }
        public int RowsSkipped { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
