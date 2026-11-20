import { CircleAlert } from 'lucide-react'
import { useId } from 'react'

/**
 * Zone de saisie, faite pour être vue tout de suite :
 *  - libellé toujours visible au-dessus (jamais un simple texte grisé dans le champ) ;
 *  - 48 px de haut, fond blanc, bordure plus marquée que celle des cartes ;
 *  - survol : la bordure fonce ; focus : bordure bleue + halo ; erreur : rouge + icône + message.
 *
 * icone : composant Lucide à gauche. unite : texte à droite (%, FCFA, titres).
 * aide : phrase sous le champ. erreur : message (remplace l'aide). Les autres props vont à <input>.
 */
export default function Champ({ libelle, icone: Icone, unite, aide, erreur, className = '', disabled, ...entree }) {
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
      <div
        className={`group flex h-12 items-center gap-3 rounded-2xl border bg-surface px-4 transition-[border-color,box-shadow] duration-150 ease-douce ${etat} ${
          disabled ? 'pointer-events-none opacity-50' : ''
        }`}
      >
        {Icone && (
          <Icone
            aria-hidden="true"
            strokeWidth={1.75}
            className={`size-5 shrink-0 transition-colors duration-150 ${erreur ? 'text-erreur' : 'text-texte-2 group-focus-within:text-action'}`}
          />
        )}
        <input
          id={id}
          disabled={disabled}
          aria-invalid={erreur ? true : undefined}
          aria-describedby={aide || erreur ? idAide : undefined}
          className="chiffres h-full min-w-0 flex-1 bg-transparent text-base text-texte outline-none placeholder:text-texte-2/70 [&::-webkit-calendar-picker-indicator]:cursor-pointer [&::-webkit-calendar-picker-indicator]:opacity-60 [&::-webkit-calendar-picker-indicator]:hover:opacity-100"
          {...entree}
        />
        {unite && <span className="shrink-0 text-sm font-medium text-texte-2">{unite}</span>}
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
