using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailToSms.Api.Migrations.EmailToSmsDb
{
    /// <inheritdoc />
    public partial class UpdateAuditLogForeignKeyBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_ForwardingRules_ForwardingRuleId",
                table: "AuditLogs");

            migrationBuilder.AddColumn<int>(
                name: "ForwardingRuleId1",
                table: "AuditLogs",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ForwardingRuleId1",
                table: "AuditLogs",
                column: "ForwardingRuleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_ForwardingRules_ForwardingRuleId",
                table: "AuditLogs",
                column: "ForwardingRuleId",
                principalTable: "ForwardingRules",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_ForwardingRules_ForwardingRuleId1",
                table: "AuditLogs",
                column: "ForwardingRuleId1",
                principalTable: "ForwardingRules",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_ForwardingRules_ForwardingRuleId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_ForwardingRules_ForwardingRuleId1",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_ForwardingRuleId1",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "ForwardingRuleId1",
                table: "AuditLogs");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_ForwardingRules_ForwardingRuleId",
                table: "AuditLogs",
                column: "ForwardingRuleId",
                principalTable: "ForwardingRules",
                principalColumn: "Id");
        }
    }
}
