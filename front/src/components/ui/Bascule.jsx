import { useLayoutEffect, useRef, useState } from 'react'

/**
 * Interrupteur à options (langue, thème, mode Simple / Expert, type de titre).
 * Un curseur blanc glisse sous l'option choisie (animation, pas de saut).
 *
 * options : [{ valeur, libelle?, icone?, libelleCache? }] — libelleCache : nom lu par le lecteur
 *           d'écran quand l'option n'a qu'une icône.
 * taille  : petite (32 px, en-tête) ou normale (40 px).
 */
export default function Bascule({ options, valeur, onChange, libelle, taille = 'normale', className = '' }) {
  const refs = useRef({})
  const groupe = useRef(null)
  const [curseur, setCurseur] = useState(null)

  // Le curseur suit l'option choisie, et se recale si les largeurs changent (langue, police chargée).
  useLayoutEffect(() => {
    const placer = () => {
      const bouton = refs.current[valeur]
      if (bouton) setCurseur({ left: bouton.offsetLeft, width: bouton.offsetWidth })
    }
    placer()
    const observateur = new ResizeObserver(placer)
    observateur.observe(groupe.current)
    return () => observateur.disconnect()
  }, [valeur, options])

  const hauteur = taille === 'petite' ? 'h-8 px-2.5 text-xs' : 'h-10 px-4 text-sm'

  return (
    <div ref={groupe} role="group" aria-label={libelle} className={`relative inline-flex max-w-full shrink-0 rounded-full border border-bordure bg-fond p-1 ${className}`}>
      {curseur && (
        <span
          aria-hidden="true"
          className="absolute top-1 bottom-1 rounded-full border border-bordure bg-surface shadow-douce transition-[left,width] duration-250 ease-douce"
          style={curseur}
        />
      )}
      {options.map(({ valeur: v, libelle: texte, icone: Icone, libelleCache }) => {
        const actif = v === valeur
        return (
          <button
            key={v}
            ref={(e) => (refs.current[v] = e)}
            type="button"
            aria-pressed={actif}
            aria-label={libelleCache}
            title={libelleCache}
            onClick={() => onChange(v)}
            className={`relative inline-flex items-center justify-center gap-1.5 rounded-full font-medium whitespace-nowrap transition-colors duration-150 ${hauteur} ${
              actif ? 'text-texte' : 'text-texte-2 hover:text-texte'
            }`}
          >
            {Icone && <Icone aria-hidden="true" className="size-4 shrink-0" strokeWidth={1.75} />}
            {texte}
          </button>
        )
      })}
    </div>
  )
}
