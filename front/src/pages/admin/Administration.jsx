import { useEffect, useState } from 'react'
import { fermerSession, sessionCourante } from '../../api/admin'
import Connexion from './Connexion'
import ImportMasse from './ImportMasse'

/** L'espace d'administration : la connexion d'abord, puis l'import. La session est vérifiée par l'API. */
export default function Administration() {
  const [compte, setCompte] = useState(undefined) // undefined : vérification en cours ; null : non connecté

  function verifier() {
    sessionCourante().then(setCompte).catch(() => setCompte(null))
  }

  useEffect(verifier, [])

  async function deconnecter() {
    try {
      await fermerSession()
    } finally {
      setCompte(null)
    }
  }

  if (compte === undefined) return <p role="status">Vérification de la session…</p>
  if (compte === null) return <Connexion onConnecte={verifier} />

  return (
    <>
      <div className="barre-compte">
        <span>{compte.nom} · {compte.role}</span>
        <button type="button" className="bouton" onClick={deconnecter}>Se déconnecter</button>
      </div>
      <ImportMasse onSessionExpiree={() => setCompte(null)} />
    </>
  )
}
