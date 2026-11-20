// Appels à l'API de l'administration. La session voyage dans un cookie HttpOnly (illisible ici) ;
// chaque écriture porte l'en-tête X-CBC-Admin exigé par l'API contre les requêtes forgées.

const EN_TETES = { 'Content-Type': 'application/json', 'X-CBC-Admin': '1' }

export class ErreurApi extends Error {
  constructor(message, statut, details = null) {
    super(message)
    this.statut = statut
    this.details = details
  }
}

async function messageErreur(reponse) {
  if (reponse.status === 429) return 'Trop de requêtes : réessayez dans une minute.'
  if (reponse.status === 401) return 'Votre session a expiré : reconnectez-vous.'
  if (reponse.status === 403) return 'Votre compte n’a pas le droit de faire cette action.'
  try {
    const probleme = await reponse.json()
    const champs = probleme.errors ? Object.values(probleme.errors).flat() : []
    return [probleme.title, ...champs].filter(Boolean).join(' ') || 'La demande a échoué.'
  } catch {
    return 'Le service est indisponible pour le moment.'
  }
}

async function appeler(chemin, { methode = 'GET', corps } = {}) {
  const reponse = await fetch(`/api/admin/${chemin}`, {
    method: methode,
    credentials: 'same-origin',
    headers: methode === 'GET' ? undefined : EN_TETES,
    body: corps === undefined ? undefined : JSON.stringify(corps),
  })
  if (reponse.status === 204) return null
  if (reponse.ok) return reponse.json()
  throw new ErreurApi(await messageErreur(reponse), reponse.status)
}

export const ouvrirSession = (email, motDePasse) => appeler('session', { methode: 'POST', corps: { email, motDePasse } })
export const fermerSession = () => appeler('session', { methode: 'DELETE' })
export const sessionCourante = () => appeler('session')
export const listerEmetteurs = () => appeler('emetteurs')

/**
 * Envoi en masse, tout ou rien. Rend le bilan de l'API :
 * { message, bilan: { lignes, enregistrees, erreurs: [{ ligne, messages }] } }.
 * Une réponse 422 (des lignes à corriger, rien d'enregistré) est un bilan, pas une exception.
 */
export async function envoyerEnMasse(route, lignes) {
  const reponse = await fetch(`/api/admin/${route}/masse`, {
    method: 'POST',
    credentials: 'same-origin',
    headers: EN_TETES,
    body: JSON.stringify({ lignes }),
  })
  if (reponse.ok || reponse.status === 422) return reponse.json()
  throw new ErreurApi(await messageErreur(reponse), reponse.status)
}
