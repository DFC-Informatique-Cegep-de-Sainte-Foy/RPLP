 use revupaire;
#----------------------------------------------------------------------------
# Procédure pour ajouter un etudiant
# @auteur Ninkeu Nya Serge Martial
#----------------------------------------------------------------------------

 
 DELIMITER |
DROP PROCEDURE IF EXISTS ajouterEtudiant |

CREATE PROCEDURE ajouterEtudiant(in p_idEtudiant INT ,
								 in p_email VARCHAR (45),
								 in p_nom  VARCHAR(45) ,
                                 in p_prenom VARCHAR(45)                              
                                )                                
BEGIN 
DECLARE violation_unicite CONDITION FOR 1062;
    
    DECLARE EXIT HANDLER FOR violation_unicite
		BEGIN 
			SELECT 'Violation de contrainte d''unicité' AS Erreur;
		END;
IF(p_email IS NULL)
     THEN 
		SELECT 'Le Courriel ne doit pas être null !' AS Erreur;
ELSE 
    
	INSERT INTO `revupaire`.`Etudiant` (`idEtudiant`  , `email`,  `nom`, `prenom`)
	VALUES (p_idEtudiant , p_email , p_nom, p_prenom);
    SELECT 'L''etudiant est ajouté' AS Message;
end if;
END |
DELIMITER ;

 -- call ajouterEtudiant(1, '126655@csfoy.ca', 'Serge', null);
 -- call ajouterEtudiant('2', '123597@csfoy.ca', 'Philippe', 'Martin');
 -- select * from `revupaire`.`Etudiant`;