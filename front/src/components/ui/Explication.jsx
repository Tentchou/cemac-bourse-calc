import { ChevronDown } from 'lucide-react'
import { useId, useState } from 'react'
import { usePreferences } from '../../contexte/Preferences'

/**
 * « Comment c'est calculé » : un lien-bouton qui déplie la formule (hauteur et opacité animées).
 * Le contenu replié est rendu inerte : ni focus, ni lecture d'écran tant qu'il est fermé.
 */
export default function Explication({ children }) {
  const { t } = usePreferences()
  const [ouvert, setOuvert] = useState(false)
  const id = useId()
  return (
    <div className="mt-6 border-t border-bordure pt-4">
      <button
        type="button"
        aria-expanded={ouvert}
        aria-controls={id}
        onClick={() => setOuvert((o) => !o)}
        className="group inline-flex items-center gap-1.5 rounded-full text-sm font-medium text-action transition-colors duration-150 hover:text-texte"
      >
        {t('expl.voir')}
        <ChevronDown aria-hidden="true" strokeWidth={2} className={`size-4 transition-transform duration-250 ease-douce ${ouvert ? 'rotate-180' : ''}`} />
      </button>
      <div id={id} className="depliable" data-ouvert={ouvert} inert={!ouvert || undefined}>
        <div>
          <div className="chiffres mt-4 overflow-x-auto rounded-2xl bg-fond px-4 py-3 text-sm leading-7 text-texte">{children}</div>
        </div>
      </div>
    </div>
  )
}
