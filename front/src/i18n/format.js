// Mise en forme des nombres, montants, pourcentages et dates selon la langue.
// Fonctions pures (testées dans format.test.js) : le contexte de langue ne fait que les lier.

const LOCALES = { fr: 'fr-FR', en: 'en-GB' }

/** Remplace les {variables} d'un texte. */
export function remplir(texte, valeurs = {}) {
  return texte.replace(/\{(\w+)\}/g, (tout, nom) => (nom in valeurs ? String(valeurs[nom]) : tout))
}

export function nombre(langue, valeur, decimales = 2) {
  if (valeur === null || valeur === undefined || Number.isNaN(Number(valeur))) return '—'
  // L’espace fine insécable (U+202F) du français manque dans certaines polices : espace insécable classique.
  return Number(valeur)
    .toLocaleString(LOCALES[langue], { minimumFractionDigits: decimales, maximumFractionDigits: decimales })
    .replace(/ /g, ' ')
}

/** Montant en francs CFA : « 102 043,84 FCFA » en français, « 102,043.84 XAF » en anglais. */
export function montant(langue, valeur, decimales = 2) {
  const texte = nombre(langue, valeur, decimales)
  return texte === '—' ? texte : `${texte} ${langue === 'fr' ? 'FCFA' : 'XAF'}`
}

/** Pourcentage déjà exprimé en % (6.5 → « 6,50 % » / « 6.50% »). */
export function pourcent(langue, valeur, decimales = 2) {
  const texte = nombre(langue, valeur, decimales)
  if (texte === '—') return texte
  return langue === 'fr' ? `${texte} %` : `${texte}%`
}

/** Date ISO « 2026-01-12 » → « 12/01/2026 » (les deux langues : usage CEMAC et britannique). */
export function date(langue, iso) {
  if (!iso) return '—'
  const [a, m, j] = iso.split('-').map(Number)
  return new Date(Date.UTC(a, m - 1, j)).toLocaleDateString(LOCALES[langue], { timeZone: 'UTC' })
}

/** Date longue pour les phrases : « jeudi 15 janvier 2026 » / « Thursday 15 January 2026 ». */
export function dateLongue(langue, iso) {
  if (!iso) return '—'
  const [a, m, j] = iso.split('-').map(Number)
  return new Date(Date.UTC(a, m - 1, j)).toLocaleDateString(LOCALES[langue], {
    timeZone: 'UTC',
    weekday: 'long',
    day: 'numeric',
    month: 'long',
    year: 'numeric',
  })
}

/** Lit un nombre saisi à la française ou à l'anglaise (« 98,50 », « 32 000 », « 98.5 »). Null si invalide. */
export function lireNombreSaisi(texte) {
  const propre = String(texte).replace(/[\s  ]/g, '').replace(',', '.')
  if (!/^\d+(\.\d+)?$/.test(propre)) return null
  return Number(propre)
}
