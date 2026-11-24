CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    migration_id character varying(150) NOT NULL,
    product_version character varying(32) NOT NULL,
    CONSTRAINT pk___ef_migrations_history PRIMARY KEY (migration_id)
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE TABLE administrateur (
        id uuid NOT NULL,
        email character varying(254) NOT NULL,
        nom character varying(200) NOT NULL,
        mot_de_passe_hash character varying(500) NOT NULL,
        role character varying(20) NOT NULL,
        actif boolean NOT NULL,
        cree_le timestamp with time zone NOT NULL,
        derniere_connexion timestamp with time zone,
        CONSTRAINT pk_administrateur PRIMARY KEY (id),
        CONSTRAINT ck_administrateur_email_minuscules CHECK (email = lower(email)),
        CONSTRAINT ck_administrateur_role CHECK (role IN ('Admin', 'Editeur'))
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE TABLE jour_non_ouvre (
        date date NOT NULL,
        libelle character varying(100) NOT NULL,
        CONSTRAINT pk_jour_non_ouvre PRIMARY KEY (date)
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE TABLE pays (
        code character varying(2) NOT NULL,
        libelle character varying(100) NOT NULL,
        CONSTRAINT pk_pays PRIMARY KEY (code),
        CONSTRAINT ck_pays_code CHECK (code IN ('CM', 'CF', 'CG', 'GA', 'GQ', 'TD'))
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE TABLE jeton_session (
        id uuid NOT NULL,
        administrateur_id uuid NOT NULL,
        jeton_hash character varying(128) NOT NULL,
        cree_le timestamp with time zone NOT NULL,
        expire_le timestamp with time zone NOT NULL,
        revoque_le timestamp with time zone,
        CONSTRAINT pk_jeton_session PRIMARY KEY (id),
        CONSTRAINT ck_jeton_session_expiration CHECK (expire_le > cree_le),
        CONSTRAINT fk_jeton_session_administrateur_administrateur_id FOREIGN KEY (administrateur_id) REFERENCES administrateur (id) ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE TABLE journal_admin (
        id uuid NOT NULL,
        administrateur_id uuid NOT NULL,
        date timestamp with time zone NOT NULL,
        action character varying(60) NOT NULL,
        entite character varying(60) NOT NULL,
        entite_id character varying(60) NOT NULL,
        avant jsonb,
        apres jsonb,
        CONSTRAINT pk_journal_admin PRIMARY KEY (id),
        CONSTRAINT fk_journal_admin_administrateur_administrateur_id FOREIGN KEY (administrateur_id) REFERENCES administrateur (id) ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE TABLE parametre (
        cle character varying(60) NOT NULL,
        valeur character varying(200) NOT NULL,
        libelle character varying(300) NOT NULL,
        modifie_le timestamp with time zone,
        modifie_par_id uuid,
        CONSTRAINT pk_parametre PRIMARY KEY (cle),
        CONSTRAINT fk_parametre_administrateur_modifie_par_id FOREIGN KEY (modifie_par_id) REFERENCES administrateur (id) ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE TABLE emetteur (
        id uuid NOT NULL,
        nom character varying(200) NOT NULL,
        sigle character varying(30),
        type character varying(20) NOT NULL,
        pays_code character varying(2) NOT NULL,
        site_web character varying(300),
        CONSTRAINT pk_emetteur PRIMARY KEY (id),
        CONSTRAINT ck_emetteur_type CHECK (type IN ('Etat', 'Entreprise', 'Supranational')),
        CONSTRAINT fk_emetteur_pays_pays_code FOREIGN KEY (pays_code) REFERENCES pays (code) ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE TABLE titre (
        id uuid NOT NULL,
        isin character varying(12) NOT NULL,
        nom character varying(200) NOT NULL,
        emetteur_id uuid NOT NULL,
        type_titre character varying(20) NOT NULL,
        nominal numeric(18,2) NOT NULL,
        date_admission date,
        statut character varying(20) NOT NULL,
        CONSTRAINT "PK_titre" PRIMARY KEY (id),
        CONSTRAINT ck_titre_nominal CHECK (nominal > 0),
        CONSTRAINT ck_titre_statut CHECK (statut IN ('Actif', 'Suspendu', 'Echu', 'Radie')),
        CONSTRAINT ck_titre_type_titre CHECK (type_titre IN ('Obligation', 'Action')),
        CONSTRAINT fk_titre_emetteur_emetteur_id FOREIGN KEY (emetteur_id) REFERENCES emetteur (id) ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE TABLE action (
        titre_id uuid NOT NULL,
        nombre_titres bigint NOT NULL,
        CONSTRAINT "PK_action" PRIMARY KEY (titre_id),
        CONSTRAINT ck_action_nombre_titres CHECK (nombre_titres > 0),
        CONSTRAINT fk_action_titre_id FOREIGN KEY (titre_id) REFERENCES titre (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE TABLE cours (
        id uuid NOT NULL,
        titre_id uuid NOT NULL,
        date_seance date NOT NULL,
        cours_reference numeric(18,4),
        cours_cloture numeric(18,4),
        volume bigint,
        coupon_couru_publie numeric(18,2),
        CONSTRAINT pk_cours PRIMARY KEY (id),
        CONSTRAINT ck_cours_positifs CHECK ((cours_reference IS NULL OR cours_reference > 0) AND (cours_cloture IS NULL OR cours_cloture > 0)),
        CONSTRAINT ck_cours_un_cours CHECK (cours_reference IS NOT NULL OR cours_cloture IS NOT NULL),
        CONSTRAINT ck_cours_volume CHECK (volume IS NULL OR volume >= 0),
        CONSTRAINT fk_cours_titres_titre_id FOREIGN KEY (titre_id) REFERENCES titre (id) ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE TABLE dividende (
        id uuid NOT NULL,
        titre_id uuid NOT NULL,
        exercice integer NOT NULL,
        montant_brut numeric(18,2) NOT NULL,
        montant_net numeric(18,2),
        date_detachement date,
        date_paiement date,
        CONSTRAINT pk_dividende PRIMARY KEY (id),
        CONSTRAINT ck_dividende_brut CHECK (montant_brut > 0),
        CONSTRAINT ck_dividende_dates CHECK (date_paiement IS NULL OR date_detachement IS NULL OR date_paiement >= date_detachement),
        CONSTRAINT ck_dividende_net CHECK (montant_net IS NULL OR (montant_net > 0 AND montant_net <= montant_brut)),
        CONSTRAINT fk_dividende_titres_titre_id FOREIGN KEY (titre_id) REFERENCES titre (id) ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE TABLE document_source (
        id uuid NOT NULL,
        titre_id uuid NOT NULL,
        type character varying(20) NOT NULL,
        libelle character varying(300) NOT NULL,
        url character varying(500) NOT NULL,
        date_publication date,
        CONSTRAINT pk_document_source PRIMARY KEY (id),
        CONSTRAINT ck_document_source_type CHECK (type IN ('Prospectus', 'Avis', 'Bulletin')),
        CONSTRAINT fk_document_source_titres_titre_id FOREIGN KEY (titre_id) REFERENCES titre (id) ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE TABLE obligation (
        titre_id uuid NOT NULL,
        taux_facial numeric(9,6) NOT NULL,
        periodicite character varying(20) NOT NULL,
        date_jouissance date NOT NULL,
        date_premier_coupon date,
        date_echeance date NOT NULL,
        mode_remboursement character varying(30) NOT NULL,
        differe_echeances integer NOT NULL,
        code_convention character varying(20) NOT NULL,
        regime_fiscal character varying(10) NOT NULL,
        CONSTRAINT "PK_obligation" PRIMARY KEY (titre_id),
        CONSTRAINT ck_obligation_code_convention CHECK (code_convention IN ('EXACT_365', 'EXACT_360', '30E_360')),
        CONSTRAINT ck_obligation_dates CHECK (date_echeance > date_jouissance),
        CONSTRAINT ck_obligation_differe CHECK (differe_echeances >= 0),
        CONSTRAINT ck_obligation_mode_remboursement CHECK (mode_remboursement IN ('InFine', 'AmortissementConstant')),
        CONSTRAINT ck_obligation_periodicite CHECK (periodicite IN ('InFine', 'Mensuelle', 'Trimestrielle', 'Semestrielle', 'Annuelle')),
        CONSTRAINT ck_obligation_premier_coupon CHECK (date_premier_coupon IS NULL OR (date_premier_coupon > date_jouissance AND date_premier_coupon <= date_echeance)),
        CONSTRAINT ck_obligation_regime_fiscal CHECK (regime_fiscal IN ('Net', 'Brut')),
        CONSTRAINT ck_obligation_taux_facial CHECK (taux_facial >= 0 AND taux_facial <= 100),
        CONSTRAINT fk_obligation_titre_id FOREIGN KEY (titre_id) REFERENCES titre (id) ON DELETE CASCADE
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    INSERT INTO parametre (cle, libelle, modifie_le, modifie_par_id, valeur)
    VALUES ('DELAI_REGLEMENT_JOURS_OUVRES', 'Délai de règlement-livraison du marché coté, en jours ouvrés (T+N)', NULL, NULL, '3');
    INSERT INTO parametre (cle, libelle, modifie_le, modifie_par_id, valeur)
    VALUES ('REGLE_REPORT_PAIEMENT', 'Report d''un paiement qui tombe un jour non ouvré', NULL, NULL, 'JOUR_OUVRE_SUIVANT');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    INSERT INTO pays (code, libelle)
    VALUES ('CF', 'République centrafricaine');
    INSERT INTO pays (code, libelle)
    VALUES ('CG', 'Congo');
    INSERT INTO pays (code, libelle)
    VALUES ('CM', 'Cameroun');
    INSERT INTO pays (code, libelle)
    VALUES ('GA', 'Gabon');
    INSERT INTO pays (code, libelle)
    VALUES ('GQ', 'Guinée équatoriale');
    INSERT INTO pays (code, libelle)
    VALUES ('TD', 'Tchad');
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE UNIQUE INDEX ix_administrateur_email ON administrateur (email);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE UNIQUE INDEX ix_cours_titre_id_date_seance ON cours (titre_id, date_seance);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE INDEX ix_dividende_titre_id_exercice ON dividende (titre_id, exercice);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE INDEX ix_document_source_titre_id ON document_source (titre_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE UNIQUE INDEX ix_emetteur_nom ON emetteur (nom);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE INDEX ix_emetteur_pays_code ON emetteur (pays_code);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE INDEX ix_jeton_session_administrateur_id ON jeton_session (administrateur_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE UNIQUE INDEX ix_jeton_session_jeton_hash ON jeton_session (jeton_hash);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE INDEX ix_journal_admin_administrateur_id ON journal_admin (administrateur_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE INDEX ix_journal_admin_date ON journal_admin (date);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE INDEX ix_journal_admin_entite_entite_id ON journal_admin (entite, entite_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE INDEX ix_parametre_modifie_par_id ON parametre (modifie_par_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE INDEX ix_titre_emetteur_id ON titre (emetteur_id);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    CREATE UNIQUE INDEX ix_titre_isin ON titre (isin);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261119154101_Initiale') THEN
    INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
    VALUES ('20261119154101_Initiale', '10.0.15');
    END IF;
END $EF$;
COMMIT;

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261121163418_IndexCoursDateSeance') THEN
    CREATE INDEX ix_cours_date_seance ON cours (date_seance);
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "migration_id" = '20261121163418_IndexCoursDateSeance') THEN
    INSERT INTO "__EFMigrationsHistory" (migration_id, product_version)
    VALUES ('20261121163418_IndexCoursDateSeance', '10.0.15');
    END IF;
END $EF$;
COMMIT;

