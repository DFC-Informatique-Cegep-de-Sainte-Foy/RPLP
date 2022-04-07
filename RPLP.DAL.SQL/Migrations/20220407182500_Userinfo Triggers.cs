using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPLP.DAL.SQL.Migrations
{
    public partial class UserinfoTriggers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER AdminInsertToUserInfo");
            migrationBuilder.Sql("DROP TRIGGER StudentInsertToUserInfo");
            migrationBuilder.Sql("DROP TRIGGER TeacherInsertToUserInfo");
        }
    }
}
