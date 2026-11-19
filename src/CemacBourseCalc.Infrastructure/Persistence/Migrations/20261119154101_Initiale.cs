using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CemacBourseCalc.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initiale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "administrateur",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    nom = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    mot_de_passe_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    actif = table.Column<bool>(type: "boolean", nullable: false),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    derniere_connexion = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_administrateur", x => x.id);
                    table.CheckConstraint("ck_administrateur_email_minuscules", "email = lower(email)");
                    table.CheckConstraint("ck_administrateur_role", "role IN ('Admin', 'Editeur')");
                });

            migrationBuilder.CreateTable(
                name: "jour_non_ouvre",
                columns: table => new
                {
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    libelle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_jour_non_ouvre", x => x.date);
                });

            migrationBuilder.CreateTable(
                name: "pays",
                columns: table => new
                {
                    code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    libelle = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pays", x => x.code);
                    table.CheckConstraint("ck_pays_code", "code IN ('CM', 'CF', 'CG', 'GA', 'GQ', 'TD')");
                });

            migrationBuilder.CreateTable(
                name: "jeton_session",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    administrateur_id = table.Column<Guid>(type: "uuid", nullable: false),
                    jeton_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    cree_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    expire_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    revoque_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_jeton_session", x => x.id);
                    table.CheckConstraint("ck_jeton_session_expiration", "expire_le > cree_le");
                    table.ForeignKey(
                        name: "fk_jeton_session_administrateur_administrateur_id",
                        column: x => x.administrateur_id,
                        principalTable: "administrateur",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "journal_admin",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    administrateur_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    action = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    entite = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    entite_id = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    avant = table.Column<string>(type: "jsonb", nullable: true),
                    apres = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_journal_admin", x => x.id);
                    table.ForeignKey(
                        name: "fk_journal_admin_administrateur_administrateur_id",
                        column: x => x.administrateur_id,
                        principalTable: "administrateur",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "parametre",
                columns: table => new
                {
                    cle = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    valeur = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    libelle = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    modifie_le = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modifie_par_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_parametre", x => x.cle);
                    table.ForeignKey(
                        name: "fk_parametre_administrateur_modifie_par_id",
                        column: x => x.modifie_par_id,
                        principalTable: "administrateur",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "emetteur",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nom = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    sigle = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    pays_code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    site_web = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_emetteur", x => x.id);
                    table.CheckConstraint("ck_emetteur_type", "type IN ('Etat', 'Entreprise', 'Supranational')");
                    table.ForeignKey(
                        name: "fk_emetteur_pays_pays_code",
                        column: x => x.pays_code,
                        principalTable: "pays",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "titre",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    isin = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    nom = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    emetteur_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type_titre = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nominal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    date_admission = table.Column<DateOnly>(type: "date", nullable: true),
                    statut = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_titre", x => x.id);
                    table.CheckConstraint("ck_titre_nominal", "nominal > 0");
                    table.CheckConstraint("ck_titre_statut", "statut IN ('Actif', 'Suspendu', 'Echu', 'Radie')");
                    table.CheckConstraint("ck_titre_type_titre", "type_titre IN ('Obligation', 'Action')");
                    table.ForeignKey(
                        name: "fk_titre_emetteur_emetteur_id",
                        column: x => x.emetteur_id,
                        principalTable: "emetteur",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "action",
                columns: table => new
                {
                    titre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nombre_titres = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_action", x => x.titre_id);
                    table.CheckConstraint("ck_action_nombre_titres", "nombre_titres > 0");
                    table.ForeignKey(
                        name: "fk_action_titre_id",
                        column: x => x.titre_id,
                        principalTable: "titre",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cours",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    titre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_seance = table.Column<DateOnly>(type: "date", nullable: false),
                    cours_reference = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    cours_cloture = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    volume = table.Column<long>(type: "bigint", nullable: true),
                    coupon_couru_publie = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cours", x => x.id);
                    table.CheckConstraint("ck_cours_positifs", "(cours_reference IS NULL OR cours_reference > 0) AND (cours_cloture IS NULL OR cours_cloture > 0)");
                    table.CheckConstraint("ck_cours_un_cours", "cours_reference IS NOT NULL OR cours_cloture IS NOT NULL");
                    table.CheckConstraint("ck_cours_volume", "volume IS NULL OR volume >= 0");
                    table.ForeignKey(
                        name: "fk_cours_titres_titre_id",
                        column: x => x.titre_id,
                        principalTable: "titre",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "dividende",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    titre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    exercice = table.Column<int>(type: "integer", nullable: false),
                    montant_brut = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    montant_net = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    date_detachement = table.Column<DateOnly>(type: "date", nullable: true),
                    date_paiement = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dividende", x => x.id);
                    table.CheckConstraint("ck_dividende_brut", "montant_brut > 0");
                    table.CheckConstraint("ck_dividende_dates", "date_paiement IS NULL OR date_detachement IS NULL OR date_paiement >= date_detachement");
                    table.CheckConstraint("ck_dividende_net", "montant_net IS NULL OR (montant_net > 0 AND montant_net <= montant_brut)");
                    table.ForeignKey(
                        name: "fk_dividende_titres_titre_id",
                        column: x => x.titre_id,
                        principalTable: "titre",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "document_source",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    titre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    libelle = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    date_publication = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_document_source", x => x.id);
                    table.CheckConstraint("ck_document_source_type", "type IN ('Prospectus', 'Avis', 'Bulletin')");
                    table.ForeignKey(
                        name: "fk_document_source_titres_titre_id",
                        column: x => x.titre_id,
                        principalTable: "titre",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "obligation",
                columns: table => new
                {
                    titre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    taux_facial = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    periodicite = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    date_jouissance = table.Column<DateOnly>(type: "date", nullable: false),
                    date_premier_coupon = table.Column<DateOnly>(type: "date", nullable: true),
                    date_echeance = table.Column<DateOnly>(type: "date", nullable: false),
                    mode_remboursement = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    differe_echeances = table.Column<int>(type: "integer", nullable: false),
                    code_convention = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    regime_fiscal = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_obligation", x => x.titre_id);
                    table.CheckConstraint("ck_obligation_code_convention", "code_convention IN ('EXACT_365', 'EXACT_360', '30E_360')");
                    table.CheckConstraint("ck_obligation_dates", "date_echeance > date_jouissance");
                    table.CheckConstraint("ck_obligation_differe", "differe_echeances >= 0");
                    table.CheckConstraint("ck_obligation_mode_remboursement", "mode_remboursement IN ('InFine', 'AmortissementConstant')");
                    table.CheckConstraint("ck_obligation_periodicite", "periodicite IN ('InFine', 'Mensuelle', 'Trimestrielle', 'Semestrielle', 'Annuelle')");
                    table.CheckConstraint("ck_obligation_premier_coupon", "date_premier_coupon IS NULL OR (date_premier_coupon > date_jouissance AND date_premier_coupon <= date_echeance)");
                    table.CheckConstraint("ck_obligation_regime_fiscal", "regime_fiscal IN ('Net', 'Brut')");
                    table.CheckConstraint("ck_obligation_taux_facial", "taux_facial >= 0 AND taux_facial <= 100");
                    table.ForeignKey(
                        name: "fk_obligation_titre_id",
                        column: x => x.titre_id,
                        principalTable: "titre",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "parametre",
                columns: new[] { "cle", "libelle", "modifie_le", "modifie_par_id", "valeur" },
                values: new object[,]
                {
                    { "DELAI_REGLEMENT_JOURS_OUVRES", "Délai de règlement-livraison du marché coté, en jours ouvrés (T+N)", null, null, "3" },
                    { "REGLE_REPORT_PAIEMENT", "Report d'un paiement qui tombe un jour non ouvré", null, null, "JOUR_OUVRE_SUIVANT" }
                });

            migrationBuilder.InsertData(
                table: "pays",
                columns: new[] { "code", "libelle" },
                values: new object[,]
                {
                    { "CF", "République centrafricaine" },
                    { "CG", "Congo" },
                    { "CM", "Cameroun" },
                    { "GA", "Gabon" },
                    { "GQ", "Guinée équatoriale" },
                    { "TD", "Tchad" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_administrateur_email",
                table: "administrateur",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cours_titre_id_date_seance",
                table: "cours",
                columns: new[] { "titre_id", "date_seance" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_dividende_titre_id_exercice",
                table: "dividende",
                columns: new[] { "titre_id", "exercice" });

            migrationBuilder.CreateIndex(
                name: "ix_document_source_titre_id",
                table: "document_source",
                column: "titre_id");

            migrationBuilder.CreateIndex(
                name: "ix_emetteur_nom",
                table: "emetteur",
                column: "nom",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_emetteur_pays_code",
                table: "emetteur",
                column: "pays_code");

            migrationBuilder.CreateIndex(
                name: "ix_jeton_session_administrateur_id",
                table: "jeton_session",
                column: "administrateur_id");

            migrationBuilder.CreateIndex(
                name: "ix_jeton_session_jeton_hash",
                table: "jeton_session",
                column: "jeton_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_journal_admin_administrateur_id",
                table: "journal_admin",
                column: "administrateur_id");

            migrationBuilder.CreateIndex(
                name: "ix_journal_admin_date",
                table: "journal_admin",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "ix_journal_admin_entite_entite_id",
                table: "journal_admin",
                columns: new[] { "entite", "entite_id" });

            migrationBuilder.CreateIndex(
                name: "ix_parametre_modifie_par_id",
                table: "parametre",
                column: "modifie_par_id");

            migrationBuilder.CreateIndex(
                name: "ix_titre_emetteur_id",
                table: "titre",
                column: "emetteur_id");

            migrationBuilder.CreateIndex(
                name: "ix_titre_isin",
                table: "titre",
                column: "isin",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "action");

            migrationBuilder.DropTable(
                name: "cours");

            migrationBuilder.DropTable(
                name: "dividende");

            migrationBuilder.DropTable(
                name: "document_source");

            migrationBuilder.DropTable(
                name: "jeton_session");

            migrationBuilder.DropTable(
                name: "jour_non_ouvre");

            migrationBuilder.DropTable(
                name: "journal_admin");

            migrationBuilder.DropTable(
                name: "obligation");

            migrationBuilder.DropTable(
                name: "parametre");

            migrationBuilder.DropTable(
                name: "titre");

            migrationBuilder.DropTable(
                name: "administrateur");

            migrationBuilder.DropTable(
                name: "emetteur");

            migrationBuilder.DropTable(
                name: "pays");
        }
    }
}
