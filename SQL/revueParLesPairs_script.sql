use revupaire;
-- ------------------------------------------------------------
-- PROCEDURE POUR AJOUTER LE PROFESSEURE
-- @auteur Olena Zagorna
-- ------------------------------------------------------------
drop procedure if exists ajouterProfesseur;
delimiter $$
create procedure ajouterProfesseure(in p_nom varchar(45), p_prenom varchar(45), p_email varchar(45))
Begin
if(p_nom is null) 
then
	select 'Fin de la procedure. Le nom ne doit pas etre null!' as Erreur;
elseif (p_email is null)
then
	select 'Fin de la procedure. E-mail ne doit pas etre null!' as Erreur;
else 
	insert into professeur(nom,prenom,email) 
		value (p_nom,p_prenom,p_email);
	select * from professeur;
end if;
End $$
delimiter ;

