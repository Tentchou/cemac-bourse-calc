# Schéma de la base (13 tables)

Généré à partir de la migration `Initiale`. Le SQL exact est dans [schema.sql](schema.sql)
(régénérer : `dotnet ef migrations script -i -p src/CemacBourseCalc.Infrastructure -s src/CemacBourseCalc.Api -o docs/schema.sql`).

Le diagramme s'affiche sur GitHub, ou dans VS Code avec l'extension « Markdown Preview Mermaid Support ».

```mermaid
erDiagram
    pays ||--o{ emetteur : "pays_code"
    emetteur ||--o{ titre : "emetteur_id"
    titre ||--o| obligation : "titre_id (TPT)"
    titre ||--o| action : "titre_id (TPT)"
    titre ||--o{ cours : "titre_id"
    titre ||--o{ dividende : "titre_id"
    titre ||--o{ document_source : "titre_id"
    administrateur ||--o{ jeton_session : "administrateur_id"
    administrateur ||--o{ journal_admin : "administrateur_id"
    administrateur |o--o{ parametre : "modifie_par_id"

    pays {
        varchar2 code PK "CM CF CG GA GQ TD"
        varchar libelle
    }
    emetteur {
        uuid id PK
        varchar nom UK
        varchar sigle "facultatif"
        varchar type "Etat Entreprise Supranational"
        varchar2 pays_code FK
        varchar site_web "facultatif"
    }
    titre {
        uuid id PK
        varchar12 isin UK
        varchar nom
        uuid emetteur_id FK
        varchar type_titre "Obligation Action"
        numeric nominal "> 0"
        date date_admission "facultatif"
        varchar statut "Actif Suspendu Echu Radie"
    }
    obligation {
        uuid titre_id PK, FK
        numeric taux_facial "0 a 100"
        varchar periodicite
        date date_jouissance
        date date_premier_coupon "facultatif"
        date date_echeance
        varchar mode_remboursement "InFine AmortissementConstant"
        int differe_echeances
        varchar code_convention "EXACT_365 EXACT_360 30E_360"
        varchar regime_fiscal "Net Brut"
    }
    action {
        uuid titre_id PK, FK
        bigint nombre_titres
    }
    cours {
        uuid id PK
        uuid titre_id FK "unique avec date_seance"
        date date_seance
        numeric cours_reference
        numeric cours_cloture
        bigint volume
        numeric coupon_couru_publie
    }
    dividende {
        uuid id PK
        uuid titre_id FK
        int exercice
        numeric montant_brut
        numeric montant_net
        date date_detachement
        date date_paiement
    }
    document_source {
        uuid id PK
        uuid titre_id FK
        varchar type "Prospectus Avis Bulletin"
        varchar libelle
        varchar url
        date date_publication
    }
    jour_non_ouvre {
        date date PK
        varchar libelle
    }
    parametre {
        varchar cle PK
        varchar valeur
        varchar libelle
        timestamptz modifie_le
        uuid modifie_par_id FK
    }
    administrateur {
        uuid id PK
        varchar email UK "en minuscules"
        varchar nom
        varchar mot_de_passe_hash
        varchar role "Admin Editeur"
        boolean actif
        timestamptz cree_le
        timestamptz derniere_connexion
    }
    jeton_session {
        uuid id PK
        uuid administrateur_id FK
        varchar jeton_hash UK
        timestamptz cree_le
        timestamptz expire_le
        timestamptz revoque_le
    }
    journal_admin {
        uuid id PK
        uuid administrateur_id FK
        timestamptz date
        varchar action
        varchar entite
        varchar entite_id
        jsonb avant
        jsonb apres
    }
```

## Données initiales

- `pays` : les 6 pays de la CEMAC.
- `parametre` : `DELAI_REGLEMENT_JOURS_OUVRES` = 3 (T+3), `REGLE_REPORT_PAIEMENT` = `JOUR_OUVRE_SUIVANT`.
