import { usePreferences } from '../../contexte/Preferences'

/** Opérateurs entourés d'un espace ; parenthèses et nombres restent collés à leurs voisins. */
const OPERATEUR = /^[=×÷+−Σ]$/

/**
 * Une formule mise en forme : les variables en bleu (leur symbole vient des textes, il change
 * avec la langue), les opérateurs en gris, les exposants en petit et surélevés.
 *
 * jetons : liste de
 *   'texte'                 un opérateur (« = », « × »…), une parenthèse ou un nombre, un par jeton
 *   { v: 'nominal' }        une variable (symbole : t('meth.sym.nominal'))
 *   { v: 'flux', k: true }  la même, indicée « k » (Σ sur les flux)
 *   { exp: [...] }          un exposant, qui contient lui-même des jetons
 * libelle : petit titre au-dessus (quand une section a deux formules).
 */
export default function Formule({ jetons, libelle }) {
  return (
    <figure className="min-w-0">
      {libelle && <figcaption className="mb-2 text-xs font-medium text-texte-2">{libelle}</figcaption>}
      <div className="overflow-x-auto rounded-2xl border border-bordure bg-fond px-5 py-4 [scrollbar-width:thin]">
        <p className="chiffres font-titre text-lg whitespace-nowrap text-texte sm:text-xl">
          <Jetons jetons={jetons} />
        </p>
      </div>
    </figure>
  )
}

/** Une variable seule, au même style que dans la formule (pour la liste des variables). */
export function Symbole({ v, k = false, className = '' }) {
  const { t } = usePreferences()
  return (
    <var className={`font-semibold text-action not-italic ${className}`}>
      {t(`meth.sym.${v}`)}
      {k && <sub className="ml-px text-[0.7em] font-medium">k</sub>}
    </var>
  )
}

function Jetons({ jetons }) {
  return jetons.map((jeton, i) => {
    if (typeof jeton === 'string') {
      return (
        <span key={i} className={`text-texte-2 ${OPERATEUR.test(jeton) ? "mx-1.5" : ""}`}>
          {jeton}
        </span>
      )
    }
    if (jeton.exp) {
      return (
        <sup key={i} className="ml-0.5 text-[0.65em]">
          <Jetons jetons={jeton.exp} />
        </sup>
      )
    }
    return <Symbole key={i} v={jeton.v} k={jeton.k} />
  })
}
