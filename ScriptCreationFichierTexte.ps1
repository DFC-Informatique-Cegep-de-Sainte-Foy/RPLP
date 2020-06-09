Get-ExecutionPolicy
Set-ExecutionPolicy Bypass

$DossierEtudiant = Get-ChildItem  -Directory 
# $repertoire = "Remise_exercices_04_Note_-_Module_420W10SF_12117"


$sousRepertoiresTravauxEtudiants = $DossierEtudiant | Get-ChildItem  -Directory 

$ListeCourrielEtudiant = $DossierEtudiant.name + ".txt"

New-Item -Path $ListeCourrielEtudiant -ItemType File 


foreach ($repertoire in $sousRepertoiresTravauxEtudiants ) {
	$repertoire.name | Add-Content -Path $ListeCourrielEtudiant 
}