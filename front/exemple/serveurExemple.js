// Mode « exemple » (npm run dev:exemple) : sert des réponses fixes à la place de l'API,
// pour voir et vérifier l'interface sans base de données. N'est jamais inclus dans le build.
// Les montants de l'obligation in fine sont ceux des tests du Domain (vérifiés à la main),
// calculés pour le 12/01/2026, 10 titres, cours 98,50 % — quelle que soit la saisie.

const ID_IN_FINE = '0193f6a0-0000-7000-8000-000000000011'
const ID_AMORTI = '0193f6a0-0000-7000-8000-000000000020'
const ID_SEMESTRIEL = '0193f6a0-0000-7000-8000-000000000038'
const ID_ACTION = '0193f6a0-0000-7000-8000-000000000045'

const titres = [
  { id: ID_IN_FINE, isin: 'CM0000000011', nom: 'Exemple in fine 6,50 % 2025-2030', emetteur: 'Émetteur A', pays: 'CM', typeTitre: 'Obligation', nominal: 10000, statut: 'Actif', tauxFacial: 6.5, dateEcheance: '2030-06-30', dernierCours: 98.5, dateDernierCours: '2026-01-12' },
  { id: ID_AMORTI, isin: 'GA0000000020', nom: 'Exemple amortissable 6,00 % 2024-2029', emetteur: 'Émetteur B', pays: 'GA', typeTitre: 'Obligation', nominal: 10000, statut: 'Actif', tauxFacial: 6, dateEcheance: '2029-09-30', dernierCours: 101, dateDernierCours: '2026-01-12' },
  { id: ID_SEMESTRIEL, isin: 'CG0000000038', nom: 'Exemple semestriel 5,00 % 2026-2028', emetteur: 'Émetteur C', pays: 'CG', typeTitre: 'Obligation', nominal: 10000, statut: 'Actif', tauxFacial: 5, dateEcheance: '2028-09-30', dernierCours: 99.75, dateDernierCours: '2026-01-12' },
  { id: ID_ACTION, isin: 'CM0000000045', nom: 'Société Exemple SA', emetteur: 'Société Exemple', pays: 'CM', typeTitre: 'Action', nominal: 10000, statut: 'Actif', tauxFacial: null, dateEcheance: null, dernierCours: 32000, dateDernierCours: '2026-01-12' },
]

const obligation = (titre, o) => ({ ...titre, obligation: { datePremierCoupon: null, ...o }, action: null })

const fiches = {
  [ID_IN_FINE]: obligation(titres[0], { tauxFacial: 6.5, periodicite: 'Annuelle', modeRemboursement: 'InFine', dateJouissance: '2025-06-30', dateEcheance: '2030-06-30', convention: 'Exact/365', regimeFiscal: 'Net' }),
  [ID_AMORTI]: obligation(titres[1], { tauxFacial: 6, periodicite: 'Annuelle', modeRemboursement: 'AmortissementConstant', dateJouissance: '2024-09-30', dateEcheance: '2029-09-30', convention: 'Exact/365', regimeFiscal: 'Net' }),
  [ID_SEMESTRIEL]: obligation(titres[2], { tauxFacial: 5, periodicite: 'Semestrielle', modeRemboursement: 'InFine', dateJouissance: '2026-03-31', dateEcheance: '2028-09-30', convention: 'Exact/360', regimeFiscal: 'Brut' }),
  [ID_ACTION]: {
    ...titres[3],
    obligation: null,
    action: {
      nombreTitres: 1000000,
      dividendes: [
        { exercice: 2025, brut: 1500, net: 1260, dateDetachement: '2026-06-15', datePaiement: '2026-06-30' },
        { exercice: 2024, brut: 1350, net: 1134, dateDetachement: '2025-06-16', datePaiement: '2025-06-30' },
      ],
    },
  },
}

const couponCouru = {
  titre: titres[0].nom, dateNegociation: '2026-01-12', delaiReglementJoursOuvres: 3, dateReglement: '2026-01-15',
  debutPeriode: '2025-06-30', finPeriode: '2026-06-30', convention: 'Exact/365', jours: 199, joursAnnee: 365,
  nominalRestant: 10000, tauxFacial: 6.5, quantite: 10, couponCouruParTitre: 354.3836, couponCouruTotal: 3543.84,
  coursPiedCoupon: 98.5, prixPiedCouponParTitre: 9850, prixPiedCouponTotal: 98500, prixPleinCouponTotal: 102043.84,
}

const flux = [
  ['2026-06-30', 166, 0.4548, 650, 630.62],
  ['2027-06-30', 531, 1.4548, 650, 590.0],
  ['2028-06-30', 897, 2.4575, 650, 551.9],
  ['2029-06-30', 1262, 3.4575, 650, 516.36],
  ['2030-06-30', 1627, 4.4575, 10650, 7915.5],
].map(([date, jours, fractionAnnee, f, valeurActuelle]) => ({
  date, jours, fractionAnnee, coupon: 650, capital: f - 650, flux: f, facteurActualisation: valeurActuelle / f, valeurActuelle,
}))

const rendement = {
  titre: titres[0].nom, dateNegociation: '2026-01-12', delaiReglementJoursOuvres: 3, dateReglement: '2026-01-15',
  coursPiedCoupon: 98.5, nominalRestant: 10000, tauxFacial: 6.5, prixPiedCouponParTitre: 9850, couponCouruParTitre: 354.38,
  prixPleinCouponParTitre: 10204.38, couponAnnuelParTitre: 650, rendementCourant: 6.599, rendementActuariel: 6.8836,
  joursAnneeActuarielle: 365, flux,
}

const ligne = (rang, debut, theorique, paiement, capital) => ({
  rang, debutPeriode: debut, dateTheorique: theorique, datePaiement: paiement, periodeReguliere: true,
  nominalDebutParTitre: 10000, couponParTitre: 650, capitalParTitre: capital / 10,
  nominalRestantApresParTitre: 10000 - capital / 10, coupon: 6500, capital, total: 6500 + capital,
})

const echeancier = {
  titre: titres[0].nom, convention: 'Exact/365', quantite: 10, dateNegociation: '2026-01-12', dateReglement: '2026-01-15',
  lignes: [
    ligne(1, '2025-06-30', '2026-06-30', '2026-06-30', 0),
    ligne(2, '2026-06-30', '2027-06-30', '2027-06-30', 0),
    ligne(3, '2027-06-30', '2028-06-30', '2028-06-30', 0),
    ligne(4, '2028-06-30', '2029-06-30', '2029-07-02', 0),
    ligne(5, '2029-06-30', '2030-06-30', '2030-07-01', 100000),
  ],
  totalCoupons: 32500, totalCapital: 100000, totalFlux: 132500,
}

const dividende = {
  titre: titres[3].nom, cours: 32000, exercice: 2025, nombreVersements: 1,
  dividendeBrut: 1500, dividendeNet: 1260, rendementBrut: 4.6875, rendementNet: 3.9375,
}

// Historique d'exemple : deux ans de séances (du lundi au vendredi) jusqu'au dernier cours du titre,
// tirées d'un générateur à graine fixe (les mêmes courbes à chaque lancement). Beaucoup de séances
// sans échange, cours alors inchangé : c'est le cas de la plupart des titres de la BVMAC.
function historiqueExemple(titre, graine) {
  let etat = graine
  const hasard = () => ((etat = (etat * 1664525 + 1013904223) % 4294967296) / 4294967296)
  const obligation = titre.typeTitre === 'Obligation'
  const dates = []
  const jour = new Date(`${titre.dateDernierCours}T00:00:00Z`)
  while (dates.length < 520) {
    const j = jour.getUTCDay()
    if (j !== 0 && j !== 6) dates.unshift(jour.toISOString().slice(0, 10))
    jour.setUTCDate(jour.getUTCDate() - 1)
  }
  // On remonte le temps depuis le dernier cours : la courbe finit exactement sur la valeur de la fiche.
  const cours = new Array(dates.length)
  const volumes = new Array(dates.length)
  let c = titre.dernierCours
  for (let i = dates.length - 1; i >= 0; i -= 1) {
    cours[i] = c
    const echange = hasard() < (obligation ? 0.3 : 0.55)
    volumes[i] = echange ? Math.round((obligation ? 20 : 5) + hasard() * (obligation ? 900 : 300)) : 0
    if (echange) {
      const pas = obligation ? (hasard() - 0.5) * 0.6 : Math.round(((hasard() - 0.48) * 900) / 5) * 5
      c = obligation ? Math.round(Math.min(Math.max(c - pas, 94), 104) * 100) / 100 : Math.max(c - pas, 1000)
    }
  }
  return dates.map((dateSeance, i) => ({ dateSeance, cours: cours[i], volume: volumes[i] }))
}

const historiques = Object.fromEntries(titres.map((titre, i) => [titre.id, historiqueExemple(titre, 7919 * (i + 1))]))

function repondre(chemin) {
  if (chemin === '/api/titres') return titres
  const fiche = /^\/api\/titres\/([0-9a-f-]{36})$/.exec(chemin)
  if (fiche) return fiches[fiche[1]] ?? null
  const historique = /^\/api\/titres\/([0-9a-f-]{36})\/cours$/.exec(chemin)
  if (historique) return historiques[historique[1]] ?? null
  if (/^\/api\/obligations\/[0-9a-f-]{36}\/coupon-couru$/.test(chemin)) return couponCouru
  if (/^\/api\/obligations\/[0-9a-f-]{36}\/rendement$/.test(chemin)) return rendement
  if (/^\/api\/obligations\/[0-9a-f-]{36}\/echeancier$/.test(chemin)) return echeancier
  if (/^\/api\/actions\/[0-9a-f-]{36}\/rendement-dividende$/.test(chemin)) return dividende
  return null
}

/** Greffon Vite : intercepte /api en mode exemple. */
export default function serveurExemple() {
  return {
    name: 'cbc-serveur-exemple',
    configureServer(serveur) {
      serveur.middlewares.use((requete, reponse, suivant) => {
        const chemin = requete.url.split('?')[0]
        if (!chemin.startsWith('/api/') || chemin.startsWith('/api/admin')) return suivant()
        const corps = repondre(chemin)
        reponse.setHeader('Content-Type', 'application/json; charset=utf-8')
        if (corps === null) {
          reponse.statusCode = 404
          reponse.end(JSON.stringify({ title: 'Introuvable', status: 404 }))
          return
        }
        // Des délais différents par route, pour voir la page se construire bloc par bloc.
        const delai = chemin.endsWith('/echeancier') ? 1600 : chemin.endsWith('/rendement') ? 1100 : chemin.endsWith('/coupon-couru') ? 700 : 500
        setTimeout(() => reponse.end(JSON.stringify(corps)), delai)
      })
    },
  }
}
