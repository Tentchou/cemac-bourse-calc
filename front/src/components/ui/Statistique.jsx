/**
 * Statistique compacte : un libellé (avec son point de couleur) et un montant.
 * point : couleur du repère, la même que dans le graphique associé (orange, action) ; aucun si absent.
 * fort : la statistique de synthèse (total), mise en avant par un fond teinté.
 * detail : une ligne discrète sous le montant (date, période…), facultative.
 */
const POINTS = { orange: 'bg-orange', action: 'bg-action' }

export default function Statistique({ libelle, valeur, point, detail, fort = false, className = '' }) {
  return (
    <div
      className={`min-w-0 rounded-2xl border p-6 ${fort ? 'border-action/50 bg-action/8' : 'border-bordure bg-surface'} ${className}`}
    >
      <p className="flex items-center gap-2 text-sm text-texte-2">
        {point && <span aria-hidden="true" className={`size-2.5 shrink-0 rounded-full ${POINTS[point]}`} />}
        {libelle}
      </p>
      <p className="chiffres mt-2 font-titre text-xl font-bold break-words sm:text-2xl">{valeur}</p>
      {detail && <p className="chiffres mt-1 text-xs text-texte-2">{detail}</p>}
    </div>
  )
}
