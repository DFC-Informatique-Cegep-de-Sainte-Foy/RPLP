# ----------------------------------------------------------------------------------
# Variables de base
# ----------------------------------------------------------------------------------
$fichierPath = "."
$filtre = "Remise*"
$nbfichiers = [System.IO.Directory]::GetFiles("$fichierPath", "$filtre").count
$fichiers = [System.IO.Directory]::GetFiles("$fichierPath", "$filtre")

# ----------------------------------------------------------------------------------
# Fonctions
# ----------------------------------------------------------------------------------
Function Get-ListeFichiers {
    
    Write-Host 'Liste des fichiers trouvés :',`n -ForegroundColor Yellow

    $compteur = 1;

    foreach ($fichier in $fichiers) {
        Write-Host $compteur, '-', (Get-Item $fichier).Basename
        $compteur += 1
    }

}

Function Get-FichierChoisi {
    
    Do {

        Write-Host `n,'>> Tapez le numéro du fichier souhaité : ' -NoNewline -ForegroundColor Yellow
        
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

    Write-Host `n, $nomFichierChoisi -ForegroundColor green

    Do {

        Write-Host `n,">> C'est le bon fichier ? " -NoNewline -ForegroundColor Yellow
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
# Actions
# ----------------------------------------------------------------------------------

# Afficher la liste des fichiers trouvés dans le répertoire
Get-ListeFichiers

# Demander à l'utilisateur de choisir un fichier
$nomFichierChoisi, $fichierChoisi = Get-FichierChoisi

# Afficher le nom du fichier choisi et demander une confirmation à l'utilisateur
$estFichierCorrect = Get-ConfirmationFichierChoisi($nomFichierChoisi)

# Exécuter action choisi
While ($estFichierCorrect.ToUpper() -eq 'N') {

    Write-Host `n
    Get-ListeFichiers
    $nomFichierChoisi, $fichierChoisi = Get-FichierChoisi
    $estFichierCorrect = Get-ConfirmationFichierChoisi($nomFichierChoisi)
    
}

if ($estFichierCorrect.ToUpper() -eq 'O') {
    
    # Demander à l'utilisateur d'entrer le nom du travail qui va apparaître sur codePost
    Write-Host `n,">> Entrez le nom souhaité pour le travail : " -NoNewline -ForegroundColor Yellow
    $nomTravail = Read-Host
    
    # Définir le chemin du répertoire temporaire selon le OS de l'utilisateur
    $repertoireTemp = "~/Temp"

    # Créer le répertoire temporaire s'il n'existe pas encore  
    if (-not (Test-Path -Path $repertoireTemp)) {
    
        try {
            New-Item -Path $repertoireTemp -ItemType Directory -ErrorAction Stop | Out-Null #-Force
        }
        catch {
            Write-Error -Message "Erreur au moment de créer le répertoire '$repertoireTemp'. Erreur : $_" -ErrorAction Stop
        }
        
        Write-Host `n,"Répertoire '$repertoireTemp' créé avec succès !"
    }
    
    # Copier le fichier dans le répertoire temporaire
    Copy-Item -Path $fichierChoisi -Destination $repertoireTemp

    # Définir le chemin du fichier dans le répertoire temporaire
    $pathTempFichier = "$repertoireTemp/$nomFichierChoisi.zip"

    # Decompresser le fichier dans le répertoire temporaire
    Expand-Archive -Path $pathTempFichier -DestinationPath $repertoireTemp -Force ##### Erreur si le path est trop long sur Windows
    #Expand-7Zip -ArchiveFileName $pathTempFichier -TargetPath $repertoireTemp
    
    # Faire le nettoyage
    $include = @("*.suo", "*.user", "*.userosscache", "*.sln.docstates", ".vs", "bin", "obj", "build", "*.class", ".settings", ".classpath", ".project")
    $exclude = @()

    $items = Get-ChildItem "$repertoireTemp/$nomFichierChoisi" -Recurse -Force -Include $include -Exclude $exclude

    foreach ($item in $items) {
        Remove-Item $item.FullName -Force -Recurse -ErrorAction SilentlyContinue
        Write-Host " Supprimer" $item.FullName
    }

    # Renommer le répertoire racine dans le répertoire temporaire    
    Write-Host `n,'Renommer le r�pertoire racine...'
    Rename-Item -Path $repertoireTemp/$nomFichierChoisi -NewName $nomTravail

    # Renommer les sous-répertoires dans le répertoire temporaire
    $sousRepertoires = Get-ChildItem "$repertoireTemp/$nomTravail" 

    foreach ($repertoire in $sousRepertoires ) {
                
        if ($repertoire.name -match '[A-Za-z]*_(?<numeroMatricule>(\d{7,}))_[A-Za-z]*') {
    
            try {

                $nouveauxNomRepertoire = $Matches.numeroMatricule + '@csfoy.ca' 
	            
                $nomRepertoireCourant = $repertoire.BaseName

                Write-Host $nomRepertoireCourant

                Rename-Item -Path $repertoireTemp/$nomTravail/$nomRepertoireCourant -NewName $nouveauxNomRepertoire
            }
            catch {

                Write-Error -Message "Erreur au moment de renommer le répertoire '$repertoire'. Erreur : $_" -ErrorAction Stop
                pause
            }
        }
    }

    # Créer le fichier .txt avec la liste de courriels des étudiants
    $sousRepertoires = Get-ChildItem "$repertoireTemp/$nomTravail" 

    $listeCourriels = 'ListeCourriels_' + $nomTravail + ".txt"

    New-Item -Path $repertoireTemp -Name $listeCourriels -ItemType File -Force

    foreach ($repertoire in $sousRepertoires ) {
	
        $repertoire.name | Add-Content -Path $repertoireTemp/$listeCourriels
    }
    
    # Copier le répertoire du travail et la liste des courriels dans le répertoire d'origine
    if (Test-Path -Path ./$nomTravail) {
    
        try {
            Remove-Item -Path ./$nomTravail -Force -Recurse
            Remove-Item -Path ./$listeCourriels -Force
        }
        catch {
            Write-Error -Message "Erreur au moment de supprimer le répertoire './$nomTravail'. Erreur : $_" -ErrorAction Stop
        }
        
        Write-Host `n,"Répertoire './$nomTravail' supprimé avec succès !"
    }

    Copy-Item -Path $repertoireTemp/$nomTravail -Recurse -Destination . -Force
    Copy-Item -Path $repertoireTemp/$listeCourriels -Destination . -Force

    # Supprimer le répertoire et les fichiers temporaires
    Remove-Item -Path $repertoireTemp -Force -Recurse
    Write-Host `n,'Répertoire temporaire supprimé avec succès !'
}

if ($estFichierCorrect.ToUpper() -eq 'Q') {
    Write-Host 'Quitter ... '
}