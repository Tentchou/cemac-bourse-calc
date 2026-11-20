import { createContext, useContext, useEffect, useState } from 'react'

// Routeur minimal (trois pages publiques) : l'historique du navigateur, sans dépendance.

const Contexte = createContext(null)

/** Reconnaît le chemin : « / », « /titres/{id} », « /methodes ». */
export function analyserChemin(chemin) {
  const fiche = /^\/titres\/([0-9a-f-]{36})\/?$/i.exec(chemin)
  if (fiche) return { page: 'fiche', id: fiche[1].toLowerCase() }
  if (/^\/methodes\/?$/.test(chemin)) return { page: 'methodes' }
  if (import.meta.env.DEV && /^\/composants\/?$/.test(chemin)) return { page: 'vitrine' }
  if (chemin === '/' || chemin === '') return { page: 'accueil' }
  return { page: 'introuvable' }
}

export function RouteurProvider({ children }) {
  const [emplacement, setEmplacement] = useState(() => ({
    chemin: window.location.pathname,
    recherche: window.location.search,
  }))

  useEffect(() => {
    const suivre = () => setEmplacement({ chemin: window.location.pathname, recherche: window.location.search })
    window.addEventListener('popstate', suivre)
    return () => window.removeEventListener('popstate', suivre)
  }, [])

  function naviguer(url, { remplacer = false } = {}) {
    if (remplacer) window.history.replaceState(null, '', url)
    else window.history.pushState(null, '', url)
    const lien = new URL(url, window.location.origin)
    setEmplacement({ chemin: lien.pathname, recherche: lien.search })
    if (!remplacer) window.scrollTo({ top: 0 })
  }

  const route = analyserChemin(emplacement.chemin)
  const parametres = new URLSearchParams(emplacement.recherche)

  return <Contexte.Provider value={{ route, parametres, naviguer, chemin: emplacement.chemin }}>{children}</Contexte.Provider>
}

export function useRoute() {
  const valeur = useContext(Contexte)
  if (!valeur) throw new Error('useRoute doit être utilisé dans RouteurProvider.')
  return valeur
}

/** Lien interne : un vrai <a href> (ouvrable dans un nouvel onglet), sans rechargement de page. */
export function Lien({ vers, children, ...autres }) {
  const { naviguer } = useRoute()
  function cliquer(evenement) {
    if (evenement.defaultPrevented || evenement.button !== 0) return
    if (evenement.metaKey || evenement.ctrlKey || evenement.shiftKey || evenement.altKey) return
    evenement.preventDefault()
    autres.onClick?.(evenement)
    naviguer(vers)
  }
  return (
    <a href={vers} {...autres} onClick={cliquer}>
      {children}
    </a>
  )
}
