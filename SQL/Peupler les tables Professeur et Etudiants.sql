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