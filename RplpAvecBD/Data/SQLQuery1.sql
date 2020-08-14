/****** Script do comando SelectTopNRows de SSMS  ******/
SELECT TOP (1000) [id]
    ,[courriel]
    ,[apiKey]
    ,[nom]
FROM [RplpDb].[dbo].[Professeur]

use RplpDb;
insert into Professeur (nom, courriel, apiKey) values ('Pierre-Francois Leon', 'pf@csfoy.ca', '000da00da0s00dasd00d0sad00das0d0000sa000');
insert into Professeur (nom, courriel, apiKey) values ('André Boumso', 'aboumso@csfoy.ca', '111gfgfdg1df111jh1k11111kjh1ki11i11o1111');
insert into Professeur (nom, courriel, apiKey) values ('Ali Awdé', 'ali@csfoy.ca', '22as222ds222t2222h2jh2g2kj2222jk22s2df22');


DELETE TOP (5)
FROM [RplpDb].[dbo].[Professeur]