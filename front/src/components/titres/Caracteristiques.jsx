import Pastille from '../ui/Pastille'

/**
 * Fiche technique : une seule surface découpée par des filets de 1 px (et non huit cartes
 * séparées), comme une fiche produit financière. Deux colonnes sur mobile, quatre au-delà.
 * elements : [{ icone, libelle, valeur }].
 */
export default function Caracteristiques({ elements }) {
  return (
    <dl className="apparition grid grid-cols-2 gap-px overflow-hidden rounded-2xl border border-bordure bg-bordure lg:grid-cols-4">
      {elements.map(({ icone, libelle, valeur }, i) => (
        <div
          key={libelle}
          className="group flex min-w-0 items-center gap-3 bg-surface p-4 transition-colors duration-150 hover:bg-rayure sm:p-5"
          // Le dernier élément d'une rangée impaire occupe toute la largeur : pas de case vide.
          style={elements.length % 2 === 1 && i === elements.length - 1 ? { gridColumn: '1 / -1' } : undefined}
        >
          <Pastille icone={icone} ton="neutre" taille="petite" className="transition-colors duration-150 group-hover:bg-action/10 group-hover:text-action max-[399px]:hidden" />
          <div className="min-w-0">
            <dt className="text-xs text-texte-2">{libelle}</dt>
            <dd className="chiffres mt-0.5 font-semibold break-words">{valeur}</dd>
          </div>
        </div>
      ))}
    </dl>
  )
}
