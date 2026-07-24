using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailToSms.Api.Migrations.EmailToSmsDb
{
    /// <inheritdoc />
    public partial class MakePhoneNumbersIndependent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "RulePhoneNumbers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "RulePhoneNumbers");
        }
    }
}
