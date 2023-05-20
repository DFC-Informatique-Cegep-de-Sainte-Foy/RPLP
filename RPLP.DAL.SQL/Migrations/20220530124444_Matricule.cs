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

                migrationBuilder.Sql(@"INSERT INTO Administrators VALUES('admin','admin','admin',1,'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','admin@rplp.com');");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Matricule",
                table: "Students");
        }
    }
}
