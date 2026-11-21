import { useCallback, useEffect, useRef, useState } from 'react'
import { listerPage } from '../api/admin'
import { lireTaille } from '../components/ui/pagination/paginer'

// La liste paginée d'une rubrique, lue page par page sur l'API.
//  - la recherche part 300 ms après la dernière frappe (pas une requête par lettre) ;
//  - une requête en cours est annulée dès qu'une autre la remplace (jamais d'ancienne page affichée) ;
//  - tout changement de filtre ou de taille ramène à la page 1 ;
//  - une page devenue trop loin (lignes supprimées, filtre) est ramenée à la dernière page.

const DELAI_RECHERCHE = 300
const CLE_TAILLE = 'cbc-admin-lignes'

function tailleMemorisee() {
  try {
    return lireTaille(localStorage.getItem(CLE_TAILLE))
  } catch {
    return lireTaille(null)
  }
}

export default function useListeAdmin(route, { onSessionExpiree } = {}) {
  const [page, setPage] = useState(1)
  const [taille, setTailleEtat] = useState(tailleMemorisee)
  const [recherche, setRechercheEtat] = useState('')
  const [rechercheEnvoyee, setRechercheEnvoyee] = useState('')
  const [filtres, setFiltres] = useState({})
  const [version, setVersion] = useState(0)
  // La réponse reçue et la demande à laquelle elle répond : la page « charge » tant que la demande
  // en cours n'a pas reçu sa réponse (les lignes précédentes restent affichées, estompées).
  const [etat, setEtat] = useState({ demande: null, donnees: null, erreur: null })
  const expiree = useRef(onSessionExpiree)
  useEffect(() => {
    expiree.current = onSessionExpiree
  }, [onSessionExpiree])

  const demande = JSON.stringify([route, page, taille, rechercheEnvoyee, filtres, version])

  // La recherche n'est envoyée qu'une fois la frappe arrêtée.
  useEffect(() => {
    const minuteur = setTimeout(() => {
      setRechercheEnvoyee(recherche.trim())
      setPage(1)
    }, DELAI_RECHERCHE)
    return () => clearTimeout(minuteur)
  }, [recherche])

  useEffect(() => {
    const controleur = new AbortController()
    listerPage(route, { page, taille, recherche: rechercheEnvoyee, ...filtres }, controleur.signal)
      .then((donnees) => {
        const pages = Math.max(1, Math.ceil(donnees.total / taille))
        if (page > pages) {
          setPage(pages)
          return
        }
        setEtat({ demande, donnees, erreur: null })
      })
      .catch((erreur) => {
        if (controleur.signal.aborted) return
        if (erreur.statut === 401) expiree.current?.()
        setEtat((e) => ({ ...e, demande, erreur }))
      })
    return () => controleur.abort()
  }, [route, page, taille, rechercheEnvoyee, filtres, version, demande])

  const setTaille = useCallback((n) => {
    setTailleEtat(n)
    setPage(1)
    try {
      localStorage.setItem(CLE_TAILLE, String(n))
    } catch {
      /* stockage indisponible : le choix vaut pour la visite */
    }
  }, [])

  const setFiltre = useCallback((cle, valeur) => {
    setFiltres((f) => ({ ...f, [cle]: valeur || undefined }))
    setPage(1)
  }, [])

  const effacer = useCallback(() => {
    setRechercheEtat('')
    setRechercheEnvoyee('')
    setFiltres({})
    setPage(1)
  }, [])

  const recharger = useCallback(() => setVersion((v) => v + 1), [])

  const filtre = recherche.trim() !== '' || Object.values(filtres).some(Boolean)

  return {
    donnees: etat.donnees,
    erreur: etat.demande === demande ? etat.erreur : null,
    chargement: etat.demande !== demande,
    page,
    taille,
    recherche,
    filtres,
    filtre,
    setPage,
    setTaille,
    setRecherche: setRechercheEtat,
    setFiltre,
    effacer,
    recharger,
  }
}
