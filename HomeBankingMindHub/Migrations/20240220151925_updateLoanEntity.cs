using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeBankingMindHub.Migrations
{
    /// <inheritdoc />
    public partial class updateLoanEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Loans");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Loans",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Loans");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Loans",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
