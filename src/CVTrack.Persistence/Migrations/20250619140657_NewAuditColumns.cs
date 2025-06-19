using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVTrack.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NewAuditColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Audits",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Audits",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Audits");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Audits");
        }
    }
}
