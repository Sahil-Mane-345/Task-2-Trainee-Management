using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraineeApi.Migrations
{
    /// <inheritdoc />
    public partial class SubmissionFileEntitySizepropername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubmisionId",
                table: "SubmissionFiles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SubmisionId",
                table: "SubmissionFiles",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "utf8mb4_0900_ai_ci");
        }
    }
}
