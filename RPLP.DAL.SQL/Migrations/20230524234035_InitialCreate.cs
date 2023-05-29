using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPLP.DAL.SQL.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Administrators",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Allocations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RepositoryId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: true),
                    TeacherId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WrittenBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RepositoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Diff_Hunk = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Original_Position = table.Column<int>(type: "int", nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    In_Reply_To_Id = table.Column<int>(type: "int", nullable: true),
                    Created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organisations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Matricule = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsTutor = table.Column<bool>(type: "bit", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Administrator_SQLDTOOrganisation_SQLDTO",
                columns: table => new
                {
                    AdministratorsId = table.Column<int>(type: "int", nullable: false),
                    OrganisationsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrator_SQLDTOOrganisation_SQLDTO", x => new { x.AdministratorsId, x.OrganisationsId });
                    table.ForeignKey(
                        name: "FK_Administrator_SQLDTOOrganisation_SQLDTO_Administrators_AdministratorsId",
                        column: x => x.AdministratorsId,
                        principalTable: "Administrators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Administrator_SQLDTOOrganisation_SQLDTO_Organisations_OrganisationsId",
                        column: x => x.OrganisationsId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Classrooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganisationId = table.Column<int>(type: "int", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classrooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Classrooms_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Repositories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrganisationId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repositories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Repositories_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClassroomId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliveryDeadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DistributionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignments_Classrooms_ClassroomId",
                        column: x => x.ClassroomId,
                        principalTable: "Classrooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Classroom_SQLDTOStudent_SQLDTO",
                columns: table => new
                {
                    ClassesId = table.Column<int>(type: "int", nullable: false),
                    StudentsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classroom_SQLDTOStudent_SQLDTO", x => new { x.ClassesId, x.StudentsId });
                    table.ForeignKey(
                        name: "FK_Classroom_SQLDTOStudent_SQLDTO_Classrooms_ClassesId",
                        column: x => x.ClassesId,
                        principalTable: "Classrooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Classroom_SQLDTOStudent_SQLDTO_Students_StudentsId",
                        column: x => x.StudentsId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Classroom_SQLDTOTeacher_SQLDTO",
                columns: table => new
                {
                    ClassesId = table.Column<int>(type: "int", nullable: false),
                    TeachersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classroom_SQLDTOTeacher_SQLDTO", x => new { x.ClassesId, x.TeachersId });
                    table.ForeignKey(
                        name: "FK_Classroom_SQLDTOTeacher_SQLDTO_Classrooms_ClassesId",
                        column: x => x.ClassesId,
                        principalTable: "Classrooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Classroom_SQLDTOTeacher_SQLDTO_Teachers_TeachersId",
                        column: x => x.TeachersId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            
            migrationBuilder.CreateTable(
                name: "UserInfo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    administratorId = table.Column<int>(type: null, nullable: true),
                    studentId = table.Column<int>(type: null, nullable: true),
                    teacherId = table.Column<int>(type: null, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfo", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserInfo_AdministratorID",
                        column: x => x.administratorId,
                        principalTable: "Administrators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserInfo_StudentID",
                        column: x => x.studentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserInfo_TeacherID",
                        column: x => x.teacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
               name: "IX_UserInfo_Email",
               table: "UserInfo",
               column: "Email",
               unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserInfo_Username",
                table: "UserInfo",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Administrator_SQLDTOOrganisation_SQLDTO_OrganisationsId",
                table: "Administrator_SQLDTOOrganisation_SQLDTO",
                column: "OrganisationsId");

            migrationBuilder.CreateIndex(
                name: "IX_Administrators_Email",
                table: "Administrators",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Administrators_Username",
                table: "Administrators",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Administrators_Username_Email",
                table: "Administrators",
                columns: new[] { "Username", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_ClassroomId",
                table: "Assignments",
                column: "ClassroomId");

            migrationBuilder.CreateIndex(
                name: "IX_Classroom_SQLDTOStudent_SQLDTO_StudentsId",
                table: "Classroom_SQLDTOStudent_SQLDTO",
                column: "StudentsId");

            migrationBuilder.CreateIndex(
                name: "IX_Classroom_SQLDTOTeacher_SQLDTO_TeachersId",
                table: "Classroom_SQLDTOTeacher_SQLDTO",
                column: "TeachersId");

            migrationBuilder.CreateIndex(
                name: "IX_Classrooms_OrganisationId",
                table: "Classrooms",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_OrganisationId",
                table: "Repositories",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_Email",
                table: "Students",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_Username",
                table: "Students",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_Username_Email",
                table: "Students",
                columns: new[] { "Username", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_Email",
                table: "Teachers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_Username",
                table: "Teachers",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_Username_Email",
                table: "Teachers",
                columns: new[] { "Username", "Email" },
                unique: true);
            migrationBuilder.Sql("CREATE OR ALTER TRIGGER AdminInsertToUserInfo " +
                "ON Administrators " +
                "AFTER INSERT " +
                "AS " +
                "BEGIN " +
                "SET NOCOUNT ON; " +
                "DECLARE @username varchar(50); " +
                "DECLARE @email varchar(100); " +
                "DECLARE @id int; " +
                "SELECT  " +
                "@username = username, " +
                "@email = email, " +
                "@id = id " +
                "FROM INSERTED; " +
                "INSERT INTO UserInfo (username, email, administratorId)  " +
                "VALUES (@username, @email, @id); " +
                "END");

            migrationBuilder.Sql("CREATE OR ALTER TRIGGER StudentInsertToUserInfo " +
                "ON Students " +
                "AFTER INSERT " +
                "AS " +
                "BEGIN " +
                "SET NOCOUNT ON; " +
                "DECLARE @username varchar(50); " +
                "DECLARE @email varchar(100); " +
                "DECLARE @id int; " +
                "SELECT  " +
                "@username = username, " +
                "@email = email, " +
                "@id = id " +
                "FROM INSERTED; " +
                "INSERT INTO UserInfo (username, email, studentId)  " +
                "VALUES (@username, @email, @id); " +
                "END");

            migrationBuilder.Sql("CREATE OR ALTER TRIGGER TeacherInsertToUserInfo " +
                "ON Teachers " +
                "AFTER INSERT " +
                "AS " +
                "BEGIN " +
                "SET NOCOUNT ON; " +
                "DECLARE @username varchar(50); " +
                "DECLARE @email varchar(100); " +
                "DECLARE @id int; " +
                "SELECT  " +
                "@username = username, " +
                "@email = email, " +
                "@id = id " +
                "FROM INSERTED; " +
                "INSERT INTO UserInfo (username, email, teacherId)  " +
                "VALUES (@username, @email, @id);" +
                "END");
            
            migrationBuilder.Sql(@"INSERT INTO Administrators (Username, FirstName, LastName, Active, Token, Email) VALUES('admin','admin','admin',1,'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','admin@rplp.com');");

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
            migrationBuilder.Sql("DROP TRIGGER AdminInsertToUserInfo");
            migrationBuilder.Sql("DROP TRIGGER StudentInsertToUserInfo");
            migrationBuilder.Sql("DROP TRIGGER TeacherInsertToUserInfo");
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS t_gestion_administrateur;");

            migrationBuilder.DropTable(
                name: "UserInfo");

            migrationBuilder.DropIndex(
                name: "IX_UserInfo_Username",
                table: "UserInfo");

            migrationBuilder.DropIndex(
                name: "IX_UserInfo_Email",
                table: "UserInfo");
            
            migrationBuilder.DropTable(
                name: "Administrator_SQLDTOOrganisation_SQLDTO");

            migrationBuilder.DropTable(
                name: "Allocations");

            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "Classroom_SQLDTOStudent_SQLDTO");

            migrationBuilder.DropTable(
                name: "Classroom_SQLDTOTeacher_SQLDTO");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Repositories");

            migrationBuilder.DropTable(
                name: "Administrators");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Classrooms");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "Organisations");
        }
    }
}
