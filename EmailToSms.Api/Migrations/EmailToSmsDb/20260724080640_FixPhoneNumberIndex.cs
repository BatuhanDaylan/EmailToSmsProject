using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailToSms.Api.Migrations.EmailToSmsDb
{
    /// <inheritdoc />
    public partial class FixPhoneNumberIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RulePhoneNumbers_PhoneNumber",
                table: "RulePhoneNumbers");

            migrationBuilder.CreateIndex(
                name: "IX_RulePhoneNumbers_UserId_PhoneNumber",
                table: "RulePhoneNumbers",
                columns: new[] { "UserId", "PhoneNumber" },
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RulePhoneNumbers_UserId_PhoneNumber",
                table: "RulePhoneNumbers");

            migrationBuilder.CreateIndex(
                name: "IX_RulePhoneNumbers_PhoneNumber",
                table: "RulePhoneNumbers",
                column: "PhoneNumber",
                unique: true,
                filter: "\"IsDeleted\" = false");
        }
    }
}
