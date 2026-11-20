import { lire } from './http/client'

const id = (valeur) => encodeURIComponent(valeur)

/**
 * Les calculs publics. Dates au format ISO « AAAA-MM-JJ » ; cours en % du nominal pour une
 * obligation, en FCFA pour une action.
 */
export const calculsService = {
  /** Prix pied de coupon, coupon couru et prix plein coupon. */
  couponCouru: (titreId, { dateNegociation, quantite, cours }, signal) =>
    lire(`obligations/${id(titreId)}/coupon-couru`, { parametres: { dateNegociation, quantite, cours }, signal }),

  /** Rendement courant et rendement actuariel, avec le détail des flux. */
  rendement: (titreId, { dateNegociation, cours }, signal) =>
    lire(`obligations/${id(titreId)}/rendement`, { parametres: { dateNegociation, cours }, signal }),

  /** Les échéances que l'acheteur percevra (ou l'échéancier complet, sans date). */
  echeancier: (titreId, { quantite, dateNegociation }, signal) =>
    lire(`obligations/${id(titreId)}/echeancier`, { parametres: { quantite, dateNegociation }, signal }),

  /** Rendement du dividende d'une action au cours donné. */
  rendementDividende: (titreId, { cours }, signal) =>
    lire(`actions/${id(titreId)}/rendement-dividende`, { parametres: { cours }, signal }),
}
