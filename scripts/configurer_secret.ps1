# Enregistre la chaine de connexion de l'API dans les user-secrets (hors du depot git).
#
#   powershell -ExecutionPolicy Bypass -File scripts\configurer_secret.ps1
#
# Le mot de passe de cemac_app est saisi masque. Il n'apparait ni a l'ecran,
# ni dans l'historique PowerShell, ni dans un fichier du depot.
# (Fichier en ASCII : Windows PowerShell 5.1 lit mal l'UTF-8 sans BOM.)

$ErrorActionPreference = 'Stop'
$api = Join-Path $PSScriptRoot '..\src\CemacBourseCalc.Api'

$securise = Read-Host -AsSecureString 'Mot de passe du compte cemac_app'
$bstr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securise)
try {
    $mdp = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($bstr)
    # Npgsql : valeur entre apostrophes, apostrophes internes doublees (si le mot de passe contient ; ou ').
    $mdpEchappe = "'" + $mdp.Replace("'", "''") + "'"
    $chaine = "Host=localhost;Port=5433;Database=cemac_bourse_calc;Username=cemac_app;Password=$mdpEchappe"
    dotnet user-secrets set 'ConnectionStrings:CemacBourseCalc' $chaine --project $api | Out-Null
    if ($LASTEXITCODE -ne 0) { throw 'dotnet user-secrets a echoue.' }
    Write-Host 'Chaine de connexion enregistree dans les user-secrets.'
}
finally {
    [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($bstr)
    Remove-Variable mdp, mdpEchappe, chaine -ErrorAction SilentlyContinue
}
