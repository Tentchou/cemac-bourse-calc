import { useCallback, useEffect, useRef, useState } from 'react'
import { normaliserErreur } from './http/ErreurApi'

/**
 * Lance une requête de service et suit son état : { donnees, erreur, chargement, recharger }.
 *
 *  - `charger(signal)` est rappelé quand `dependances` changent ; la requête dépassée est annulée
 *    (AbortController) : une réponse tardive ne peut jamais écraser la nouvelle ;
 *  - pendant un nouveau chargement, `donnees` garde l'ancien résultat (l'écran l'estompe) ;
 *  - `erreur` est toujours une ErreurApi, jamais une annulation ;
 *  - `charger` peut rendre null : rien à demander, pas de requête.
 */
export function useRequete(charger, dependances) {
  const [etat, setEtat] = useState({ donnees: null, erreur: null, chargement: true })
  const [tour, setTour] = useState(0)
  const chargerRef = useRef(charger)
  chargerRef.current = charger

  useEffect(() => {
    const controle = new AbortController()
    const promesse = chargerRef.current(controle.signal)
    if (!promesse) {
      setEtat({ donnees: null, erreur: null, chargement: false })
      return undefined
    }
    setEtat((e) => ({ ...e, erreur: null, chargement: true }))
    promesse
      .then((donnees) => {
        if (!controle.signal.aborted) setEtat({ donnees, erreur: null, chargement: false })
      })
      .catch((brute) => {
        const erreur = normaliserErreur(brute)
        if (!erreur.estAnnulee && !controle.signal.aborted) setEtat({ donnees: null, erreur, chargement: false })
      })
    return () => controle.abort()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [...dependances, tour])

  const recharger = useCallback(() => setTour((n) => n + 1), [])
  return { ...etat, recharger }
}
