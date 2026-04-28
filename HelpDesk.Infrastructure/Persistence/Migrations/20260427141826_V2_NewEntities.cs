using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDesk.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class V2_NewEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "Tickets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Tickets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsEscalated",
                table: "Tickets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ReopenCount",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SlaBreached",
                table: "Tickets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SlaDeadline",
                table: "Tickets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SlaStatus",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "AuditLogs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OldValue",
                table: "AuditLogDetails",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NewValue",
                table: "AuditLogDetails",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CsatResponses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RespondentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClosingAgentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CsatResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CsatResponses_AspNetUsers_ClosingAgentId",
                        column: x => x.ClosingAgentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CsatResponses_AspNetUsers_RespondentId",
                        column: x => x.RespondentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CsatResponses_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    DepartmentHeadId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_AspNetUsers_DepartmentHeadId",
                        column: x => x.DepartmentHeadId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EmailLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipientUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    AttemptCount = table.Column<int>(type: "int", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailLogs_AspNetUsers_RecipientUserId",
                        column: x => x.RecipientUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EscalationRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Trigger = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EscalatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EscalatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AcknowledgedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AcknowledgedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EscalationRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EscalationRecords_AspNetUsers_AcknowledgedById",
                        column: x => x.AcknowledgedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EscalationRecords_AspNetUsers_EscalatedById",
                        column: x => x.EscalatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EscalationRecords_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KbArticles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    NotHelpfulCount = table.Column<int>(type: "int", nullable: false),
                    HelpfulCount = table.Column<int>(type: "int", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastUpdatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KbArticles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KbArticles_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KbArticles_AspNetUsers_LastUpdatedById",
                        column: x => x.LastUpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KbArticles_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NotificationPreferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationPreferences_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecurringTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TicketTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RecurrencePattern = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CronExpression = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaxOccurrences = table.Column<int>(type: "int", nullable: true),
                    RunCount = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    NextRunAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastRunAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignToAgentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RaiseOnBehalfOfId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecurringTemplates_AspNetUsers_AssignToAgentId",
                        column: x => x.AssignToAgentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringTemplates_AspNetUsers_CreatedByAdminId",
                        column: x => x.CreatedByAdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringTemplates_AspNetUsers_RaiseOnBehalfOfId",
                        column: x => x.RaiseOnBehalfOfId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringTemplates_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SlaPolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FirstResponseMinutes = table.Column<int>(type: "int", nullable: false),
                    ResolutionMinutes = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlaPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SlaRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SlaDeadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PausedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalPausedMinutes = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsBreached = table.Column<bool>(type: "bit", nullable: false),
                    BreachedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsOverridden = table.Column<bool>(type: "bit", nullable: false),
                    OverriddenById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OverrideReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlaRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SlaRecords_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemSettings_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "KbArticleVersions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KbArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SavedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SavedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KbArticleVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KbArticleVersions_AspNetUsers_SavedById",
                        column: x => x.SavedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KbArticleVersions_KbArticles_KbArticleId",
                        column: x => x.KbArticleId,
                        principalTable: "KbArticles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecurringTemplateRuns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GeneratedTicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActualRunAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringTemplateRuns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecurringTemplateRuns_RecurringTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "RecurringTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecurringTemplateRuns_Tickets_GeneratedTicketId",
                        column: x => x.GeneratedTicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_DepartmentId",
                table: "Tickets",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_DepartmentId",
                table: "AspNetUsers",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CsatResponses_ClosingAgentId",
                table: "CsatResponses",
                column: "ClosingAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_CsatResponses_RespondentId",
                table: "CsatResponses",
                column: "RespondentId");

            migrationBuilder.CreateIndex(
                name: "IX_CsatResponses_TicketId",
                table: "CsatResponses",
                column: "TicketId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_DepartmentHeadId",
                table: "Departments",
                column: "DepartmentHeadId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailLogs_IsSuccess",
                table: "EmailLogs",
                column: "IsSuccess");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLogs_RecipientUserId",
                table: "EmailLogs",
                column: "RecipientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLogs_SentAt",
                table: "EmailLogs",
                column: "SentAt");

            migrationBuilder.CreateIndex(
                name: "IX_EscalationRecords_AcknowledgedById",
                table: "EscalationRecords",
                column: "AcknowledgedById");

            migrationBuilder.CreateIndex(
                name: "IX_EscalationRecords_EscalatedAt",
                table: "EscalationRecords",
                column: "EscalatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_EscalationRecords_EscalatedById",
                table: "EscalationRecords",
                column: "EscalatedById");

            migrationBuilder.CreateIndex(
                name: "IX_EscalationRecords_TicketId",
                table: "EscalationRecords",
                column: "TicketId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KbArticles_AuthorId",
                table: "KbArticles",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_KbArticles_CategoryId",
                table: "KbArticles",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_KbArticles_LastUpdatedById",
                table: "KbArticles",
                column: "LastUpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_KbArticles_Status",
                table: "KbArticles",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_KbArticleVersions_KbArticleId",
                table: "KbArticleVersions",
                column: "KbArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_KbArticleVersions_SavedById",
                table: "KbArticleVersions",
                column: "SavedById");

            migrationBuilder.CreateIndex(
                name: "IX_KbArticleVersions_VersionNumber",
                table: "KbArticleVersions",
                column: "VersionNumber");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationPreferences_UserId_EventType",
                table: "NotificationPreferences",
                columns: new[] { "UserId", "EventType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTemplateRuns_GeneratedTicketId",
                table: "RecurringTemplateRuns",
                column: "GeneratedTicketId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTemplateRuns_ScheduledAt",
                table: "RecurringTemplateRuns",
                column: "ScheduledAt");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTemplateRuns_TemplateId",
                table: "RecurringTemplateRuns",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTemplates_AssignToAgentId",
                table: "RecurringTemplates",
                column: "AssignToAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTemplates_CategoryId",
                table: "RecurringTemplates",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTemplates_CreatedByAdminId",
                table: "RecurringTemplates",
                column: "CreatedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTemplates_IsActive",
                table: "RecurringTemplates",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTemplates_NextRunAt",
                table: "RecurringTemplates",
                column: "NextRunAt");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringTemplates_RaiseOnBehalfOfId",
                table: "RecurringTemplates",
                column: "RaiseOnBehalfOfId");

            migrationBuilder.CreateIndex(
                name: "IX_SlaPolicies_Priority",
                table: "SlaPolicies",
                column: "Priority",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SlaRecords_SlaDeadline",
                table: "SlaRecords",
                column: "SlaDeadline");

            migrationBuilder.CreateIndex(
                name: "IX_SlaRecords_Status",
                table: "SlaRecords",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SlaRecords_TicketId",
                table: "SlaRecords",
                column: "TicketId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_Key",
                table: "SystemSettings",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_UpdatedById",
                table: "SystemSettings",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Departments_DepartmentId",
                table: "AspNetUsers",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_AspNetUsers_UserId",
                table: "AuditLogs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Departments_DepartmentId",
                table: "Tickets",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Departments_DepartmentId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_AspNetUsers_UserId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Departments_DepartmentId",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "CsatResponses");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "EmailLogs");

            migrationBuilder.DropTable(
                name: "EscalationRecords");

            migrationBuilder.DropTable(
                name: "KbArticleVersions");

            migrationBuilder.DropTable(
                name: "NotificationPreferences");

            migrationBuilder.DropTable(
                name: "RecurringTemplateRuns");

            migrationBuilder.DropTable(
                name: "SlaPolicies");

            migrationBuilder.DropTable(
                name: "SlaRecords");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "KbArticles");

            migrationBuilder.DropTable(
                name: "RecurringTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_DepartmentId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_DepartmentId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "IsEscalated",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ReopenCount",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "SlaBreached",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "SlaDeadline",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "SlaStatus",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "OldValue",
                table: "AuditLogDetails",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NewValue",
                table: "AuditLogDetails",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);
        }
    }
}
