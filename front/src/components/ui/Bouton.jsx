import { LoaderCircle } from 'lucide-react'
import { Lien } from '../../routeur/Routeur'

/**
 * Le bouton unique du site, en forme de pilule.
 *
 * variante : action (bleu, l'action principale d'un écran — une seule par écran),
 *            secondaire (contour), fantome (sans contour, pour les liens d'appoint), whatsapp.
 * taille   : petite (40 px), normale (48 px, même hauteur qu'un champ), icone (rond de 40 px).
 * icone    : composant Lucide affiché avant le texte. chargement : remplace l'icône par un cercle qui tourne.
 * vers     : lien interne (sans rechargement) ; href : lien externe. Sinon, un <button>.
 */
const BASE =
  'relative inline-flex shrink-0 items-center justify-center gap-2 rounded-full font-medium whitespace-nowrap select-none ' +
  'transition-[background-color,border-color,color,box-shadow,transform,filter] duration-150 ease-douce ' +
  'active:scale-[0.97] disabled:pointer-events-none disabled:opacity-50 aria-disabled:pointer-events-none aria-disabled:opacity-50 ' +
  '[&_svg]:size-[1.125rem] [&_svg]:shrink-0'

const VARIANTES = {
  action: 'bg-action text-sur-action shadow-douce hover:shadow-haute hover:brightness-110',
  secondaire: 'border border-bordure-champ bg-surface text-texte hover:border-texte-2 hover:bg-voile',
  fantome: 'text-texte-2 hover:bg-voile hover:text-texte',
  whatsapp: 'bg-whatsapp text-sur-whatsapp shadow-douce hover:shadow-haute hover:brightness-110',
}

const TAILLES = {
  petite: 'h-10 px-4 text-sm',
  normale: 'h-12 px-6 text-[0.9375rem]',
  icone: 'size-10',
}

export default function Bouton({
  variante = 'secondaire',
  taille = 'normale',
  icone: Icone,
  chargement = false,
  vers,
  href,
  className = '',
  children,
  ...autres
}) {
  const classes = `${BASE} ${VARIANTES[variante]} ${TAILLES[taille]} ${className}`
  const contenu = (
    <>
      {chargement ? (
        <LoaderCircle aria-hidden="true" strokeWidth={2} className="animate-spin" />
      ) : (
        Icone && <Icone aria-hidden="true" strokeWidth={1.75} />
      )}
      {children}
    </>
  )

  if (vers) return <Lien vers={vers} className={classes} {...autres}>{contenu}</Lien>
  if (href) return <a href={href} className={classes} {...autres}>{contenu}</a>
  return (
    <button type="button" className={classes} aria-busy={chargement || undefined} {...autres}>
      {contenu}
    </button>
  )
}
