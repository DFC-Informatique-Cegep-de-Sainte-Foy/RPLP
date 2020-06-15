# ----------------------------------------------------------------------------------
#
# Variables de base 1
#
# ----------------------------------------------------------------------------------
$pathCourant = "."
$filtre = "*Remise*"
$nbfichiers = [System.IO.Directory]::GetFiles("$pathCourant", "$filtre").count
$fichiers = [System.IO.Directory]::GetFiles("$pathCourant", "$filtre")


# ----------------------------------------------------------------------------------
#
# Fonctions
#
# ----------------------------------------------------------------------------------
Function Get-ListeFichiers {
    
    Write-Host "Liste des fichiers trouvés :" -ForegroundColor Yellow

    $compteur = 1;

    foreach ($fichier in $fichiers) {
        Write-Host $compteur, "-", (Get-Item $fichier).Basename
        $compteur += 1
    }
}

Function Get-FichierChoisi {
    
    Do {
       
        Write-Host ">> Tapez le numéro du fichier souhaité : " -NoNewline -ForegroundColor Yellow

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

Function Get-Fichiers($extension, $path) {
   
    $include = @("*$extension")
    $exclude = @()

    $items = Get-ChildItem $path -Recurse -Force -Include $include -Exclude $exclude

    return $items
}

Function Get-TextExplicatif-FichierMDJ($pathTempFichierMessageImportant) {

    Write-Host $pathTempFichierMessageImportant

    if (-not (Test-Path -PATH $pathTempFichierMessageImportant -PathType Leaf)) {

        New-Item -PATH $repertoireTempRPLP -Name $nomFichierMessageImportant -ItemType File -Force

        Write-Host "Création de la liste des étudiants qui ont envoyé des fichier $extension ..."
    }

$textExplicatif = @"  
        
--------------------- FICHIERS .MDJ --------------------- 
        
Les étudiants suivants ont envoyé des fichiers .mdj et 
il n'y avait pas de fichiers .jpg ou .png dans le même 
répertoire où se trouvait le fichier .mdj
        
Ils doivent donc exporter leurs diagrammes au format image 
et envoyer à nouveau leurs travail, avant d'envoyer le 
travail de tous les étudiants à codePost
        
"@

    $textExplicatif | Add-Content -PATH $pathTempFichierMessageImportant
    
}

Function Get-TextExplicatif-FichierSVG($pathTempFichierMessageImportant) {
             
    if (-not (Test-Path -PATH $pathTempFichierMessageImportant -PathType Leaf)) {

        New-Item -PATH $repertoireTempRPLP -Name $nomFichierMessageImportant -ItemType File -Force

        Write-Host "Création de la liste des étudiants qui ont envoyé des fichier $extension ..."
    }
   
$textExplicatif = @"  

        
--------------------- FICHIERS .SVG --------------------- 
        
Les étudiants suivants ont envoyé des fichiers .svg et 
il n'y avait pas de fichiers .jpg ou .png dans le même 
répertoire où se trouvait le fichier .svg
        
S'il s'agit d'un diagramme, ils doivent donc exporter 
leurs diagrammes au format image et envoyer à nouveau 
leurs travail, avant d'envoyer le travail de tous les 
étudiants à codePost
       
"@
    
    $textExplicatif | Add-Content -PATH $pathTempFichierMessageImportant
}

Function Get-ListeEtudiants-MauvaisFichier-AucunFichierImage($pathTempFichierMessageImportant, $items) {
    
    $qteEtudiants = 0

    foreach ($item in $items) {
            
        $fichiers = Get-ChildItem $item.Directory -Force -File

        foreach ($fichier in $fichiers) {

            $fichierImageTrouve =  $false
                
            Write-Host $fichier

            $extension = [IO.Path]::GetExtension($fichier)
                
            if ($extension -eq ".jpg" -or $extension -eq ".png") {
                
                $fichierImageTrouve = $true
                break
            }
        }

        Write-Host $fichierImageTrouve

        if ($fichierImageTrouve -eq $false) {

            if ($item.Directory -match '(?<nom>[A-Za-z]*)_(?<numeroMatricule>(\d{7,}))_[A-Za-z]*') {

                    $etudiant = $Matches.numeroMatricule + " - " + $Matches.nom

                    Write-Host "Ajouter dans la liste ..."
                   
                    $etudiant | Add-Content -PATH $pathTempFichierMessageImportant

                    Write-Host $etudiant
                    
                    $qteEtudiants++
            }
        }
                
        #pause            
    }

    [Environment]::NewLine | Add-Content -PATH $pathTempFichierMessageImportant 
    "TOTAL : ", $qteEtudiants, " étudiants" | Add-Content -PATH $pathTempFichierMessageImportant -NoNewline
}

# ---------------------------------------------------------------------------------- 
#
# Actions
#
# ----------------------------------------------------------------------------------

if ($nbfichiers -gt 1) {
    
    # Afficher la liste des fichiers ZIP trouv�s dans le r�pertoire, qui contient le mot 'Remise' dans leur nom 
    Get-ListeFichiers

    # Demander � l'utilisateur de choisir l'un des fichiers
    $nomFichierChoisi, $fichierChoisi = Get-FichierChoisi

    # Afficher le nom du fichier choisi et demander une confirmation � l'utilisateur
    $estFichierCorrect = Get-ConfirmationFichierChoisi($nomFichierChoisi)
}
elseif ($nbfichiers -eq 1) {

    # Choisir automatiquement le seule fichier ZIP qui contient le mot 'Remise' dans son nom
    $fichierChoisi = Get-Item $fichiers
    $nomFichierChoisi = $fichierChoisi.Basename

    # Afficher le nom du fichier choisi et demander une confirmation � l'utilisateur
    $estFichierCorrect = Get-ConfirmationFichierChoisi($nomFichierChoisi)
}
else {
    
    Write-Host "Aucun fichier trouvé !" -ForegroundColor Red
    Pause
    [Environment]::Exit(1)
}


# Ex�cuter l'action choisie par l'utilisateur
While ($estFichierCorrect.ToUpper() -eq 'N') {

    Get-ListeFichiers
    $nomFichierChoisi, $fichierChoisi = Get-FichierChoisi
    $estFichierCorrect = Get-ConfirmationFichierChoisi($nomFichierChoisi)
    
}

if ($estFichierCorrect.ToUpper() -eq 'O') {
    
    # Demander � l'utilisateur d'entrer le nom du travail qui va appara�tre sur codePost
    Write-Host ">> Entrez le nom souhaité pour le travail : " -NoNewline -ForegroundColor Yellow
    $nomTravail = Read-Host  
    

    # ----------------------------------------------------------------------------------
    #
    # Variables de base 2
    #
    # ----------------------------------------------------------------------------------
    $repertoireTempSO = [system.io.path]::GetTempPath()
    $repertoireTempRPLP = Join-Path -PATH $repertoireTempSO -ChildPath "TempRPLP"

    $nomFichierMessageImportant = $nomTravail + "_Message_Important" + ".txt"
    $pathTempFichierMessageImportant = Join-Path -PATH $repertoireTempRPLP -ChildPath $nomFichierMessageImportant

   
    # V�rifier si le r�pertoire temporaire existe d�j�
    if (Test-Path -PATH $repertoireTempRPLP) {
    
        try {
            Write-Host "Répertoire temporaire existe déjà !"
            
            # Supprimer le r�pertoire et les fichiers temporaires
            Remove-Item -PATH $repertoireTempRPLP -Force -Recurse

            Write-Host "Répertoire temporaire supprimé avec succès !"
        }
        catch {
            Write-Error -Message "Erreur au moment de supprimer le répertoire '$repertoireTempRPLP'. Erreur : $_" -ErrorAction Stop
        }
    }

    # Cr�er le r�pertoire temporaire
    Write-Host "Création du répertoire temporaire ..."
    
    New-Item -PATH $repertoireTempRPLP -ItemType Directory -ErrorAction Stop | Out-Null 
    
    Write-Host "Répertoire '$repertoireTempRPLP' créé avec succès !"    
        
    # Cr�er le r�pertoire du travail dans le r�pertoire temporaire
    $pathRepertoireTempTravail = Join-Path -PATH $repertoireTempRPLP -ChildPath $nomTravail
    
    New-Item -PATH $pathRepertoireTempTravail -ItemType Directory -ErrorAction Stop | Out-Null 
        
    Write-Host "Répertoire '$nomTravail' créé avec succès dans le répertoire temporaire !"

    # Copier le fichier ZIP dans le r�pertoire temporaire
    Copy-Item -PATH $fichierChoisi -Destination $repertoireTempRPLP
    Write-Host "Fichier '$nomFichierChoisi.zip' copié avec succ�s dans le répertoire temporaire !"

    # D�finir le chemin du fichier dans le r�pertoire temporaire
    $pathTempFichier = Join-Path -PATH $repertoireTempRPLP -ChildPath "$nomFichierChoisi.zip"

    # Decompresser le fichier ZIP dans le r�pertoire temporaire
    Expand-Archive -PATH $pathTempFichier -DestinationPath $pathRepertoireTempTravail -Force 
    Write-Host "Decompresser le fichier ZIP ..."
    #pause

    # V�rifier les �tudiants qui ont soumis des fichiers .mdj
    $extension = ".mdj"
        
    Write-Host "Vérifier la présence de fichiers .mdj ..."

    $items = Get-Fichiers($extension)($pathRepertoireTempTravail)

    if ($items.length -gt 0) {

        Get-TextExplicatif-FichierMDJ($pathTempFichierMessageImportant)
        Write-Host $nomFichierMessageImportant

        Get-ListeEtudiants-MauvaisFichier-AucunFichierImage($pathTempFichierMessageImportant)($items)
    }
    
    # V�rifier les �tudiants qui ont soumis des fichiers .svg
    $extension = ".svg"
    
    Write-Host "Vérifier la présence de fichiers .svg ..."

    $items = Get-Fichiers($extension)($pathRepertoireTempTravail)

    if ($items.length -gt 0) {

        Get-TextExplicatif-FichierSVG($pathTempFichierMessageImportant)
        Write-Host $nomFichierMessageImportant

        Get-ListeEtudiants-MauvaisFichier-AucunFichierImage($pathTempFichierMessageImportant)($items)
    }
      
    #pause
    
    # Faire le nettoyage
    $include = @("*.suo", "*.user", "*.userosscache", "*.sln.docstates", ".vs", "bin", "obj", "build", "*.class", ".settings", ".classpath", ".project", "*.mdj")
    $exclude = @()

    $items = Get-ChildItem $pathRepertoireTempTravail -Recurse -Force -Include $include -Exclude $exclude
  
    Write-Host "Nettoyage ..."
    
    foreach ($item in $items) {
                
        Remove-Item $item.FullName -Force -Recurse -ErrorAction SilentlyContinue
        Write-Host " Supprimer " $item.FullName
    }
        
    #pause
   
    # Renommer les sous-r�pertoires dans le r�pertoire temporaire
    $sousRepertoires = Get-ChildItem $pathRepertoireTempTravail 

    Write-Host "Renommer les sous-répertoires dans le répertoire temporaire ..."

    foreach ($repertoire in $sousRepertoires ) {
                
        if ($repertoire.name -match '[A-Za-z]*_(?<numeroMatricule>(\d{7,}))_[A-Za-z]*') {
    
            try {

                $nouveauxNomRepertoire = $Matches.numeroMatricule + '@csfoy.ca' 
	            
                $nomRepertoireCourant = $repertoire.BaseName
                Write-Host $nomRepertoireCourant
                $pathRepertoireCourant = Join-Path -PATH $pathRepertoireTempTravail -ChildPath $nomRepertoireCourant

                Rename-Item -PATH $pathRepertoireCourant -NewName $nouveauxNomRepertoire -Force
            }
            catch {

                Write-Error -Message "Erreur au moment de renommer le répertoire '$repertoire'. Erreur : $_" -ErrorAction Stop
            }
        }
    }

    # Cr�er le fichier .txt avec la liste de courriels des �tudiants
    $sousRepertoires = Get-ChildItem $pathRepertoireTempTravail

    $listeCourriels = $nomTravail + "_Liste_Courriels" + ".txt"
    $pathListeCourriels = Join-Path -PATH $repertoireTempRPLP -ChildPath $listeCourriels

    Write-Host "Création de la liste avec le courriel de tous les étudiants qui ont envoyé leur travail ..."

    New-Item -PATH $repertoireTempRPLP -Name $listeCourriels -ItemType File -Force

    foreach ($repertoire in $sousRepertoires ) {
	
        $repertoire.name | Add-Content -PATH $pathListeCourriels

        Write-Host $repertoire.Name
    }
    
    # Copier le r�pertoire du travail et les fichiers .txt dans le r�pertoire courant
    $pathCourantTravail = Join-Path -PATH $pathCourant -ChildPath $nomTravail
    $pathCourantEtoile = Join-Path -PATH $pathCourant -ChildPath "*"
    $pathTempRPLPEtoile = Join-Path -PATH $repertoireTempRPLP -ChildPath "*"

    if (Test-Path -PATH $pathCourantTravail) {
 
        Write-Host "Le répertoire $nomTravail existe déjà dans le répertoire courant !"

        try {
            Remove-Item -PATH $pathCourantTravail -Force -Recurse
            Remove-Item -PATH $pathCourantEtoile -Include $listeCourriels -Force

            $pathCourantFichierMessageImportant = Join-Path -PATH $pathCourant -ChildPath $nomFichierMessageImportant
            
            Write-Host $nomFichierMessageImportant
            Write-Host $pathCourantFichierMessageImportant

            if (Test-Path -PATH $pathCourantFichierMessageImportant) {
                
                Remove-Item -PATH $pathCourantEtoile -Include $nomFichierMessageImportant -Force
            }

        }
        catch {
            Write-Error -Message "Erreur au moment de supprimer le répertoire '$pathCourantTravail'. Erreur : $_" -ErrorAction Stop
        }
  
        Write-Host "Répertoire '$pathCourantTravail' supprimé avec succès !"
    }

    Write-Host "Copier le répertoire du travail et fichiers .txt dans le répertoire courant !"
    
    Copy-Item -PATH $pathRepertoireTempTravail -Recurse -Destination $pathCourant -Force
    Copy-Item -PATH $pathTempRPLPEtoile -Include "*$nomTravail*.txt" -Force
    
    #pause

    # Supprimer le r�pertoire et les fichiers temporaires
    Remove-Item -PATH $repertoireTempRPLP -Force -Recurse
    Write-Host "Répertoire temporaire supprimé avec succès !"
   
    #pause
}

if ($estFichierCorrect.ToUpper() -eq 'Q') {
    Write-Host "Quitter ..."
}