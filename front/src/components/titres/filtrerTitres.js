// Filtre de la liste publique : texte libre (nom, émetteur, ISIN), type et pays. Fonction pure, testée.

const sansAccents = (s) =>
  String(s ?? '')
    .normalize('NFD')
    .replace(/\p{Diacritic}/gu, '')
    .toLowerCase()

export function filtrerTitres(titres, { texte = '', type = 'tous', pays = null } = {}) {
  const mots = sansAccents(texte).split(/\s+/).filter(Boolean)
  return titres.filter((titre) => {
    if (type !== 'tous' && titre.typeTitre !== type) return false
    if (pays && titre.pays !== pays) return false
    const cible = sansAccents(`${titre.nom} ${titre.emetteur} ${titre.isin}`)
    return mots.every((mot) => cible.includes(mot))
  })
}

/** Les chiffres clés de l'accueil. */
export function compterTitres(titres) {
  return {
    obligations: titres.filter((t) => t.typeTitre === 'Obligation').length,
    actions: titres.filter((t) => t.typeTitre === 'Action').length,
    pays: [...new Set(titres.map((t) => t.pays))].sort(),
  }
}
