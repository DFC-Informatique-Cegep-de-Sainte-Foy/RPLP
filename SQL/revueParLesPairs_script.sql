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

#----------------------------------------------------------------------------
# Procédure pour ajouter le cours 
# @auteur Ninkeu Nya Serge Martial
#----------------------------------------------------------------------------

DELIMITER |

DROP PROCEDURE IF EXISTS ajouterCours|
CREATE PROCEDURE ajouterCours(	 in p_nom VARCHAR(15) ,
                                 in p_session VARCHAR(15) ,
                                 in p_Professeur_idProfesseur VARCHAR(15)
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


#----------------------------------------------------------------------------
# Procédure pour ajouter le Travail
# @auteur Ninkeu Nya Serge Martial
#----------------------------------------------------------------------------

 describe travail;
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