USE [RPLP]
GO

Delete From Repositories;
Delete from Assignments;
Delete From Organisations;
Delete From Classrooms;

Delete From Classroom_SQLDTOStudent_SQLDTO;
Delete From Classroom_SQLDTOTeacher_SQLDTO;
Delete From Administrator_SQLDTOOrganisation_SQLDTO;

Delete From Students;
Delete from Administrators;
Delete from Teachers;

INSERT INTO Organisations (name, active) VALUES ('TestingRPLP',1);
INSERT INTO Organisations (name, active) VALUES ('OrganisationTest',1);
INSERT INTO Organisations (name, active) VALUES ('OrganisationTest2',1);

INSERT INTO Classrooms (name,OrganisationName, active) VALUES ('TestingRPLP-classroom-1','TestingRPLP', 1);
INSERT INTO Classrooms (name,OrganisationName, active) VALUES ('TestingRPLP-classroom-2','TestingRPLP', 1);
INSERT INTO Classrooms (name,OrganisationName, active) VALUES ('TestingRPLP-classroom-3','TestingRPLP', 1);
INSERT INTO Classrooms (name,OrganisationName, active) VALUES ('OrganisationTest-classroom-1','OrganisationTest', 1);
INSERT INTO Classrooms (name,OrganisationName, active) VALUES ('OrganisationTest-classroom-2','OrganisationTest', 1);
INSERT INTO Classrooms (name,OrganisationName, active) VALUES ('OrganisationTest2-classroom-1','OrganisationTest2', 1);
INSERT INTO Classrooms (name,OrganisationName, active) VALUES ('OrganisationTest2-classroom-2','OrganisationTest2', 1);

INSERT INTO [dbo].[Assignments] ([Name],[ClassroomName],[Description],[DistributionDate] ,[Active],[Classroom_SQLDTOId]) VALUES
('Exercice1','TestingRPLP-classroom-1','assignment presentation du script',CURRENT_TIMESTAMP,1, (SELECT id from Classrooms where name like 'TestingRPLP-classroom-1'));
INSERT INTO [dbo].[Assignments] ([Name],[ClassroomName],[Description],[DistributionDate] ,[Active],[Classroom_SQLDTOId]) VALUES
('Exercice2','TestingRPLP-classroom-1','assignment presentation du script',CURRENT_TIMESTAMP,1, (SELECT id from Classrooms where name like 'TestingRPLP-classroom-1'));
INSERT INTO [dbo].[Assignments] ([Name],[ClassroomName],[Description],[DistributionDate] ,[Active],[Classroom_SQLDTOId]) VALUES
('Exercice3','TestingRPLP-classroom-1','assignment presentation du script',CURRENT_TIMESTAMP,1, (SELECT id from Classrooms where name like 'TestingRPLP-classroom-1'));
INSERT INTO [dbo].[Assignments] ([Name],[ClassroomName],[Description],[DistributionDate] ,[Active],[Classroom_SQLDTOId]) VALUES
('Exercice4','TestingRPLP-classroom-1','assignment presentation du script',CURRENT_TIMESTAMP,1, (SELECT id from Classrooms where name like 'TestingRPLP-classroom-1'));
     
INSERT INTO [dbo].[Assignments] ([Name],[ClassroomName],[Description],[DistributionDate] ,[Active],[Classroom_SQLDTOId]) VALUES
('Exercice1.1','TestingRPLP-classroom-2','assignment presentation du script',CURRENT_TIMESTAMP,1, (SELECT id from Classrooms where name like 'TestingRPLP-classroom-2'));  
INSERT INTO [dbo].[Assignments] ([Name],[ClassroomName],[Description],[DistributionDate] ,[Active],[Classroom_SQLDTOId]) VALUES
('Exercice1.2','TestingRPLP-classroom-2','assignment presentation du script',CURRENT_TIMESTAMP,1, (SELECT id from Classrooms where name like 'TestingRPLP-classroom-2'));

INSERT INTO [dbo].[Assignments] ([Name],[ClassroomName],[Description],[DistributionDate] ,[Active],[Classroom_SQLDTOId]) VALUES
('Devoir1','TestingRPLP-classroom-3','assignment presentation du script',CURRENT_TIMESTAMP,1, (SELECT id from Classrooms where name like 'TestingRPLP-classroom-3'));
INSERT INTO [dbo].[Assignments] ([Name],[ClassroomName],[Description],[DistributionDate] ,[Active],[Classroom_SQLDTOId]) VALUES
('Devoir2','TestingRPLP-classroom-3','assignment presentation du script',CURRENT_TIMESTAMP,1, (SELECT id from Classrooms where name like 'TestingRPLP-classroom-3'));

INSERT INTO [dbo].[Assignments] ([Name],[ClassroomName],[Description],[DistributionDate] ,[Active],[Classroom_SQLDTOId]) VALUES
('TP1','OrganisationTest-classroom-1','assignment presentation du script',CURRENT_TIMESTAMP,1, (SELECT id from Classrooms where name like 'OrganisationTest-classroom-1'));  
INSERT INTO [dbo].[Assignments] ([Name],[ClassroomName],[Description],[DistributionDate] ,[Active],[Classroom_SQLDTOId]) VALUES
('TP2','OrganisationTest-classroom-1','assignment presentation du script',CURRENT_TIMESTAMP,1, (SELECT id from Classrooms where name like 'OrganisationTest-classroom-1'));

INSERT INTO [dbo].[Assignments] ([Name],[ClassroomName],[Description],[DistributionDate] ,[Active],[Classroom_SQLDTOId]) VALUES
('Exercice1','OrganisationTest-classroom-2','assignment presentation du script',CURRENT_TIMESTAMP,1, (SELECT id from Classrooms where name like 'OrganisationTest-classroom-2'));
INSERT INTO [dbo].[Assignments] ([Name],[ClassroomName],[Description],[DistributionDate] ,[Active],[Classroom_SQLDTOId]) VALUES
('Exercice2','OrganisationTest-classroom-2','assignment presentation du script',CURRENT_TIMESTAMP,1, (SELECT id from Classrooms where name like 'OrganisationTest-classroom-2'));
INSERT INTO [dbo].[Assignments] ([Name],[ClassroomName],[Description],[DistributionDate] ,[Active],[Classroom_SQLDTOId]) VALUES
('Exercice3','OrganisationTest-classroom-2','assignment presentation du script',CURRENT_TIMESTAMP,1, (SELECT id from Classrooms where name like 'OrganisationTest-classroom-2'));
     
INSERT INTO [dbo].[Administrators]([Username],[FirstName],[LastName],[Active],[Token],[Email]) VALUES 
('Ikeameatbol','Jonathan','Blouin',1,'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','blouinjonathan07@gmail.com');

INSERT INTO [dbo].[Administrator_SQLDTOOrganisation_SQLDTO] ([AdministratorsId] ,[OrganisationsId]) VALUES
           ((Select id from Administrators where Username like 'Ikeameatbol'),(Select id from Organisations where Name like 'TestingRPLP'));

INSERT INTO [dbo].[Administrator_SQLDTOOrganisation_SQLDTO] ([AdministratorsId] ,[OrganisationsId]) VALUES
           ((Select id from Administrators where Username like 'Ikeameatbol'),(Select id from Organisations where Name like 'OrganisationTest'));
 
INSERT INTO [dbo].[Administrator_SQLDTOOrganisation_SQLDTO] ([AdministratorsId] ,[OrganisationsId]) VALUES
           ((Select id from Administrators where Username like 'Ikeameatbol'),(Select id from Organisations where Name like 'OrganisationTest2'));

INSERT INTO [dbo].[Teachers]([Username],[FirstName],[LastName],[Active],[Email]) VALUES
		('ThPaquet','Thierry','Paquet',1 ,'dremaziel@gmail.com');

INSERT INTO [dbo].[Classroom_SQLDTOTeacher_SQLDTO]([ClassesId],[TeachersId])VALUES
	 ((Select id from Classrooms where Name like 'OrganisationTest-classroom-1'), (Select id from Teachers where Username like 'ThPaquet'));

GO



