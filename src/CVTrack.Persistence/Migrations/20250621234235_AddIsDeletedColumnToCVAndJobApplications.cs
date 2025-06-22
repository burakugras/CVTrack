using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVTrack.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedColumnToCVAndJobApplications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "JobApplications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CVs",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CVs");
        }
    }
}
