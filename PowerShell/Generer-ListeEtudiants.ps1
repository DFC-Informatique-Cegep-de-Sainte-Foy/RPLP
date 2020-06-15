# ----------------------------------------------------------------------------------
#
# Variables de base
#
# ----------------------------------------------------------------------------------
$pathCourant = "."
$filtre = "*.csv"
$nbfichiers = [System.IO.Directory]::GetFiles("$pathCourant", "$filtre").count
$fichiers = [System.IO.Directory]::GetFiles("$pathCourant", "$filtre")


# ----------------------------------------------------------------------------------
#
# Fonctions
#
# ----------------------------------------------------------------------------------
Function Get-ListeFichiers {

    Write-Host "Liste des fichiers .CSV trouvÃ©s :" -ForegroundColor Yellow

    $compteur = 1;

    foreach ($fichier in $fichiers) {
        Write-Host $compteur, "-", (Get-Item $fichier).Basename
        $compteur += 1
    }
}

Function Get-FichierChoisi {
    
    Do {
       
        Write-Host ">> Tapez le numÃ©ro du fichier souhaitÃ© : " -NoNewline -ForegroundColor Yellow

        $NofichierChoisi = Read-Host 

        $compteur = 1;

        foreach ($fichier in $fichiers) {
   
            if ($compteur -eq $NofichierChoisi) {
                $fichierChoisi = Get-Item $fichier
                $nomFichierChoisi = $fichierChoisi.Basename
            }

            $compteur += 1
        }

    }While ($NofichierChoisi -lt 1 -or $NofichierChoisi -gt $nbfichiers)
        
    return $nomFichierChoisi, $fichierChoisi
}

Function Get-ConfirmationFichierChoisi($nomFichierChoisi) {
    
    Write-Host $nomFichierChoisi -ForegroundColor green

    Do {

        Write-Host ">> C'est le bon fichier ? " -NoNewline -ForegroundColor Yellow
        Write-Host "O" -NoNewline -ForegroundColor Cyan
        Write-Host "/" -NoNewline -ForegroundColor Yellow
        Write-Host "N" -NoNewline -ForegroundColor Cyan
        Write-Host " (" -NoNewline -ForegroundColor Yellow
        Write-Host "Q" -NoNewline -ForegroundColor Cyan
        Write-Host " - Quitter) " -NoNewline -ForegroundColor Yellow
      
        $estFichierCorrect = Read-Host

    }While ($estFichierCorrect.ToUpper() -ne 'O' -and $estFichierCorrect.ToUpper() -ne 'N' -and $estFichierCorrect.ToUpper() -ne 'Q')

    return $estFichierCorrect
}


# ---------------------------------------------------------------------------------- 
#
# Actions
#
# ----------------------------------------------------------------------------------

if ($nbfichiers -gt 1) {
    
    Write-Host "## GÃ©nÃ©rer la liste des Ã©tudiants inscrits dans le cours :" -ForegroundColor Yellow
    [Environment]::NewLine

    # Afficher la liste des fichiers CSV trouvés dans le répertoire
    Get-ListeFichiers

    # Demander à l'utilisateur de choisir l'un des fichiers
    $nomFichierChoisi, $fichierChoisi = Get-FichierChoisi

    # Afficher le nom du fichier choisi et demander une confirmation à l'utilisateur
    $estFichierCorrect = Get-ConfirmationFichierChoisi($nomFichierChoisi)
}
elseif ($nbfichiers -eq 1) {

    Write-Host "## GÃ©nÃ©rer la liste des Ã©tudiants inscrits dans le cours :" -ForegroundColor Yellow
    [Environment]::NewLine

    # Choisir automatiquement le seule fichier CSV
    $fichierChoisi = Get-Item $fichiers
    $nomFichierChoisi = $fichierChoisi.Basename

    # Afficher le nom du fichier choisi et demander une confirmation à l'utilisateur
    $estFichierCorrect = Get-ConfirmationFichierChoisi($nomFichierChoisi)
}
else {
    
    Write-Host "Aucun fichier CSV trouvÃ© !" -ForegroundColor Red
    Pause
    [Environment]::Exit(1)
}


# Exécuter l'action choisie par l'utilisateur
While ($estFichierCorrect.ToUpper() -eq 'N') {

    Get-ListeFichiers
    $nomFichierChoisi, $fichierChoisi = Get-FichierChoisi
    $estFichierCorrect = Get-ConfirmationFichierChoisi($nomFichierChoisi)
    
}

if ($estFichierCorrect.ToUpper() -eq 'O') {
    
    # Générer la liste
    Write-Host "GÃ©nÃ©rer la liste des Ã©tudiants ..."  

    $listeEtudiantsCSV = Import-Csv -PATH $fichierChoisi -header Matricule -Delimiter ";"

    $listeEtudiantsTXT = $nomFichierChoisi + ".txt"
    New-Item -PATH $pathCourant -Name $listeEtudiantsTXT -ItemType File -Force

    foreach ($uneLigne in $listeEtudiantsCSV) 
    { 

        Write-Host $uneLigne
        
        if ($uneLigne -match '@{Matricule=="(?<numeroMatricule>(\d{7,}))' -or 
        
            $uneLigne -match '@{Matricule=(?<numeroMatricule>(\d{7,}))') {


            $unCourriel = $Matches.numeroMatricule + "@csfoy.ca"
            $unCourriel | Add-Content -PATH $listeEtudiantsTXT

        }
    }
    
    #pause
}