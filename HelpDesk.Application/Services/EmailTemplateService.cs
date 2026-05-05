using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Domain.Entities;
using RazorLight;
using System.Reflection;

namespace HelpDesk.Application.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IRazorLightEngine _engine;

        public EmailTemplateService(Assembly? templateAssembly = null)
        {
            var assembly = templateAssembly ?? typeof(EmailTemplateService).Assembly;
            _engine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(assembly)
                .SetOperatingAssembly(assembly)
                .UseMemoryCachingProvider()
                .Build();
        }

        public async Task<string> RenderTicketEmailAsync(TicketEmailModel model)
        {
            try
            {
                return await _engine.CompileRenderAsync("HelpDesk.Infrastructure.Templates.EmailTemplates.TicketCreated", model);
            }
            catch
            {
                return BuildFallbackHtml(model);
            }
        }

        public async Task<string> RenderTicketViewAsync(HelpDesk.Application.DTOs.Ticket.TicketDto model)
        {
            return await _engine.CompileRenderAsync("HelpDesk.Infrastructure.Templates.EmailTemplates.TicketView", model);
        }

        public string RenderPlainText(TicketEmailModel model) => 
            $"""
            {model.SystemName} — Ticket Notification
            ==========================================
            Hi {model.RecipientName},

            {model.MessageBody}

            Ticket ID : #{model.TicketId}
            Title     : {model.TicketTitle}
            Status    : {model.Status}
            Priority  : {model.Priority}
            Category  : {model.Category}
            {(model.AssignedAgent != null ? $"Assigned To: {model.AssignedAgent}" : "")}
            {(model.SlaDeadline.HasValue ? $"SLA Deadline: {model.SlaDeadline:dd MMM yyyy, HH:mm} UTC" : "")}

            View ticket: {model.TicketUrl}

            ---
            Manage preferences: {model.PreferencesUrl}
            """;

        private static string BuildFallbackHtml(TicketEmailModel model)
            => $"""
            <html><body style="font-family:Arial,sans-serif;padding:20px;">
            <h2>{model.SystemName}</h2>
            <p>Hi {model.RecipientName},</p>
            <p>{model.MessageBody}</p>
            <p><b>Ticket:</b> #{model.TicketId} — {model.TicketTitle}<br/>
            <b>Status:</b> {model.Status} | <b>Priority:</b> {model.Priority}</p>
            <a href="{model.TicketUrl}" style="background:#1F3864;color:#fff;padding:10px 20px;text-decoration:none;border-radius:4px;">View Ticket</a>
            </body></html>
            """;
    }
}
