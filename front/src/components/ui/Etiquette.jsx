/**
 * Étiquette pilule (type de titre, statut, ISIN…).
 * ton : neutre, action (bleu), orange (pastille orange + texte #A9520B lisible sur clair), erreur.
 * point : petite pastille de couleur avant le texte. icone : composant Lucide.
 */
const TONS = {
  neutre: 'border-bordure bg-surface text-texte-2',
  action: 'border-action/30 bg-action/8 text-action',
  orange: 'border-orange/40 bg-orange/10 text-orange-texte',
  erreur: 'border-erreur/30 bg-erreur/8 text-erreur',
}

export default function Etiquette({ ton = 'neutre', point = false, icone: Icone, className = '', children, ...autres }) {
  return (
    <span
      className={`inline-flex h-7 max-w-full items-center gap-1.5 rounded-full border px-3 text-xs font-medium whitespace-nowrap ${TONS[ton]} ${className}`}
      {...autres}
    >
      {point && <span aria-hidden="true" className={`size-1.5 shrink-0 rounded-full ${ton === 'orange' ? 'bg-orange' : 'bg-current'}`} />}
      {Icone && <Icone aria-hidden="true" className="size-3.5 shrink-0" strokeWidth={1.75} />}
      <span className="truncate">{children}</span>
    </span>
  )
}
