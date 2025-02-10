using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CDE.Migrations
{
    public partial class FixModelVisitPlan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitPlans_AspNetUsers_CreatedBy",
                table: "VisitPlans");

            migrationBuilder.DropIndex(
                name: "IX_VisitPlans_CreatedBy",
                table: "VisitPlans");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "VisitPlans",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "VisitPlans",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_VisitPlans_CreatedBy",
                table: "VisitPlans",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitPlans_AspNetUsers_CreatedBy",
                table: "VisitPlans",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
