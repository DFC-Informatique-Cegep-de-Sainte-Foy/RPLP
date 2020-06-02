# ***************************************
# partie rename repertoire principal

$repertoirePrincipale = Get-ChildItem  -Directory 
# $repertoire = "Remise_exercices_04_Note_-_Module_420W10SF_12117"

$repertoirePrincipale.name -match 'Remise_(?<nouveauxNom>([A-Za-z]*_?[0-9])+)' 
$nouveauxNomRepertoirePrincipale = $Matches.nouveauxNom
$nouveauxNomRepertoirePrincipale 

Rename-Item -Path $repertoirePrincipale -NewName $nouveauxNomRepertoirePrincipale

# **************************************
# partie rename les sous-repertoires
$sousRepertoiresTravauxEtudiants = $nouveauxNomRepertoirePrincipale | Get-ChildItem  -Directory 

foreach ($repertoire in $sousRepertoiresTravauxEtudiants ) {
    $repertoire.name -match '[A-Za-z]*_(?<numeroMatricule>(\d{7,}))_[A-Za-z]*' 
	$nouveauxNomSouRepertoire = $Matches.numeroMatricule + '@csfoy.ca' 
	Rename-Item -Path $repertoire -NewName $nouveauxNomSouRepertoire 
}
