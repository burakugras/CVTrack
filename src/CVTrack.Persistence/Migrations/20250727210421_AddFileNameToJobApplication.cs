using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVTrack.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFileNameToJobApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "JobApplications",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "JobApplications");
        }
    }
}
