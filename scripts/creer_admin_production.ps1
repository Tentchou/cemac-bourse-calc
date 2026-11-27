# Cree un compte de l'administration sur la base de PRODUCTION (Neon), depuis ce PC.
#
#   powershell -ExecutionPolicy Bypass -File scripts\creer_admin_production.ps1 -Hote <hote Neon sans -pooler>
#
# Le mot de passe de cemac_app (Neon) puis celui du nouveau compte sont saisis masques. La chaine de
# connexion ne vit que dans ce processus : ni a l'ecran, ni dans l'historique, ni dans un fichier.
# Utilise la derniere compilation locale (--no-build) : l'API locale peut rester lancee.
# (Fichier en ASCII : Windows PowerShell 5.1 lit mal l'UTF-8 sans BOM.)

param([Parameter(Mandatory = $true)][string]$Hote)

$ErrorActionPreference = 'Stop'
$api = Join-Path $PSScriptRoot '..\src\CemacBourseCalc.Api'

$securise = Read-Host -AsSecureString 'Mot de passe du compte cemac_app sur Neon'
$bstr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securise)
try {
    $mdp = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($bstr)
    $mdpEchappe = "'" + $mdp.Replace("'", "''") + "'"
    $env:ConnectionStrings__CemacBourseCalc = "Host=$Hote;Port=5432;Database=cemac_bourse_calc;Username=cemac_app;Password=$mdpEchappe;SSL Mode=Require;Channel Binding=Require"
    $env:ASPNETCORE_ENVIRONMENT = 'Production'
    dotnet run --project $api --no-build --no-launch-profile -- creer-admin
    if ($LASTEXITCODE -ne 0) { throw 'La creation du compte a echoue (voir le message ci-dessus).' }
}
finally {
    [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($bstr)
    Remove-Item Env:ConnectionStrings__CemacBourseCalc, Env:ASPNETCORE_ENVIRONMENT -ErrorAction SilentlyContinue
    Remove-Variable mdp, mdpEchappe -ErrorAction SilentlyContinue
}
