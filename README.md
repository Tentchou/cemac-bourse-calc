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
- Le schéma des 13 tables se lit sans base de données : [docs/schema.md](docs/schema.md) (diagramme)
  et [docs/schema.sql](docs/schema.sql) (le SQL exact des migrations).

## Démarrer

Prérequis : .NET SDK 10, Node 22, PostgreSQL 16 ou plus (démarré : `start-postgres.bat` pour le serveur portable).

1. Créer la base et ses deux comptes. Le serveur local de développement est un PostgreSQL 17
   portable sur le port **5433**, partagé avec d'autres projets (une base et des comptes par projet).

   - avec `psql` (les mots de passe sont demandés au clavier) :

     ```bash
     psql -p 5433 -U postgres -f scripts/creer_base.sql
     ```

   - ou depuis DbVisualizer : suivre les étapes de `scripts/creer_base_dbvisualizer.sql`.

   - `cemac_app` : le compte de l'API, propriétaire de la base, sans droits de superutilisateur.
   - `cemac_lecture` : lecture seule, pour consulter les tables (DbVisualizer…).

   Puis enregistrer la chaîne de connexion de l'API, jamais dans un fichier versionné :

   ```bash
   dotnet user-secrets set "ConnectionStrings:CemacBourseCalc" "Host=localhost;Port=5433;Database=cemac_bourse_calc;Username=cemac_app;Password=…" --project src/CemacBourseCalc.Api
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

   Sans API ni base, pour voir l'interface publique avec des données d'exemple :

   ```bash
   cd front && npm run dev:exemple
   ```

4. Créer le premier compte de l'administration (le mot de passe est demandé au clavier, masqué) :

   ```bash
   dotnet run --project src/CemacBourseCalc.Api -- creer-admin
   ```

   Aucune route publique ne crée de compte. Ensuite, un Admin connecté peut en créer d'autres.

5. Les tests :

   ```bash
   dotnet test
   ```

## Administration

Deux rôles : **Admin** (tout, dont la création des comptes) et **Editeur** (saisie des données).

- Connexion : `POST api/admin/session` ; déconnexion : `DELETE` ; compte connecté : `GET`.
- Session de 8 heures dans un cookie `HttpOnly`, `Secure`, `SameSite=Strict`. Seule l'empreinte
  SHA-256 du jeton est en base : la déconnexion révoque la session.
- Mots de passe : 12 caractères au moins, empreinte PBKDF2 (ASP.NET Core Identity), jamais en clair.
- 5 essais de connexion par minute et par IP ; même message que l'e-mail soit inconnu ou le mot de passe faux.
- Toute écriture sur `api/admin` exige l'en-tête `X-CBC-Admin: 1` (protection CSRF).
- Chaque création est inscrite au journal (`journal_admin`) dans la même transaction.

| Saisie manuelle | Saisie en masse (tout ou rien, 500 lignes au plus) |
|---|---|
| `POST api/admin/emetteurs` | `POST api/admin/emetteurs/masse` |
| `POST api/admin/obligations` | `POST api/admin/obligations/masse` |
| `POST api/admin/actions` | `POST api/admin/actions/masse` |
| `POST api/admin/cours` | `POST api/admin/cours/masse` |
| `POST api/admin/dividendes` | `POST api/admin/dividendes/masse` |
| `POST api/admin/jours-non-ouvres` | `POST api/admin/jours-non-ouvres/masse` |
| `POST api/admin/administrateurs` (Admin) | — |

**Import de fichier (.xlsx ou .csv)** : le fichier est lu dans le navigateur (rien n'est envoyé
avant la vérification), avec des colonnes reconnues dans n'importe quel ordre, les nombres
français (`98,50`, `10 000`), les dates `jj/mm/aaaa`, les CSV en `;` ou `,`, UTF-8 ou Windows-1252.
L'ISIN et le nom (ou sigle) de l'émetteur sont traduits en identifiants. Un modèle à télécharger
existe pour chaque type (Excel : une feuille « Saisie » et une feuille « Aide »). S'il reste une
seule ligne à corriger, rien n'est envoyé : on corrige le fichier et on le recharge. Tests :
`cd front && npm test`.

La saisie en masse n'a aucune règle propre : chaque ligne passe par la commande manuelle.
S'il y a une seule erreur, rien n'est enregistré et la réponse (422) liste toutes les lignes à
corriger, avec leur numéro. Les doublons à l'intérieur d'un envoi sont signalés aussi.

## Feuille de route

- [x] Squelette, conventions de décompte, ISIN CEMAC, titres, liste publique
- [x] Schéma des 13 tables (identifiants GUID)
- [x] Coupon couru et prix plein coupon (Domain + API `GET api/obligations/{id}/coupon-couru`)
- [x] Site public en React + Tailwind CSS 4 : accueil, fiche obligation (calculateur Simple/Expert,
      échéancier et graphique), fiche action (dividende), méthodes ; clair/sombre, français/anglais,
      dès 360 px. Polices Plus Jakarta Sans (titres) et Inter (texte), icônes Lucide.
- [x] API : fiche d'un titre `GET api/titres/{id}` (caractéristiques, dernier cours, dividendes ;
      404 si inconnu) et liste `GET api/titres` enrichie (taux, échéance, dernier cours)
- [x] Échéancier : in fine, amortissement constant, différé, périodes irrégulières, report au jour ouvré
      (`GET api/obligations/{id}/echeancier`)
- [x] Rendement actuariel et rendement courant (`GET api/obligations/{id}/rendement`),
      rendement du dividende (`GET api/actions/{id}/rendement-dividende`)
- [x] Administration côté API : authentification, rôles, saisie manuelle et en masse, journal
- [x] Écrans de l'administration : connexion et import de fichier (`/admin`)
- [ ] Formulaires de saisie manuelle à l'écran
- [ ] Essai de bout en bout du site public avec la vraie API et la base
