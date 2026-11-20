/**
 * Filtres en pilules (un seul choix actif), par exemple les pays.
 * Sur un écran étroit, la rangée défile à l'horizontale dans sa propre bande (jamais la page),
 * avec un fondu sur le bord droit pour montrer qu'il y a une suite.
 *
 * options : [{ valeur, libelle, prefixe? }] — prefixe : petit élément avant le texte (code pays, drapeau).
 */
export default function Pilules({ options, valeur, onChange, libelle, className = '' }) {
  return (
    <div className={`relative min-w-0 ${className}`}>
      <div
        role="group"
        aria-label={libelle}
        className="flex gap-2 overflow-x-auto pb-1 [scrollbar-width:none] max-sm:[mask-image:linear-gradient(to_right,black_85%,transparent)] sm:flex-wrap [&::-webkit-scrollbar]:hidden"
      >
        {options.map(({ valeur: v, libelle: texte, prefixe }) => {
          const actif = v === valeur
          return (
            <button
              key={v ?? 'tous'}
              type="button"
              aria-pressed={actif}
              onClick={() => onChange(v)}
              className={`inline-flex h-10 shrink-0 items-center gap-2 rounded-full border px-4 text-sm font-medium transition-[background-color,border-color,color] duration-150 ease-douce ${
                actif ? 'border-texte bg-texte text-fond' : 'border-bordure bg-surface text-texte-2 hover:border-bordure-champ hover:text-texte'
              }`}
            >
              {prefixe}
              {texte}
            </button>
          )
        })}
      </div>
    </div>
  )
}
