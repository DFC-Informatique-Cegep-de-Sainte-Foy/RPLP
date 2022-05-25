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

INSERT INTO Classrooms (name,OrganisationName, active) VALUES ('TestingRPLP-classroom-8','TestingRPLP', 1);

INSERT INTO [dbo].[Assignments] ([Name],[ClassroomName],[Description],[DistributionDate] ,[Active],[Classroom_SQLDTOId]) VALUES
('Devoir1','TestingRPLP-classroom-8','assignment presentation du script',CURRENT_TIMESTAMP,1, (SELECT id from Classrooms where name like 'TestingRPLP-classroom-8'));
INSERT INTO [dbo].[Assignments] ([Name],[ClassroomName],[Description],[DistributionDate] ,[Active],[Classroom_SQLDTOId]) VALUES
('Devoir2','TestingRPLP-classroom-8','assignment presentation du script',CURRENT_TIMESTAMP,1, (SELECT id from Classrooms where name like 'TestingRPLP-classroom-8'));
  
INSERT INTO [dbo].[Administrators]([Username],[FirstName],[LastName],[Active],[Token],[Email]) VALUES 
('Ikeameatbol','Jonathan','Blouin',1,'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','blouinjonathan07@gmail.com');

INSERT INTO [dbo].[Administrator_SQLDTOOrganisation_SQLDTO] ([AdministratorsId] ,[OrganisationsId]) VALUES
           ((Select id from Administrators where Username like 'Ikeameatbol'),(Select id from Organisations where Name like 'TestingRPLP'));


