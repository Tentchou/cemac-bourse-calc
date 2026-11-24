-- Base de production sur Neon : la base cemac_bourse_calc et ses deux comptes.
--
-- À lancer UNE fois, avec le psql du PostgreSQL local, connecté au projet Neon avec le compte
-- propriétaire créé par Neon (neondb_owner) :
--   psql "host=<hôte Neon SANS -pooler> dbname=neondb user=neondb_owner sslmode=require" -f scripts/neon_comptes.sql
--
-- Les mots de passe sont demandés par \password : saisie masquée, et psql n'envoie au serveur
-- que l'empreinte SCRAM. Rien n'est écrit dans ce fichier, dans l'historique du terminal, dans
-- l'historique de l'éditeur SQL de Neon ni dans git. Choisissez des mots de passe longs
-- (20 caractères ou plus, au hasard) : ils ne servent qu'à l'API et à la lecture, jamais à la main.
--
-- Les comptes sont créés en SQL, pas dans la console de Neon : un compte créé dans la console
-- reçoit le rôle neon_superuser, beaucoup trop large pour l'API.
--
--   cemac_app      : le compte de l'API. Propriétaire de la base, il crée les tables par le
--                    script de migration. Ce n'est PAS un superutilisateur.
--   cemac_lecture  : lecture seule, pour consulter les tables. Il ne peut rien modifier.

\set ON_ERROR_STOP on

CREATE ROLE cemac_app LOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE NOINHERIT;
CREATE ROLE cemac_lecture LOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE NOINHERIT;

\echo 'Mot de passe du compte cemac_app (deux fois) :'
\password cemac_app
\echo 'Mot de passe du compte cemac_lecture (deux fois) :'
\password cemac_lecture

CREATE DATABASE cemac_bourse_calc OWNER cemac_app ENCODING 'UTF8' TEMPLATE template0;

-- Seuls nos deux comptes (et le propriétaire du projet) peuvent se connecter à la base.
REVOKE ALL ON DATABASE cemac_bourse_calc FROM PUBLIC;
GRANT CONNECT ON DATABASE cemac_bourse_calc TO cemac_app, cemac_lecture;

\connect cemac_bourse_calc

-- Les réglages suivants sont faits au nom de cemac_app, propriétaire de la base et de ses tables.
SET ROLE cemac_app;

-- Lecture seule : droit de voir le schéma, et SELECT sur toutes les tables que cemac_app
-- créera ensuite (le script de migration compris).
GRANT USAGE ON SCHEMA public TO cemac_lecture;
ALTER DEFAULT PRIVILEGES FOR ROLE cemac_app IN SCHEMA public GRANT SELECT ON TABLES TO cemac_lecture;

RESET ROLE;

\echo 'Base cemac_bourse_calc créée sur Neon. Étape suivante : le script de migration, avec le compte cemac_app.'
