import { useEffect, useRef, useState } from 'react'
import { usePreferences } from '../../contexte/Preferences'
import { graduations } from './graduations'

const MARGE_GAUCHE = 64 // place des graduations
const MARGE_HAUTE = 28 // place de l'étiquette au-dessus de la plus haute barre
const HAUTEUR_TRACE = 200
const MARGE_BASSE = 32 // place des dates
const PAS_MINIMUM = 56

/**
 * Barres empilées par date de paiement : coupon (orange) en bas, capital (bleu action) au-dessus.
 * Axe gradué, lignes de repère pointillées, barres qui poussent depuis l'axe, survol qui met en
 * avant la barre. Dessiné à la taille réelle de son cadre (le texte garde sa taille) ; défile
 * dans son cadre s'il y a beaucoup d'échéances, jamais la page.
 */
export default function GraphiqueEcheancier({ lignes }) {
  const { t, nombre, date } = usePreferences()
  const cadre = useRef(null)
  const [largeurCadre, setLargeurCadre] = useState(0)
  const [survol, setSurvol] = useState(null)

  useEffect(() => {
    const observateur = new ResizeObserver(([e]) => setLargeurCadre(Math.floor(e.contentRect.width)))
    observateur.observe(cadre.current)
    return () => observateur.disconnect()
  }, [])

  const { haut, pas } = graduations(Math.max(...lignes.map((l) => l.total)))
  const largeur = Math.max(MARGE_GAUCHE + lignes.length * PAS_MINIMUM, largeurCadre)
  const pasX = (largeur - MARGE_GAUCHE) / lignes.length
  const barre = Math.min(40, pasX * 0.5)
  const y = (v) => MARGE_HAUTE + HAUTEUR_TRACE * (1 - v / haut)
  const libelleDate = (iso) => (lignes.length <= 12 ? `${iso.slice(5, 7)}/${iso.slice(0, 4)}` : iso.slice(0, 4))
  const reperes = Array.from({ length: Math.round(haut / pas) + 1 }, (_, i) => i * pas)

  return (
    <figure className="min-w-0">
      <figcaption className="mb-4 flex flex-wrap justify-end gap-x-5 gap-y-2 text-xs text-texte-2">
        <span className="inline-flex items-center gap-2">
          <span aria-hidden="true" className="size-2.5 rounded-full bg-orange" />
          {t('col.coupon')}
        </span>
        <span className="inline-flex items-center gap-2">
          <span aria-hidden="true" className="size-2.5 rounded-full bg-action" />
          {t('col.capital')}
        </span>
      </figcaption>

      <div ref={cadre} className="overflow-x-auto overscroll-x-contain [scrollbar-width:thin]">
        {largeurCadre > 0 && (
          <svg
            role="img"
            aria-label={t('ech.graphique')}
            width={largeur}
            height={MARGE_HAUTE + HAUTEUR_TRACE + MARGE_BASSE}
            className="chiffres block select-none"
            onMouseLeave={() => setSurvol(null)}
          >
            {/* Repères horizontaux et graduations */}
            {reperes.map((v) => (
              <g key={v}>
                <line x1={MARGE_GAUCHE} x2={largeur} y1={y(v)} y2={y(v)} stroke="var(--cbc-bordure)" strokeDasharray={v === 0 ? undefined : '3 5'} />
                <text x={MARGE_GAUCHE - 12} y={y(v) + 4} textAnchor="end" fontSize="11" fill="var(--cbc-texte-2)">
                  {nombre(v, 0)}
                </text>
              </g>
            ))}

            {lignes.map((ligne, i) => {
              const centre = MARGE_GAUCHE + pasX * i + pasX / 2
              const x = centre - barre / 2
              const hauteurCoupon = Math.max(y(0) - y(ligne.coupon), 4)
              const yCoupon = y(0) - hauteurCoupon
              const yTotal = Math.min(y(ligne.total), yCoupon)
              const actif = survol === i
              return (
                <g
                  key={ligne.rang}
                  onMouseEnter={() => setSurvol(i)}
                  className="transition-opacity duration-150"
                  style={{ opacity: survol !== null && !actif ? 0.45 : 1 }}
                >
                  <title>{`${date(ligne.datePaiement)} — ${t('col.coupon')} ${nombre(ligne.coupon)} · ${t('col.capital')} ${nombre(ligne.capital)}`}</title>
                  {/* Zone de survol sur toute la hauteur de la colonne */}
                  <rect x={centre - pasX / 2 + 4} y={MARGE_HAUTE - 20} width={pasX - 8} height={HAUTEUR_TRACE + 20} rx="10" fill={actif ? 'var(--cbc-voile)' : 'transparent'} />
                  <g className="pousse" style={{ '--rang': i }}>
                    {ligne.capital > 0 && <rect x={x} y={yTotal} width={barre} height={Math.max(yCoupon - yTotal - 2, 0)} rx="6" fill="var(--cbc-action)" />}
                    <rect x={x} y={yCoupon} width={barre} height={hauteurCoupon} rx={Math.min(6, hauteurCoupon / 2)} fill="var(--cbc-orange)" />
                  </g>
                  <text x={centre} y={yTotal - 9} textAnchor="middle" fontSize="11" fontWeight={actif ? 700 : 600} fill="var(--cbc-texte)">
                    {nombre(ligne.total, 0)}
                  </text>
                  <text x={centre} y={y(0) + 20} textAnchor="middle" fontSize="11" fill={actif ? 'var(--cbc-texte)' : 'var(--cbc-texte-2)'}>
                    {libelleDate(ligne.datePaiement)}
                  </text>
                </g>
              )
            })}
          </svg>
        )}
      </div>
    </figure>
  )
}
