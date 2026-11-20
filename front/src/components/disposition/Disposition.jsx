import { Info } from 'lucide-react'
import { usePreferences } from '../../contexte/Preferences'
import BarreChargement from '../ui/BarreChargement'
import EnTete from './EnTete'
import PiedDePage from './PiedDePage'

/** Le cadre de toutes les pages publiques : en-tête, mention éducative, contenu, pied de page. */
export default function Disposition({ children }) {
  const { t } = usePreferences()
  return (
    <div className="flex min-h-dvh flex-col">
      <a href="#contenu" className="sr-only z-50 rounded-full bg-action px-4 py-2 text-sur-action focus:not-sr-only focus:fixed focus:top-3 focus:left-3">
        {t('nav.contenu')}
      </a>
      <BarreChargement />
      <EnTete />

      <p role="note" className="mx-auto mt-4 flex w-full max-w-6xl items-center gap-2.5 px-4 text-xs text-texte-2 sm:px-6 sm:text-sm">
        <span aria-hidden="true" className="inline-grid size-5 shrink-0 place-items-center rounded-full bg-orange text-[#14213D]">
          <Info className="size-3" strokeWidth={2.5} />
        </span>
        <span className="flex-1">{t('site.avertissement')}</span>
        {import.meta.env.MODE === 'exemple' && (
          <span className="shrink-0 rounded-full bordure-fine bg-surface px-2.5 py-0.5 font-medium text-orange-texte">{t('site.exemple')}</span>
        )}
      </p>

      {/* tabIndex -1 : le lien d'évitement et « Haut de page » peuvent y poser le focus. */}
      <main id="contenu" tabIndex={-1} className="flex-1 outline-none">
        {children}
      </main>

      <PiedDePage />
    </div>
  )
}
