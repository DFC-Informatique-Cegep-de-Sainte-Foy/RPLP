USE [RPLP]
GO

Delete From Organisations;
Delete from Assignments;
Delete From Classroom_SQLDTOStudent_SQLDTO;
Delete From Classrooms;
Delete From Students;
Delete From Repositories;

INSERT INTO Organisations (name, active) VALUES ('TestingRPLP',1);

INSERT INTO Classrooms (name, active) VALUES ('TestingRPLP-classroom-1', 1);

INSERT INTO Students (username, firstName, lastName, active) VALUES ('catdrugg','Jonathan','Blouin', 1);
INSERT INTO Students (username, firstName, lastName, active) VALUES ('thpaquet','Thierry','Paquet', 1);
INSERT INTO Students (username, firstName, lastName, active) VALUES ('1996167','Melissa','Lachapelle', 1);

INSERT INTO [dbo].[Assignments] ([Name],[ClassroomName],[Description],[DistributionDate] ,[Active],[Classroom_SQLDTOId]) VALUES
('TestPresentation','TestingRPLP-classroom-1','assignment presentation du script',CURRENT_TIMESTAMP,1, (SELECT id from Classrooms where name like 'TestingRPLP-classroom-1'));
          

INSERT INTO [dbo].[Classroom_SQLDTOStudent_SQLDTO] (ClassesId,StudentsId) VALUES 
((SELECT id from Classrooms where name like 'TestingRPLP-classroom-1'),(SELECT id from Students where Username like 'catdrugg'));

INSERT INTO [dbo].[Classroom_SQLDTOStudent_SQLDTO] (ClassesId,StudentsId) VALUES 
((SELECT id from Classrooms where name like 'TestingRPLP-classroom-1'),(SELECT id from Students where Username like 'ThPaquet'));

INSERT INTO [dbo].[Classroom_SQLDTOStudent_SQLDTO] (ClassesId,StudentsId) VALUES 
((SELECT id from Classrooms where name like 'TestingRPLP-classroom-1'),(SELECT id from Students where Username like '1996167'));

INSERT INTO [dbo].[Repositories] ([Name],[OrganisationName],[FullName],[Active]) VALUES
('testpresentation-catdrugg','TestingRPLP','TestingRPLP/testpresentation-catdrugg',1);

INSERT INTO [dbo].[Repositories] ([Name],[OrganisationName],[FullName],[Active]) VALUES
('testpresentation-ThPaquet','TestingRPLP','TestingRPLP/testpresentation-ThPaquet',1);

INSERT INTO [dbo].[Repositories] ([Name],[OrganisationName],[FullName],[Active]) VALUES
('testpresentation-1996167','TestingRPLP','TestingRPLP/testpresentation-1996167',1);

GO



