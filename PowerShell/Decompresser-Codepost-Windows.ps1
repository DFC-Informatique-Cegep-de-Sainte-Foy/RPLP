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
    
    Write-Host 'Liste des fichiers trouv�s :',`n -ForegroundColor Yellow

    $compteur = 1;

    foreach ($fichier in $fichiers) {
        Write-Host $compteur, '-', (Get-Item $fichier).Basename
        $compteur += 1
    }

}

Function Get-FichierChoisi {
    
    Do {

        Write-Host `n,'>> Tapez le num�ro du fichier souhait� : ' -NoNewline -ForegroundColor Yellow
        
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

# Afficher la liste des fichiers trouv�s dans le r�pertoire
Get-ListeFichiers

# Demander � l'utilisateur de choisir un fichier
$nomFichierChoisi, $fichierChoisi = Get-FichierChoisi

# Afficher le nom du fichier choisi et demander une confirmation � l'utilisateur
$estFichierCorrect = Get-ConfirmationFichierChoisi($nomFichierChoisi)

# Ex�cuter action choisi
While ($estFichierCorrect.ToUpper() -eq 'N') {

    Write-Host `n
    Get-ListeFichiers
    $nomFichierChoisi, $fichierChoisi = Get-FichierChoisi
    $estFichierCorrect = Get-ConfirmationFichierChoisi($nomFichierChoisi)
    
}

if ($estFichierCorrect.ToUpper() -eq 'O') {
    
    # Demander � l'utilisateur d'entrer le nom du travail qui va appara�tre sur codePost
    Write-Host `n,">> Entrez le nom souhait� pour le travail : " -NoNewline -ForegroundColor Yellow
    $nomTravail = Read-Host  
    
    # D�finir le chemin du r�pertoire temporaire selon le OS de l'utilisateur
    $repertoireTemp = "c:\Temp"

    # Cr�er le r�pertoire temporaire s'il n'existe pas encore  
    if (-not (Test-Path -Path $repertoireTemp)) {
    
        try {
            New-Item -Path $repertoireTemp -ItemType Directory -ErrorAction Stop | Out-Null #-Force
        }
        catch {
            Write-Error -Message "Erreur au moment de cr�er le r�pertoire '$repertoireTemp'. Erreur : $_" -ErrorAction Stop
        }
        
        Write-Host `n,"R�pertoire '$repertoireTemp' cr�� avec succ�s !"
    }
    
    # Copier le fichier dans le r�pertoire temporaire
    Copy-Item -Path $fichierChoisi -Destination $repertoireTemp

    # D�finir le chemin du fichier dans le r�pertoire temporaire
    $pathTempFichier = "$repertoireTemp\$nomFichierChoisi.zip"

    # Decompresser le fichier dans le r�pertoire temporaire
    #Expand-Archive -Path $pathTempFichier -DestinationPath $repertoireTemp -Force ##### Erreur si le path est trop long sur Windows
    Expand-7Zip -ArchiveFileName $pathTempFichier -TargetPath $repertoireTemp
    
    # Faire le nettoyage
    $include = @("*.suo", "*.user", "*.userosscache", "*.sln.docstates", ".vs", "bin", "obj", "build", "*.class", ".settings", ".classpath", ".project")
    $exclude = @()

    $items = Get-ChildItem "$repertoireTemp\$nomFichierChoisi" -Recurse -Force -Include $include -Exclude $exclude

    foreach ($item in $items) {
        Remove-Item $item.FullName -Force -Recurse -ErrorAction SilentlyContinue
        Write-Host " Supprimer" $item.FullName
    }

    # Renommer le r�pertoire racine dans le r�pertoire temporaire    
    Write-Host `n,'Renommer le r�pertoire racine...'
    Rename-Item -Path $repertoireTemp\$nomFichierChoisi -NewName $nomTravail

    # Renommer les sous-r�pertoires dans le r�pertoire temporaire
    $sousRepertoires = Get-ChildItem "$repertoireTemp\$nomTravail" 

    foreach ($repertoire in $sousRepertoires ) {
                
        if ($repertoire.name -match '[A-Za-z]*_(?<numeroMatricule>(\d{7,}))_[A-Za-z]*') {
    
            try {

                $nouveauxNomRepertoire = $Matches.numeroMatricule + '@csfoy.ca' 
	            
                $nomRepertoireCourant = $repertoire.BaseName
                Write-Host $nomRepertoireCourant
                Rename-Item -Path $repertoireTemp\$nomTravail\$nomRepertoireCourant -NewName $nouveauxNomRepertoire
            }
            catch {

                Write-Error -Message "Erreur au moment de renommer le r�pertoire '$repertoire'. Erreur : $_" -ErrorAction Stop
                pause
            }
        }
    }

    # Cr�er le fichier .txt avec la liste de courriels des �tudiants
    $sousRepertoires = Get-ChildItem "$repertoireTemp\$nomTravail" 

    $listeCourriels = 'ListeCourriels_' + $nomTravail + ".txt"

    New-Item -Path $repertoireTemp -Name $listeCourriels -ItemType File -Force

    foreach ($repertoire in $sousRepertoires ) {
	
        $repertoire.name | Add-Content -Path $repertoireTemp\$listeCourriels
    }
    
    # Copier le r�pertoire du travail et la liste de courriels dans le r�pertoire d'origine
    if (Test-Path -Path .\$nomTravail) {
    
        try {
            Remove-Item �Path .\$nomTravail -Force -Recurse
        }
        catch {
            Write-Error -Message "Erreur au moment de supprimer le r�pertoire '.\$nomTravail'. Erreur : $_" -ErrorAction Stop
        }
        
        Write-Host `n,"R�pertoire '.\$nomTravail' supprim� avec succ�s !"
    }
    
    Copy-Item -Path $repertoireTemp\$nomTravail -Recurse -Destination . -Force
    Copy-Item -Path $repertoireTemp\$listeCourriels -Destination . -Force

    # Supprimer le r�pertoire et les fichiers temporaires
    Remove-Item �Path $repertoireTemp -Force -Recurse
    Write-Host `n,'R�pertoire temporaire supprim� avec succ�s !'
}

if ($estFichierCorrect.ToUpper() -eq 'Q') {
    Write-Host 'Quitter ... '
}