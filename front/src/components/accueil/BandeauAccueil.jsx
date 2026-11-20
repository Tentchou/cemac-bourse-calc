import { Banknote, ChartNoAxesCombined, Globe, Search, X } from 'lucide-react'
import { usePreferences } from '../../contexte/Preferences'
import Bouton from '../ui/Bouton'
import LienInstitution from '../ui/LienInstitution'
import { Forme } from '../ui/Squelettes'
import ApercuCalcul from './ApercuCalcul'

/**
 * Bandeau d'accueil, sur fond clair : à gauche le titre, la recherche et les chiffres clés ;
 * à droite l'aperçu d'un calcul qui se joue tout seul. Une trame de points très légère,
 * estompée sur les bords, donne de la profondeur sans charger.
 */
export default function BandeauAccueil({ texte, onTexte, onChercher, chiffres, chargement, lienExemple }) {
  const { t } = usePreferences()
  const icones = { obligations: Banknote, actions: ChartNoAxesCombined, pays: Globe }

  return (
    <section className="relative -mx-4 overflow-x-clip px-4 sm:-mx-6 sm:px-6">
      <div
        aria-hidden="true"
        className="pointer-events-none absolute inset-x-0 -top-8 -bottom-8 [mask-image:radial-gradient(ellipse_at_center,black_35%,transparent_75%)]"
        style={{
          backgroundImage: 'radial-gradient(color-mix(in srgb, var(--cbc-texte) 14%, transparent) 1px, transparent 1px)',
          backgroundSize: '20px 20px',
        }}
      />

      <div className="relative grid grid-cols-[minmax(0,1fr)] items-center gap-12 py-4 lg:grid-cols-[minmax(0,1fr)_440px] lg:py-10">
        <div className="apparition min-w-0">
          <div className="flex flex-wrap items-center gap-2">
            <p className="inline-flex h-7 items-center gap-2 rounded-full border border-bordure bg-surface px-3 text-xs font-semibold tracking-wide text-texte-2">
              <span aria-hidden="true" className="size-1.5 rounded-full bg-orange" />
              {t('institution.marche')}
            </p>
            <span className="flex flex-wrap gap-2">
              <LienInstitution code="bvmac" role />
              <LienInstitution code="cosumaf" role />
            </span>
          </div>
          <h1 className="mt-6 text-[2.125rem] leading-[1.08] font-extrabold sm:text-5xl lg:text-[3.5rem]">
            {t('accueil.titre.1')} <span className="text-action">{t('accueil.titre.2')}</span>
          </h1>
          <p className="mt-6 max-w-xl text-base text-texte-2 sm:text-lg">{t('accueil.accroche')}</p>

          <form
            role="search"
            onSubmit={(e) => {
              e.preventDefault()
              onChercher()
            }}
            className="mt-8 flex h-14 max-w-xl items-center gap-2 rounded-full border border-bordure-champ bg-surface p-1.5 pl-5 shadow-douce transition-[border-color,box-shadow] duration-150 focus-within:border-action focus-within:ring-4 focus-within:ring-action/15 hover:border-texte-2"
          >
            <Search aria-hidden="true" className="size-5 shrink-0 text-texte-2" strokeWidth={1.75} />
            <input
              type="search"
              value={texte}
              onChange={(e) => onTexte(e.target.value)}
              placeholder={t('accueil.recherche')}
              aria-label={t('accueil.recherche')}
              className="min-w-0 flex-1 bg-transparent text-base text-texte outline-none placeholder:text-texte-2/80 [&::-webkit-search-cancel-button]:hidden"
            />
            {texte && (
              <button type="button" onClick={() => onTexte('')} aria-label={t('liste.effacer')} className="grid size-9 shrink-0 place-items-center rounded-full text-texte-2 transition-colors hover:bg-voile">
                <X className="size-4" strokeWidth={1.75} />
              </button>
            )}
            <Bouton type="submit" variante="action" taille="petite" icone={Search} className="h-11" aria-label={t('accueil.chercher')}>
              <span className="hidden sm:inline">{t('accueil.chercher')}</span>
            </Bouton>
          </form>

          {/* Chiffres clés, en ligne, séparés par des filets */}
          <dl className="mt-8 flex flex-wrap items-center gap-y-3 divide-x divide-bordure">
            {chiffres.map(({ cle, valeur }) => {
              const Icone = icones[cle]
              return (
                <div key={cle} className="flex items-center gap-1.5 px-3 first:pl-0 last:pr-0 sm:gap-2 sm:px-4">
                  <Icone aria-hidden="true" className="hidden size-4 text-texte-2 sm:block" strokeWidth={1.75} />
                  <dd className="chiffres font-titre text-base font-bold sm:text-lg">{chargement && cle !== 'pays' ? <Forme className="h-5 w-4" /> : valeur}</dd>
                  <dt className="text-xs text-texte-2 sm:text-sm">{t(valeur === 1 && cle !== 'pays' ? `cle.${cle}.1` : `cle.${cle}`)}</dt>
                </div>
              )
            })}
          </dl>
        </div>

        <div className="apparition min-w-0" style={{ '--rang': 3 }}>
          <ApercuCalcul lienExemple={lienExemple} />
        </div>
      </div>
    </section>
  )
}
