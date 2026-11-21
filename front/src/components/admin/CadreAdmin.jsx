import { LogOut } from 'lucide-react'
import { useCallback, useEffect, useState } from 'react'
import { lireCompteurs } from '../../api/admin'
import { GROUPES, RUBRIQUES, rubriqueDuChemin } from '../../admin/rubriques'
import { usePreferences } from '../../contexte/Preferences'
import { Lien, useRoute } from '../../routeur/Routeur'
import Reglages from '../disposition/Reglages'
import { Logo } from '../icones/Icones'
import PageRubrique from './PageRubrique'

/**
 * Le cadre de l'administration : en-tête (logo, langue, thème, compte, déconnexion), menu des rubriques
 * avec leur nombre de lignes, et la rubrique ouverte (adresse /admin/{rubrique}).
 * Grand écran : menu à gauche. Mobile (< 900 px) : bande de pilules qui défile sous l'en-tête.
 */
export default function CadreAdmin({ compte, onDeconnecter, onSessionExpiree }) {
  const { t, nombre } = usePreferences()
  const { chemin, naviguer } = useRoute()
  const rubrique = rubriqueDuChemin(chemin)
  const [compteurs, setCompteurs] = useState(null)
  const [version, setVersion] = useState(0)

  // /admin (ou une rubrique inconnue) mène à la première rubrique, sans entrée de plus dans l'historique.
  useEffect(() => {
    if (!rubrique) naviguer(`/admin/${RUBRIQUES[0].cle}`, { remplacer: true })
  }, [rubrique, naviguer])

  useEffect(() => {
    const controleur = new AbortController()
    lireCompteurs(controleur.signal)
      .then(setCompteurs)
      .catch((e) => {
        if (!controleur.signal.aborted && e.statut === 401) onSessionExpiree()
      })
    return () => controleur.abort()
  }, [version, onSessionExpiree])

  const relireCompteurs = useCallback(() => setVersion((v) => v + 1), [])

  useEffect(() => {
    if (rubrique) document.title = `${t(`admin.rubrique.${rubrique.cle}`)} · ${t('admin.etiquette')} · CEMAC Bourse Calc`
  }, [rubrique, t])

  const initiales = compte.nom
    .split(/\s+/)
    .map((m) => m[0])
    .join('')
    .slice(0, 2)
    .toUpperCase()

  return (
    <div className="min-h-dvh bg-fond text-texte">
      <header className="sticky top-0 z-30 border-b border-bordure bg-surface">
        <div className="mx-auto flex h-16 max-w-[1360px] items-center gap-3 px-4">
          <Lien vers={`/admin/${RUBRIQUES[0].cle}`} className="flex min-w-0 items-center gap-2.5 rounded-lg font-titre font-bold">
            <Logo className="size-8 shrink-0" />
            <span className="truncate max-[420px]:sr-only">CEMAC Bourse Calc</span>
          </Lien>
          <span className="rounded-full border border-bordure bg-voile px-2.5 py-0.5 text-xs font-semibold text-texte-2 max-sm:hidden">
            {t('admin.etiquette')}
          </span>
          <div className="ml-auto flex items-center gap-2">
            <Reglages />
            <span className="flex items-center gap-2 text-sm max-[420px]:hidden" title={`${compte.nom} · ${compte.role}`}>
              <span aria-hidden="true" className="grid size-8 place-items-center rounded-full border border-bordure bg-voile text-xs font-semibold">
                {initiales}
              </span>
              <span className="max-w-40 truncate max-md:sr-only">
                {compte.nom} · {compte.role}
              </span>
            </span>
            <button
              type="button"
              onClick={onDeconnecter}
              title={t('admin.deconnexion')}
              aria-label={t('admin.deconnexion')}
              className="grid size-10 place-items-center rounded-full text-texte-2 transition-colors hover:bg-voile hover:text-texte"
            >
              <LogOut aria-hidden="true" className="size-[1.125rem]" strokeWidth={1.75} />
            </button>
          </div>
        </div>
      </header>

      <div className="mx-auto grid max-w-[1360px] grid-cols-1 gap-6 px-4 pb-12 min-[900px]:grid-cols-[232px_minmax(0,1fr)] min-[900px]:pt-6">
        <nav
          aria-label={t('admin.menu')}
          className="sticky top-16 z-20 -mx-4 flex gap-1.5 overflow-x-auto border-b border-bordure bg-fond px-4 py-2.5 [scrollbar-width:none] min-[900px]:top-22 min-[900px]:mx-0 min-[900px]:flex-col min-[900px]:gap-5 min-[900px]:self-start min-[900px]:border-0 min-[900px]:p-0"
        >
          {GROUPES.map((groupe) => (
            <div key={groupe} className="contents min-[900px]:flex min-[900px]:flex-col min-[900px]:gap-1">
              <h2 className="mx-3 mb-1 font-sans text-[0.6875rem] font-semibold tracking-[0.08em] text-texte-2 uppercase max-[899px]:hidden">
                {t(`admin.groupe.${groupe}`)}
              </h2>
              {RUBRIQUES.filter((r) => r.groupe === groupe).map((r) => {
                const actif = r === rubrique
                const n = compteurs?.[r.compteur]
                const Icone = r.icone
                return (
                  <Lien
                    key={r.cle}
                    vers={`/admin/${r.cle}`}
                    aria-current={actif ? 'page' : undefined}
                    className={[
                      'flex shrink-0 items-center gap-2.5 text-sm font-medium whitespace-nowrap transition-colors',
                      // Mobile : pilules
                      'rounded-full border px-3.5 py-1.5',
                      actif ? 'border-action bg-action text-sur-action' : 'border-bordure bg-surface text-texte-2 hover:text-texte',
                      // Grand écran : lignes du menu
                      'min-[900px]:rounded-xl min-[900px]:px-3 min-[900px]:py-2.5 min-[900px]:text-[0.9375rem]',
                      actif
                        ? 'min-[900px]:border-bordure min-[900px]:bg-surface min-[900px]:text-texte min-[900px]:shadow-[inset_3px_0_0_var(--color-action)]'
                        : 'min-[900px]:border-transparent min-[900px]:bg-transparent min-[900px]:hover:bg-voile',
                    ].join(' ')}
                  >
                    <Icone aria-hidden="true" className="size-[1.125rem] shrink-0 max-[899px]:hidden" strokeWidth={1.75} />
                    {t(`admin.rubrique.${r.cle}`)}
                    {n !== undefined && <span className={`chiffres ml-auto text-xs ${actif ? 'max-[899px]:text-sur-action' : 'text-texte-2'}`}>{nombre(n, 0)}</span>}
                  </Lien>
                )
              })}
            </div>
          ))}
        </nav>

        <main id="contenu-admin" className="min-w-0 scroll-mt-28 pt-2 min-[900px]:pt-0">
          {rubrique && (
            <PageRubrique key={rubrique.cle} rubrique={rubrique} onModifie={relireCompteurs} onSessionExpiree={onSessionExpiree} />
          )}
        </main>
      </div>
    </div>
  )
}
