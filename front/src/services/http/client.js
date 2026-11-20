import axios from 'axios'
import { brancherIntercepteurs } from './intercepteurs'
import { parametresUtiles } from './parametres'

/** Délai maximal d'une requête : au-delà, l'écran explique que le service est lent. */
export const DELAI_REQUETE = 10_000

/**
 * Le client HTTP unique de l'application publique. Les services l'utilisent ; les écrans,
 * jamais directement. En développement, /api est relayé vers l'API par Vite (ou servi par
 * les données d'exemple en mode « exemple »).
 */
export const client = brancherIntercepteurs(
  axios.create({
    baseURL: '/api',
    timeout: DELAI_REQUETE,
    headers: { Accept: 'application/json' },
    paramsSerializer: { serialize: (p) => new URLSearchParams(parametresUtiles(p)).toString() },
  }),
)

/** GET qui rend directement le corps de la réponse. */
export async function lire(chemin, { parametres, signal } = {}) {
  const reponse = await client.get(chemin, { params: parametres, signal })
  return reponse.data
}
