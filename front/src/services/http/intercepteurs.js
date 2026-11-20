import { estPassagere, normaliserErreur } from './ErreurApi'
import { suiviChargement } from './suiviChargement'

const PAUSE_AVANT_NOUVEL_ESSAI = 600

let langueCourante = 'fr'

/** Appelé par les préférences : chaque requête annonce la langue du visiteur. */
export function definirLangue(langue) {
  langueCourante = langue === 'en' ? 'en' : 'fr'
}

/** Attend `ms`, sauf si la requête est annulée entre-temps. */
function patienter(ms, signal) {
  return new Promise((resoudre, rejeter) => {
    const annuler = () => rejeter(new DOMException('Annulée', 'AbortError'))
    if (signal?.aborted) return annuler()
    const minuterie = setTimeout(resoudre, ms)
    signal?.addEventListener(
      'abort',
      () => {
        clearTimeout(minuterie)
        annuler()
      },
      { once: true },
    )
  })
}

/**
 * Branche les intercepteurs sur un client Axios :
 *  - requête : langue du visiteur (Accept-Language), début du suivi de chargement ;
 *  - réponse : fin du suivi ; une seule nouvelle tentative pour une LECTURE en panne passagère
 *    (réseau coupé, 502/503/504), après 600 ms ; toute erreur devient une ErreurApi.
 * `silencieux: true` dans la config d'une requête la cache de la barre de progression.
 */
export function brancherIntercepteurs(client) {
  client.interceptors.request.use((config) => {
    config.headers.set('Accept-Language', langueCourante)
    if (!config.silencieux) suiviChargement.debut()
    return config
  })

  client.interceptors.response.use(
    (reponse) => {
      if (!reponse.config.silencieux) suiviChargement.fin()
      return reponse
    },
    async (erreurBrute) => {
      const config = erreurBrute.config
      if (config && !config.silencieux) suiviChargement.fin()
      const erreur = normaliserErreur(erreurBrute)

      const lecture = config && (config.method ?? 'get').toLowerCase() === 'get'
      if (lecture && !config.dejaReessaye && estPassagere(erreur)) {
        config.dejaReessaye = true
        try {
          await patienter(PAUSE_AVANT_NOUVEL_ESSAI, config.signal)
        } catch (annulation) {
          throw normaliserErreur(annulation)
        }
        return client.request(config)
      }
      throw erreur
    },
  )
  return client
}
