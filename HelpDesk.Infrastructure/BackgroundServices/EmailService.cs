using HelpDesk.Application.Interfaces;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Domain.Entities;
using HelpDesk.Infrastructure.Persistence;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace HelpDesk.Infrastructure.BackgroundServices
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;
        private readonly AppDbContext _context;

        // Retry delays: 30s, 2min, 10min
        private static readonly int[] RetryDelaysSeconds = { 30, 120, 600 };
        private const int MaxAttempts = 3;

        public EmailService(IConfiguration config, ILogger<EmailService> logger, AppDbContext context)
        {
            _config = config;
            _logger = logger;
            _context = context;
        }

        public async Task SendAsync(EmailMessage message, CancellationToken ct = default)
        {
            Exception? lastEx = null;

            for (int attempt = 1; attempt <= MaxAttempts; attempt++)
            {
                try
                {
                    await SendViaMailKitAsync(message, ct);
                    await LogEmailAsync(message, success: true, attempt, null, ct);

                    _logger.LogInformation("Email sent successfully to {ToEmail} | Event: {EventType} | Attempt: {Attempt}", message.ToEmail, message.EventType, attempt);
                    return;
                }
                catch (Exception ex)
                {
                    lastEx = ex;
                    _logger.LogWarning("Email attempt {Attempt}/{Max} failed for {ToEmail}: {Error}", attempt, MaxAttempts, message.ToEmail, ex.Message);

                    if (attempt < MaxAttempts)
                    {
                        var delayMs = RetryDelaysSeconds[attempt - 1] * 1000;
                        _logger.LogInformation("Retrying in {Delay}s...", RetryDelaysSeconds[attempt - 1]);
                        await Task.Delay(delayMs, ct);
                    }
                }
            }

            // All attempts failed
            await LogEmailAsync(message, success: false, MaxAttempts, lastEx?.Message, ct);
            _logger.LogError("All {Max} email attempts failed for {ToEmail} | Event: {EventType} | LastError: {Error}",MaxAttempts, message.ToEmail, message.EventType, lastEx?.Message);
        }

        private async Task SendViaMailKitAsync(EmailMessage message, CancellationToken ct)
        {
            var smtp = _config.GetSection("Smtp");
            var host = smtp["Host"] ?? throw new InvalidOperationException("SMTP Host not configured.");
            var port = int.Parse(smtp["Port"] ?? "587");
            var username = smtp["Username"] ?? throw new InvalidOperationException("SMTP Username not configured.");
            var password = smtp["Password"] ?? throw new InvalidOperationException("SMTP Password not configured.");
            var fromAddress = smtp["FromAddress"] ?? "chauhanvivek1800@gmail.com";
            var fromName = smtp["FromName"] ?? "HelpDesk Support";

            // Build MIME message
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress(fromName, fromAddress));
            mimeMessage.To.Add(MailboxAddress.Parse(message.ToEmail));
            mimeMessage.Subject = message.Subject;

            // Multipart: plain text + HTML
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = message.HtmlBody,
                TextBody = message.PlainTextBody
            };
            mimeMessage.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            // Choose security option based on port
            var secureOption = port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;

            await client.ConnectAsync(host, port, secureOption, ct);
            await client.AuthenticateAsync(username, password, ct);
            await client.SendAsync(mimeMessage, ct);
            await client.DisconnectAsync(quit: true, ct);
        }

        private async Task LogEmailAsync(EmailMessage message,bool success,int attemptCount,string? failure,CancellationToken ct)
        {
            try
            {
                var log = new EmailLog
                {
                    Id = Guid.NewGuid(),
                    RecipientUserId = message.RecipientUserId,
                    ToEmail = message.ToEmail,
                    Subject = message.Subject,
                    EventType = message.EventType,
                    IsSuccess = success,
                    AttemptCount = attemptCount,
                    FailureReason = failure,
                    SentAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "HelpDesk System"
                };
                await _context.EmailLogs.AddAsync(log, ct);
                await _context.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to write email log for {ToEmail}", message.ToEmail);
            }
        }
    }
}