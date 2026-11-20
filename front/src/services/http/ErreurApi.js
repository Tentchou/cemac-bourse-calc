// Une seule forme d'erreur pour toute l'application, quelle que soit la panne.
// Fonctions pures (testées dans ErreurApi.test.js) : aucune dépendance à Axios ici.

/** Les familles d'erreurs que l'écran sait expliquer. */
export const TYPES_ERREUR = Object.freeze({
  annulee: 'annulee', // requête dépassée (l'écran a changé) : jamais affichée
  reseau: 'reseau', // pas de réponse du serveur
  delai: 'delai', // le serveur a mis trop de temps
  refus: 'refus', // 400 / 422 : la saisie ou le calcul est refusé, message de l'API
  introuvable: 'introuvable', // 404
  limite: 'limite', // 429 : trop de requêtes
  serveur: 'serveur', // 5xx ou réponse inattendue
})

/** Clé de texte traduite par type (le message de l'API passe avant, s'il existe). */
const CLES = {
  reseau: 'erreur.reseau',
  delai: 'erreur.delai',
  refus: 'erreur.refus',
  introuvable: 'erreur.introuvable',
  limite: 'erreur.limite',
  serveur: 'erreur.serveur',
}

export class ErreurApi extends Error {
  /**
   * @param {object} p
   * @param {string} p.type une valeur de TYPES_ERREUR
   * @param {number} [p.statut] code HTTP (0 sans réponse)
   * @param {string|null} [p.message] message lisible venu de l'API (« title » des problem details)
   * @param {Record<string,string[]>|null} [p.details] erreurs par champ (400 de validation)
   */
  constructor({ type, statut = 0, message = null, details = null }) {
    super(message ?? type)
    this.name = 'ErreurApi'
    this.type = type
    this.statut = statut
    this.cle = message ? null : (CLES[type] ?? 'erreur.serveur')
    this.message = message ?? ''
    this.details = details
  }

  get estAnnulee() {
    return this.type === TYPES_ERREUR.annulee
  }
}

/** Une erreur peut-elle être réessayée sans risque (panne passagère) ? */
export function estPassagere(erreur) {
  return erreur.type === TYPES_ERREUR.reseau || (erreur.type === TYPES_ERREUR.serveur && [502, 503, 504].includes(erreur.statut))
}

/**
 * Transforme n'importe quelle erreur (Axios ou autre) en ErreurApi.
 * Ne lit que la forme de l'objet : { code, name, response: { status, data } }.
 */
export function normaliserErreur(erreur) {
  if (erreur instanceof ErreurApi) return erreur
  if (erreur?.code === 'ERR_CANCELED' || erreur?.name === 'CanceledError' || erreur?.name === 'AbortError') {
    return new ErreurApi({ type: TYPES_ERREUR.annulee })
  }

  const reponse = erreur?.response
  if (!reponse) {
    const delai = erreur?.code === 'ECONNABORTED' || erreur?.code === 'ETIMEDOUT'
    return new ErreurApi({ type: delai ? TYPES_ERREUR.delai : TYPES_ERREUR.reseau })
  }

  const statut = reponse.status
  const probleme = reponse.data && typeof reponse.data === 'object' ? reponse.data : null
  const titre = typeof probleme?.title === 'string' && probleme.title.trim() ? probleme.title.trim() : null

  if (statut === 400 || statut === 422) {
    return new ErreurApi({ type: TYPES_ERREUR.refus, statut, message: titre, details: probleme?.errors ?? null })
  }
  if (statut === 404) return new ErreurApi({ type: TYPES_ERREUR.introuvable, statut })
  if (statut === 429) return new ErreurApi({ type: TYPES_ERREUR.limite, statut })
  return new ErreurApi({ type: TYPES_ERREUR.serveur, statut })
}
