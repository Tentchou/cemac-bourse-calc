import { useEffect, useState, useSyncExternalStore } from 'react'
import { usePreferences } from '../../contexte/Preferences'
import { suiviChargement } from '../../services'

const DELAI_AVANT_AFFICHAGE = 200

/**
 * Fine barre bleue tout en haut de l'écran tant qu'une requête est en cours (intercepteurs Axios).
 * Elle n'apparaît qu'au-delà de 200 ms : une réponse rapide ne fait rien clignoter.
 * Les skeletons restent l'indicateur principal ; la barre dit « ça travaille » partout à la fois.
 */
export default function BarreChargement() {
  const { t } = usePreferences()
  const enCours = useSyncExternalStore(suiviChargement.abonner, suiviChargement.lire)
  const [visible, setVisible] = useState(false)

  useEffect(() => {
    if (!enCours) {
      const fin = setTimeout(() => setVisible(false), 0)
      return () => clearTimeout(fin)
    }
    const debut = setTimeout(() => setVisible(true), DELAI_AVANT_AFFICHAGE)
    return () => clearTimeout(debut)
  }, [enCours])

  return (
    <div
      role="progressbar"
      aria-label={t('chargement.global')}
      aria-hidden={!visible}
      className={`pointer-events-none fixed inset-x-0 top-0 z-[60] h-0.5 overflow-hidden transition-opacity duration-250 ${visible ? 'opacity-100' : 'opacity-0'}`}
    >
      <div className="progresse h-full w-2/5 rounded-full bg-action" />
    </div>
  )
}
