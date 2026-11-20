import Carte from '../ui/Carte'
import Explication from '../ui/Explication'
import Pastille from '../ui/Pastille'

/**
 * Une carte de résultat de calcul : pastille + libellé, grand chiffre, aide courte,
 * un contenu libre (children, ex. une barre de composition) et l'explication dépliable. principal : le résultat mis en avant (bordure bleue, chiffre plus grand).
 */
export default function Resultat({ icone, ton = 'action', libelle, valeur, aide, principal = false, explication, className = '', style, children }) {
  return (
    <Carte accent={principal} className={`flex flex-col ${className}`} style={style}>
      <div className="flex items-center gap-3">
        <Pastille icone={icone} ton={ton} taille="petite" />
        <p className="text-sm font-medium text-texte-2">{libelle}</p>
      </div>
      <p
        aria-live="polite"
        className={`chiffres mt-4 font-titre leading-tight font-bold tracking-tight break-words ${principal ? 'text-[2rem] sm:text-[2.5rem]' : 'text-2xl'}`}
      >
        {valeur}
      </p>
      {aide && <p className="mt-2 text-sm text-texte-2">{aide}</p>}
      {children}
      {explication && <Explication>{explication}</Explication>}
    </Carte>
  )
}
