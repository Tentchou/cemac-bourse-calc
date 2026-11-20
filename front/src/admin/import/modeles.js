// Les colonnes de chaque fichier d'import. Une colonne = un champ de la commande de saisie manuelle de l'API :
// l'import ne porte aucune règle métier, il traduit le fichier et l'API contrôle chaque ligne.
//
// Types de colonne :
//   texte · nombre · entier · date · choix (liste fermée)
//   emetteur : le nom ou le sigle d'un émetteur existant → son identifiant
//   isin     : l'ISIN d'un titre existant → son identifiant

const PAYS = [
  { valeur: 'CM', libelle: 'Cameroun' },
  { valeur: 'CF', libelle: 'Centrafrique', alias: ['République centrafricaine', 'RCA'] },
  { valeur: 'CG', libelle: 'Congo', alias: ['République du Congo'] },
  { valeur: 'GA', libelle: 'Gabon' },
  { valeur: 'GQ', libelle: 'Guinée équatoriale' },
  { valeur: 'TD', libelle: 'Tchad' },
]

const TYPES_EMETTEUR = [
  { valeur: 'Etat', libelle: 'État' },
  { valeur: 'Entreprise', libelle: 'Entreprise', alias: ['Société'] },
  { valeur: 'Supranational', libelle: 'Supranational' },
]

const PERIODICITES = [
  { valeur: 'Mensuelle', libelle: 'Mensuelle' },
  { valeur: 'Trimestrielle', libelle: 'Trimestrielle' },
  { valeur: 'Semestrielle', libelle: 'Semestrielle' },
  { valeur: 'Annuelle', libelle: 'Annuelle' },
  { valeur: 'InFine', libelle: 'In fine' },
]

const REMBOURSEMENTS = [
  { valeur: 'InFine', libelle: 'In fine' },
  { valeur: 'AmortissementConstant', libelle: 'Amortissement constant', alias: ['Amortissable'] },
]

const CONVENTIONS = [
  { valeur: 'EXACT_365', libelle: 'Exact/365', alias: ['ACT/365', 'Actual/365'] },
  { valeur: 'EXACT_360', libelle: 'Exact/360', alias: ['ACT/360', 'Actual/360'] },
  { valeur: '30E_360', libelle: '30E/360', alias: ['30/360'] },
]

const REGIMES = [
  { valeur: 'Net', libelle: 'Net' },
  { valeur: 'Brut', libelle: 'Brut' },
]

const ISIN = { champ: 'isin', entete: 'ISIN', type: 'texte', obligatoire: true, longueurMax: 12, aide: '12 caractères, ex. CM0000000000' }
const NOM = { champ: 'nom', entete: 'Nom', type: 'texte', obligatoire: true, longueurMax: 200 }
const EMETTEUR = { champ: 'emetteurId', entete: 'Émetteur', type: 'emetteur', obligatoire: true, aide: 'nom ou sigle d’un émetteur déjà saisi' }
const NOMINAL = { champ: 'nominal', entete: 'Nominal', type: 'nombre', obligatoire: true, aide: 'en FCFA' }
const ADMISSION = { champ: 'dateAdmission', entete: "Date d'admission", type: 'date' }
const TITRE = { champ: 'titreId', entete: 'ISIN', type: 'isin', obligatoire: true, aide: 'ISIN d’un titre déjà saisi' }

export const MODELES = {
  emetteurs: {
    titre: 'Émetteurs',
    route: 'emetteurs',
    colonnes: [
      { champ: 'nom', entete: 'Nom', type: 'texte', obligatoire: true, longueurMax: 200 },
      { champ: 'sigle', entete: 'Sigle', type: 'texte', longueurMax: 30 },
      { champ: 'type', entete: 'Type', type: 'choix', obligatoire: true, choix: TYPES_EMETTEUR },
      { champ: 'paysCode', entete: 'Pays', type: 'choix', obligatoire: true, choix: PAYS },
      { champ: 'siteWeb', entete: 'Site web', type: 'texte', longueurMax: 300, aide: 'https://…' },
    ],
  },
  obligations: {
    titre: 'Obligations',
    route: 'obligations',
    colonnes: [
      ISIN, NOM, EMETTEUR, NOMINAL, ADMISSION,
      { champ: 'tauxFacial', entete: 'Taux facial', type: 'nombre', obligatoire: true, aide: 'en %, ex. 6,50' },
      { champ: 'periodicite', entete: 'Périodicité', type: 'choix', obligatoire: true, choix: PERIODICITES },
      { champ: 'dateJouissance', entete: 'Date de jouissance', type: 'date', obligatoire: true },
      { champ: 'datePremierCoupon', entete: 'Date du premier coupon', type: 'date', aide: 'si le premier coupon est irrégulier' },
      { champ: 'dateEcheance', entete: "Date d'échéance", type: 'date', obligatoire: true },
      { champ: 'modeRemboursement', entete: 'Remboursement', type: 'choix', obligatoire: true, choix: REMBOURSEMENTS },
      { champ: 'differeEcheances', entete: 'Différé', type: 'entier', defaut: 0, aide: 'nombre d’échéances sans remboursement, 0 par défaut' },
      { champ: 'codeConvention', entete: 'Convention', type: 'choix', obligatoire: true, choix: CONVENTIONS },
      { champ: 'regimeFiscal', entete: 'Régime fiscal', type: 'choix', obligatoire: true, choix: REGIMES },
    ],
  },
  actions: {
    titre: 'Actions',
    route: 'actions',
    colonnes: [
      ISIN, NOM, EMETTEUR, NOMINAL, ADMISSION,
      { champ: 'nombreTitres', entete: 'Nombre de titres', type: 'entier', obligatoire: true, aide: 'actions composant le capital' },
    ],
  },
  cours: {
    titre: 'Cours',
    route: 'cours',
    colonnes: [
      TITRE,
      { champ: 'dateSeance', entete: 'Date de séance', type: 'date', obligatoire: true },
      { champ: 'coursReference', entete: 'Cours de référence', type: 'nombre', aide: 'obligation : % du nominal ; action : FCFA' },
      { champ: 'coursCloture', entete: 'Cours de clôture', type: 'nombre' },
      { champ: 'volume', entete: 'Volume', type: 'entier' },
      { champ: 'couponCouruPublie', entete: 'Coupon couru publié', type: 'nombre', aide: 'en FCFA, tel que publié au bulletin' },
    ],
  },
  dividendes: {
    titre: 'Dividendes',
    route: 'dividendes',
    colonnes: [
      TITRE,
      { champ: 'exercice', entete: 'Exercice', type: 'entier', obligatoire: true },
      { champ: 'montantBrut', entete: 'Dividende brut', type: 'nombre', obligatoire: true, aide: 'par action, en FCFA' },
      { champ: 'montantNet', entete: 'Dividende net', type: 'nombre' },
      { champ: 'dateDetachement', entete: 'Date de détachement', type: 'date' },
      { champ: 'datePaiement', entete: 'Date de paiement', type: 'date' },
    ],
  },
  joursNonOuvres: {
    titre: 'Jours non ouvrés',
    route: 'jours-non-ouvres',
    colonnes: [
      { champ: 'date', entete: 'Date', type: 'date', obligatoire: true },
      { champ: 'libelle', entete: 'Libellé', type: 'texte', obligatoire: true, longueurMax: 100 },
    ],
  },
}

/** Le texte d'aide d'une colonne : ce qu'elle attend, pour le modèle à télécharger et l'écran. */
export function aideColonne(colonne) {
  const morceaux = [colonne.obligatoire ? 'Obligatoire' : 'Facultatif']
  if (colonne.choix) morceaux.push(`choix : ${colonne.choix.map((c) => c.libelle).join(', ')}`)
  else if (colonne.type === 'date') morceaux.push('date jj/mm/aaaa')
  if (colonne.aide) morceaux.push(colonne.aide)
  return morceaux.join(' · ')
}
