// Calcul des dates du calendrier, sur des chaînes ISO « AAAA-MM-JJ » (jamais d'objet Date
// local : aucun décalage de fuseau possible). Fonctions pures, testées dans dates.test.js.

const ISO = /^(\d{4})-(\d{2})-(\d{2})$/

const deux = (n) => String(n).padStart(2, '0')

/** Nombre de jours d'un mois (m de 1 à 12). */
export function joursDansMois(a, m) {
  return new Date(Date.UTC(a, m, 0)).getUTCDate()
}

/** { a, m, j } → « AAAA-MM-JJ » ; null si la date n'existe pas (31/02…). */
export function versIso(a, m, j) {
  if (!Number.isInteger(a) || !Number.isInteger(m) || !Number.isInteger(j)) return null
  if (a < 1900 || a > 2199 || m < 1 || m > 12 || j < 1 || j > joursDansMois(a, m)) return null
  return `${a}-${deux(m)}-${deux(j)}`
}

/** « AAAA-MM-JJ » → { a, m, j } ; null si la chaîne n'est pas une date réelle. */
export function lireIso(iso) {
  const r = ISO.exec(iso ?? '')
  if (!r) return null
  const [a, m, j] = [Number(r[1]), Number(r[2]), Number(r[3])]
  return versIso(a, m, j) ? { a, m, j } : null
}

const versJour = (iso) => {
  const { a, m, j } = lireIso(iso)
  return Date.UTC(a, m - 1, j) / 86_400_000
}
const depuisJour = (n) => new Date(n * 86_400_000).toISOString().slice(0, 10)

export function ajouterJours(iso, n) {
  return depuisJour(versJour(iso) + n)
}

/** Ajoute des mois en gardant le jour, ramené au dernier jour du mois s'il n'existe pas (31/01 + 1 → 28/02). */
export function ajouterMois(iso, n) {
  const { a, m, j } = lireIso(iso)
  const total = a * 12 + (m - 1) + n
  const na = Math.floor(total / 12)
  const nm = (total % 12) + 1
  return versIso(na, nm, Math.min(j, joursDansMois(na, nm)))
}

/** Jour de la semaine, lundi = 0 … dimanche = 6 (la semaine boursière commence le lundi). */
export function jourSemaine(iso) {
  const { a, m, j } = lireIso(iso)
  return (new Date(Date.UTC(a, m - 1, j)).getUTCDay() + 6) % 7
}

export const estWeekend = (iso) => jourSemaine(iso) >= 5

/** Les 42 jours (6 semaines, lundi en tête) affichés pour un mois. */
export function grilleMois(a, m) {
  const premier = versIso(a, m, 1)
  const debut = ajouterJours(premier, -jourSemaine(premier))
  return Array.from({ length: 42 }, (_, i) => {
    const iso = ajouterJours(debut, i)
    return { iso, jour: Number(iso.slice(8)), horsMois: Number(iso.slice(5, 7)) !== m }
  })
}

/** La date est-elle dans les bornes (incluses, facultatives) ? Les chaînes ISO se comparent telles quelles. */
export function dansBornes(iso, min, max) {
  return (!min || iso >= min) && (!max || iso <= max)
}

/** Ramène une date dans les bornes. */
export function borner(iso, min, max) {
  if (min && iso < min) return min
  if (max && iso > max) return max
  return iso
}

/** Date du jour, dans le fuseau du visiteur. */
export function aujourdhui(maintenant = new Date()) {
  return `${maintenant.getFullYear()}-${deux(maintenant.getMonth() + 1)}-${deux(maintenant.getDate())}`
}

/** « AAAA-MM-JJ » → « JJ/MM/AAAA » pour la zone de saisie. */
export function formaterSaisie(iso) {
  const d = lireIso(iso)
  return d ? `${deux(d.j)}/${deux(d.m)}/${d.a}` : ''
}

/**
 * Pendant la frappe : garde les chiffres et pose les barres toutes seules (« 12012026 » → « 12/01/2026 »).
 * Une barre tapée après un seul chiffre complète par un zéro (« 1/ » → « 01/ »).
 */
export function masquerSaisie(texte) {
  const morceaux = String(texte)
    .replace(/[.\-\s]/g, '/')
    .split('/')
  let chiffres = ''
  morceaux.forEach((morceau, i) => {
    let c = morceau.replace(/\D/g, '')
    const suivi = i < morceaux.length - 1
    // « 1/ » : le visiteur a fini le jour ou le mois en un chiffre.
    if (suivi && c.length === 1 && chiffres.length < 4 && chiffres.length % 2 === 0) c = `0${c}`
    chiffres += c
  })
  chiffres = chiffres.slice(0, 8)
  if (chiffres.length <= 2) return chiffres + (texte.endsWith('/') && chiffres.length === 2 ? '/' : '')
  if (chiffres.length <= 4) {
    const t = `${chiffres.slice(0, 2)}/${chiffres.slice(2)}`
    return t + (texte.endsWith('/') && chiffres.length === 4 ? '/' : '')
  }
  return `${chiffres.slice(0, 2)}/${chiffres.slice(2, 4)}/${chiffres.slice(4)}`
}

/** Lit une date tapée : « 12/01/2026 », « 12.01.2026 », « 12012026 » ou « 2026-01-12 ». Null si invalide. */
export function lireSaisie(texte) {
  const propre = String(texte ?? '').trim()
  if (ISO.test(propre)) return lireIso(propre) ? propre : null
  const r = /^(\d{1,2})[/.\- ](\d{1,2})[/.\- ](\d{4})$/.exec(propre) ?? /^(\d{2})(\d{2})(\d{4})$/.exec(propre)
  if (!r) return null
  return versIso(Number(r[3]), Number(r[2]), Number(r[1]))
}

/**
 * Le jour de séance le plus proche : un samedi ou un dimanche recule au vendredi ; si ce vendredi
 * sort des bornes (min), on avance au lundi suivant.
 */
export function jourDeSeance(iso, min, max) {
  const date = borner(iso, min, max)
  if (!estWeekend(date)) return date
  const vendredi = ajouterJours(date, 4 - jourSemaine(date))
  if (!min || vendredi >= min) return vendredi
  return borner(ajouterJours(date, 7 - jourSemaine(date)), min, max)
}
