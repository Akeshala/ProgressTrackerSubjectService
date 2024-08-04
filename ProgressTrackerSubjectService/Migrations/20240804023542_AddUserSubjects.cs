using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProgressTrackerSubjectService.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSubjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSubjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SubjectIds = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSubjects", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSubjects_UserId",
                table: "UserSubjects",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSubjects");
        }
    }
}
