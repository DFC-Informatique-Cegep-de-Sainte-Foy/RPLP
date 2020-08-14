using Microsoft.EntityFrameworkCore.Migrations;

namespace RplpAvecBD.Migrations
{
    public partial class ajoutChampNomProfesseur : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "apiKey",
                table: "Professeur",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(45)",
                oldMaxLength: 45,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "nom",
                table: "Professeur",
                maxLength: 45,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "nom",
                table: "Professeur");

            migrationBuilder.AlterColumn<string>(
                name: "apiKey",
                table: "Professeur",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 40,
                oldNullable: true);
        }
    }
}
