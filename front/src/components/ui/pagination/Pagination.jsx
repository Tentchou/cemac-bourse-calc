import { ChevronLeft, ChevronRight } from 'lucide-react'
import { usePreferences } from '../../../contexte/Preferences'
import { pagesVisibles, TAILLES_PAGE } from './paginer'

/**
 * Barre de pagination, sous un tableau : « 11–20 sur 40 », choix du nombre de lignes, pages.
 * Sur mobile (< 640 px), les numéros laissent place à « Page 2 / 4 » pour tenir sur une ligne.
 *
 * etat : le résultat de paginer() (page, pages, total, debut, fin).
 * onPage(n), onTaille(n) : changements demandés ; taille : la taille courante.
 * Masquée quand tout tient dans la plus petite page.
 */
export default function Pagination({ etat, taille, onPage, onTaille, libelle, className = '' }) {
  const { t } = usePreferences()
  const { page, pages, total, debut, fin } = etat
  if (total <= TAILLES_PAGE[0]) return null

  const bouton =
    'inline-grid h-10 min-w-10 place-items-center rounded-full px-3 text-sm font-medium transition-colors duration-150 ' +
    'disabled:pointer-events-none disabled:opacity-40'
  const neutre = 'text-texte hover:bg-voile'

  return (
    <nav aria-label={libelle ?? t('pagination.nav')} className={`mt-3 flex flex-wrap items-center justify-between gap-x-4 gap-y-2 ${className}`}>
      <p className="chiffres text-sm text-texte-2" aria-live="polite">
        {t('pagination.plage', { debut, fin, total })}
      </p>

      <div className="flex items-center gap-3">
        <label className="flex items-center gap-2 text-sm text-texte-2 max-sm:sr-only">
          {t('pagination.lignes')}
          <select
            value={taille}
            onChange={(e) => onTaille(Number(e.target.value))}
            className="h-10 rounded-full border border-bordure-champ bg-surface px-3 text-sm text-texte"
          >
            {TAILLES_PAGE.map((n) => (
              <option key={n} value={n}>
                {n}
              </option>
            ))}
          </select>
        </label>

        <div className="flex items-center gap-1">
          <button type="button" className={`${bouton} ${neutre}`} onClick={() => onPage(page - 1)} disabled={page <= 1} aria-label={t('pagination.precedente')}>
            <ChevronLeft aria-hidden="true" className="size-[1.125rem]" />
          </button>

          {/* Mobile : la position seulement */}
          <span className="chiffres px-2 text-sm text-texte sm:hidden">{t('pagination.page', { page, pages })}</span>

          {/* Écran large : les numéros */}
          <ul className="flex items-center gap-1 max-sm:hidden">
            {pagesVisibles(page, pages).map((n, i) =>
              n === '…' ? (
                <li key={`trou-${i}`} aria-hidden="true" className="w-6 text-center text-texte-2">
                  …
                </li>
              ) : (
                <li key={n}>
                  <button
                    type="button"
                    className={`${bouton} chiffres ${n === page ? 'bg-action text-sur-action' : neutre}`}
                    onClick={() => n !== page && onPage(n)}
                    aria-current={n === page ? 'page' : undefined}
                    aria-label={t('pagination.aller', { n })}
                  >
                    {n}
                  </button>
                </li>
              ),
            )}
          </ul>

          <button type="button" className={`${bouton} ${neutre}`} onClick={() => onPage(page + 1)} disabled={page >= pages} aria-label={t('pagination.suivante')}>
            <ChevronRight aria-hidden="true" className="size-[1.125rem]" />
          </button>
        </div>
      </div>
    </nav>
  )
}
