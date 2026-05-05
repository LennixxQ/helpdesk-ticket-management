using HelpDesk.Domain.Entities;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface IEmailTemplateService
    {
        Task<string> RenderTicketEmailAsync(TicketEmailModel model);
        string RenderPlainText(TicketEmailModel model);
    }
}
