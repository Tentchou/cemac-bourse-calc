/**
 * Icône dans un rond teinté : le repère visuel d'une carte (chiffre clé, résultat, caractéristique).
 * ton : action, orange, neutre, erreur. taille : petite (32 px), normale (40 px), grande (48 px).
 */
const TONS = {
  action: 'bg-action/10 text-action',
  orange: 'bg-orange/15 text-orange-texte',
  neutre: 'bg-voile text-texte-2',
  erreur: 'bg-erreur/10 text-erreur',
}

const TAILLES = {
  petite: 'size-8 [&_svg]:size-4',
  normale: 'size-10 [&_svg]:size-5',
  grande: 'size-12 [&_svg]:size-6',
}

export default function Pastille({ icone: Icone, ton = 'action', taille = 'normale', className = '' }) {
  return (
    <span aria-hidden="true" className={`grid shrink-0 place-items-center rounded-full ${TONS[ton]} ${TAILLES[taille]} ${className}`}>
      <Icone strokeWidth={1.75} />
    </span>
  )
}
