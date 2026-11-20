import { ChevronRight } from 'lucide-react'
import { useEffect, useState } from 'react'
import { usePreferences } from '../../contexte/Preferences'
import Carte from '../ui/Carte'
import { Forme } from '../ui/Squelettes'
import Drapeau from './Drapeau'
import { PAYS_CEMAC } from './drapeaux'

/**
 * Carte de l'Afrique : les 6 pays de la CEMAC mis en évidence, chacun avec un badge
 * (drapeau + nombre de titres) relié au pays par un trait fin — les pays sont trop proches
 * pour poser les badges dessus sans qu'ils se chevauchent.
 * À droite (ou dessous sur mobile), la même information en liste lisible, avec le nom du pays.
 *
 * comptes : { CM: 3, … } ; valeur : pays choisi (ou null) ; onChange(code | null).
 * Le tracé (≈ 145 Ko) est chargé à part, à la première apparition de la carte.
 */
export default function CarteCemac({ comptes, valeur, onChange, chargement = false }) {
  const { t } = usePreferences()
  const [trace, setTrace] = useState(null)
  const [survol, setSurvol] = useState(null)

  useEffect(() => {
    let actif = true
    import('./afrique.json').then((module) => actif && setTrace(module.default))
    return () => {
      actif = false
    }
  }, [])

  const choisir = (code) => onChange(valeur === code ? null : code)
  const libelle = (code) => `${t(`pays.${code}`)} : ${((comptes[code] ?? 0) === 1 ? t('carte.titres.1') : t('carte.titres', { n: comptes[code] ?? 0 }))}`

  const remplissage = (code) =>
    code === valeur
      ? 'var(--cbc-action)'
      : code === survol
        ? 'color-mix(in srgb, var(--cbc-action) 55%, var(--cbc-surface))'
        : 'color-mix(in srgb, var(--cbc-action) 28%, var(--cbc-surface))'

  return (
    <Carte className="apparition grid grid-cols-[minmax(0,1fr)] gap-6 lg:grid-cols-[minmax(0,1fr)_300px] lg:items-center">
      {/* La carte */}
      <div className="relative mx-auto w-full max-w-[400px]">
        {trace ? (
          <>
            <svg viewBox={`0 0 ${trace.largeur} ${trace.hauteur}`} className="block h-auto w-full" aria-hidden="true">
              <path d={trace.autres} fill="color-mix(in srgb, var(--cbc-texte) 7%, var(--cbc-surface))" stroke="var(--cbc-surface)" strokeWidth="0.8" />
              {PAYS_CEMAC.map((code) => (
                <path
                  key={code}
                  d={trace.cemac[code].d}
                  fill={remplissage(code)}
                  stroke="var(--cbc-surface)"
                  strokeWidth="0.8"
                  className="cursor-pointer transition-[fill] duration-150"
                  onMouseEnter={() => setSurvol(code)}
                  onMouseLeave={() => setSurvol(null)}
                  onClick={() => choisir(code)}
                />
              ))}
              {/* Traits de liaison et points au centre des pays */}
              {PAYS_CEMAC.map((code) => {
                const { centre, ancre } = trace.cemac[code]
                const fort = code === survol || code === valeur
                return (
                  <g key={code} className="pointer-events-none">
                    <line
                      x1={centre[0]}
                      y1={centre[1]}
                      x2={ancre[0]}
                      y2={ancre[1]}
                      stroke={fort ? 'var(--cbc-action)' : 'var(--cbc-texte-2)'}
                      strokeOpacity={fort ? 1 : 0.5}
                      strokeWidth="1.2"
                      className="transition-[stroke] duration-150"
                    />
                    <circle cx={centre[0]} cy={centre[1]} r="4" fill="var(--cbc-surface)" stroke="var(--cbc-action)" strokeWidth="2" />
                  </g>
                )
              })}
            </svg>

            {/* Badges : drapeau + nombre de titres */}
            {PAYS_CEMAC.map((code, i) => {
              const { ancre, cote } = trace.cemac[code]
              const actif = code === valeur
              return (
                <button
                  key={code}
                  type="button"
                  aria-pressed={actif}
                  aria-label={libelle(code)}
                  title={libelle(code)}
                  onClick={() => choisir(code)}
                  onMouseEnter={() => setSurvol(code)}
                  onMouseLeave={() => setSurvol(null)}
                  onFocus={() => setSurvol(code)}
                  onBlur={() => setSurvol(null)}
                  className={`apparition absolute inline-flex h-7 items-center gap-1.5 rounded-full border pr-2.5 pl-1.5 text-xs font-bold shadow-douce transition-[background-color,border-color,color,transform] duration-150 hover:scale-105 sm:h-8 sm:text-sm ${
                    actif ? 'border-action bg-action text-sur-action' : 'border-bordure bg-surface text-texte hover:border-action'
                  }`}
                  style={{
                    left: `${(ancre[0] / trace.largeur) * 100}%`,
                    top: `${(ancre[1] / trace.hauteur) * 100}%`,
                    translate: cote === 'gauche' ? '-100% -50%' : '0 -50%',
                    '--rang': i,
                  }}
                >
                  <Drapeau code={code} className="h-3.5 sm:h-4" />
                  <span className="chiffres">{chargement ? '·' : (comptes[code] ?? 0)}</span>
                </button>
              )
            })}
          </>
        ) : (
          <Forme className="aspect-[600/816] w-full rounded-3xl" />
        )}
      </div>

      {/* La même information, en liste lisible */}
      <ul className="grid grid-cols-1 gap-2 sm:grid-cols-2 lg:grid-cols-1">
        {PAYS_CEMAC.map((code) => {
          const actif = code === valeur
          return (
            <li key={code}>
              <button
                type="button"
                aria-pressed={actif}
                onClick={() => choisir(code)}
                onMouseEnter={() => setSurvol(code)}
                onMouseLeave={() => setSurvol(null)}
                className={`group flex h-12 w-full items-center gap-3 rounded-2xl border px-3 text-left text-sm transition-[background-color,border-color,color] duration-150 ${
                  actif
                    ? 'border-action bg-action/10 text-texte'
                    : code === survol
                      ? 'border-bordure-champ bg-rayure text-texte'
                      : 'border-bordure bg-surface text-texte hover:border-bordure-champ'
                }`}
              >
                <Drapeau code={code} className="h-4" />
                <span className="min-w-0 flex-1 truncate font-medium">{t(`pays.${code}`)}</span>
                <span className="chiffres rounded-full bg-voile px-2 py-0.5 text-xs font-semibold text-texte-2">
                  {chargement ? '·' : (comptes[code] ?? 0)}
                </span>
                <ChevronRight aria-hidden="true" strokeWidth={1.75} className="hidden size-4 text-texte-2 transition-transform duration-150 group-hover:translate-x-0.5 lg:block" />
              </button>
            </li>
          )
        })}
      </ul>
    </Carte>
  )
}
