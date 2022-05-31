using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPLP.DAL.SQL.Migrations
{
    public partial class Matricule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Matricule",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Matricule",
                table: "Students");
        }
    }
}
