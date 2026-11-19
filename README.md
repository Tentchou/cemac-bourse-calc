# CEMAC Bourse Calc

Calculateur éducatif pour les titres cotés à la BVMAC (Afrique centrale, CEMAC) : coupon couru,
prix plein coupon, échéancier des coupons et des remboursements, rendement. Une fiche par titre,
avec l'explication de chaque calcul.

> Outil éducatif, pas un conseil en investissement.

## Périmètre

- Les six pays de la CEMAC : Cameroun, Centrafrique, Congo, Gabon, Guinée équatoriale, Tchad.
- Les obligations et actions cotées à la BVMAC. Montants en francs CFA (XAF).
- Les caractéristiques des titres viennent des prospectus et des avis publiés (COSUMAF, BVMAC) ;
  elles sont saisies à la main dans l'administration.

## Architecture

```
front/                         React (JSX) + Vite
src/CemacBourseCalc.Api        ASP.NET Core : contrôleurs fins, validation, limitation des requêtes
src/CemacBourseCalc.Application  CQRS avec MediatR : Queries (lecture) et Commands (admin)
src/CemacBourseCalc.Domain     Calcul financier en C# pur, en decimal, sans aucune dépendance
src/CemacBourseCalc.Infrastructure  PostgreSQL : EF Core pour écrire, Dapper pour lire (Npgsql)
tests/CemacBourseCalc.Domain.Tests  xUnit, cas vérifiés à la main
```

- Les conventions de décompte sont interchangeables : `IConventionDecompte`, une classe par
  convention (Exact/365, Exact/360, 30E/360). La convention est portée par chaque titre.
- Les dates sont des `DateOnly` (colonnes `date`), les montants des `decimal` (colonnes `numeric`).

## Démarrer

Prérequis : .NET SDK 10, Node 22, PostgreSQL 16 ou plus.

1. Créer la base et ses deux comptes (les mots de passe sont demandés au clavier) :

   ```bash
   psql -U postgres -f scripts/creer_base.sql
   ```

   - `cemac_app` : le compte de l'API, propriétaire de la base, sans droits de superutilisateur.
   - `cemac_lecture` : lecture seule, pour consulter les tables (DbVisualizer…).

   Puis enregistrer la chaîne de connexion de l'API, jamais dans un fichier versionné :

   ```bash
   dotnet user-secrets set "ConnectionStrings:CemacBourseCalc" "Host=localhost;Port=5432;Database=cemac_bourse_calc;Username=cemac_app;Password=…" --project src/CemacBourseCalc.Api
   ```

2. Appliquer les migrations :

   ```bash
   dotnet tool restore
   dotnet ef database update -p src/CemacBourseCalc.Infrastructure -s src/CemacBourseCalc.Api
   ```

   Les migrations passent par le compte `cemac_app` (celui des user-secrets) : les tables lui
   appartiennent, et `cemac_lecture` reçoit automatiquement le droit de les lire.

3. Lancer l'API puis le front :

   ```bash
   dotnet run --project src/CemacBourseCalc.Api --launch-profile http
   ```

   ```bash
   cd front && npm install && npm run dev
   ```

4. Les tests :

   ```bash
   dotnet test
   ```

## Feuille de route

- [x] Squelette, conventions de décompte, ISIN CEMAC, titres, liste publique
- [x] Schéma des 13 tables (identifiants GUID)
- [x] Coupon couru et prix plein coupon (Domain + API `GET api/obligations/{id}/coupon-couru`)
- [ ] Écran du calculateur avec l'explication pas à pas
- [ ] Échéancier : in fine, amortissement constant, différé, report au jour ouvré
- [ ] Rendement actuariel, rendement courant, rendement du dividende
- [ ] Administration (authentification, saisie des titres et des cours)
- [ ] Fiches titres et graphique de l'échéancier
