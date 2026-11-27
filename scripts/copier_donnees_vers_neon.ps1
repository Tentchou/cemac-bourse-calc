# Copie les donnees du marche de la base LOCALE vers la base de PRODUCTION (Neon), en une transaction.
#
#   powershell -ExecutionPolicy Bypass -File scripts\copier_donnees_vers_neon.ps1 -Hote <hote Neon sans -pooler>
#
# Copie : emetteurs, titres (obligations, actions), cours, dividendes, documents sources, jours non ouvres.
# Ne copie PAS : les comptes d'administration, leurs sessions, le journal (propres a chaque base), ni les
# pays et parametres (deja crees par le script de migration).
# Refuse si la base Neon contient deja des titres : rien n'est jamais ecrase. Tout ou rien.
# Les deux mots de passe (cemac_app local, cemac_app Neon) sont saisis masques et ne vivent que dans ce processus.
# (Fichier en ASCII : Windows PowerShell 5.1 lit mal l'UTF-8 sans BOM.)

param(
    [Parameter(Mandatory = $true)][string]$Hote,
    [string]$Outils = 'C:\Users\El bresilio\Tools\postgres17\pgsql\bin',
    [int]$PortLocal = 5433
)

$ErrorActionPreference = 'Stop'
$tables = 'emetteur', 'titre', 'obligation', 'action', 'cours', 'dividende', 'document_source', 'jour_non_ouvre'
$psql = Join-Path $Outils 'psql.exe'
$pgDump = Join-Path $Outils 'pg_dump.exe'
$neon = "host=$Hote dbname=cemac_bourse_calc user=cemac_app sslmode=require channel_binding=require"
$local = "host=localhost port=$PortLocal dbname=cemac_bourse_calc user=cemac_app"
$fichier = Join-Path $env:TEMP ('cemac_donnees_' + [guid]::NewGuid().ToString('N') + '.sql')

function Lire-MotDePasse([string]$invite) {
    $s = Read-Host -AsSecureString $invite
    $b = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($s)
    try { return [Runtime.InteropServices.Marshal]::PtrToStringBSTR($b) }
    finally { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($b) }
}

$mdpLocal = Lire-MotDePasse 'Mot de passe de cemac_app LOCAL (port 5433)'
$mdpNeon = Lire-MotDePasse 'Mot de passe de cemac_app sur NEON'
try {
    # 1. La base Neon doit etre vide de titres.
    $env:PGPASSWORD = $mdpNeon
    $deja = & $psql $neon -tA -c 'SELECT count(*) FROM titre;'
    if ($LASTEXITCODE -ne 0) { throw 'Connexion a Neon impossible.' }
    if ([int]$deja -gt 0) { throw "La base Neon contient deja $deja titre(s) : copie annulee, rien n'a ete modifie." }

    # 2. Export des donnees locales (donnees seules, dans l'ordre des dependances).
    $env:PGPASSWORD = $mdpLocal
    $arguments = @($local, '--data-only', '--no-owner', '--no-privileges', '-f', $fichier)
    foreach ($t in $tables) { $arguments += "--table=public.$t" }
    & $pgDump @arguments
    if ($LASTEXITCODE -ne 0) { throw 'Export local impossible.' }

    # 3. Import sur Neon en une seule transaction : la moindre erreur annule tout.
    $env:PGPASSWORD = $mdpNeon
    & $psql $neon -q -1 -v ON_ERROR_STOP=1 -f $fichier
    if ($LASTEXITCODE -ne 0) { throw 'Import sur Neon refuse : rien n a ete enregistre.' }

    # 4. Bilan.
    $bilan = ($tables | ForEach-Object { "SELECT '$_' AS table_copiee, count(*) AS lignes FROM $_" }) -join ' UNION ALL '
    & $psql $neon -c $bilan
}
finally {
    Remove-Item Env:PGPASSWORD -ErrorAction SilentlyContinue
    Remove-Item $fichier -ErrorAction SilentlyContinue
    Remove-Variable mdpLocal, mdpNeon -ErrorAction SilentlyContinue
}
