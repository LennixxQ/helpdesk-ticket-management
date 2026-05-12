using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDesk.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OptimizePerformance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tickets_IsDeleted",
                table: "Tickets",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Status_CreatedAt",
                table: "Tickets",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_IsDeleted",
                table: "Departments",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_IsDeleted",
                table: "Categories",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityName_EntityId_PerformedAt",
                table: "AuditLogs",
                columns: new[] { "EntityName", "EntityId", "PerformedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tickets_IsDeleted",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_Status_CreatedAt",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Departments_IsDeleted",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Categories_IsDeleted",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_EntityName_EntityId_PerformedAt",
                table: "AuditLogs");
        }
    }
}
