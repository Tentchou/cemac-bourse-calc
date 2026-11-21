// Historique des cours d'un titre : périodes, résumé, axe des cours, point le plus proche.
// Fonctions pures (testées dans historique.test.js). L'API rend les séances de la plus ancienne
// à la plus récente ; tout est calculé ici, sans rappeler l'API à chaque changement de période.

/** Les périodes proposées, en mois ; « tout » : tout l'historique reçu. */
export const PERIODES = [
  { cle: '1m', mois: 1 },
  { cle: '3m', mois: 3 },
  { cle: '6m', mois: 6 },
  { cle: '1a', mois: 12 },
  { cle: 'tout', mois: null },
]
export const PERIODE_DEFAUT = '1a'

/** Période lue d'une source non fiable (stockage du navigateur) : seules les clés proposées passent. */
export function lirePeriode(valeur) {
  return PERIODES.some((p) => p.cle === valeur) ? valeur : PERIODE_DEFAUT
}

/** « 2026-11-20 » moins n mois, en ISO ; le jour est ramené à la fin du mois si besoin (31/05 − 3 mois → 28/02). */
export function moinsMois(iso, mois) {
  const [a, m, j] = iso.split('-').map(Number)
  const total = a * 12 + (m - 1) - mois
  const annee = Math.floor(total / 12)
  const mois0 = total - annee * 12
  const dernierJour = new Date(Date.UTC(annee, mois0 + 1, 0)).getUTCDate()
  const jour = Math.min(j, dernierJour)
  return `${String(annee).padStart(4, '0')}-${String(mois0 + 1).padStart(2, '0')}-${String(jour).padStart(2, '0')}`
}

/**
 * Les séances de la période, comptée depuis la dernière séance (pas depuis aujourd'hui : un titre
 * sans cours récent garde sa courbe). Le début est trouvé par dichotomie : les points sont triés.
 */
export function filtrerPeriode(points, cle) {
  const periode = PERIODES.find((p) => p.cle === cle)
  if (!periode?.mois || points.length === 0) return points
  const debut = moinsMois(points[points.length - 1].dateSeance, periode.mois)
  let bas = 0
  let haut = points.length
  while (bas < haut) {
    const milieu = (bas + haut) >> 1
    if (points[milieu].dateSeance < debut) bas = milieu + 1
    else haut = milieu
  }
  return bas === 0 ? points : points.slice(bas)
}

/** Variation en % de a vers b ; null si elle n'a pas de sens. */
export function variationPourcent(a, b) {
  if (!(a > 0) || b === null || b === undefined) return null
  return ((b - a) / a) * 100
}

/**
 * Le résumé d'une période, en un seul passage : premier et dernier cours, variation, plus haut et
 * plus bas (avec leur date), titres échangés, séances avec échanges. Null pour une période vide.
 */
export function resumer(points) {
  if (points.length === 0) return null
  let haut = points[0]
  let bas = points[0]
  let volume = 0
  let seancesEchange = 0
  for (const p of points) {
    if (p.cours > haut.cours) haut = p
    if (p.cours < bas.cours) bas = p
    if (p.volume > 0) {
      volume += p.volume
      seancesEchange += 1
    }
  }
  const premier = points[0]
  const dernier = points[points.length - 1]
  return {
    premier,
    dernier,
    variation: dernier.cours - premier.cours,
    variationPourcent: variationPourcent(premier.cours, dernier.cours),
    haut,
    bas,
    volume,
    seances: points.length,
    seancesEchange,
  }
}

/**
 * Les séances du tableau, de la plus récente à la plus ancienne, chacune avec sa variation par
 * rapport à la séance précédente (la plus ancienne reçue n'en a pas).
 */
export function lignesTableau(points) {
  const lignes = new Array(points.length)
  for (let i = 0; i < points.length; i += 1) {
    const precedent = i > 0 ? points[i - 1].cours : null
    lignes[points.length - 1 - i] = {
      ...points[i],
      variationPourcent: precedent === null ? null : variationPourcent(precedent, points[i].cours),
    }
  }
  return lignes
}

// Retire les traînes de virgule flottante (0.30000000000000004) d'un multiple du pas.
const arrondir = (valeur, pas) => Number((Math.round(valeur / pas) * pas).toFixed(Math.max(0, 2 - Math.floor(Math.log10(pas)))))

/**
 * Un axe « rond » qui encadre [min, max] (pas de 1, 2, 2,5 ou 5 × 10^n, environ 4 intervalles).
 * Un cours constant est encadré d'une marge de 1 %, pour que la courbe ne colle pas à un bord.
 */
export function axeCours(min, max) {
  if (!Number.isFinite(min) || !Number.isFinite(max)) return { bas: 0, haut: 1, pas: 1 }
  let a = Math.min(min, max)
  let b = Math.max(min, max)
  if (b === a) {
    const marge = Math.abs(b) * 0.01 || 1
    a -= marge
    b += marge
  }
  const brut = (b - a) / 4
  const puissance = 10 ** Math.floor(Math.log10(brut))
  const norme = brut / puissance
  const pas = (norme <= 1 ? 1 : norme <= 2 ? 2 : norme <= 2.5 ? 2.5 : norme <= 5 ? 5 : 10) * puissance
  return { bas: arrondir(Math.floor(a / pas) * pas, pas), haut: arrondir(Math.ceil(b / pas) * pas, pas), pas }
}

/** Les graduations d'un axe, du bas vers le haut. */
export function reperes({ bas, haut, pas }) {
  const n = Math.round((haut - bas) / pas)
  return Array.from({ length: n + 1 }, (_, i) => arrondir(bas + i * pas, pas))
}

/** Les indices des dates écrites sous l'axe : la première, la dernière et des intermédiaires régulières. */
export function indicesDates(nombre, maximum) {
  if (nombre <= 0) return []
  if (nombre === 1) return [0]
  const n = Math.max(2, Math.min(maximum, nombre))
  return Array.from({ length: n }, (_, i) => Math.round((i * (nombre - 1)) / (n - 1)))
}

/** L'indice de la séance la plus proche d'une abscisse (séances régulièrement espacées), borné. */
export function indiceProche(x, gauche, pasX, nombre) {
  if (nombre <= 0) return -1
  const i = pasX > 0 ? Math.round((x - gauche) / pasX) : 0
  return Math.min(Math.max(i, 0), nombre - 1)
}
