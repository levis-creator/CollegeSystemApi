using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CollegeSystemApi.Migrations
{
    /// <inheritdoc />
    public partial class AcademicYear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "AcademicYears",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Year",
                table: "AcademicYears");
        }
    }
}
