-- Création de la base CEMAC Bourse Calc et de ses deux comptes.
--
-- À lancer UNE fois, avec un compte superutilisateur du serveur local (port 5433) :
--   psql -h localhost -p 5433 -U postgres -d postgres -f scripts/creer_base.sql
--
-- Les mots de passe sont demandés par \password : la saisie est masquée, et le
-- serveur ne reçoit que l'empreinte SCRAM. Ils ne sont écrits ni dans ce fichier,
-- ni dans l'historique du terminal, ni dans le journal du serveur, ni dans git.
--
--   cemac_app      : le compte de l'API. Propriétaire de la base, il crée les
--                    tables par les migrations. Ce n'est PAS un superutilisateur.
--   cemac_lecture  : lecture seule, pour consulter les tables (DbVisualizer).
--                    Il ne peut rien modifier ni supprimer.

\set ON_ERROR_STOP on

CREATE ROLE cemac_app LOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE NOINHERIT;
CREATE ROLE cemac_lecture LOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE NOINHERIT;

\echo 'Mot de passe du compte cemac_app (deux fois) :'
\password cemac_app
\echo 'Mot de passe du compte cemac_lecture (deux fois) :'
\password cemac_lecture

CREATE DATABASE cemac_bourse_calc OWNER cemac_app ENCODING 'UTF8' TEMPLATE template0;

-- Personne d'autre (kori_app, kori_owner…) ne peut se connecter à la nouvelle base.
REVOKE ALL ON DATABASE cemac_bourse_calc FROM PUBLIC;
GRANT CONNECT ON DATABASE cemac_bourse_calc TO cemac_app, cemac_lecture;

\connect cemac_bourse_calc

-- Le schéma public appartient à cemac_app ; les autres comptes n'y créent rien.
ALTER SCHEMA public OWNER TO cemac_app;
REVOKE CREATE ON SCHEMA public FROM PUBLIC;

-- Lecture seule : droit de voir le schéma, et SELECT sur toutes les tables
-- que cemac_app créera ensuite (les migrations comprises).
GRANT USAGE ON SCHEMA public TO cemac_lecture;
ALTER DEFAULT PRIVILEGES FOR ROLE cemac_app IN SCHEMA public GRANT SELECT ON TABLES TO cemac_lecture;

\echo 'Base cemac_bourse_calc créée.'
