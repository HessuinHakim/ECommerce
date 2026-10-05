using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOtpEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OTP",
                table: "ApplicationUserOTPs",
                newName: "OtpHash");

            migrationBuilder.RenameColumn(
                name: "IsValid",
                table: "ApplicationUserOTPs",
                newName: "IsUsed");

            migrationBuilder.AddColumn<int>(
                name: "AttemptsCount",
                table: "ApplicationUserOTPs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Purpose",
                table: "ApplicationUserOTPs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttemptsCount",
                table: "ApplicationUserOTPs");

            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "ApplicationUserOTPs");

            migrationBuilder.RenameColumn(
                name: "OtpHash",
                table: "ApplicationUserOTPs",
                newName: "OTP");

            migrationBuilder.RenameColumn(
                name: "IsUsed",
                table: "ApplicationUserOTPs",
                newName: "IsValid");
        }
    }
}
