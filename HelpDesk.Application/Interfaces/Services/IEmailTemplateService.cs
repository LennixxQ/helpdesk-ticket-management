using HelpDesk.Domain.Entities;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface IEmailTemplateService
    {
        Task<string> RenderTicketEmailAsync(TicketEmailModel model);
        Task<string> RenderCsatSurveyAsync(TicketEmailModel model);
        Task<string> RenderCsatSurveyViewAsync(HelpDesk.Application.DTOs.Ticket.TicketDto model);
        Task<string> RenderTicketViewAsync(HelpDesk.Application.DTOs.Ticket.TicketDto model);
        string RenderPlainText(TicketEmailModel model);
    }
}
