// Les exemples chiffrés de la page Méthodes et de l'accueil, sur de vrais titres. Calculés ici, avec les mêmes règles que l'API
// (Domain/Calculs), plutôt qu'écrits à la main : la page ne peut pas afficher un chiffre faux.
// Vérifiés dans exemples.test.js.

/** Arrondi au centime, au plus proche, 0,005 monte (comme Arrondis.Montant côté API). */
export function arrondir(valeur) {
  // toPrecision(15) efface l'erreur binaire (1,005 × 100 = 100,4999…) avant d'arrondir.
  return (Math.sign(valeur) * Math.round(Number((Math.abs(valeur) * 100).toPrecision(15)))) / 100
}

/**
 * Obligation de l'exemple : un vrai titre, l'ECMR 6,25 % NET 2022-2029 (CM0000020305), à la séance
 * du 20/11/2026 (bulletin officiel de la cote n° 2643). Nominal 10 000, coupon annuel le 27/05,
 * Exact/365, amortissement constant de 2 000 par an après deux échéances de coupon seul : il reste
 * 6 000 par titre. La BVMAC publie un coupon couru de 186,99 FCFA par titre pour ce règlement.
 */
export const OBLIGATION = {
  titre: 'ECMR 6,25 % NET 2022-2029',
  isin: 'CM0000020305',
  pays: 'CM',
  bulletin: 2643,
  nominal: 10_000,
  taux: 6.25,
  base: 365,
  echeances: 7,
  differe: 2,
  premiereEcheance: '2023-05-27',
  dateNegociation: '2026-11-20',
  dateReglement: '2026-11-25',
  dernierCoupon: '2026-05-27',
  quantite: 100,
  cours: 100,
  couruPublie: 186.99,
}

/** Date ISO décalée de n années (même jour, même mois). */
const plusAnnees = (iso, n) => `${Number(iso.slice(0, 4)) + n}${iso.slice(4)}`

/**
 * Échéancier d'un amortissement constant, coupon annuel : les `differe` premières échéances ne
 * versent que le coupon, puis parts égales arrondies au centime ; la dernière prend le reste
 * (comme Amortissement). Les dates sont les dates théoriques du prospectus.
 */
export function exempleEcheancier({ nominal, taux, echeances, differe, premiereEcheance } = OBLIGATION) {
  const parts = echeances - differe
  const part = arrondir(nominal / parts)
  let restant = nominal
  return Array.from({ length: echeances }, (_, i) => {
    const debut = restant
    const capital = i < differe ? 0 : i === echeances - 1 ? restant : part
    const coupon = arrondir((debut * taux) / 100)
    restant = arrondir(restant - capital)
    return { rang: i + 1, date: plusAnnees(premiereEcheance, i), debut, coupon, capital, total: arrondir(coupon + capital), restant }
  })
}

/** Nominal restant au règlement : ce qui n'a pas été remboursé aux échéances passées. */
export function nominalRestant(o = OBLIGATION) {
  const passees = exempleEcheancier(o).filter((l) => l.date <= o.dateReglement)
  return passees.length ? passees.at(-1).restant : o.nominal
}

/** Coupon couru : N × t × j ÷ B, par titre (non arrondi) puis pour la quantité (arrondi une fois). */
export function exempleCouponCouru(o = OBLIGATION) {
  const nominal = nominalRestant(o)
  const jours = joursConvention('EXACT_365', o.dernierCoupon, o.dateReglement)
  const parTitre = (nominal * (o.taux / 100) * jours) / o.base
  return { nominal, jours, parTitre, total: arrondir(parTitre * o.quantite) }
}

/** Prix plein coupon : (N × C ÷ 100 + CC) × Q. */
export function exemplePrixPlein(o = OBLIGATION) {
  const { nominal, parTitre: couru } = exempleCouponCouru(o)
  const piedParTitre = (nominal * o.cours) / 100
  const pleinParTitre = piedParTitre + couru
  return { couru, piedParTitre, pleinParTitre, pied: arrondir(piedParTitre * o.quantite), couruTotal: arrondir(couru * o.quantite), total: arrondir(pleinParTitre * o.quantite) }
}

/**
 * Rendements : courant = t ÷ C × 100 ; actuariel = r tel que P = Σ F ÷ (1 + r)^(j ÷ 365),
 * trouvé par dichotomie (comme RendementObligation). P est le prix plein d'un titre, coupon couru
 * arrondi au centime comme le publie la BVMAC ; les flux sont les échéances après le règlement.
 */
export function exempleRendement(o = OBLIGATION) {
  const { piedParTitre, couru } = exemplePrixPlein(o)
  const prix = arrondir(piedParTitre + arrondir(couru))
  const flux = exempleEcheancier(o)
    .filter((l) => l.date > o.dateReglement)
    .map((l) => ({ rang: l.rang, date: l.date, jours: joursConvention('EXACT_365', o.dateReglement, l.date), montant: l.total }))
  const valeur = (r) => flux.reduce((s, f) => s + f.montant / (1 + r) ** (f.jours / 365), 0)

  let bas = -0.99
  let haut = 10
  for (let i = 0; i < 200 && haut - bas > 1e-12; i++) {
    const milieu = (bas + haut) / 2
    if (valeur(milieu) > prix) bas = milieu
    else haut = milieu
  }
  const r = (bas + haut) / 2

  return {
    prix,
    courant: (o.taux / o.cours) * 100,
    actuariel: r * 100,
    flux: flux.map((f) => ({ ...f, valeurActuelle: f.montant / (1 + r) ** (f.jours / 365) })),
  }
}

/**
 * Action de l'exemple : SAFACAM (CM0000010017), cours de clôture de 38 500 FCFA le 20/11/2026,
 * dividende brut de 2 200 FCFA pour l'exercice 2025 (bulletin n° 2643, p. 5).
 */
export const ACTION = { titre: 'SAFACAM', isin: 'CM0000010017', exercice: 2025, dividende: 2_200, cours: 38_500, date: '2026-11-20' }

/** Rendement du dividende : dividende ÷ cours × 100. */
export function exempleDividende({ dividende, cours } = ACTION) {
  return { dividende, rendement: (dividende / cours) * 100 }
}

/** Jours entre deux dates ISO selon la convention (mêmes règles que Conventions/*.cs). */
export function joursConvention(code, debut, fin) {
  const [a1, m1, j1] = debut.split('-').map(Number)
  const [a2, m2, j2] = fin.split('-').map(Number)
  if (code === '30E_360') return 360 * (a2 - a1) + 30 * (m2 - m1) + (Math.min(j2, 30) - Math.min(j1, 30))
  return (Date.UTC(a2, m2 - 1, j2) - Date.UTC(a1, m1 - 1, j1)) / 86_400_000
}

/** Les trois conventions, comparées sur la même période (31/01 → 31/03/2026). */
export const PERIODE_CONVENTIONS = { debut: '2026-01-31', fin: '2026-03-31' }

export const CONVENTIONS = [
  { code: 'EXACT_365', base: 365 },
  { code: 'EXACT_360', base: 360 },
  { code: '30E_360', base: 360 },
]
