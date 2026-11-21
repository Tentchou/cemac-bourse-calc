import { ChevronDown, CircleAlert } from 'lucide-react'
import { useId } from 'react'

/**
 * Liste de choix (select natif : clavier, lecteur d'écran et mobile sans effort), au gabarit de Champ :
 * libellé au-dessus, 48 px, bordure marquée, focus bleu, erreur rouge + message.
 *
 * options : [{ valeur, libelle }]. vide : le texte de l'option vide (« Choisir… ») ; sans lui, pas d'option vide.
 */
export default function ChampChoix({ libelle, options, vide, aide, erreur, className = '', ...select }) {
  const id = useId()
  const idAide = `${id}-aide`
  const etat = erreur
    ? 'border-erreur ring-4 ring-erreur/12'
    : 'border-bordure-champ hover:border-texte-2 focus-within:border-action focus-within:ring-4 focus-within:ring-action/15'

  return (
    <div className={`min-w-0 ${className}`}>
      <label htmlFor={id} className="mb-2 block text-sm font-medium text-texte">
        {libelle}
      </label>
      <div className={`relative h-12 rounded-2xl border bg-surface transition-[border-color,box-shadow] duration-150 ease-douce ${etat}`}>
        <select
          id={id}
          aria-invalid={erreur ? true : undefined}
          aria-describedby={aide || erreur ? idAide : undefined}
          className="h-full w-full cursor-pointer appearance-none truncate rounded-2xl bg-transparent pr-10 pl-4 text-base text-texte outline-none"
          {...select}
        >
          {vide !== undefined && <option value="">{vide}</option>}
          {options.map((o) => (
            <option key={o.valeur} value={o.valeur}>
              {o.libelle}
            </option>
          ))}
        </select>
        <ChevronDown aria-hidden="true" strokeWidth={1.75} className="pointer-events-none absolute top-1/2 right-4 size-4 -translate-y-1/2 text-texte-2" />
      </div>
      {(erreur || aide) && (
        <p id={idAide} className={`mt-2 flex items-start gap-1.5 text-xs leading-5 ${erreur ? 'text-erreur' : 'text-texte-2'}`}>
          {erreur && <CircleAlert aria-hidden="true" className="mt-0.5 size-3.5 shrink-0" strokeWidth={2} />}
          {erreur || aide}
        </p>
      )}
    </div>
  )
}
