import { ArrowUp } from 'lucide-react'
import { usePreferences } from '../../contexte/Preferences'
import { Lien, useRoute } from '../../routeur/Routeur'
import { Logo } from '../icones/Icones'
import LienInstitution from '../ui/LienInstitution'
import { LIENS } from './liens'

const ANNEE = new Date().getFullYear()

/** Titre d'une colonne du pied. */
function TitreColonne({ children }) {
  return <h2 className="mb-4 text-xs font-semibold tracking-wide text-texte uppercase">{children}</h2>
}

/**
 * Pied de page en 3 colonnes (empilées sur mobile) : la marque et sa mention, les pages du site,
 * les sources officielles. Dessous : © et « Haut de page ».
 */
export default function PiedDePage() {
  const { t } = usePreferences()
  const { route } = useRoute()

  function remonter() {
    const doux = window.matchMedia?.('(prefers-reduced-motion: reduce)').matches ? 'auto' : 'smooth'
    window.scrollTo({ top: 0, behavior: doux })
    // Le focus repart du début de la page, comme au chargement.
    document.getElementById('contenu')?.focus({ preventScroll: true })
  }

  return (
    <footer className="mt-24 border-t border-bordure/70 bg-surface/40">
      <div className="mx-auto grid max-w-6xl gap-10 px-4 pt-12 pb-8 text-sm text-texte-2 sm:grid-cols-2 sm:px-6 lg:grid-cols-[minmax(0,2fr)_minmax(0,1fr)_minmax(0,1fr)]">
        <div className="flex flex-col gap-4 sm:col-span-2 lg:col-span-1">
          <Lien vers="/" className="flex w-fit items-center gap-2.5 rounded-full font-titre text-[0.9375rem] font-bold text-texte focus-visible:ring-2 focus-visible:ring-action focus-visible:outline-none">
            <Logo className="size-8 shrink-0" />
            {t('site.nom')}
          </Lien>
          <p className="max-w-sm leading-6">
            {t('pied.texte')} {t('pied.conseil')}
          </p>
        </div>

        <nav aria-label={t('pied.site')}>
          <TitreColonne>{t('pied.site')}</TitreColonne>
          <ul className="flex flex-col gap-1">
            {LIENS.map(({ vers, cle, icone: Icone, pages }) => {
              const actif = pages.includes(route.page)
              return (
                <li key={vers}>
                  <Lien
                    vers={vers}
                    aria-current={actif ? 'page' : undefined}
                    className={`-ml-2 inline-flex h-9 items-center gap-2 rounded-full px-2 transition-colors duration-150 hover:text-texte focus-visible:ring-2 focus-visible:ring-action focus-visible:outline-none ${actif ? 'font-medium text-action' : ''}`}
                  >
                    <Icone aria-hidden="true" className="size-4" strokeWidth={1.75} />
                    {t(cle)}
                  </Lien>
                </li>
              )
            })}
          </ul>
        </nav>

        <div>
          <TitreColonne>{t('pied.sources')}</TitreColonne>
          <ul className="flex flex-col items-start gap-2">
            <li>
              <LienInstitution code="bvmac" role />
            </li>
            <li>
              <LienInstitution code="cosumaf" role />
            </li>
          </ul>
        </div>
      </div>

      <div className="mx-auto flex max-w-6xl items-center justify-between gap-4 border-t border-bordure/70 px-4 py-5 text-xs text-texte-2 sm:px-6">
        <span className="chiffres">
          © {ANNEE} {t('site.nom')}
        </span>
        <button
          type="button"
          onClick={remonter}
          className="inline-flex h-9 items-center gap-1.5 rounded-full border border-bordure bg-surface px-3.5 font-medium text-texte-2 transition-colors duration-150 hover:border-bordure-champ hover:text-texte focus-visible:ring-2 focus-visible:ring-action focus-visible:outline-none"
        >
          <ArrowUp aria-hidden="true" className="size-3.5" strokeWidth={2} />
          {t('pied.haut')}
        </button>
      </div>
    </footer>
  )
}
