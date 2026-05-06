using HelpDesk.Application.DTOs.Export;
using HelpDesk.Application.DTOs.Report;
using HelpDesk.Application.Interfaces.Repositories;
using HelpDesk.Application.Interfaces.Services;
using HelpDesk.Domain.Enums;
using System.Globalization;
using System.Text;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using CsvHelper.Configuration;
using CsvHelper;

namespace HelpDesk.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _uow;
        private readonly IAuditService _auditService;

        public ReportService(IUnitOfWork uow, IAuditService auditService)
        {
            _uow = uow;
            _auditService = auditService;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> ExportAuditLogCsvAsync(DateTime from, DateTime to)
        {
            var records = await _uow.AuditLogs.GetForExportAsync(from, to);
            await _auditService.LogActionAsync("REPORT_EXPORTED", "AuditLog", null, $"Format: CSV, Range: {from:d} to {to:d}");
            return WriteCsv(records);
        }

        public async Task<byte[]> ExportTicketsCsvAsync(ReportFilterDto filter)
        {
            var tickets = await _uow.TicketReports.GetForReportAsync(filter);

            var records = tickets.Select(t => new TicketExportDto
            {
                TicketId = t.Id.ToString()[..8],
                Title = t.Title,
                Status = t.Status.ToString(),
                Priority = t.Priority.ToString(),
                Category = t.Category?.Name ?? "N/A",
                RaisedBy = t.RaisedByUser?.FullName ?? "N/A",
                AssignedAgent = t.AssignedAgent?.FullName ?? "Unassigned",
                Department = t.Department?.Name ?? "N/A",
                CreatedAt = t.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                SlaDeadline = t.SlaDeadline?.ToString("yyyy-MM-dd HH:mm") ?? "N/A",
                SlaStatus = t.SlaStatus.ToString(),
                IsEscalated = t.IsEscalated ? "Yes" : "No"
            }).ToList();

            await _auditService.LogActionAsync("REPORT_EXPORTED", "Tickets", null, $"Format: CSV, Range: {filter.From:d} to {filter.To:d}");
            return WriteCsv(records);
        }

        public async Task<byte[]> ExportTicketsPdfAsync(ReportFilterDto filter)
        {
            var tickets = await _uow.TicketReports.GetForReportAsync(filter);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(24);
                    page.DefaultTextStyle(t => t.FontSize(9));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("HelpDesk — Ticket Report")
                                .FontSize(16).Bold().FontColor(Color.FromHex("#1F3864"));
                            col.Item().Text(
                                $"{filter.From:dd MMM yyyy} to {filter.To:dd MMM yyyy}")
                                .FontSize(10).FontColor(Color.FromHex("#64748b"));
                        });
                        row.ConstantItem(130).AlignRight()
                            .Text($"Generated: {DateTime.UtcNow:dd MMM yyyy HH:mm}")
                            .FontSize(9).FontColor(Color.FromHex("#94a3b8"));
                    });

                    page.Content().PaddingTop(12).Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(55);  // ID
                            cols.RelativeColumn(3);   // Title
                            cols.RelativeColumn(1.2f);// Status
                            cols.RelativeColumn(1);   // Priority
                            cols.RelativeColumn(1.2f);// Category
                            cols.RelativeColumn(2);   // Raised By
                            cols.RelativeColumn(2);   // Agent
                            cols.RelativeColumn(1.5f);// Created
                            cols.RelativeColumn(1);   // SLA
                        });

                        table.Header(header =>
                        {
                            foreach (var col in new[] { "ID","Title","Status","Priority",
                                                     "Category","Raised By","Agent",
                                                     "Created","SLA" })
                            {
                                header.Cell()
                                      .Background(Color.FromHex("#1F3864"))
                                      .Padding(6)
                                      .Text(col)
                                      .FontColor(Colors.White).Bold().FontSize(9);
                            }
                        });

                        bool alt = false;
                        foreach (var t in tickets)
                        {
                            var bg = alt ? Color.FromHex("#f1f5f9") : Colors.White;
                            alt = !alt;

                            IContainer Cell(IContainer c) => c.Background(bg).Padding(5);

                            table.Cell().Element(Cell).Text(t.Id.ToString()[..8]).FontSize(8);
                            table.Cell().Element(Cell).Text(
                                t.Title.Length > 40 ? t.Title[..40] + "…" : t.Title);
                            table.Cell().Element(Cell).Text(t.Status.ToString()).FontColor(
                                t.Status == TicketStatus.Closed ? Color.FromHex("#16a34a") :
                                t.Status == TicketStatus.Resolved ? Color.FromHex("#0284c7") :
                                t.SlaBreached ? Color.FromHex("#dc2626") :
                                Colors.Black);
                            table.Cell().Element(Cell).Text(t.Priority.ToString()).FontColor(
                                t.Priority == TicketPriority.Critical ? Color.FromHex("#dc2626") :
                                t.Priority == TicketPriority.High ? Color.FromHex("#ea580c") :
                                Colors.Black);
                            table.Cell().Element(Cell).Text(t.Category?.Name ?? "N/A");
                            table.Cell().Element(Cell).Text(t.RaisedByUser?.FullName ?? "N/A");
                            table.Cell().Element(Cell).Text(t.AssignedAgent?.FullName ?? "Unassigned");
                            table.Cell().Element(Cell).Text(t.CreatedAt.ToString("dd/MM/yy HH:mm"));
                            table.Cell().Element(Cell).Text(t.SlaStatus.ToString()).FontColor(
                                t.SlaStatus == SlaStatus.Breached ? Color.FromHex("#dc2626") :
                                t.SlaStatus == SlaStatus.Warning ? Color.FromHex("#d97706") :
                                Color.FromHex("#16a34a"));
                        }
                    });

                    page.Footer().Row(row =>
                    {
                        row.RelativeItem().Text($"Total: {tickets.Count} tickets")
                            .FontSize(9).FontColor(Color.FromHex("#64748b"));
                        row.ConstantItem(80).AlignRight().Text(x =>
                        {
                            x.Span("").FontSize(9);
                            x.CurrentPageNumber();
                            x.Span(" / ");
                            x.TotalPages();
                        });
                    });
                });
            });

            await _auditService.LogActionAsync("REPORT_EXPORTED", "Tickets", null, $"Format: PDF, Range: {filter.From:d} to {filter.To:d}");
            return document.GeneratePdf();
        }

        public async Task<ReportDataDto> GetTicketVolumeReportAsync(ReportFilterDto filter)
        {
            var tickets = await _uow.TicketReports.GetForReportAsync(filter);
            await _auditService.LogActionAsync("REPORT_GENERATED", "TicketVolume", null, $"Range: {filter.From:d} to {filter.To:d}");
            return new ReportDataDto
            {
                TotalTickets = tickets.Count,
                ByStatus = tickets.GroupBy(t => t.Status.ToString())
                                      .ToDictionary(g => g.Key, g => g.Count()),
                ByPriority = tickets.GroupBy(t => t.Priority.ToString())
                                      .ToDictionary(g => g.Key, g => g.Count()),
                ByCategory = tickets.GroupBy(t => t.Category?.Name ?? "N/A")
                                      .ToDictionary(g => g.Key, g => g.Count()),
                ByDay = tickets.GroupBy(t => t.CreatedAt.ToString("yyyy-MM-dd"))
                                      .ToDictionary(g => g.Key, g => g.Count())
            };
        }

        public async Task<AgentPerformanceReportDto> GetAgentPerformanceReportAsync(
            Guid? agentId, ReportFilterDto filter)
        {
            if (agentId is null) return new AgentPerformanceReportDto();

            var agent = await _uow.Users.GetByIdAsync(agentId.Value);
            var tickets = await _uow.TicketReports.GetForReportAsync(filter with { AgentId = agentId });

            await _auditService.LogActionAsync("REPORT_GENERATED", "AgentPerformance", agentId, $"Agent: {agent?.FullName}, Range: {filter.From:d} to {filter.To:d}");

            var resolved = tickets.Where(t =>
                t.Status == TicketStatus.Resolved || t.Status == TicketStatus.Closed).ToList();

            var avgHours = resolved.Any()
                ? resolved.Where(t => t.LastModifiedAt.HasValue)
                          .Average(t => (t.LastModifiedAt!.Value - t.CreatedAt).TotalHours)
                : 0;

            var slaPct = resolved.Any()
                ? (double)resolved.Count(t => !t.SlaBreached) / resolved.Count * 100 : 0;

            var csatScore = await _uow.Csat.GetAverageScoreForAgentAsync(
                agentId.Value, filter.From, filter.To);

            return new AgentPerformanceReportDto
            {
                AgentId = agentId.Value,
                AgentName = agent?.FullName ?? "Unknown",
                TotalResolved = resolved.Count,
                AvgResolutionHours = Math.Round(avgHours, 2),
                SlaCompliancePct = Math.Round(slaPct, 2),
                CsatScore = csatScore
            };
        }

        public async Task<SlaComplianceReportDto> GetSlaComplianceReportAsync(ReportFilterDto filter)
        {
            var tickets = await _uow.TicketReports.GetForReportAsync(filter);
            await _auditService.LogActionAsync("REPORT_GENERATED", "SlaCompliance", null, $"Range: {filter.From:d} to {filter.To:d}");
            var withSla = tickets.Where(t => t.SlaDeadline.HasValue).ToList();

            var withinSla = withSla.Count(t => !t.SlaBreached);
            var breached = withSla.Count(t => t.SlaBreached);
            var pct = withSla.Any() ? (double)withinSla / withSla.Count * 100 : 0;

            return new SlaComplianceReportDto
            {
                TotalTickets = withSla.Count,
                WithinSla = withinSla,
                Breached = breached,
                CompliancePct = Math.Round(pct, 2),
                BreachedByPriority = withSla.Where(t => t.SlaBreached)
                                            .GroupBy(t => t.Priority.ToString())
                                            .ToDictionary(g => g.Key, g => g.Count())
            };
        }

        public async Task<AgentSelfServiceReportDto> GetAgentSelfServiceReportAsync(Guid agentId)
        {
            var now = DateTime.UtcNow;
            var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var filter = new ReportFilterDto { From = monthStart, To = now, AgentId = agentId };
            var tickets = await _uow.TicketReports.GetForReportAsync(filter);
            var resolved = tickets.Where(t =>
                t.Status == TicketStatus.Resolved || t.Status == TicketStatus.Closed).ToList();

            var avgHours = resolved.Any()
                ? resolved.Where(t => t.LastModifiedAt.HasValue)
                          .Average(t => (t.LastModifiedAt!.Value - t.CreatedAt).TotalHours)
                : 0;

            var slaPct = resolved.Any()
                ? (double)resolved.Count(t => !t.SlaBreached) / resolved.Count * 100 : 0;

            var csatScore = await _uow.Csat.GetAverageScoreForAgentAsync(agentId, monthStart, now);
            var comments = await _uow.TicketReports.GetCommentCountByAgentAsync(agentId, monthStart, now);

            return new AgentSelfServiceReportDto
            {
                ResolvedThisMonth = resolved.Count,
                AvgResolutionHours = Math.Round(avgHours, 2),
                SlaCompliancePct = Math.Round(slaPct, 2),
                CsatScore = csatScore,
                TotalComments = comments
            };
        }

        private static byte[] WriteCsv<T>(IEnumerable<T> records)
        {
            using var ms = new MemoryStream();
            using var writer = new StreamWriter(ms,
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
            using var csv = new CsvWriter(writer,
                new CsvConfiguration(CultureInfo.InvariantCulture));
            csv.WriteRecords(records);
            writer.Flush();
            return ms.ToArray();
        }
    }
}
