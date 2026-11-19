-- Création de la base CEMAC Bourse Calc et de ses deux comptes.
--
-- À lancer UNE fois, avec le compte d'installation « postgres » :
--   psql -U postgres -f scripts/creer_base.sql
--
-- Les mots de passe sont demandés au clavier : ils ne sont écrits ni dans ce
-- fichier, ni dans l'historique du terminal, ni dans le dépôt git.
--
--   cemac_app      : le compte de l'API. Propriétaire de la base, il crée les
--                    tables par les migrations. Ce n'est PAS un superutilisateur.
--   cemac_lecture  : lecture seule, pour consulter les tables (DbVisualizer).
--                    Il ne peut rien modifier ni supprimer.

\prompt 'Mot de passe du compte cemac_app : ' mdp_app
\prompt 'Mot de passe du compte cemac_lecture : ' mdp_lecture

CREATE ROLE cemac_app LOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE PASSWORD :'mdp_app';
CREATE ROLE cemac_lecture LOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE PASSWORD :'mdp_lecture';

CREATE DATABASE cemac_bourse_calc OWNER cemac_app ENCODING 'UTF8' TEMPLATE template0;

REVOKE ALL ON DATABASE cemac_bourse_calc FROM PUBLIC;
GRANT CONNECT ON DATABASE cemac_bourse_calc TO cemac_app, cemac_lecture;

\connect cemac_bourse_calc

-- Lecture seule : droit de voir le schéma, et SELECT sur toutes les tables
-- que cemac_app créera ensuite (les migrations comprises).
GRANT USAGE ON SCHEMA public TO cemac_lecture;
ALTER DEFAULT PRIVILEGES FOR ROLE cemac_app IN SCHEMA public GRANT SELECT ON TABLES TO cemac_lecture;
