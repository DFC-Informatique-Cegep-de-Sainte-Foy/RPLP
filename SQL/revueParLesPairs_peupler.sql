use revupaire;
-- ------------------------------------------------------------
-- PROCEDURE POUR AJOUTER LE PROFESSEURE
-- @auteur Olena Zagorna
-- ------------------------------------------------------------
drop procedure if exists ajouterProfesseur;
delimiter $$
create procedure ajouterProfesseur(in p_nom varchar(45), p_prenom varchar(45), p_email varchar(45))
Begin
-- -------------------------------------
-- VARIABLES LOCALES
-- -------------------------------------
	declare v_nbProfesseur int default 0;
-- -------------------------------------
-- VALIDATIONS
-- -------------------------------------
	if(p_nom is null) then
		select 'Fin de la procedure. Le nom ne doit pas etre null!' as Erreur;
	elseif (p_email is null) then
		select 'Fin de la procedure. E-mail ne doit pas etre null!' as Erreur;
	else 
		select count(*) into v_nbProfesseur from professeur 	
		where (nom = p_nom && email = p_email);
		
        if (v_nbProfesseur > 0) then
			select 'Fin de la procedure. Le professeur est deja dans la base de donnee!' as Erreur;
        else
			insert into professeur(nom,prenom,email) 
			value (p_nom,p_prenom,p_email);
            SELECT 'Le professeur est ajouté' AS Message;
		end if;
	end if;
End $$
delimiter ;

-- call ajouterProfesseur("Toto","Camille", "1522475@csfoy.ca");
-- call ajouterProfesseur("Toto",null, "1522475@csfoy.ca");
-- call ajouterProfesseur("Nouveaux","Anna", "1525485@csfoy.ca");
-- select * from professeur;


use revupaire;
-- ------------------------------------------------------------
-- PROCEDURE POUR AJOUTER L'ADMIN
-- @auteur Olena Zagorna
-- ------------------------------------------------------------
drop procedure if exists ajouterAdmin;
delimiter $$
create procedure ajouterAdmin(in p_nom varchar(45), p_prenom varchar(45), p_email varchar(45))
Begin
-- -------------------------------------
-- VARIABLES LOCALES
-- -------------------------------------
	declare v_idAdmin int default 0;
    declare v_nbAdmin int default 0; 
    declare v_isAdmin boolean default 0;
-- -------------------------------------
-- VALIDATIONS
-- -------------------------------------
	if(p_nom is null) then
		select 'Fin de la procedure. Le nom ne doit pas etre null!' as Erreur;
	elseif (p_email is null) then
		select 'Fin de la procedure. E-mail ne doit pas etre null!' as Erreur;
	else 
		select count(*) into v_nbAdmin from professeur 	
		where (nom = p_nom && prenom = p_prenom && email = p_email);
        
        if (v_nbAdmin = 0) then
			insert into professeur(nom,prenom,email) 
			value (p_nom,p_prenom,p_email);
			
			select idProfesseur into v_idAdmin from professeur 
			order by idProfesseur DESC limit 1;
		   
			update professeur set isAdmin = 1 where idProfesseur like v_idAdmin;
            SELECT 'L\'admin est ajouté' AS Message;
		else
			select isAdmin into v_isAdmin from professeur 	
			where (nom = p_nom && prenom = p_prenom && email = p_email);
			
			if (v_isAdmin like '1') then 
				select 'Fin de la procedure.L\'admin est deja attribue' as Message;
            else 
				update professeur set isAdmin = 1 where idProfesseur like v_idAdmin;
                select 'L\'admin est attribue' as Message;
            end if;
		end if;
		
	end if;
End $$
delimiter ;


use revupaire;
#----------------------------------------------------------------------------
# Procédure pour ajouter un etudiant
# @auteur Ninkeu Nya Serge Martial
#----------------------------------------------------------------------------
DELIMITER |
DROP PROCEDURE IF EXISTS ajouterEtudiant |

CREATE PROCEDURE ajouterEtudiant(in p_email VARCHAR (45),
								 in p_nom  VARCHAR(45) ,
                                 in p_prenom VARCHAR(45)                              
                                )                                
BEGIN 
-- -------------------------------------
-- VARIABLES LOCALES
-- -------------------------------------
declare v_nbEtudiant int default 0;

-- -------------------------------------
-- VALIDATIONS
-- -------------------------------------
IF(p_email IS NULL) THEN 
		SELECT 'Le Courriel ne doit pas être null !' AS Erreur;
ELSE 
	select count(*) into v_nbEtudiant from etudiant 	
		where (email = p_email);
    if (v_nbEtudiant > 0) then
		select 'Fin de la procedure. L\'etudiante est deja dans la base de donnee!' as Erreur;
	else    
		INSERT INTO `revupaire`.`Etudiant` ( `email`,  `nom`, `prenom`)
		VALUES ( p_email , p_nom, p_prenom);
		SELECT 'L''etudiant est ajouté' AS Message;
	end if;
end if;
END |
DELIMITER ;

-- call ajouterEtudiant('126655@csfoy.ca', 'Serge', null);
-- call ajouterEtudiant('123597@csfoy.ca', 'Philippe', 'Martin');
-- select * from `revupaire`.`Etudiant`;


use revupaire;
#----------------------------------------------------------------------------
# Procédure pour ajouter le cours 
# @auteur Ninkeu Nya Serge Martial
#----------------------------------------------------------------------------
DELIMITER |
DROP PROCEDURE IF EXISTS ajouterCours|
CREATE PROCEDURE ajouterCours(	 in p_nom VARCHAR(45) ,
                                 in p_session VARCHAR(25) ,
                                 in p_Professeur_idProfesseur int(11)
                                 )
                                
BEGIN 
-- -------------------------------------
-- VARIABLES LOCALES
-- -------------------------------------
	declare v_nbCours int default 0;
-- -------------------------------------
-- VALIDATIONS
-- -------------------------------------
	if(p_nom is null) then
		select 'Fin de la procedure. Le nom ne doit pas etre null!' as Erreur;
	elseif (p_session is null) then
		select 'Fin de la procedure. Session ne doit pas etre null!' as Erreur;
	elseif (p_Professeur_idProfesseur is null) then
		select 'Fin de la procedure. IdProfesseur ne doit pas etre null!' as Erreur;
	else 
		select count(*) into v_nbCours from cours 	
		where (nom = p_nom && session = p_session && Professeur_idProfesseur = p_Professeur_idProfesseur);
        if (v_nbCours > 0 ) then 
			select 'Fin de la procedure. Le cours est deja dans la base de donnee!' as Erreur;
		else
			INSERT INTO `revupaire`.`Cours` (`nom`, `session`, `Professeur_idProfesseur`)
			VALUES (p_nom , p_session , p_Professeur_idProfesseur);
			SELECT 'Le cours est ajouté' AS Message;
		end if;
	end if;
END |
DELIMITER ;

-- call ajoutercours('POO', 'H2020', '1');
-- call ajoutercours('BD', null, '1');
-- select * from `revupaire`.`Cours`;


use revupaire;
#----------------------------------------------------------------------------
# Procédure pour ajouter le Travail
# @auteur Ninkeu Nya Serge Martial
#----------------------------------------------------------------------------
DELIMITER |
DROP PROCEDURE IF EXISTS ajouterTravail|

CREATE PROCEDURE ajouterTravail( in p_nom  VARCHAR(45) ,
                                 in p_nbPoints INT ,                                
                                 in p_dateRemise DATETIME ,
                                 in p_creeSurCodePost TINYINT(1) ,
                                 in p_nbRemise INT ,
                                 in p_nbManquant INT ,
                                 in p_Cours_idCours INT 
                                 )                                
BEGIN 
-- -------------------------------------
-- VARIABLES LOCALES
-- -------------------------------------
	declare v_nbTravail int default 0;
-- -------------------------------------
-- VALIDATIONS
-- -------------------------------------
	if(p_nom is null) then
		select 'Fin de la procedure. Le nom ne doit pas etre null!' as Erreur;
	elseif (p_nbPoints is null) then
		select 'Fin de la procedure. Nombre points ne doit pas etre null!' as Erreur;
	elseif (p_dateRemise is null) then
		select 'Fin de la procedure. Date de remise ne doit pas etre null!' as Erreur;
	elseif (p_Cours_idCours is null) then
		select 'Fin de la procedure. IdCours ne doit pas etre null!' as Erreur;
	else 
		select count(*) into v_nbTravail from travail 	
		where (nom = p_nom && nbPoints = p_nbPoints && dateRemise = p_dateRemise && Cours_idCours=p_Cours_idCours);
        if (v_nbTravail > 0 ) then 
			select 'Fin de la procedure. Le travail est deja dans la base de donnee!' as Erreur;
		else
			INSERT INTO `revupaire`.`Travail` (`nom`,   `nbPoints`,  `dateRemise`, `creeSurCodePost`,`nbRemise`,  `nbManquant`, `Cours_idCours`)
			VALUES (p_nom , p_nbPoints , p_dateRemise , p_creeSurCodePost, p_nbRemise, p_nbManquant,p_Cours_idCours);
			SELECT 'Travail est ajouté' AS Message;
		end if;
	end if;

END |
DELIMITER ;

-- call ajouterTravail( 'ExerciceNo4', '100', '20200610', '1', '1', '0','1');
-- call ajouterTravail('ExerciceNo5', '100', '20200610', '1', '1', '0','1');
-- select * from `revupaire`.`Travail` ;


use revupaire;
-- ----------------------------------------------------------------------------------------
-- Peupler la table professeur
-- Peupler manuellement la table professeur avec l'appel de la procedure ajouterProfesseur
-- @auteur Ninkeu Nya Serge Martial
-- ----------------------------------------------------------------------------------------

call ajouterProfesseur('Filion' ,'Alain', 'afilion@csfoy.ca');
call ajouterProfesseur('Ducheneau' ,'Jean-Pierre', 'jpduchesneau@csfoy.ca');
call ajouterProfesseur('Boumso','Andre' , 'aboumso@csfoy.ca');
call ajouterProfesseur('Awdé' ,'Ali', 'aawde@csfoy.ca');
call ajouterProfesseur('Laflamme', 'Robert' ,'rlaflamme@csfoy.ca');
call ajouterProfesseur('Parent', 'Alain' , 'aparent@csfoy.ca');
call ajouterProfesseur('Tousignant' ,'René', 'rtousignang@csfoy.ca');
call ajouterProfesseur('Leon' ,'Pierre-François', 'pfleon@csfoy.ca');
call ajouterProfesseur('Roy' ,'Claude', 'clroy@csfoy.ca');

-- select * from `RevusParLesPairs`.`professeur`;


 use revupaire; 
 -- ---------------------------------------------------------------------------------------------------------
 -- Peupler la table Etudiant
 -- A partir du site http://www.generatedata.com
 -- Les emails adaptés au domaine du cegep (+"@csfoy.ca") 
 --  @auteur Ninkeu Nya Serge Martial
 -- ---------------------------------------------------------------------------------------------------------
 
INSERT INTO `Etudiant` (`email`,`nom`,`prenom`) VALUES ("Donec@csfoy.ca","Glenn","Hunter"),("penatibus@csfoy.ca","Pitts","Yuri"),("dictum.placerat.augue@csfoy.ca","Allen","Keefe"),("per@csfoy.ca","Church","Indira"),("pede.ultrices@csfoy.ca","Medina","Fulton");
INSERT INTO `Etudiant` (`email`,`nom`,`prenom`) VALUES ("gravida.sit@csfoy.ca","Carr","Keiko"),("auctor@csfoy.ca","Cooley","Clio"),("Phasellus.nulla@csfoy.ca","French","Yoshi"),("metus@csfoy.ca","Palmer","Kathleen"),("velit.in.aliquet@csfoy.ca","Grimes","Forrest");
INSERT INTO `Etudiant` (`email`,`nom`,`prenom`) VALUES ("ac.libero@csfoy.ca","Hardin","Tanner"),("amet@csfoy.ca","Farley","Jarrod"),("vitae.diam@csfoy.ca","Garza","Griffith"),("Nunc.pulvinar@csfoy.ca","Madden","Rana"),("felis.purus@csfoy.ca","Lott","Quin");
INSERT INTO `Etudiant` (`email`,`nom`,`prenom`) VALUES ("Nullam.scelerisque.neque@csfoy.ca","Young","Lilah"),("cursus.in.hendrerit@csfoy.ca","Sloan","Magee"),("Mauris.non@csfoy.ca","Bird","Garth"),("risus@csfoy.ca","Stone","Christine"),("consectetuer.adipiscing.elit@csfoy.ca","Stokes","Ashton");
INSERT INTO `Etudiant` (`email`,`nom`,`prenom`) VALUES ("sapien.cursus@csfoy.ca","Scott","James"),("turpis@csfoy.ca","Sims","Fritz"),("auctor.nunc.nulla@csfoy.ca","Johnston","Melinda"),("nisi.magna.sed@csfoy.ca","Hicks","Kevin"),("gravida.mauris.ut@csfoy.ca","Tillman","Raya");
INSERT INTO `Etudiant` (`email`,`nom`,`prenom`) VALUES ("euismod@csfoy.ca","Mccoy","Silas"),("cursus@csfoy.ca","Medina","Buffy"),("eu.augue.porttitor@csfoy.ca","Russo","Stacey"),("sit.amet.risus@csfoy.ca","Sherman","Elliott"),("rhoncus.Donec@csfoy.ca","Clark","Cairo");
INSERT INTO `Etudiant` (`email`,`nom`,`prenom`) VALUES ("vehicula@csfoy.ca","West","Bertha"),("rutrum@csfoy.ca","Quinn","Jasmine"),("lobortis.quis@csfoy.ca","Dominguez","Yuli"),("enim@csfoy.ca","Sears","Richard"),("quis@csfoy.ca","Oliver","Lysandra");
INSERT INTO `Etudiant` (`email`,`nom`,`prenom`) VALUES ("a.odio@csfoy.ca","Cohen","Debra"),("egestas.Fusce@csfoy.ca","Vargas","Amaya"),("convallis.in@csfoy.ca","Noel","Devin"),("Aenean.eget.magna@csfoy.ca","Hart","Wing"),("facilisis.Suspendisse.commodo@csfoy.ca","Bradley","Stacey");
INSERT INTO `Etudiant` (`email`,`nom`,`prenom`) VALUES ("convallis@csfoy.ca","Hammond","Hu"),("ac@csfoy.ca","Crawford","Julie"),("eu.elit@csfoy.ca","Trevino","Noel"),("natoque.penatibus@csfoy.ca","George","Lamar"),("suscipit.nonummy.Fusce@csfoy.ca","Hughes","Noel");
INSERT INTO `Etudiant` (`email`,`nom`,`prenom`) VALUES ("ipsum.nunc@csfoy.ca","Calderon","Zeus"),("scelerisque.neque.sed@csfoy.ca","Mcleod","Samuel"),("malesuada.malesuada.Integer@csfoy.ca","Dotson","Katell"),("ipsum.dolor.sit@csfoy.ca","Dickerson","Kenneth"),("auctor.vitae.aliquet@csfoy.ca","Berger","Kelly");
INSERT INTO `Etudiant` (`email`,`nom`,`prenom`) VALUES ("ultrices.iaculis@csfoy.ca","Gay","Kaye"),("sociis.natoque@csfoy.ca","Oneill","Nathaniel"),("erat@csfoy.ca","Velazquez","Robin"),("semper.rutrum.Fusce@csfoy.ca","Goodwin","Ina"),("neque@csfoy.ca","Mckinney","Bruce");
INSERT INTO `Etudiant` (`email`,`nom`,`prenom`) VALUES ("lacus.Etiam@csfoy.ca","King","Rafael"),("est.congue@csfoy.ca","Beck","Jin"),("Duis@csfoy.ca","Franco","Laura"),("est@csfoy.ca","Shelton","Kevin"),("mollis@csfoy.ca","Chang","Kelsie");
INSERT INTO `Etudiant` (`email`,`nom`,`prenom`) VALUES ("arcu.Vestibulum@csfoy.ca","Frazier","Kerry"),("adipiscing.Mauris@csfoy.ca","Holmes","Shafira"),("vehicula@csfoy.ca","Marquez","Kelly"),("eu.dolor.egestas@csfoy.ca","Franco","Hakeem"),("Curabitur@csfoy.ca","Bell","Elvis");
INSERT INTO `Etudiant` (`email`,`nom`,`prenom`) VALUES ("eu.euismod@csfoy.ca","Gutierrez","Violet"),("imperdiet@csfoy.ca","Yang","Isabelle"),("nec@csfoy.ca","Hopkins","Alfreda"),("id.risus@csfoy.ca","Santos","Charlotte"),("mauris.ipsum.porta@csfoy.ca","Williams","Janna");
INSERT INTO `Etudiant` (`email`,`nom`,`prenom`) VALUES ("vel.turpis@csfoy.ca","Daugherty","Jerome"),("vitae@csfoy.ca","Robinson","Georgia"),("auctor@csfoy.ca","Marsh","Galena"),("enim.Etiam.imperdiet@csfoy.ca","Kent","Adam"),("eget@csfoy.ca","Barry","Shad");
INSERT INTO `Etudiant` (`email`,`nom`,`prenom`) VALUES ("quam@csfoy.ca","Stephens","Ursa"),("enim.condimentum.eget@csfoy.ca","Rivas","Whoopi"),("metus.eu.erat@csfoy.ca","Small","Andrew"),("semper@csfoy.ca","Nunez","Jillian"),("Curabitur.egestas.nunc@csfoy.ca","Underwood","Keaton");
INSERT INTO `Etudiant` (`email`,`nom`,`prenom`) VALUES ("elit.Nulla@csfoy.ca","Mcclure","Baker"),("dui.nec.tempus@csfoy.ca","Sweet","Kiara"),("Cum.sociis@csfoy.ca","Lambert","Garrison"),("tristique.senectus.et@csfoy.ca","Fletcher","Blaze"),("nonummy@csfoy.ca","Perez","Clinton");
INSERT INTO `Etudiant` (`email`,`nom`,`prenom`) VALUES ("Nam.nulla.magna@csfoy.ca","Alvarado","Lucy"),("ac.mi.eleifend@csfoy.ca","Roy","Lester"),("non@csfoy.ca","Shields","Jane"),("malesuada@csfoy.ca","Chase","Katell"),("dolor.tempus@csfoy.ca","Woodward","Chaney");
INSERT INTO `Etudiant` (`email`,`nom`,`prenom`) VALUES ("in.consectetuer.ipsum@csfoy.ca","Kramer","Jarrod"),("justo.Praesent@csfoy.ca","Johns","Eugenia"),("risus.varius@csfoy.ca","Ramirez","Ariana"),("ultrices@csfoy.ca","Velasquez","Fredericka"),("at.pretium.aliquet@csfoy.ca","Raymond","Rebecca");
INSERT INTO `Etudiant` (`email`,`nom`,`prenom`) VALUES ("blandit@csfoy.ca","Ochoa","Galena"),("facilisis.Suspendisse.commodo@csfoy.ca","Green","Anne"),("ornare@csfoy.ca","Wood","Jasper"),("tincidunt.Donec@csfoy.ca","Chan","Unity"),("tempor.est@csfoy.ca","Velasquez","Gillian");

-- select * from `RevusParLesPairs`.`Etudiant`;


use revupaire; 
 -- -------------------------------------------------------------------------------------------------------
 -- Peupler la table Cours
 -- @auteur Olena Zagorna
 -- -------------------------------------------------------------------------------------------------------
call ajouterCours ('420-W30-SF - POOII - 4361','Hiver 2020',8);
call ajouterCours ('420-W30-SF - POOII - 4368','Hiver 2020',8);
call ajouterCours ('420-W31-SF - AA - 4368','Hiver 2020',6);
call ajouterCours ('420-569-SF - PS - 12207','Hiver 2020',8);
call ajouterCours ('420-419-SF - BD - 12030','Hiver 2020',2);
call ajouterCours ('420-219-SF - DM - 10054','Automne 2019',1);
call ajouterCours ('420-329-SF - PG - 10057','Automne 2019',3);
call ajouterCours ('420-339-SF - CR - 10056','Automne 2019',9);
call ajouterCours ('420-459-SF - CS - 12032','Hiver 2020',4);
call ajouterCours ('420-100-SF - EI - 12179','Hiver 2019',5);
 
-- select * from cours;

use revupaire;
 -- ---------------------------------------------------------------------------------------------------------
 -- Peupler la table Travail
 -- A partir du site http://www.generatedata.com
 --  @auteur Ninkeu Nya Serge Martial
 -- ---------------------------------------------------------------------------------------------------------

INSERT INTO `Travail` (`nom`,`nbPoints`,`dateRemise`,`creeSurCodePost`,`nbRemise`,`nbManquant`,`Cours_idCours`) VALUES ("Q7R 9N4",100,"15/11/20",0,15,10,1),("C9I 5A9",100,"18/09/20",1,7,11,1),("R0U 9C7",100,"23/07/19",0,16,12,5),("P6M 0P2",100,"10/08/20",1,9,11,2),("Y0E 9U9",100,"17/07/19",0,4,1,1);
INSERT INTO `Travail` (`nom`,`nbPoints`,`dateRemise`,`creeSurCodePost`,`nbRemise`,`nbManquant`,`Cours_idCours`) VALUES ("E5B 3B2",100,"20/02/20",1,18,15,6),("A0U 5V9",100,"12/04/21",0,15,11,7),("E7Z 9O3",100,"21/02/21",1,15,4,10),("S2Z 5L6",100,"16/08/20",0,3,20,4),("A8N 4C2",100,"05/03/21",1,9,4,10);
INSERT INTO `Travail` (`nom`,`nbPoints`,`dateRemise`,`creeSurCodePost`,`nbRemise`,`nbManquant`,`Cours_idCours`) VALUES ("F1G 3Q6",100,"29/04/21",0,6,20,6),("P5F 8Z3",100,"12/06/20",1,6,3,10),("Z0R 8M4",100,"13/08/20",1,5,19,6),("O7R 6Y7",100,"25/03/20",0,8,18,8),("K8Q 4A1",100,"18/01/20",1,12,5,5);
INSERT INTO `Travail` (`nom`,`nbPoints`,`dateRemise`,`creeSurCodePost`,`nbRemise`,`nbManquant`,`Cours_idCours`) VALUES ("N3R 7L1",100,"01/11/19",0,12,18,7),("V1J 7X8",100,"06/10/20",1,2,9,1),("H9U 4F6",100,"22/05/21",1,8,19,10),("I5E 3W7",100,"17/05/21",0,5,12,6),("Z2O 4C9",100,"25/08/19",0,19,7,7);
INSERT INTO `Travail` (`nom`,`nbPoints`,`dateRemise`,`creeSurCodePost`,`nbRemise`,`nbManquant`,`Cours_idCours`) VALUES ("H4G 3Q4",100,"12/06/20",0,3,1,1),("B7L 7R9",100,"21/01/21",1,2,14,9),("T1A 2Y4",100,"24/05/20",1,20,14,3),("I9V 8C5",100,"25/03/20",0,11,10,8),("X4Z 0N1",100,"03/06/21",1,5,17,9);
INSERT INTO `Travail` (`nom`,`nbPoints`,`dateRemise`,`creeSurCodePost`,`nbRemise`,`nbManquant`,`Cours_idCours`) VALUES ("I0H 2T5",100,"09/03/21",0,8,9,6),("B3D 8E4",100,"15/02/20",1,3,8,8),("B2H 0G1",100,"25/11/20",1,16,3,7),("P2M 6O5",100,"18/01/21",0,11,16,9),("Q9Z 7Q3",100,"12/02/20",1,6,20,2);
INSERT INTO `Travail` (`nom`,`nbPoints`,`dateRemise`,`creeSurCodePost`,`nbRemise`,`nbManquant`,`Cours_idCours`) VALUES ("X0J 8P2",100,"02/03/20",0,16,16,10),("R2C 7V2",100,"16/09/20",0,19,12,6),("Q4Q 6D6",100,"25/04/21",0,3,7,2),("V1L 0O3",100,"09/10/20",0,1,4,4),("E2B 6P0",100,"07/09/20",0,9,17,3);
INSERT INTO `Travail` (`nom`,`nbPoints`,`dateRemise`,`creeSurCodePost`,`nbRemise`,`nbManquant`,`Cours_idCours`) VALUES ("Q4X 8T1",100,"19/09/20",0,19,9,1),("Y1W 6A1",100,"25/11/20",0,10,18,5),("P7U 2I5",100,"16/03/21",1,5,15,2),("H1N 9K4",100,"24/02/21",1,13,8,7),("L0B 0I6",100,"04/05/21",1,8,17,9);
INSERT INTO `Travail` (`nom`,`nbPoints`,`dateRemise`,`creeSurCodePost`,`nbRemise`,`nbManquant`,`Cours_idCours`) VALUES ("N2I 1O9",100,"27/05/21",0,19,20,2),("S1D 4C8",100,"30/10/19",0,3,15,1),("S3P 2W2",100,"07/12/20",0,2,11,7),("A7A 2Y9",100,"15/11/19",0,17,20,4),("O8B 2E3",100,"08/09/19",0,8,3,6);
INSERT INTO `Travail` (`nom`,`nbPoints`,`dateRemise`,`creeSurCodePost`,`nbRemise`,`nbManquant`,`Cours_idCours`) VALUES ("Q1J 8F1",100,"26/10/20",1,19,8,4),("P4D 3M2",100,"14/05/20",1,8,17,6),("I8W 0K8",100,"15/06/19",0,6,16,9),("T9S 6B4",100,"31/03/20",1,7,12,4),("K7H 7W0",100,"29/07/20",1,16,17,9);
INSERT INTO `Travail` (`nom`,`nbPoints`,`dateRemise`,`creeSurCodePost`,`nbRemise`,`nbManquant`,`Cours_idCours`) VALUES ("Q4F 3Y5",100,"13/07/20",1,5,9,10),("Q6V 3I9",100,"06/10/19",1,1,17,9),("U4U 0Y5",100,"16/11/19",0,2,13,7),("H8A 9W1",100,"28/03/20",0,18,5,10),("E1T 3Q1",100,"30/04/20",1,14,3,10);
INSERT INTO `Travail` (`nom`,`nbPoints`,`dateRemise`,`creeSurCodePost`,`nbRemise`,`nbManquant`,`Cours_idCours`) VALUES ("H5G 4B3",100,"12/01/21",0,1,4,4),("L6C 8A1",100,"09/11/20",0,2,13,8),("G4W 4Y4",100,"15/05/20",0,18,17,1),("N1J 9W1",100,"01/03/21",1,16,12,6),("N2F 8R1",100,"24/11/19",1,4,12,2);
INSERT INTO `Travail` (`nom`,`nbPoints`,`dateRemise`,`creeSurCodePost`,`nbRemise`,`nbManquant`,`Cours_idCours`) VALUES ("I2E 1E9",100,"26/12/19",0,3,3,1),("J8X 6P4",100,"15/09/20",1,7,3,10),("Q8P 1X1",100,"05/08/19",0,9,3,10),("C3P 2B1",100,"22/03/20",0,11,2,4),("H9E 5V8",100,"23/10/20",0,12,7,2);
INSERT INTO `Travail` (`nom`,`nbPoints`,`dateRemise`,`creeSurCodePost`,`nbRemise`,`nbManquant`,`Cours_idCours`) VALUES ("U2P 1V3",100,"16/03/21",0,5,4,8),("E1J 7C2",100,"11/04/20",1,15,13,1),("B2X 8S1",100,"28/04/20",0,13,19,2),("D7T 1Y8",100,"10/05/20",1,17,14,8),("I3O 6B6",100,"16/05/21",1,17,14,4);
INSERT INTO `Travail` (`nom`,`nbPoints`,`dateRemise`,`creeSurCodePost`,`nbRemise`,`nbManquant`,`Cours_idCours`) VALUES ("Y1E 6P9",100,"01/03/20",0,2,14,9),("Q8O 8M3",100,"08/12/19",0,1,9,7),("M8E 0F6",100,"07/05/20",1,19,12,2),("K0C 3U3",100,"18/07/19",0,13,18,3),("T3W 0F1",100,"23/10/20",1,1,11,1);
INSERT INTO `Travail` (`nom`,`nbPoints`,`dateRemise`,`creeSurCodePost`,`nbRemise`,`nbManquant`,`Cours_idCours`) VALUES ("A1O 3O1",100,"18/01/20",0,6,3,7),("P2R 0N7",100,"29/06/19",0,8,18,6),("N4U 6H2",100,"20/12/19",1,7,19,6),("Y7P 4D2",100,"25/10/19",0,5,1,8),("Y3X 0K6",100,"19/04/21",0,6,7,2);
INSERT INTO `Travail` (`nom`,`nbPoints`,`dateRemise`,`creeSurCodePost`,`nbRemise`,`nbManquant`,`Cours_idCours`) VALUES ("L2D 0Y8",100,"28/09/20",0,20,19,9),("X6H 1L9",100,"01/02/21",0,12,1,5),("M8U 6A3",100,"27/04/21",1,10,2,2),("J6X 3A8",100,"01/11/20",1,14,12,4),("B3B 7H4",100,"19/10/20",1,1,7,10);
INSERT INTO `Travail` (`nom`,`nbPoints`,`dateRemise`,`creeSurCodePost`,`nbRemise`,`nbManquant`,`Cours_idCours`) VALUES ("E6L 4T1",100,"26/04/20",0,2,9,4),("Y3R 9M1",100,"08/01/21",1,18,4,2),("E7R 1N5",100,"17/10/19",1,18,17,10),("Q8U 4R2",100,"26/06/19",0,6,17,9),("E8M 6G6",100,"12/04/20",0,3,8,8);
INSERT INTO `Travail` (`nom`,`nbPoints`,`dateRemise`,`creeSurCodePost`,`nbRemise`,`nbManquant`,`Cours_idCours`) VALUES ("H5M 7A2",100,"10/07/19",0,9,4,3),("I1D 8M2",100,"02/11/19",0,10,16,10),("G9I 7Q0",100,"20/11/20",0,1,6,3),("C7T 8V5",100,"14/12/19",1,13,20,7),("G8C 2D6",100,"03/08/19",0,11,2,10);
INSERT INTO `Travail` (`nom`,`nbPoints`,`dateRemise`,`creeSurCodePost`,`nbRemise`,`nbManquant`,`Cours_idCours`) VALUES ("L4J 9I1",100,"21/01/21",1,9,14,3),("K8J 4L0",100,"10/11/20",0,3,3,4),("E1Z 4D2",100,"19/02/21",0,18,14,2),("P1T 7R9",100,"01/05/20",1,5,10,3),("G8T 0X8",100,"29/01/21",1,7,8,5);

-- select * from travail;
