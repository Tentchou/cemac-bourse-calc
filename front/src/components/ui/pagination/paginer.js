// Pagination d'une liste déjà chargée. Fonctions pures, testées.
// Toute valeur venue de l'extérieur (adresse, stockage du navigateur) est contrôlée ici :
// une page ou une taille invalide retombe sur une valeur sûre, jamais d'erreur ni de page vide.

/** Les tailles de page proposées ; la première est la taille par défaut. */
export const TAILLES_PAGE = [10, 20, 50]

/** Taille de page lue d'une source non fiable : seules les valeurs proposées sont acceptées. */
export function lireTaille(valeur) {
  const n = Number(valeur)
  return TAILLES_PAGE.includes(n) ? n : TAILLES_PAGE[0]
}

/** Numéro de page lu d'une source non fiable : un entier ≥ 1 en chiffres seulement, sinon 1. */
export function lirePage(valeur) {
  if (typeof valeur !== 'string' && typeof valeur !== 'number') return 1
  const texte = String(valeur).trim()
  if (!/^\d{1,6}$/.test(texte)) return 1
  return Math.max(1, Number(texte))
}

/**
 * Découpe la page demandée. La page est ramenée dans [1, pages] (une liste vide a 1 page).
 * Retourne aussi la plage affichée (debut–fin, comptés à partir de 1) pour « 11–20 sur 40 ».
 */
export function paginer(lignes, page, taille) {
  const total = lignes.length
  const pages = Math.max(1, Math.ceil(total / taille))
  const courante = Math.min(Math.max(1, page), pages)
  const debut = (courante - 1) * taille
  const fin = Math.min(debut + taille, total)
  return { lignes: lignes.slice(debut, fin), page: courante, pages, total, debut: total ? debut + 1 : 0, fin }
}

/**
 * Les boutons de page à afficher : toujours la première et la dernière, la courante et ses voisines,
 * « … » pour les trous. Au plus 7 éléments, quel que soit le nombre de pages.
 * Exemple : page 6 sur 12 → [1, '…', 5, 6, 7, '…', 12].
 */
export function pagesVisibles(page, pages) {
  if (pages <= 7) return Array.from({ length: pages }, (_, i) => i + 1)
  // Près d'un bord, on montre 5 pages d'un côté pour garder 7 éléments.
  const de = page <= 4 ? 2 : page >= pages - 3 ? pages - 4 : page - 1
  const a = page <= 4 ? 5 : page >= pages - 3 ? pages - 1 : page + 1
  const milieu = Array.from({ length: a - de + 1 }, (_, i) => de + i)
  return [1, ...(de > 2 ? ['…'] : []), ...milieu, ...(a < pages - 1 ? ['…'] : []), pages]
}

/**
 * L'état d'une page paginée par le serveur (seules ses lignes ont été lues), au même format que paginer().
 * total : le nombre de lignes du filtre ; nombre : les lignes reçues pour cette page.
 */
export function etatPage(total, page, taille, nombre) {
  const pages = Math.max(1, Math.ceil(total / taille))
  const debut = nombre ? (page - 1) * taille + 1 : 0
  return { page, pages, total, debut, fin: nombre ? debut + nombre - 1 : 0 }
}
