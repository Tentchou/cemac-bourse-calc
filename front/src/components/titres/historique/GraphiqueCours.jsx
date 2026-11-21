import { useEffect, useId, useMemo, useRef, useState } from 'react'
import { usePreferences } from '../../../contexte/Preferences'
import { axeCours, indiceProche, indicesDates, reperes } from './historique'

const MARGE_GAUCHE = 60 // place des graduations
const MARGE_DROITE = 14 // le dernier point (et son halo) reste dans le cadre
const MARGE_HAUTE = 12
const ECART_VOLUME = 14 // entre la courbe et les barres de volume
const HAUTEUR_VOLUME = 40
const MARGE_BASSE = 28 // place des dates
const LARGEUR_BULLE = 184
const ESPACE_DATE = 110 // largeur réservée à une date sous l'axe (« 12/01/2026 » et son espace) : jamais de chevauchement

/** Nombre de décimales d'un pas d'axe (1 → 0, 0,5 → 1, 0,25 → 2). */
function decimalesPas(pas) {
  let d = 0
  while (d < 4 && Math.abs(Math.round(pas * 10 ** d) - pas * 10 ** d) > 1e-9) d += 1
  return d
}

/**
 * La courbe des cours de clôture, séance par séance (les séances sont régulièrement espacées :
 * pas de trou pour les week-ends), avec la surface teintée dessous et les volumes en barres.
 * Survol à la souris, au doigt ou au clavier (flèches, Début, Fin) : un repère vertical et une
 * bulle donnent la date, le cours et le volume. Dessinée à la taille réelle de son cadre.
 *
 * points : [{ dateSeance, cours, volume }], de la plus ancienne à la plus récente (au moins 2).
 * formatCours(v) : le cours tel qu'affiché (« 100,50 % » ou « 32 000 FCFA »).
 */
export default function GraphiqueCours({ points, formatCours, libelle }) {
  const { t, nombre, date } = usePreferences()
  const cadre = useRef(null)
  const [largeur, setLargeur] = useState(0)
  const [actif, setActif] = useState(null)
  const idDegrade = `degrade-${useId().replace(/[^a-zA-Z0-9_-]/g, '')}`

  useEffect(() => {
    const observateur = new ResizeObserver(([e]) => setLargeur(Math.floor(e.contentRect.width)))
    observateur.observe(cadre.current)
    return () => observateur.disconnect()
  }, [])

  const hauteurCourbe = largeur && largeur < 640 ? 180 : 240
  const basCourbe = MARGE_HAUTE + hauteurCourbe
  const hautVolume = basCourbe + ECART_VOLUME
  const hauteur = hautVolume + HAUTEUR_VOLUME + MARGE_BASSE
  const n = points.length
  const pasX = n > 1 ? (largeur - MARGE_GAUCHE - MARGE_DROITE) / (n - 1) : 0

  // Tout le dessin est calculé en un passage, et seulement quand les points ou la largeur changent :
  // le survol ne redessine que le repère et la bulle.
  const dessin = useMemo(() => {
    if (!largeur || n < 2) return null
    let min = Infinity
    let max = -Infinity
    let volumeMax = 0
    for (const p of points) {
      if (p.cours < min) min = p.cours
      if (p.cours > max) max = p.cours
      if (p.volume > volumeMax) volumeMax = p.volume
    }
    const axe = axeCours(min, max)
    const y = (v) => MARGE_HAUTE + hauteurCourbe * (1 - (v - axe.bas) / (axe.haut - axe.bas))
    const x = (i) => MARGE_GAUCHE + i * pasX

    let ligne = ''
    for (let i = 0; i < n; i += 1) ligne += `${i ? 'L' : 'M'}${x(i).toFixed(1)},${y(points[i].cours).toFixed(1)}`
    const surface = `${ligne}L${x(n - 1).toFixed(1)},${basCourbe}L${x(0).toFixed(1)},${basCourbe}Z`

    // Les volumes : un seul chemin pour toutes les barres (des centaines de séances restent légères).
    const largeurBarre = Math.max(1, Math.min(8, pasX * 0.6))
    let volumes = ''
    if (volumeMax > 0) {
      for (let i = 0; i < n; i += 1) {
        const v = points[i].volume
        if (!(v > 0)) continue
        const h = Math.max(2, (v / volumeMax) * HAUTEUR_VOLUME)
        volumes += `M${(x(i) - largeurBarre / 2).toFixed(1)},${(hautVolume + HAUTEUR_VOLUME - h).toFixed(1)}h${largeurBarre.toFixed(1)}v${h.toFixed(1)}h${(-largeurBarre).toFixed(1)}Z`
      }
    }
    return { axe, y, x, ligne, surface, volumes, graduations: reperes(axe), decimales: decimalesPas(axe.pas) }
  }, [points, largeur, n, pasX, hauteurCourbe, basCourbe, hautVolume])

  const choisir = (evenement) => {
    const rect = evenement.currentTarget.getBoundingClientRect()
    setActif(indiceProche(evenement.clientX - rect.left, MARGE_GAUCHE, pasX, n))
  }

  const clavier = (evenement) => {
    const touches = { ArrowLeft: -1, ArrowRight: 1, ArrowDown: -1, ArrowUp: 1 }
    let suivant = null
    if (evenement.key in touches) suivant = Math.min(Math.max((actif ?? n - 1) + touches[evenement.key], 0), n - 1)
    else if (evenement.key === 'Home') suivant = 0
    else if (evenement.key === 'End') suivant = n - 1
    else if (evenement.key === 'Escape') suivant = null
    else return
    evenement.preventDefault()
    setActif(suivant)
  }

  const point = actif !== null && dessin ? points[actif] : null
  const xActif = point ? dessin.x(actif) : 0
  const dernier = points[n - 1]
  const description = (p) =>
    `${date(p.dateSeance)} : ${formatCours(p.cours)}, ${p.volume > 0 ? t('hist.titresEchanges', { n: nombre(p.volume, 0) }) : t('hist.sansEchange')}`

  return (
    <figure className="min-w-0">
      <div ref={cadre} className="relative">
        {dessin && (
          <svg
            role="img"
            aria-label={`${libelle}. ${t('hist.clavier')}`}
            tabIndex={0}
            width={largeur}
            height={hauteur}
            className="chiffres block touch-pan-y rounded-lg select-none focus-visible:outline-offset-4"
            onPointerMove={choisir}
            onPointerDown={choisir}
            onPointerLeave={(e) => e.pointerType === 'mouse' && setActif(null)}
            onKeyDown={clavier}
            onBlur={() => setActif(null)}
          >
            <defs>
              <linearGradient id={idDegrade} x1="0" x2="0" y1="0" y2="1">
                <stop offset="0%" stopColor="var(--cbc-action)" stopOpacity="0.22" />
                <stop offset="100%" stopColor="var(--cbc-action)" stopOpacity="0" />
              </linearGradient>
            </defs>

            {/* Repères horizontaux et graduations */}
            {dessin.graduations.map((v) => (
              <g key={v}>
                <line x1={MARGE_GAUCHE} x2={largeur - MARGE_DROITE} y1={dessin.y(v)} y2={dessin.y(v)} stroke="var(--cbc-bordure)" strokeDasharray="3 5" />
                <text x={MARGE_GAUCHE - 10} y={dessin.y(v) + 4} textAnchor="end" fontSize="11" fill="var(--cbc-texte-2)">
                  {nombre(v, dessin.decimales)}
                </text>
              </g>
            ))}

            <path d={dessin.surface} fill={`url(#${idDegrade})`} className="fondu" />
            <path key={`${n}-${dernier.dateSeance}`} d={dessin.ligne} pathLength="1" fill="none" stroke="var(--cbc-action)" strokeWidth="2" strokeLinejoin="round" strokeLinecap="round" className="trace" />

            {/* Volumes, sous la courbe, sur leur propre ligne de base */}
            <line x1={MARGE_GAUCHE} x2={largeur - MARGE_DROITE} y1={hautVolume + HAUTEUR_VOLUME} y2={hautVolume + HAUTEUR_VOLUME} stroke="var(--cbc-bordure)" />
            {dessin.volumes && <path d={dessin.volumes} fill="var(--cbc-texte-2)" opacity="0.45" />}
            <text x={MARGE_GAUCHE - 10} y={hautVolume + HAUTEUR_VOLUME / 2 + 4} textAnchor="end" fontSize="11" fill="var(--cbc-texte-2)">
              {t('hist.vol')}
            </text>

            {/* Dates : la première et la dernière ancrées au bord, pour ne jamais sortir du cadre */}
            {indicesDates(n, Math.min(5, Math.floor((largeur - MARGE_GAUCHE - MARGE_DROITE) / ESPACE_DATE))).map((i, k, tous) => (
              <text
                key={i}
                x={dessin.x(i)}
                y={hauteur - 8}
                textAnchor={k === 0 ? 'start' : k === tous.length - 1 ? 'end' : 'middle'}
                fontSize="11"
                fill="var(--cbc-texte-2)"
              >
                {date(points[i].dateSeance)}
              </text>
            ))}

            {/* Le dernier cours, en orange (pastille de la charte) */}
            <circle cx={dessin.x(n - 1)} cy={dessin.y(dernier.cours)} r="4.5" fill="var(--cbc-orange)" stroke="var(--cbc-surface)" strokeWidth="2" />

            {point && (
              <g pointerEvents="none">
                <line x1={xActif} x2={xActif} y1={MARGE_HAUTE} y2={hautVolume + HAUTEUR_VOLUME} stroke="var(--cbc-texte-2)" strokeDasharray="2 4" />
                <circle cx={xActif} cy={dessin.y(point.cours)} r="5.5" fill="var(--cbc-action)" stroke="var(--cbc-surface)" strokeWidth="2.5" />
              </g>
            )}
          </svg>
        )}

        {point && (
          <div
            aria-hidden="true"
            className="pointer-events-none absolute top-0 rounded-xl border border-bordure bg-surface px-3 py-2 text-sm shadow-haute"
            style={{ width: LARGEUR_BULLE, left: Math.min(Math.max(xActif - LARGEUR_BULLE / 2, 0), Math.max(largeur - LARGEUR_BULLE, 0)) }}
          >
            <p className="text-xs text-texte-2">{date(point.dateSeance)}</p>
            <p className="chiffres font-titre text-base font-bold">{formatCours(point.cours)}</p>
            <p className="chiffres text-xs text-texte-2">{point.volume > 0 ? t('hist.titresEchanges', { n: nombre(point.volume, 0) }) : t('hist.sansEchange')}</p>
          </div>
        )}

        {/* Annonce du point choisi au clavier, pour les lecteurs d'écran */}
        <p className="sr-only" aria-live="polite">
          {point ? description(point) : ''}
        </p>
      </div>
    </figure>
  )
}
