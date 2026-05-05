using HelpDesk.Domain.Entities;

namespace HelpDesk.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendAsync(EmailMessage message, CancellationToken ct = default);
    }
}
