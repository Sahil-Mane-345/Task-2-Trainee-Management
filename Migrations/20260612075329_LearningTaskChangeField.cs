using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraineeApi.Migrations
{
    /// <inheritdoc />
    public partial class LearningTaskChangeField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpectedTechStack",
                table: "LearningTasks",
                newName: "ExpectedTech");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpectedTech",
                table: "LearningTasks",
                newName: "ExpectedTechStack");
        }
    }
}
