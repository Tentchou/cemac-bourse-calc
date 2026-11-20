import { ChevronRight, Menu, X } from 'lucide-react'
import { useEffect, useRef, useState } from 'react'
import { usePreferences } from '../../contexte/Preferences'
import { Lien, useRoute } from '../../routeur/Routeur'
import { Logo } from '../icones/Icones'
import { LIENS } from './liens'
import Reglages from './Reglages'

/**
 * En-tête collant. Grand écran (≥ 768 px) : logo, liens, langue et thème sur une ligne.
 * Mobile : logo et bouton menu seulement ; le menu glisse sous l'en-tête avec les liens
 * puis les réglages. Il se ferme avec Échap (le focus revient au bouton), par un clic
 * en dehors, ou en changeant de page.
 */
export default function EnTete() {
  const { t } = usePreferences()
  const { route, chemin } = useRoute()
  const [menuOuvert, setMenuOuvert] = useState(false)
  const [cheminVu, setCheminVu] = useState(chemin)
  const entete = useRef(null)
  const bouton = useRef(null)

  // Changement de page (lien, retour arrière) : le menu se referme.
  if (chemin !== cheminVu) {
    setCheminVu(chemin)
    setMenuOuvert(false)
  }

  useEffect(() => {
    if (!menuOuvert) return undefined
    const echap = (e) => {
      if (e.key !== 'Escape') return
      setMenuOuvert(false)
      bouton.current?.focus()
    }
    const dehors = (e) => {
      if (!entete.current?.contains(e.target)) setMenuOuvert(false)
    }
    document.addEventListener('keydown', echap)
    document.addEventListener('pointerdown', dehors)
    return () => {
      document.removeEventListener('keydown', echap)
      document.removeEventListener('pointerdown', dehors)
    }
  }, [menuOuvert])

  const lienClasses = (actif) =>
    `inline-flex h-10 items-center gap-2 rounded-full px-4 text-sm font-medium transition-colors duration-150 ease-douce focus-visible:ring-2 focus-visible:ring-action focus-visible:outline-none ${
      actif ? 'bg-action/10 text-action' : 'text-texte-2 hover:bg-voile hover:text-texte'
    }`

  return (
    <header ref={entete} className="sticky top-0 z-30 border-b border-bordure/70 bg-fond/85 backdrop-blur-xl">
      <div className="mx-auto flex h-16 max-w-6xl items-center gap-3 px-4 sm:px-6">
        <Lien
          vers="/"
          className="flex min-w-0 items-center gap-2.5 rounded-full font-titre text-[0.9375rem] font-bold tracking-tight focus-visible:ring-2 focus-visible:ring-action focus-visible:outline-none"
        >
          <Logo className="size-9 shrink-0" />
          <span className="truncate">{t('site.nom')}</span>
        </Lien>

        <nav aria-label={t('nav.principale')} className="ml-4 hidden items-center gap-1 md:flex">
          {LIENS.map(({ vers, cle, icone: Icone, pages }) => {
            const actif = pages.includes(route.page)
            return (
              <Lien key={vers} vers={vers} aria-current={actif ? 'page' : undefined} className={lienClasses(actif)}>
                <Icone aria-hidden="true" className="size-4" strokeWidth={1.75} />
                {t(cle)}
              </Lien>
            )
          })}
        </nav>

        <div className="ml-auto hidden md:block">
          <Reglages />
        </div>

        <button
          ref={bouton}
          type="button"
          onClick={() => setMenuOuvert((o) => !o)}
          aria-expanded={menuOuvert}
          aria-controls="menu-mobile"
          aria-label={menuOuvert ? t('nav.fermer') : t('nav.menu')}
          className={`ml-auto inline-grid size-10 shrink-0 place-items-center rounded-full border transition-colors duration-150 focus-visible:ring-2 focus-visible:ring-action focus-visible:outline-none md:hidden ${
            menuOuvert ? 'border-texte bg-texte text-fond' : 'border-bordure bg-surface text-texte hover:bg-voile'
          }`}
        >
          {menuOuvert ? <X className="size-5" strokeWidth={1.75} /> : <Menu className="size-5" strokeWidth={1.75} />}
        </button>
      </div>

      {/* Menu mobile : glisse sous l'en-tête. Inerte une fois fermé (ni focus, ni lecteur d'écran). */}
      <div id="menu-mobile" className="depliable md:hidden" data-ouvert={menuOuvert} inert={!menuOuvert || undefined}>
        <div>
          <div className="mx-auto max-w-6xl px-4 pb-4">
            <nav aria-label={t('nav.principale')} className="flex flex-col gap-1 border-t border-bordure/70 pt-3">
              {LIENS.map(({ vers, cle, icone: Icone, pages }) => {
                const actif = pages.includes(route.page)
                return (
                  <Lien key={vers} vers={vers} aria-current={actif ? 'page' : undefined} className={`${lienClasses(actif)} h-12`} onClick={() => setMenuOuvert(false)}>
                    <Icone aria-hidden="true" className="size-5" strokeWidth={1.75} />
                    <span className="flex-1">{t(cle)}</span>
                    <ChevronRight aria-hidden="true" className="size-4 opacity-60" strokeWidth={1.75} />
                  </Lien>
                )
              })}
            </nav>
            <Reglages lignes className="mt-3 border-t border-bordure/70 pt-3" />
          </div>
        </div>
      </div>
    </header>
  )
}
