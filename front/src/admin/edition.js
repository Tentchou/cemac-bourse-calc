// Modification d'un titre dans l'administration : préremplir le formulaire depuis la fiche de l'API,
// puis bâtir la commande de modification. Fonctions pures, testées dans edition.test.js.
// La saisie modifiée est relue par lireLigne, comme un ajout : mêmes formats, mêmes contrôles.

/** Les statuts d'un titre, dans l'ordre de leur vie. « Radie » : retiré du site public, historique gardé. */
export const STATUTS = ['Actif', 'Suspendu', 'Echu', 'Radie']

/** Les rubriques dont les lignes se modifient (titres seulement, pour l'instant). */
export const RUBRIQUES_MODIFIABLES = ['obligations', 'actions']

/**
 * Les valeurs du formulaire pour un titre lu par l'API : des textes, comme si l'administrateur les avait
 * tapés (6.5 → « 6,5 » en français), les dates en ISO (ChampDate), les choix par leur valeur.
 */
export function valeursFormulaire(modele, titre, langue) {
  const valeurs = { statut: titre.statut }
  for (const { champ, type } of modele.colonnes) {
    const v = titre[champ]
    if (v === null || v === undefined) valeurs[champ] = ''
    else if (typeof v === 'number') valeurs[champ] = type === 'nombre' && langue === 'fr' ? String(v).replace('.', ',') : String(v)
    else valeurs[champ] = String(v)
  }
  return valeurs
}

/**
 * La commande de modification : la ligne lue par lireLigne, sans l'ISIN (il ne change jamais),
 * avec le statut choisi. Null si le statut n'est pas l'un des statuts connus.
 */
export function commandeModification(commande, statut) {
  if (!STATUTS.includes(statut)) return null
  const { isin: _isin, ...reste } = commande
  return { ...reste, statut }
}

/** Le titre a-t-il un historique ? Si oui, « Supprimer » le radie au lieu de l'effacer. */
export function aUnHistorique(titre) {
  return (titre?.nombreCours ?? 0) + (titre?.nombreDividendes ?? 0) + (titre?.nombreDocuments ?? 0) > 0
}
