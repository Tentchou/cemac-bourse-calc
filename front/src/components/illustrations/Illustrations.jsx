import { useEffect, useRef, useState } from 'react'
import './illustrations.css'

/**
 * Trois petites scènes animées (SVG + CSS, sans bibliothèque) qui montrent chaque étape
 * sans avoir à lire : choisir un titre, saisir l'opération, lire le calcul.
 * Couleurs : variables de la charte, donc justes en clair comme en sombre.
 * L'animation ne tourne que lorsque la scène est visible à l'écran.
 */

function useVisible() {
  const ref = useRef(null)
  const [visible, setVisible] = useState(false)
  useEffect(() => {
    const observateur = new IntersectionObserver(([entree]) => setVisible(entree.isIntersecting), { threshold: 0.3 })
    observateur.observe(ref.current)
    return () => observateur.disconnect()
  }, [])
  return [ref, visible]
}

function Scene({ children }) {
  const [ref, visible] = useVisible()
  return (
    <svg ref={ref} data-visible={visible} viewBox="0 0 240 150" className="illu block h-auto w-full" aria-hidden="true">
      {children}
    </svg>
  )
}

const TEXTE = 'var(--cbc-texte)'
const TEXTE_2 = 'var(--cbc-texte-2)'
const SURFACE = 'var(--cbc-surface)'
const ACTION = 'var(--cbc-action)'
const ORANGE = 'var(--cbc-orange)'
const BORDURE = 'var(--cbc-bordure)'

/** Étape 1 : une liste de titres, le curseur choisit la deuxième ligne. */
export function IllustrationChoisir() {
  const lignes = [
    { pastille: ORANGE, titre: 70, valeur: 28 },
    { pastille: ACTION, titre: 82, valeur: 24 },
    { pastille: TEXTE_2, titre: 60, valeur: 30 },
  ]
  return (
    <Scene>
      <rect x="16" y="12" width="208" height="126" rx="12" fill={SURFACE} stroke={BORDURE} />
      <rect className="illu-ligne-choisie" x="22" y="60" width="196" height="34" rx="9" fill={ACTION} fillOpacity="0.12" stroke={ACTION} strokeOpacity="0.5" />
      {lignes.map((l, i) => {
        const y = 24 + i * 38
        return (
          <g key={i}>
            <circle cx="42" cy={y + 15} r="6" fill={l.pastille} fillOpacity={i === 2 ? 0.35 : 0.9} />
            <rect x="56" y={y + 8} width={l.titre} height="6" rx="3" fill={TEXTE} fillOpacity="0.75" />
            <rect x="56" y={y + 18} width="44" height="4" rx="2" fill={TEXTE_2} fillOpacity="0.45" />
            <rect x={206 - l.valeur} y={y + 12} width={l.valeur} height="6" rx="3" fill={TEXTE} fillOpacity="0.6" />
          </g>
        )
      })}
      <circle className="illu-onde" cx="150" cy="84" r="12" fill="none" stroke={ACTION} strokeWidth="2" />
      <g className="illu-curseur">
        <path d="M0 0 L0 17 L4.6 12.6 L7.8 19.6 L10.6 18.4 L7.4 11.4 L13.4 11.4 Z" fill={TEXTE} stroke={SURFACE} strokeWidth="1.4" strokeLinejoin="round" />
      </g>
    </Scene>
  )
}

/** Étape 2 : trois champs (date, quantité, cours) se remplissent, puis on appuie sur Calculer. */
export function IllustrationSaisir() {
  const champs = [72, 28, 44]
  return (
    <Scene>
      {champs.map((largeur, i) => {
        const y = 14 + i * 34
        return (
          <g key={i}>
            <rect x="20" y={y} width="200" height="26" rx="9" fill={SURFACE} stroke="var(--cbc-bordure-champ)" />
            <rect className={`illu-focus-${i + 1}`} x="18" y={y - 2} width="204" height="30" rx="11" fill="none" stroke={ACTION} strokeWidth="2" strokeOpacity="0.9" />
            <rect x="30" y={y + 8} width="10" height="10" rx="3" fill={TEXTE_2} fillOpacity="0.45" />
            <rect className={`illu-texte-${i + 1}`} x="48" y={y + 10} width={largeur} height="6" rx="3" fill={TEXTE} fillOpacity="0.8" />
            {i === 1 && <rect x="186" y={y + 10} width="22" height="6" rx="3" fill={TEXTE_2} fillOpacity="0.4" />}
            {i === 2 && <rect x="198" y={y + 10} width="10" height="6" rx="3" fill={TEXTE_2} fillOpacity="0.4" />}
          </g>
        )
      })}
      <g className="illu-bouton">
        <rect x="20" y="118" width="200" height="24" rx="12" fill={ACTION} />
        <rect x="100" y="127" width="40" height="6" rx="3" fill="var(--cbc-sur-action)" />
      </g>
    </Scene>
  )
}

/** Étape 3 : le résultat s'affiche, sa composition se remplit, la formule se déroule, une coche valide. */
export function IllustrationLire() {
  return (
    <Scene>
      <rect x="16" y="12" width="208" height="126" rx="12" fill={SURFACE} stroke={ACTION} strokeOpacity="0.55" />
      <circle cx="34" cy="30" r="7" fill={ACTION} fillOpacity="0.15" />
      <rect x="48" y="27" width="54" height="6" rx="3" fill={TEXTE_2} fillOpacity="0.5" />
      <rect className="illu-montant" x="28" y="44" width="128" height="14" rx="4" fill={TEXTE} />
      <rect className="illu-barre-1" x="28" y="68" width="158" height="6" rx="3" fill={ACTION} />
      <rect className="illu-barre-2" x="189" y="68" width="19" height="6" rx="3" fill={ORANGE} />
      <rect x="28" y="84" width="180" height="42" rx="8" fill="var(--cbc-fond)" />
      <rect className="illu-formule-1" x="36" y="93" width="118" height="5" rx="2.5" fill={TEXTE} fillOpacity="0.55" />
      <rect className="illu-formule-2" x="36" y="103" width="146" height="5" rx="2.5" fill={TEXTE} fillOpacity="0.55" />
      <rect className="illu-formule-3" x="36" y="113" width="88" height="5" rx="2.5" fill={ACTION} fillOpacity="0.8" />
      <g className="illu-coche">
        <circle cx="198" cy="34" r="11" fill={ACTION} />
        <path d="M192.5 34.5 L196.5 38.5 L203.5 30.5" fill="none" stroke="var(--cbc-sur-action)" strokeWidth="2.4" strokeLinecap="round" strokeLinejoin="round" />
      </g>
    </Scene>
  )
}
