// L'aperçu d'un bulletin de la cote lu par l'API : chiffres du résumé, filtres du tableau, lignes à envoyer.
// Fonctions pures, testées.

export const FILTRES_BULLETIN = ['tout', 'ecarts', 'problemes', 'transactions']

const aUnEcart = (l) => Boolean(l.controle && !l.controle.identique)
const aUnProbleme = (l) => Boolean(l.probleme)
const aDesTransactions = (l) => (l.volume ?? 0) > 0

/** Les chiffres du résumé. */
export function resumerBulletin(analyse) {
  const lignes = analyse?.lignes ?? []
  const controlees = lignes.filter((l) => l.controle)
  return {
    lus: lignes.length,
    controles: controlees.length,
    identiques: controlees.filter((l) => l.controle.identique).length,
    ecarts: lignes.filter(aUnEcart).length,
    problemes: lignes.filter(aUnProbleme).length,
    transactions: lignes.filter(aDesTransactions).length,
  }
}

/** Les lignes affichées pour un filtre (tout, ecarts, problemes, transactions). */
export function filtrerBulletin(lignes, filtre) {
  switch (filtre) {
    case 'ecarts':
      return lignes.filter(aUnEcart)
    case 'problemes':
      return lignes.filter(aUnProbleme)
    case 'transactions':
      return lignes.filter(aDesTransactions)
    default:
      return lignes
  }
}

/**
 * Les commandes de l'import en masse des cours (la même route que le fichier CSV), seulement si le bulletin
 * est importable : sinon null. Le cours de référence n'est pas lu dans le bulletin.
 */
export function commandesCours(analyse) {
  if (!analyse?.importable) return null
  return analyse.lignes.map((l) => ({
    titreId: l.titreId,
    dateSeance: analyse.dateSeance,
    coursReference: null,
    coursCloture: l.coursCloture,
    volume: l.volume,
    couponCouruPublie: l.couponCouruPublie,
  }))
}
