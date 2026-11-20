// Les exemples chiffrés de la page Méthodes. Calculés ici, avec les mêmes règles que l'API
// (Domain/Calculs), plutôt qu'écrits à la main : la page ne peut pas afficher un chiffre faux.
// Vérifiés dans exemples.test.js.

/** Arrondi au centime, au plus proche, 0,005 monte (comme Arrondis.Montant côté API). */
export function arrondir(valeur) {
  // toPrecision(15) efface l'erreur binaire (1,005 × 100 = 100,4999…) avant d'arrondir.
  return (Math.sign(valeur) * Math.round(Number((Math.abs(valeur) * 100).toPrecision(15)))) / 100
}

/** Obligation de l'exemple : 10 000 FCFA à 6,50 %, Exact/365, coupon annuel. */
export const OBLIGATION = { nominal: 10_000, taux: 6.5, base: 365, joursCourus: 90, quantite: 100, cours: 98.5 }

/** Coupon couru : N × t × j ÷ B, par titre (non arrondi) puis pour la quantité (arrondi une fois). */
export function exempleCouponCouru({ nominal, taux, base, joursCourus, quantite } = OBLIGATION) {
  const parTitre = (nominal * (taux / 100) * joursCourus) / base
  return { parTitre, total: arrondir(parTitre * quantite) }
}

/** Prix plein coupon : (N × C ÷ 100 + CC) × Q. */
export function exemplePrixPlein(o = OBLIGATION) {
  const { parTitre: couru } = exempleCouponCouru(o)
  const piedParTitre = (o.nominal * o.cours) / 100
  const pleinParTitre = piedParTitre + couru
  return { couru, piedParTitre, pleinParTitre, total: arrondir(pleinParTitre * o.quantite) }
}

/**
 * Rendements : courant = t ÷ C × 100 ; actuariel = r tel que P = Σ F ÷ (1 + r)^(j ÷ 365),
 * trouvé par dichotomie (comme RendementObligation). Flux restants de l'exemple : le coupon
 * dans 275 jours (365 − 90), puis coupon + capital dans 640 jours.
 */
export function exempleRendement(o = OBLIGATION) {
  const { pleinParTitre: prix } = exemplePrixPlein(o)
  const coupon = (o.nominal * o.taux) / 100
  const flux = [
    { jours: o.base - o.joursCourus, montant: coupon },
    { jours: 2 * o.base - o.joursCourus, montant: coupon + o.nominal },
  ]
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
 * Échéancier d'un amortissement constant : 4 échéances annuelles, sans différé.
 * Part = N ÷ 4 arrondie au centime ; la dernière prend le reste (comme Amortissement).
 */
export function exempleEcheancier({ nominal, taux } = OBLIGATION, echeances = 4) {
  const part = arrondir(nominal / echeances)
  let restant = nominal
  return Array.from({ length: echeances }, (_, i) => {
    const debut = restant
    const capital = i === echeances - 1 ? restant : part
    const coupon = arrondir((debut * taux) / 100)
    restant = arrondir(restant - capital)
    return { rang: i + 1, debut, coupon, capital, total: arrondir(coupon + capital) }
  })
}

/** Dividende : (acompte + solde) ÷ cours × 100. */
export const ACTION = { acompte: 500, solde: 1_000, cours: 25_000 }

export function exempleDividende({ acompte, solde, cours } = ACTION) {
  const dividende = acompte + solde
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
