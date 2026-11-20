import { Info } from 'lucide-react'
import { Symbole } from './Formule'

/**
 * Les blocs d'une méthode, sous sa formule : la liste des variables, l'exemple pas à pas
 * et les points d'attention. Même rythme partout : titre de bloc, puis contenu 16 px plus bas.
 */

/** Petit titre d'un bloc (« Variables », « Exemple »…), avec son icône. */
export function TitreBloc({ icone: Icone, children }) {
  return (
    <h3 className="mb-4 flex items-center gap-2 text-xs font-semibold tracking-wide text-texte-2 uppercase">
      <Icone aria-hidden="true" className="size-4" strokeWidth={1.75} />
      {children}
    </h3>
  )
}

/**
 * Variables d'une formule : symbole, sens, et la valeur prise dans l'exemple (facultative).
 * Lignes rayées, comme les tableaux.
 * lignes : [{ v, k?, libelle, valeur? }]
 */
export function Variables({ lignes }) {
  return (
    <dl className="overflow-hidden rounded-2xl border border-bordure">
      {lignes.map(({ v, k, libelle, valeur }) => (
        <div key={`${v}${k ? 'k' : ''}`} className="flex items-start gap-3 px-4 py-3 text-sm even:bg-rayure">
          <dt className="w-8 shrink-0 font-titre text-base leading-6">
            <Symbole v={v} k={k} />
          </dt>
          {/* Sur un écran étroit, la valeur passe sous le libellé au lieu de l'écraser. */}
          <dd className="flex min-w-0 flex-1 flex-wrap items-baseline justify-between gap-x-4">
            <span className="min-w-0 leading-6 text-texte-2">{libelle}</span>
            {valeur !== undefined && <span className="chiffres leading-6 font-medium whitespace-nowrap text-texte">{valeur}</span>}
          </dd>
        </div>
      ))}
    </dl>
  )
}

/**
 * Exemple chiffré pas à pas : une phrase de contexte, puis des étapes numérotées
 * (libellé, calcul, résultat). La dernière étape est le résultat, mis en avant.
 * etapes : [{ libelle, calcul?, resultat }]
 */
export function Exemple({ contexte, etapes }) {
  return (
    <div>
      <p className="mb-5 text-sm leading-6 text-texte-2">{contexte}</p>
      <ol className="flex flex-col gap-3">
        {etapes.map(({ libelle, calcul, resultat }, i) => {
          const dernier = i === etapes.length - 1
          return (
            <li
              key={libelle}
              className={`flex gap-3 rounded-2xl border p-4 ${dernier ? 'border-action/50 bg-action/6' : 'border-bordure bg-surface'}`}
            >
              <span
                aria-hidden="true"
                className={`chiffres grid size-6 shrink-0 place-items-center rounded-full text-xs font-semibold ${dernier ? 'bg-action text-sur-action' : 'bg-voile text-texte-2'}`}
              >
                {i + 1}
              </span>
              <div className="min-w-0 flex-1">
                <p className="text-sm leading-6 text-texte-2">{libelle}</p>
                <p className="chiffres mt-1 flex flex-wrap items-baseline gap-x-2 gap-y-1 text-sm leading-6">
                  {calcul && <span className="text-texte">{calcul}</span>}
                  {calcul && <span className="text-texte-2">=</span>}
                  <strong className={`font-semibold ${dernier ? 'text-base text-action' : 'text-texte'}`}>{resultat}</strong>
                </p>
              </div>
            </li>
          )
        })}
      </ol>
    </div>
  )
}

/** Points d'attention : une courte liste, chaque point précédé d'une pastille orange. */
export function Notes({ notes }) {
  return (
    <ul className="grid gap-3 sm:grid-cols-2">
      {notes.map((note) => (
        <li key={note} className="flex items-start gap-3 text-sm leading-6 text-texte-2">
          <span aria-hidden="true" className="mt-0.5 inline-grid size-5 shrink-0 place-items-center rounded-full bg-orange/15 text-orange-texte">
            <Info className="size-3" strokeWidth={2.5} />
          </span>
          {note}
        </li>
      ))}
    </ul>
  )
}
