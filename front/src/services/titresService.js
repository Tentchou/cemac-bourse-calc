import { lire } from './http/client'

/** Les titres cotés (lecture publique). */
export const titresService = {
  /** Tous les titres, avec leur dernier cours. */
  lister: (signal) => lire('titres', { signal }),

  /** La fiche complète d'un titre : caractéristiques, dernier cours, dividendes pour une action. */
  obtenir: (id, signal) => lire(`titres/${encodeURIComponent(id)}`, { signal }),

  /** L'historique des cours (environ cinq ans de séances), de la plus ancienne à la plus récente. */
  historique: (id, signal) => lire(`titres/${encodeURIComponent(id)}/cours`, { signal }),
}
