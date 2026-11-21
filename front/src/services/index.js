// Point d'entrée de la couche « services » : les écrans n'importent que d'ici.
//
//   services/
//     http/client.js           client Axios unique (baseURL /api, délai 10 s, paramètres vides retirés)
//     http/intercepteurs.js    langue, suivi du chargement, 1 nouvel essai pour une lecture en panne passagère
//     http/ErreurApi.js        une seule forme d'erreur (type, statut, clé de texte ou message de l'API)
//     http/suiviChargement.js  nombre de requêtes en cours (barre de progression)
//     titresService.js         liste, fiche et historique des cours des titres
//     calculsService.js        coupon couru, rendements, échéancier, dividende
//     useRequete.js            état d'une requête dans un écran (chargement, erreur, recharger, annulation)

export { calculsService } from './calculsService'
export { ErreurApi, TYPES_ERREUR } from './http/ErreurApi'
export { definirLangue } from './http/intercepteurs'
export { suiviChargement } from './http/suiviChargement'
export { titresService } from './titresService'
export { useRequete } from './useRequete'
