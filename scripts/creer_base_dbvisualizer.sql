-- Création de la base CEMAC Bourse Calc depuis DbVisualizer, sur le serveur
-- PostgreSQL 17 local (port 5433), sans toucher à la base kori_ams.
--
-- Même résultat que creer_base.sql (prévu pour psql), en trois étapes.
-- Les deux mots de passe se tapent dans l'éditeur à la place de
-- <MOT_DE_PASSE_APP> et <MOT_DE_PASSE_LECTURE>, puis on exécute.
-- NE PAS ENREGISTRER ce fichier une fois les mots de passe tapés : il est dans git.


-- ============================================================================
-- ÉTAPE 0 — avec ta connexion KORI actuelle (kori_owner) : vérifier les droits.
-- Il faut rolsuper = true, OU rolcreaterole = true ET rolcreatedb = true.
-- Sinon, arrête-toi ici et dis-le-moi.
-- ============================================================================

SELECT rolname, rolsuper, rolcreaterole, rolcreatedb
FROM pg_roles
WHERE rolname = current_user;


-- ============================================================================
-- ÉTAPE 1 — toujours avec la connexion KORI : les deux comptes et la base.
-- CREATE DATABASE refuse de tourner dans une transaction : laisser l'auto-commit
-- activé (réglage par défaut de DbVisualizer).
-- ============================================================================

-- cemac_app : le compte de l'API. Propriétaire de la base, sans privilèges serveur.
CREATE ROLE cemac_app LOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE PASSWORD '<MOT_DE_PASSE_APP>';

-- cemac_lecture : lecture seule, pour consulter les tables dans DbVisualizer.
CREATE ROLE cemac_lecture LOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE PASSWORD '<MOT_DE_PASSE_LECTURE>';

-- Pour donner la base à cemac_app, le compte qui la crée doit pouvoir agir en son nom
-- (droit retiré à l'étape 3).
GRANT cemac_app TO CURRENT_USER;

CREATE DATABASE cemac_bourse_calc OWNER cemac_app ENCODING 'UTF8' TEMPLATE template0;

-- Personne d'autre (kori_app, kori_owner…) ne peut se connecter à la nouvelle base.
REVOKE ALL ON DATABASE cemac_bourse_calc FROM PUBLIC;
GRANT CONNECT ON DATABASE cemac_bourse_calc TO cemac_app, cemac_lecture;


-- ============================================================================
-- ÉTAPE 2 — même compte, mais connecté à la base cemac_bourse_calc.
-- Dans DbVisualizer : propriétés de la connexion KORI → Database = cemac_bourse_calc,
-- se reconnecter, exécuter ce bloc, puis remettre Database = kori_ams.
-- (La connexion n'a pas le droit CONNECT, accordé juste avant, si le compte n'est pas
-- superutilisateur : dans ce cas, exécuter d'abord, sur kori_ams :
--   GRANT CONNECT ON DATABASE cemac_bourse_calc TO CURRENT_USER;
-- et le retirer à l'étape 3.)
-- ============================================================================

GRANT USAGE ON SCHEMA public TO cemac_lecture;
ALTER DEFAULT PRIVILEGES FOR ROLE cemac_app IN SCHEMA public GRANT SELECT ON TABLES TO cemac_lecture;


-- ============================================================================
-- ÉTAPE 3 — retour sur kori_ams : retirer les droits temporaires.
-- ============================================================================

REVOKE cemac_app FROM CURRENT_USER;
REVOKE CONNECT ON DATABASE cemac_bourse_calc FROM CURRENT_USER;
