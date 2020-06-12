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

