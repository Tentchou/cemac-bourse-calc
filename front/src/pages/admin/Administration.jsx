import { useCallback, useEffect, useState } from 'react'
import { fermerSession, sessionCourante } from '../../api/admin'
import CadreAdmin from '../../components/admin/CadreAdmin'
import { usePreferences } from '../../contexte/Preferences'
import Connexion from './Connexion'

/** L'espace d'administration : la connexion d'abord, puis les rubriques. La session est vérifiée par l'API. */
export default function Administration() {
  const { t } = usePreferences()
  const [compte, setCompte] = useState(undefined) // undefined : vérification en cours ; null : non connecté

  const verifier = useCallback(() => {
    sessionCourante()
      .then(setCompte)
      .catch(() => setCompte(null))
  }, [])

  useEffect(verifier, [verifier])

  const sessionExpiree = useCallback(() => setCompte(null), [])

  async function deconnecter() {
    try {
      await fermerSession()
    } finally {
      setCompte(null)
    }
  }

  if (compte === undefined)
    return (
      <p role="status" className="grid min-h-dvh place-items-center bg-fond text-sm text-texte-2">
        {t('admin.verification')}
      </p>
    )
  if (compte === null) return <Connexion onConnecte={verifier} />

  return <CadreAdmin compte={compte} onDeconnecter={deconnecter} onSessionExpiree={sessionExpiree} />
}
