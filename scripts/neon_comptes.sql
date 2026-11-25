-- Base de production sur Neon : la base cemac_bourse_calc et ses deux comptes.
--
-- À lancer UNE fois, avec le psql du PostgreSQL local, connecté au projet Neon avec le compte
-- propriétaire créé par Neon (neondb_owner) :
--   psql "host=<hôte Neon SANS -pooler> dbname=neondb user=neondb_owner sslmode=require" -f scripts/neon_comptes.sql
--
-- Aucun mot de passe ici. Neon refuse les empreintes SCRAM envoyées par \password (« Neon only
-- supports being given plaintext passwords ») : les mots de passe sont générés ensuite par Neon,
-- dans la console (Connect → rôle → Reset password), affichés une seule fois, longs et aléatoires.
-- Ils ne passent ni par ce fichier, ni par l'historique du terminal ou de l'éditeur SQL, ni par git.
--
-- Les comptes sont créés en SQL, pas dans la console de Neon : un compte créé dans la console
-- reçoit le rôle neon_superuser, beaucoup trop large pour l'API. (Réinitialiser son mot de passe
-- dans la console ne change pas ses droits : à vérifier après coup avec pg_auth_members.)
--
--   cemac_app      : le compte de l'API. Propriétaire de la base, il crée les tables par le
--                    script de migration. Ce n'est PAS un superutilisateur.
--   cemac_lecture  : lecture seule, pour consulter les tables. Il ne peut rien modifier.

\set ON_ERROR_STOP on

CREATE ROLE cemac_app LOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE NOINHERIT;
CREATE ROLE cemac_lecture LOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE NOINHERIT;

-- La console de Neon refuse de réinitialiser le mot de passe d'un rôle qui n'en a pas
-- (« cannot update password for role without password ») : un premier mot de passe aléatoire,
-- tiré par le serveur et jamais affiché, sert seulement à débloquer « Reset password ».
DO $x$
BEGIN
    EXECUTE format('ALTER ROLE cemac_app PASSWORD %L', gen_random_uuid()::text || gen_random_uuid()::text);
    EXECUTE format('ALTER ROLE cemac_lecture PASSWORD %L', gen_random_uuid()::text || gen_random_uuid()::text);
END
$x$;

-- PostgreSQL 16+ : le créateur d'un rôle peut l'administrer, mais pas agir en son nom. Pour donner
-- la base à cemac_app (CREATE DATABASE … OWNER) puis régler ses droits par défaut, le compte qui
-- lance ce script doit pouvoir faire SET ROLE cemac_app. INHERIT FALSE : il n'hérite d'aucun droit.
GRANT cemac_app TO CURRENT_USER WITH SET TRUE, INHERIT FALSE;

CREATE DATABASE cemac_bourse_calc OWNER cemac_app ENCODING 'UTF8' TEMPLATE template0;

-- Le compte qui lance ce script (neondb_owner, propriétaire du projet) garde le droit de s'y connecter.
SELECT current_user AS createur \gset

-- Les réglages suivants sont faits au nom de cemac_app, propriétaire de la base et de ses tables :
-- seul le propriétaire peut retirer ou donner des droits sur la base.
SET ROLE cemac_app;

-- Seuls nos deux comptes et le propriétaire du projet peuvent se connecter à la base.
REVOKE ALL ON DATABASE cemac_bourse_calc FROM PUBLIC;
GRANT CONNECT ON DATABASE cemac_bourse_calc TO cemac_lecture, :"createur";

RESET ROLE;

\connect cemac_bourse_calc

SET ROLE cemac_app;

-- Lecture seule : droit de voir le schéma, et SELECT sur toutes les tables que cemac_app
-- créera ensuite (le script de migration compris).
GRANT USAGE ON SCHEMA public TO cemac_lecture;
ALTER DEFAULT PRIVILEGES FOR ROLE cemac_app IN SCHEMA public GRANT SELECT ON TABLES TO cemac_lecture;

RESET ROLE;

\echo 'Base cemac_bourse_calc créée sur Neon. Étape suivante : générer les mots de passe dans la console Neon (Reset password).'
