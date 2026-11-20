// Lecture d'un CSV. Excel en français enregistre souvent en « ; » et en Windows-1252 : les deux sont reconnus.
import Papa from 'papaparse'

/** UTF-8 (avec ou sans BOM) d'abord ; si les octets n'en sont pas, Windows-1252 (CSV d'Excel sous Windows). */
export function decoderTexte(octets) {
  try {
    return new TextDecoder('utf-8', { fatal: true }).decode(octets)
  } catch {
    return new TextDecoder('windows-1252').decode(octets)
  }
}

/** Le séparateur le plus présent dans la ligne des en-têtes : « ; » (Excel en français), « , » ou tabulation. */
export function separateur(texte) {
  const entetes = texte.split(/\r?\n/, 1)[0] ?? ''
  const compte = (c) => entetes.split(c).length - 1
  return [';', ',', '\t'].reduce((meilleur, c) => (compte(c) > compte(meilleur) ? c : meilleur), ';')
}

/** Les lignes blanches sont gardées pour conserver la numérotation du fichier. */
export function analyserCsv(texte) {
  const resultat = Papa.parse(texte, { delimiter: separateur(texte), skipEmptyLines: false })
  const lignes = resultat.data
  // Papa rend une dernière ligne vide quand le fichier se termine par un retour à la ligne.
  if (lignes.length && lignes.at(-1).length === 1 && lignes.at(-1)[0] === '') lignes.pop()
  return lignes
}
