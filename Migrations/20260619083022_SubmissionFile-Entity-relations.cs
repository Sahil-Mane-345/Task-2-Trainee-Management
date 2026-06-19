using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraineeApi.Migrations
{
    /// <inheritdoc />
    public partial class SubmissionFileEntityrelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploadedByUser",
                table: "SubmissionFiles");

            migrationBuilder.AddColumn<Guid>(
                name: "SubmisionId",
                table: "SubmissionFiles",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "utf8mb4_0900_ai_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "SubmissionId",
                table: "SubmissionFiles",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "utf8mb4_0900_ai_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "SubmissionFiles",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "utf8mb4_0900_ai_ci");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionFiles_SubmissionId",
                table: "SubmissionFiles",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionFiles_UserId",
                table: "SubmissionFiles",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionFiles_Submissions_SubmissionId",
                table: "SubmissionFiles",
                column: "SubmissionId",
                principalTable: "Submissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionFiles_Users_UserId",
                table: "SubmissionFiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionFiles_Submissions_SubmissionId",
                table: "SubmissionFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionFiles_Users_UserId",
                table: "SubmissionFiles");

            migrationBuilder.DropIndex(
                name: "IX_SubmissionFiles_SubmissionId",
                table: "SubmissionFiles");

            migrationBuilder.DropIndex(
                name: "IX_SubmissionFiles_UserId",
                table: "SubmissionFiles");

            migrationBuilder.DropColumn(
                name: "SubmisionId",
                table: "SubmissionFiles");

            migrationBuilder.DropColumn(
                name: "SubmissionId",
                table: "SubmissionFiles");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SubmissionFiles");

            migrationBuilder.AddColumn<string>(
                name: "UploadedByUser",
                table: "SubmissionFiles",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
