using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Domain.Entities;
using RazorLight;
using System.Reflection;

namespace HelpDesk.Application.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IRazorLightEngine _engine;

        private readonly ITimeZoneConverterService? _timeZoneConverter;

        public EmailTemplateService(Assembly? templateAssembly = null, ITimeZoneConverterService? timeZoneConverter = null)
        {
            var assembly = templateAssembly ?? typeof(EmailTemplateService).Assembly;
            _engine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(assembly)
                .SetOperatingAssembly(assembly)
                .UseMemoryCachingProvider()
                .Build();
            _timeZoneConverter = timeZoneConverter;
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

        public async Task<string> RenderCsatSurveyAsync(TicketEmailModel model)
        {
            try
            {
                return await _engine.CompileRenderAsync("HelpDesk.Infrastructure.Templates.EmailTemplates.CsatSurvey", model);
            }
            catch
            {
                return BuildFallbackHtml(model);
            }
        }

        public async Task<string> RenderCsatSurveyViewAsync(HelpDesk.Application.DTOs.Ticket.TicketDto model)
        {
            return await _engine.CompileRenderAsync("HelpDesk.Infrastructure.Templates.EmailTemplates.CsatSurveyView", model);
        }

        public async Task<string> RenderTicketViewAsync(HelpDesk.Application.DTOs.Ticket.TicketDto model)
        {
            var viewBag = new System.Dynamic.ExpandoObject();
            var dict = viewBag as IDictionary<string, object>;
            var tzName = _timeZoneConverter?.GetTimeZoneAbbreviation() ?? "UTC";
            dict.Add("TimeZoneName", tzName);
            dict.Add("FormatDate", new Func<DateTime, string>(dt => 
                _timeZoneConverter != null 
                    ? $"{_timeZoneConverter.ConvertToLocal(dt):MMM dd, HH:mm} {tzName}"
                    : $"{dt:MMM dd, HH:mm} UTC"));

            return await _engine.CompileRenderAsync("HelpDesk.Infrastructure.Templates.EmailTemplates.TicketView", model, viewBag);
        }

        public async Task<string> RenderPasswordChangedAsync(SecurityEmailModel model)
        {
            return await _engine.CompileRenderAsync("HelpDesk.Infrastructure.Templates.EmailTemplates.PasswordChanged", model);
        }

        public async Task<string> RenderWelcomeAsync(WelcomeEmailModel model)
        {
            return await _engine.CompileRenderAsync("HelpDesk.Infrastructure.Templates.EmailTemplates.Welcome", model);
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
            {(model.SlaDeadline.HasValue ? $"SLA Deadline: {model.SlaDeadline:dd MMM yyyy, HH:mm} {model.TimeZoneName}" : "")}

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
