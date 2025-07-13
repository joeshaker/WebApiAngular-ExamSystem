using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiAngular.Migrations
{
    /// <inheritdoc />
    public partial class Cascade2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Options_SelectedOptionId",
                table: "Answers");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Options_SelectedOptionId",
                table: "Answers",
                column: "SelectedOptionId",
                principalTable: "Options",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Options_SelectedOptionId",
                table: "Answers");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Options_SelectedOptionId",
                table: "Answers",
                column: "SelectedOptionId",
                principalTable: "Options",
                principalColumn: "Id");
        }
    }
}
