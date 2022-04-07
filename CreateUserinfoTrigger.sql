CREATE OR ALTER TRIGGER AdminInsertToUserInfo 
ON Administrators 
AFTER INSERT 
AS 
BEGIN
    SET NOCOUNT ON;
    DECLARE @username varchar(50);
    DECLARE @email varchar(100);
    DECLARE @id int;

    SELECT 
        @username = username,
        @email = email,
        @id = id
    FROM INSERTED;

        INSERT INTO UserInfo (username, email, administratorId) 
        VALUES (@username, @email, @id);
END