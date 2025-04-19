using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeSystemApi.Migrations
{
    /// <inheritdoc />
    public partial class StudentAddProgramme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProgrammeId",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Students_ProgrammeId",
                table: "Students",
                column: "ProgrammeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Programmes_ProgrammeId",
                table: "Students",
                column: "ProgrammeId",
                principalTable: "Programmes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Programmes_ProgrammeId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_ProgrammeId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ProgrammeId",
                table: "Students");
        }
    }
}
