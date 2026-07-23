using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace EmailToSms.Api.Migrations.EmailToSmsDb
{
    /// <inheritdoc />
    public partial class InitialAppDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    EmailAddress = table.Column<string>(type: "text", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    ClientSecret = table.Column<string>(type: "text", nullable: false),
                    LastCheckedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RulePhoneNumbers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RulePhoneNumbers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ForwardingRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SenderKeyword = table.Column<string>(type: "text", nullable: true),
                    SubjectKeyword = table.Column<string>(type: "text", nullable: true),
                    BodyKeyword = table.Column<string>(type: "text", nullable: true),
                    EmailAccountId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForwardingRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForwardingRules_EmailAccounts_EmailAccountId",
                        column: x => x.EmailAccountId,
                        principalTable: "EmailAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    Details = table.Column<string>(type: "text", nullable: false),
                    IsSuccess = table.Column<bool>(type: "boolean", nullable: false),
                    EmailAccountId = table.Column<int>(type: "integer", nullable: true),
                    ForwardingRuleId = table.Column<int>(type: "integer", nullable: true),
                    ForwardingRuleId1 = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_EmailAccounts_EmailAccountId",
                        column: x => x.EmailAccountId,
                        principalTable: "EmailAccounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AuditLogs_ForwardingRules_ForwardingRuleId",
                        column: x => x.ForwardingRuleId,
                        principalTable: "ForwardingRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AuditLogs_ForwardingRules_ForwardingRuleId1",
                        column: x => x.ForwardingRuleId1,
                        principalTable: "ForwardingRules",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ForwardingRulePhoneNumbers",
                columns: table => new
                {
                    ForwardingRulesId = table.Column<int>(type: "integer", nullable: false),
                    TargetPhoneNumbersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForwardingRulePhoneNumbers", x => new { x.ForwardingRulesId, x.TargetPhoneNumbersId });
                    table.ForeignKey(
                        name: "FK_ForwardingRulePhoneNumbers_ForwardingRules_ForwardingRulesId",
                        column: x => x.ForwardingRulesId,
                        principalTable: "ForwardingRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ForwardingRulePhoneNumbers_RulePhoneNumbers_TargetPhoneNumb~",
                        column: x => x.TargetPhoneNumbersId,
                        principalTable: "RulePhoneNumbers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EmailAccountId",
                table: "AuditLogs",
                column: "EmailAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ForwardingRuleId",
                table: "AuditLogs",
                column: "ForwardingRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ForwardingRuleId1",
                table: "AuditLogs",
                column: "ForwardingRuleId1");

            migrationBuilder.CreateIndex(
                name: "IX_EmailAccounts_UserId_EmailAddress",
                table: "EmailAccounts",
                columns: new[] { "UserId", "EmailAddress" },
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_ForwardingRulePhoneNumbers_TargetPhoneNumbersId",
                table: "ForwardingRulePhoneNumbers",
                column: "TargetPhoneNumbersId");

            migrationBuilder.CreateIndex(
                name: "IX_ForwardingRules_EmailAccountId",
                table: "ForwardingRules",
                column: "EmailAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_RulePhoneNumbers_PhoneNumber",
                table: "RulePhoneNumbers",
                column: "PhoneNumber",
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "ForwardingRulePhoneNumbers");

            migrationBuilder.DropTable(
                name: "ForwardingRules");

            migrationBuilder.DropTable(
                name: "RulePhoneNumbers");

            migrationBuilder.DropTable(
                name: "EmailAccounts");
        }
    }
}
