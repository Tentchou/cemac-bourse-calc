// Lecture des cellules d'un fichier importé (CSV ou Excel), au format français ou international.
// Chaque fonction rend { valeur } ou { erreur } : jamais d'exception, pour lister toutes les erreurs d'un coup.

/** « Date d'échéance » → « datedecheance » : pour comparer des en-têtes ou des choix sans accents ni ponctuation. */
export function normaliser(texte) {
  return String(texte ?? '')
    .normalize('NFD')
    .replace(/[̀-ͯ]/g, '')
    .toLowerCase()
    .replace(/[^a-z0-9]/g, '')
}

export function estVide(cellule) {
  return cellule === null || cellule === undefined || String(cellule).trim() === ''
}

export function lireTexte(cellule, { longueurMax } = {}) {
  const texte = String(cellule).trim()
  if (longueurMax && texte.length > longueurMax) return { erreur: `${longueurMax} caractères au plus` }
  return { valeur: texte }
}

/**
 * Nombres acceptés : 98,50 · 98.50 · 10 000 · 1 234 567,89 · 1.234.567,89 · 1,234,567.89 · 6,5 %.
 * Une seule virgule ou un seul point est lu comme séparateur décimal (usage français : 1,234 = 1,234).
 */
export function lireNombre(cellule) {
  if (typeof cellule === 'number') return Number.isFinite(cellule) ? { valeur: cellule } : { erreur: 'nombre attendu' }

  const s = String(cellule).trim().replace(/[\s  ]/g, '').replace(/%$/, '')
  let texte = null
  if (/^-?\d+([.,]\d+)?$/.test(s)) texte = s.replace(',', '.')
  else if (/^-?\d{1,3}(\.\d{3})+(,\d+)?$/.test(s)) texte = s.replace(/\./g, '').replace(',', '.')
  else if (/^-?\d{1,3}(,\d{3})+(\.\d+)?$/.test(s)) texte = s.replace(/,/g, '')

  return texte === null ? { erreur: `« ${String(cellule).trim()} » n'est pas un nombre` } : { valeur: Number(texte) }
}

export function lireEntier(cellule) {
  const nombre = lireNombre(cellule)
  if (nombre.erreur) return nombre
  return Number.isSafeInteger(nombre.valeur) ? nombre : { erreur: `« ${String(cellule).trim()} » n'est pas un nombre entier` }
}

function dateIso(annee, mois, jour) {
  const d = new Date(Date.UTC(annee, mois - 1, jour))
  if (d.getUTCFullYear() !== annee || d.getUTCMonth() !== mois - 1 || d.getUTCDate() !== jour) return null
  return `${String(annee).padStart(4, '0')}-${String(mois).padStart(2, '0')}-${String(jour).padStart(2, '0')}`
}

/** Dates acceptées : une cellule Date d'Excel, jj/mm/aaaa (ou jj-mm-aaaa, jj.mm.aaaa) et aaaa-mm-jj. Rend aaaa-mm-jj. */
export function lireDate(cellule) {
  if (cellule instanceof Date) {
    if (Number.isNaN(cellule.getTime())) return { erreur: 'date illisible' }
    // read-excel-file rend les dates d'Excel en UTC, sans décalage horaire.
    return { valeur: dateIso(cellule.getUTCFullYear(), cellule.getUTCMonth() + 1, cellule.getUTCDate()) }
  }

  const s = String(cellule).trim()
  let iso = null
  let m = /^(\d{1,2})[/.-](\d{1,2})[/.-](\d{4})$/.exec(s)
  if (m) iso = dateIso(Number(m[3]), Number(m[2]), Number(m[1]))
  m = /^(\d{4})-(\d{1,2})-(\d{1,2})$/.exec(s)
  if (m) iso = dateIso(Number(m[1]), Number(m[2]), Number(m[3]))

  return iso ? { valeur: iso } : { erreur: `« ${s} » n'est pas une date (jj/mm/aaaa)` }
}

/** Un choix dans une liste : la valeur attendue par l'API, ou l'un de ses autres noms, sans tenir compte des accents ni de la casse. */
export function lireChoix(cellule, choix) {
  const cherche = normaliser(cellule)
  const trouve = choix.find((c) => [c.valeur, c.libelle, ...(c.alias ?? [])].some((nom) => normaliser(nom) === cherche))
  return trouve
    ? { valeur: trouve.valeur }
    : { erreur: `« ${String(cellule).trim()} » n'est pas reconnu (choix : ${choix.map((c) => c.libelle).join(', ')})` }
}
