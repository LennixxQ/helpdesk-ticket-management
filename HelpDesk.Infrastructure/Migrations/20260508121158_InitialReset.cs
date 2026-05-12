using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDesk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialReset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //    migrationBuilder.CreateTable(
            //        name: "AuditLogs",
            //        columns: table => new
            //        {
            //            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            EntityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //            EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
            //            PerformedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //            ActorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //            ActorEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //            ActorRole = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //            PerformedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
            //            AdditionalNotes = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_AuditLogs", x => x.Id);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "Categories",
            //        columns: table => new
            //        {
            //            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //            Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
            //            IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
            //            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //            LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            IsDeleted = table.Column<bool>(type: "bit", nullable: false),
            //            DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_Categories", x => x.Id);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "Roles",
            //        columns: table => new
            //        {
            //            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            //            NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            //            ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_Roles", x => x.Id);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "SlaPolicies",
            //        columns: table => new
            //        {
            //            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            Priority = table.Column<int>(type: "int", nullable: false),
            //            FirstResponseMinutes = table.Column<int>(type: "int", nullable: false),
            //            ResolutionMinutes = table.Column<int>(type: "int", nullable: false),
            //            IsActive = table.Column<bool>(type: "bit", nullable: false),
            //            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //            LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            IsDeleted = table.Column<bool>(type: "bit", nullable: false),
            //            DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_SlaPolicies", x => x.Id);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "AuditLogDetails",
            //        columns: table => new
            //        {
            //            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            AuditLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            FieldName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //            OldValue = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
            //            NewValue = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_AuditLogDetails", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_AuditLogDetails_AuditLogs_AuditLogId",
            //                column: x => x.AuditLogId,
            //                principalTable: "AuditLogs",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "RoleClaims",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(type: "int", nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_RoleClaims", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_RoleClaims_Roles_RoleId",
            //                column: x => x.RoleId,
            //                principalTable: "Roles",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "Comments",
            //        columns: table => new
            //        {
            //            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
            //            TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //            LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            IsDeleted = table.Column<bool>(type: "bit", nullable: false),
            //            DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_Comments", x => x.Id);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "CsatResponses",
            //        columns: table => new
            //        {
            //            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            RespondentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            ClosingAgentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            Score = table.Column<int>(type: "int", nullable: false),
            //            Comments = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
            //            SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //            LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            IsDeleted = table.Column<bool>(type: "bit", nullable: false),
            //            DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_CsatResponses", x => x.Id);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "Departments",
            //        columns: table => new
            //        {
            //            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //            IsActive = table.Column<bool>(type: "bit", nullable: false),
            //            DepartmentHeadId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            //            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //            LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            IsDeleted = table.Column<bool>(type: "bit", nullable: false),
            //            DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_Departments", x => x.Id);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "Users",
            //        columns: table => new
            //        {
            //            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            //            Role = table.Column<int>(type: "int", nullable: false),
            //            IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
            //            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //            LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            //            DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            //            MfaSecretKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            IsMfaEnabled = table.Column<bool>(type: "bit", nullable: false),
            //            UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            //            NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            //            Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            //            NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            //            EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
            //            PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
            //            TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
            //            LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
            //            LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
            //            AccessFailedCount = table.Column<int>(type: "int", nullable: false)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_Users", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_Users_Departments_DepartmentId",
            //                column: x => x.DepartmentId,
            //                principalTable: "Departments",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.SetNull);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "EmailLogs",
            //        columns: table => new
            //        {
            //            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            RecipientUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            ToEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //            Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
            //            EventType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //            IsSuccess = table.Column<bool>(type: "bit", nullable: false),
            //            AttemptCount = table.Column<int>(type: "int", nullable: false),
            //            FailureReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
            //            SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //            LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            IsDeleted = table.Column<bool>(type: "bit", nullable: false),
            //            DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_EmailLogs", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_EmailLogs_Users_RecipientUserId",
            //                column: x => x.RecipientUserId,
            //                principalTable: "Users",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Restrict);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "KbArticles",
            //        columns: table => new
            //        {
            //            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
            //            Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //            Tags = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
            //            Status = table.Column<int>(type: "int", nullable: false),
            //            ViewCount = table.Column<int>(type: "int", nullable: false),
            //            HelpfulCount = table.Column<int>(type: "int", nullable: false),
            //            NotHelpfulCount = table.Column<int>(type: "int", nullable: false),
            //            VersionNumber = table.Column<int>(type: "int", nullable: false),
            //            CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            LastUpdatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //            LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            IsDeleted = table.Column<bool>(type: "bit", nullable: false),
            //            DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_KbArticles", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_KbArticles_Categories_CategoryId",
            //                column: x => x.CategoryId,
            //                principalTable: "Categories",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Restrict);
            //            table.ForeignKey(
            //                name: "FK_KbArticles_Users_AuthorId",
            //                column: x => x.AuthorId,
            //                principalTable: "Users",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Restrict);
            //            table.ForeignKey(
            //                name: "FK_KbArticles_Users_LastUpdatedById",
            //                column: x => x.LastUpdatedById,
            //                principalTable: "Users",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Restrict);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "NotificationPreferences",
            //        columns: table => new
            //        {
            //            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            EventType = table.Column<int>(type: "int", nullable: false),
            //            IsEnabled = table.Column<bool>(type: "bit", nullable: false),
            //            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //            LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            IsDeleted = table.Column<bool>(type: "bit", nullable: false),
            //            DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_NotificationPreferences", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_NotificationPreferences_Users_UserId",
            //                column: x => x.UserId,
            //                principalTable: "Users",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "RecurringTemplates",
            //        columns: table => new
            //        {
            //            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            TemplateName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
            //            TicketTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
            //            Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
            //            Priority = table.Column<int>(type: "int", nullable: false),
            //            RecurrencePattern = table.Column<int>(type: "int", nullable: false),
            //            CronExpression = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
            //            StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            MaxOccurrences = table.Column<int>(type: "int", nullable: true),
            //            RunCount = table.Column<int>(type: "int", nullable: false),
            //            IsActive = table.Column<bool>(type: "bit", nullable: false),
            //            NextRunAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            LastRunAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            CreatedByAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            AssignToAgentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            //            RaiseOnBehalfOfId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //            LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            IsDeleted = table.Column<bool>(type: "bit", nullable: false),
            //            DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_RecurringTemplates", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_RecurringTemplates_Categories_CategoryId",
            //                column: x => x.CategoryId,
            //                principalTable: "Categories",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Restrict);
            //            table.ForeignKey(
            //                name: "FK_RecurringTemplates_Users_AssignToAgentId",
            //                column: x => x.AssignToAgentId,
            //                principalTable: "Users",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Restrict);
            //            table.ForeignKey(
            //                name: "FK_RecurringTemplates_Users_CreatedByAdminId",
            //                column: x => x.CreatedByAdminId,
            //                principalTable: "Users",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Restrict);
            //            table.ForeignKey(
            //                name: "FK_RecurringTemplates_Users_RaiseOnBehalfOfId",
            //                column: x => x.RaiseOnBehalfOfId,
            //                principalTable: "Users",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Restrict);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "SystemSettings",
            //        columns: table => new
            //        {
            //            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
            //            Value = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
            //            Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
            //            UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            UpdatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_SystemSettings", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_SystemSettings_Users_UpdatedById",
            //                column: x => x.UpdatedById,
            //                principalTable: "Users",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.SetNull);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "UserClaims",
            //        columns: table => new
            //        {
            //            Id = table.Column<int>(type: "int", nullable: false)
            //                .Annotation("SqlServer:Identity", "1, 1"),
            //            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_UserClaims", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_UserClaims_Users_UserId",
            //                column: x => x.UserId,
            //                principalTable: "Users",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "UserLogins",
            //        columns: table => new
            //        {
            //            LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //            ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //            ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
            //            table.ForeignKey(
            //                name: "FK_UserLogins_Users_UserId",
            //                column: x => x.UserId,
            //                principalTable: "Users",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "UserRoles",
            //        columns: table => new
            //        {
            //            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
            //            table.ForeignKey(
            //                name: "FK_UserRoles_Roles_RoleId",
            //                column: x => x.RoleId,
            //                principalTable: "Roles",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //            table.ForeignKey(
            //                name: "FK_UserRoles_Users_UserId",
            //                column: x => x.UserId,
            //                principalTable: "Users",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "UserTokens",
            //        columns: table => new
            //        {
            //            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //            Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
            //            Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
            //            table.ForeignKey(
            //                name: "FK_UserTokens_Users_UserId",
            //                column: x => x.UserId,
            //                principalTable: "Users",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "KbArticleVersions",
            //        columns: table => new
            //        {
            //            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            VersionNumber = table.Column<int>(type: "int", nullable: false),
            //            Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
            //            Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //            KbArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            SavedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            SavedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_KbArticleVersions", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_KbArticleVersions_KbArticles_KbArticleId",
            //                column: x => x.KbArticleId,
            //                principalTable: "KbArticles",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //            table.ForeignKey(
            //                name: "FK_KbArticleVersions_Users_SavedByUserId",
            //                column: x => x.SavedByUserId,
            //                principalTable: "Users",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Restrict);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "Tickets",
            //        columns: table => new
            //        {
            //            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
            //            Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
            //            Priority = table.Column<int>(type: "int", nullable: false),
            //            Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
            //            IsEscalated = table.Column<bool>(type: "bit", nullable: false),
            //            ReopenCount = table.Column<int>(type: "int", nullable: false),
            //            IsArchived = table.Column<bool>(type: "bit", nullable: false),
            //            SlaDeadline = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            SlaStatus = table.Column<int>(type: "int", nullable: false),
            //            SlaBreached = table.Column<bool>(type: "bit", nullable: false),
            //            IsResolvedViaKb = table.Column<bool>(type: "bit", nullable: false),
            //            ResolvedViaKbArticleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            //            ArchivedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            AffectedAsset = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            RelatedTicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            //            CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            RaisedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            AssignedAgentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            //            DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            //            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //            LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            LastModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
            //            IsDeleted = table.Column<bool>(type: "bit", nullable: false),
            //            DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_Tickets", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_Tickets_Categories_CategoryId",
            //                column: x => x.CategoryId,
            //                principalTable: "Categories",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Restrict);
            //            table.ForeignKey(
            //                name: "FK_Tickets_Departments_DepartmentId",
            //                column: x => x.DepartmentId,
            //                principalTable: "Departments",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.SetNull);
            //            table.ForeignKey(
            //                name: "FK_Tickets_KbArticles_ResolvedViaKbArticleId",
            //                column: x => x.ResolvedViaKbArticleId,
            //                principalTable: "KbArticles",
            //                principalColumn: "Id");
            //            table.ForeignKey(
            //                name: "FK_Tickets_Tickets_RelatedTicketId",
            //                column: x => x.RelatedTicketId,
            //                principalTable: "Tickets",
            //                principalColumn: "Id");
            //            table.ForeignKey(
            //                name: "FK_Tickets_Users_AssignedAgentId",
            //                column: x => x.AssignedAgentId,
            //                principalTable: "Users",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Restrict);
            //            table.ForeignKey(
            //                name: "FK_Tickets_Users_RaisedByUserId",
            //                column: x => x.RaisedByUserId,
            //                principalTable: "Users",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Restrict);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "EscalationRecords",
            //        columns: table => new
            //        {
            //            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
            //            Trigger = table.Column<int>(type: "int", nullable: false),
            //            EscalatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //            EscalatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            //            EscalatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            AcknowledgedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            //            AcknowledgedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //            LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            IsDeleted = table.Column<bool>(type: "bit", nullable: false),
            //            DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_EscalationRecords", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_EscalationRecords_Tickets_TicketId",
            //                column: x => x.TicketId,
            //                principalTable: "Tickets",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //            table.ForeignKey(
            //                name: "FK_EscalationRecords_Users_AcknowledgedByUserId",
            //                column: x => x.AcknowledgedByUserId,
            //                principalTable: "Users",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Restrict);
            //            table.ForeignKey(
            //                name: "FK_EscalationRecords_Users_EscalatedByUserId",
            //                column: x => x.EscalatedByUserId,
            //                principalTable: "Users",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Restrict);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "RecurringTemplateRuns",
            //        columns: table => new
            //        {
            //            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            GeneratedTicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            //            ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            ActualRunAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            IsSuccess = table.Column<bool>(type: "bit", nullable: false),
            //            FailureReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_RecurringTemplateRuns", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_RecurringTemplateRuns_RecurringTemplates_TemplateId",
            //                column: x => x.TemplateId,
            //                principalTable: "RecurringTemplates",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //            table.ForeignKey(
            //                name: "FK_RecurringTemplateRuns_Tickets_GeneratedTicketId",
            //                column: x => x.GeneratedTicketId,
            //                principalTable: "Tickets",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.SetNull);
            //        });

            //    migrationBuilder.CreateTable(
            //        name: "SlaRecords",
            //        columns: table => new
            //        {
            //            Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //            SlaDeadline = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            PausedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            TotalPausedMinutes = table.Column<int>(type: "int", nullable: false),
            //            Status = table.Column<int>(type: "int", nullable: false),
            //            IsBreached = table.Column<bool>(type: "bit", nullable: false),
            //            BreachedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            IsOverridden = table.Column<bool>(type: "bit", nullable: false),
            //            OverriddenById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            //            OverrideReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
            //            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            //            CreatedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
            //            LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
            //            IsDeleted = table.Column<bool>(type: "bit", nullable: false),
            //            DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
            //            DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //        },
            //        constraints: table =>
            //        {
            //            table.PrimaryKey("PK_SlaRecords", x => x.Id);
            //            table.ForeignKey(
            //                name: "FK_SlaRecords_Tickets_TicketId",
            //                column: x => x.TicketId,
            //                principalTable: "Tickets",
            //                principalColumn: "Id",
            //                onDelete: ReferentialAction.Cascade);
            //        });

            //    migrationBuilder.CreateIndex(
            //        name: "IX_AuditLogDetails_AuditLogId",
            //        table: "AuditLogDetails",
            //        column: "AuditLogId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_AuditLogs_EntityId",
            //        table: "AuditLogs",
            //        column: "EntityId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_AuditLogs_PerformedAt",
            //        table: "AuditLogs",
            //        column: "PerformedAt");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Categories_Name",
            //        table: "Categories",
            //        column: "Name",
            //        unique: true);

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Comments_TicketId",
            //        table: "Comments",
            //        column: "TicketId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Comments_UserId",
            //        table: "Comments",
            //        column: "UserId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_CsatResponses_ClosingAgentId",
            //        table: "CsatResponses",
            //        column: "ClosingAgentId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_CsatResponses_RespondentId",
            //        table: "CsatResponses",
            //        column: "RespondentId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_CsatResponses_TicketId",
            //        table: "CsatResponses",
            //        column: "TicketId",
            //        unique: true);

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Departments_DepartmentHeadId",
            //        table: "Departments",
            //        column: "DepartmentHeadId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Departments_Name",
            //        table: "Departments",
            //        column: "Name",
            //        unique: true);

            //    migrationBuilder.CreateIndex(
            //        name: "IX_EmailLogs_RecipientUserId",
            //        table: "EmailLogs",
            //        column: "RecipientUserId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_EscalationRecords_AcknowledgedByUserId",
            //        table: "EscalationRecords",
            //        column: "AcknowledgedByUserId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_EscalationRecords_EscalatedByUserId",
            //        table: "EscalationRecords",
            //        column: "EscalatedByUserId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_EscalationRecords_TicketId",
            //        table: "EscalationRecords",
            //        column: "TicketId",
            //        unique: true);

            //    migrationBuilder.CreateIndex(
            //        name: "IX_KbArticles_AuthorId",
            //        table: "KbArticles",
            //        column: "AuthorId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_KbArticles_CategoryId",
            //        table: "KbArticles",
            //        column: "CategoryId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_KbArticles_LastUpdatedById",
            //        table: "KbArticles",
            //        column: "LastUpdatedById");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_KbArticleVersions_KbArticleId",
            //        table: "KbArticleVersions",
            //        column: "KbArticleId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_KbArticleVersions_SavedByUserId",
            //        table: "KbArticleVersions",
            //        column: "SavedByUserId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_NotificationPreferences_UserId_EventType",
            //        table: "NotificationPreferences",
            //        columns: new[] { "UserId", "EventType" },
            //        unique: true);

            //    migrationBuilder.CreateIndex(
            //        name: "IX_RecurringTemplateRuns_GeneratedTicketId",
            //        table: "RecurringTemplateRuns",
            //        column: "GeneratedTicketId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_RecurringTemplateRuns_TemplateId",
            //        table: "RecurringTemplateRuns",
            //        column: "TemplateId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_RecurringTemplates_AssignToAgentId",
            //        table: "RecurringTemplates",
            //        column: "AssignToAgentId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_RecurringTemplates_CategoryId",
            //        table: "RecurringTemplates",
            //        column: "CategoryId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_RecurringTemplates_CreatedByAdminId",
            //        table: "RecurringTemplates",
            //        column: "CreatedByAdminId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_RecurringTemplates_IsActive",
            //        table: "RecurringTemplates",
            //        column: "IsActive");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_RecurringTemplates_NextRunAt",
            //        table: "RecurringTemplates",
            //        column: "NextRunAt");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_RecurringTemplates_RaiseOnBehalfOfId",
            //        table: "RecurringTemplates",
            //        column: "RaiseOnBehalfOfId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_RoleClaims_RoleId",
            //        table: "RoleClaims",
            //        column: "RoleId");

            //    migrationBuilder.CreateIndex(
            //        name: "RoleNameIndex",
            //        table: "Roles",
            //        column: "NormalizedName",
            //        unique: true,
            //        filter: "[NormalizedName] IS NOT NULL");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_SlaPolicies_Priority",
            //        table: "SlaPolicies",
            //        column: "Priority",
            //        unique: true);

            //    migrationBuilder.CreateIndex(
            //        name: "IX_SlaRecords_SlaDeadline",
            //        table: "SlaRecords",
            //        column: "SlaDeadline");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_SlaRecords_TicketId",
            //        table: "SlaRecords",
            //        column: "TicketId",
            //        unique: true);

            //    migrationBuilder.CreateIndex(
            //        name: "IX_SystemSettings_Key",
            //        table: "SystemSettings",
            //        column: "Key",
            //        unique: true);

            //    migrationBuilder.CreateIndex(
            //        name: "IX_SystemSettings_UpdatedById",
            //        table: "SystemSettings",
            //        column: "UpdatedById");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Tickets_AssignedAgentId",
            //        table: "Tickets",
            //        column: "AssignedAgentId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Tickets_CategoryId",
            //        table: "Tickets",
            //        column: "CategoryId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Tickets_CreatedAt",
            //        table: "Tickets",
            //        column: "CreatedAt");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Tickets_DepartmentId",
            //        table: "Tickets",
            //        column: "DepartmentId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Tickets_RaisedByUserId",
            //        table: "Tickets",
            //        column: "RaisedByUserId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Tickets_RelatedTicketId",
            //        table: "Tickets",
            //        column: "RelatedTicketId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Tickets_ResolvedViaKbArticleId",
            //        table: "Tickets",
            //        column: "ResolvedViaKbArticleId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Tickets_Status",
            //        table: "Tickets",
            //        column: "Status");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_UserClaims_UserId",
            //        table: "UserClaims",
            //        column: "UserId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_UserLogins_UserId",
            //        table: "UserLogins",
            //        column: "UserId");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_UserRoles_RoleId",
            //        table: "UserRoles",
            //        column: "RoleId");

            //    migrationBuilder.CreateIndex(
            //        name: "EmailIndex",
            //        table: "Users",
            //        column: "NormalizedEmail");

            //    migrationBuilder.CreateIndex(
            //        name: "IX_Users_DepartmentId",
            //        table: "Users",
            //        column: "DepartmentId");

            //    migrationBuilder.CreateIndex(
            //        name: "UserNameIndex",
            //        table: "Users",
            //        column: "NormalizedUserName",
            //        unique: true,
            //        filter: "[NormalizedUserName] IS NOT NULL");

            //    migrationBuilder.AddForeignKey(
            //        name: "FK_Comments_Tickets_TicketId",
            //        table: "Comments",
            //        column: "TicketId",
            //        principalTable: "Tickets",
            //        principalColumn: "Id",
            //        onDelete: ReferentialAction.Cascade);

            //    migrationBuilder.AddForeignKey(
            //        name: "FK_Comments_Users_UserId",
            //        table: "Comments",
            //        column: "UserId",
            //        principalTable: "Users",
            //        principalColumn: "Id",
            //        onDelete: ReferentialAction.Restrict);

            //    migrationBuilder.AddForeignKey(
            //        name: "FK_CsatResponses_Tickets_TicketId",
            //        table: "CsatResponses",
            //        column: "TicketId",
            //        principalTable: "Tickets",
            //        principalColumn: "Id",
            //        onDelete: ReferentialAction.Restrict);

            //    migrationBuilder.AddForeignKey(
            //        name: "FK_CsatResponses_Users_ClosingAgentId",
            //        table: "CsatResponses",
            //        column: "ClosingAgentId",
            //        principalTable: "Users",
            //        principalColumn: "Id",
            //        onDelete: ReferentialAction.Restrict);

            //    migrationBuilder.AddForeignKey(
            //        name: "FK_CsatResponses_Users_RespondentId",
            //        table: "CsatResponses",
            //        column: "RespondentId",
            //        principalTable: "Users",
            //        principalColumn: "Id",
            //        onDelete: ReferentialAction.Restrict);

            //    migrationBuilder.AddForeignKey(
            //        name: "FK_Departments_Users_DepartmentHeadId",
            //        table: "Departments",
            //        column: "DepartmentHeadId",
            //        principalTable: "Users",
            //        principalColumn: "Id",
            //        onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Users_DepartmentHeadId",
                table: "Departments");

            migrationBuilder.DropTable(
                name: "AuditLogDetails");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "CsatResponses");

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
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "SlaPolicies");

            migrationBuilder.DropTable(
                name: "SlaRecords");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "RecurringTemplates");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "KbArticles");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
