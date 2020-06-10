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
			where (nom = p_nom && prenom = p_prenom && email = p_email);
		if (v_nbProfesseur > 0) 
        then
			select 'Fin de la procedure. Le professeur est deja dans la base de donnee!' as Erreur;
        else
			insert into professeur(nom,prenom,email) 
				value (p_nom,p_prenom,p_email);
			select * from professeur;
		end if;
	end if;
End $$
delimiter ;

