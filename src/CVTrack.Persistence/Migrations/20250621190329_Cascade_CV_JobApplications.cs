using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVTrack.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Cascade_CV_JobApplications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_CVs_CVId",
                table: "JobApplications");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_CVs_CVId",
                table: "JobApplications",
                column: "CVId",
                principalTable: "CVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobApplications_CVs_CVId",
                table: "JobApplications");

            migrationBuilder.AddForeignKey(
                name: "FK_JobApplications_CVs_CVId",
                table: "JobApplications",
                column: "CVId",
                principalTable: "CVs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
