import { DRAPEAUX } from './drapeaux'

/** Drapeau d'un pays de la CEMAC (format 4:3, coins légèrement arrondis, filet fin). Décoratif. */
export default function Drapeau({ code, className = 'h-3.5' }) {
  if (!DRAPEAUX[code]) return null
  return (
    <img
      src={DRAPEAUX[code]}
      alt=""
      aria-hidden="true"
      width="20"
      height="15"
      className={`aspect-[4/3] w-auto shrink-0 rounded-[3px] object-cover ring-1 ring-black/10 ${className}`}
    />
  )
}
