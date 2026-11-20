/**
 * Barre de composition : de quoi est fait un montant (ex. prix pied de coupon + coupon couru).
 * La barre s'étire à l'apparition ; chaque part garde sa couleur dans la légende.
 * parts : [{ libelle, valeur, texte, couleur: 'action' | 'orange' }] — texte : la valeur déjà formatée.
 */
const COULEURS = { action: 'bg-action', orange: 'bg-orange' }

export default function Composition({ parts }) {
  const total = parts.reduce((s, p) => s + Math.max(p.valeur, 0), 0) || 1
  return (
    <div className="mt-6">
      <div className="etire flex h-2.5 gap-0.5 overflow-hidden rounded-full" aria-hidden="true">
        {parts.map((p) => (
          <span
            key={p.libelle}
            className={`${COULEURS[p.couleur]} rounded-full`}
            // Une part minuscule reste visible (au moins 2 %).
            style={{ flexGrow: Math.max(p.valeur / total, 0.02), flexBasis: 0 }}
          />
        ))}
      </div>
      <dl className="mt-3 flex flex-wrap justify-between gap-x-6 gap-y-2 text-sm">
        {parts.map((p) => (
          <div key={p.libelle} className="flex min-w-0 items-center gap-2">
            <span aria-hidden="true" className={`size-2.5 shrink-0 rounded-full ${COULEURS[p.couleur]}`} />
            <dt className="text-texte-2">{p.libelle}</dt>
            <dd className="chiffres font-semibold">{p.texte}</dd>
          </div>
        ))}
      </dl>
    </div>
  )
}
