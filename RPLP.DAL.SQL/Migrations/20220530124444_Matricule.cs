using Microsoft.EntityFrameworkCore.Migrations;
using System.Collections.Generic;

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

            migrationBuilder.Sql(@"CREATE TRIGGER t_gestion_administrateur 
                                      ON Administrators
                                      FOR INSERT
                                      AS

                                    DECLARE @si_admin INT
                                    DECLARE @nombre_admin INT

                                    BEGIN 
	                                    SELECT @si_admin = COUNT(*)
	                                    FROM Administrators
	                                    WHERE Email = 'admin@rplp.com' AND Active = 1

	                                    SELECT @nombre_admin = COUNT(*)
	                                    FROM Administrators

	                                    IF(@nombre_admin > 1)
		                                    BEGIN
			                                    IF(@si_admin = 1)
				                                    BEGIN
					                                    UPDATE Administrators SET active = 0 WHERE Email = 'admin@rplp.com';
				                                    END;
		                                    END;
                                    END;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Matricule",
                table: "Students");


            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS t_gestion_administrateur;");
        }
    }
}
