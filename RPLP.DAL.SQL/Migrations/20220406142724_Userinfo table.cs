﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPLP.DAL.SQL.Migrations
{
    public partial class Userinfotable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserInfo");

            migrationBuilder.DropIndex(
                name: "IX_UserInfo_Username",
                table: "UserInfo");

            migrationBuilder.DropIndex(
                name: "IX_UserInfo_Email",
                table: "UserInfo");


        }
    }
}