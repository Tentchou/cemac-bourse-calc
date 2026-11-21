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
  return (await lireErreur(reponse)).message
}

/** Le message de l'API, et ses erreurs par champ (« TauxFacial » → [« … »]) quand elle en donne. */
async function lireErreur(reponse) {
  if (reponse.status === 429) return { message: 'Trop de requêtes : réessayez dans une minute.' }
  if (reponse.status === 401) return { message: 'Votre session a expiré : reconnectez-vous.' }
  if (reponse.status === 403) return { message: 'Votre compte n’a pas le droit de faire cette action.' }
  try {
    const probleme = await reponse.json()
    const champs = probleme.errors ? Object.values(probleme.errors).flat() : []
    return {
      message: [probleme.title, ...champs].filter(Boolean).join(' ') || 'La demande a échoué.',
      details: probleme.errors ?? null,
    }
  } catch {
    return { message: 'Le service est indisponible pour le moment.' }
  }
}

async function appeler(chemin, { methode = 'GET', corps, signal } = {}) {
  const reponse = await fetch(`/api/admin/${chemin}`, {
    method: methode,
    credentials: 'same-origin',
    headers: methode === 'GET' ? undefined : EN_TETES,
    body: corps === undefined ? undefined : JSON.stringify(corps),
    signal,
  })
  if (reponse.status === 204) return null
  if (reponse.ok) return reponse.json()
  const { message, details } = await lireErreur(reponse)
  throw new ErreurApi(message, reponse.status, details)
}

/** Les paramètres d'une liste en adresse : les valeurs vides ne sont pas envoyées. */
export function parametresListe(parametres) {
  const recherche = new URLSearchParams()
  for (const [cle, valeur] of Object.entries(parametres)) {
    if (valeur !== null && valeur !== undefined && String(valeur).trim() !== '') recherche.set(cle, String(valeur).trim())
  }
  return recherche.toString()
}

export const ouvrirSession = (email, motDePasse) => appeler('session', { methode: 'POST', corps: { email, motDePasse } })
export const fermerSession = () => appeler('session', { methode: 'DELETE' })
export const sessionCourante = () => appeler('session')
export const listerEmetteurs = () => appeler('emetteurs/choix')

/** Une page d'une rubrique : { lignes, total, page, taille }. */
export const listerPage = (route, parametres, signal) => appeler(`${route}?${parametresListe(parametres)}`, { signal })
export const lireCompteurs = (signal) => appeler('compteurs', { signal })
export const listerSeances = (signal) => appeler('cours/seances', { signal })

/** Création d'une ligne : { id, message }. Une saisie refusée lève une ErreurApi avec ses erreurs par champ. */
export const creer = (route, commande) => appeler(route, { methode: 'POST', corps: commande })

/**
 * Lecture d'un bulletin de la cote (PDF) par l'API : l'aperçu, sans rien enregistrer.
 * Le fichier part en multipart ; l'en-tête X-CBC-Admin est exigé comme pour toute écriture.
 */
export async function analyserBulletin(fichier) {
  const corps = new FormData()
  corps.append('fichier', fichier)
  const reponse = await fetch('/api/admin/bulletins/analyse', {
    method: 'POST',
    credentials: 'same-origin',
    headers: { 'X-CBC-Admin': '1' },
    body: corps,
  })
  if (reponse.ok) return reponse.json()
  const { message, details } = await lireErreur(reponse)
  throw new ErreurApi(message, reponse.status, details)
}

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
