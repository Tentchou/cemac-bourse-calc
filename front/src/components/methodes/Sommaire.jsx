import { useEffect, useRef, useState } from 'react'

/**
 * Sommaire d'une longue page, qui suit la lecture (la section visible est mise en avant).
 *
 * Grand écran : colonne collante à gauche, une entrée par ligne, filet bleu sur l'entrée active.
 * Mobile et tablette : bande de pilules collée sous l'en-tête, qui défile dans sa propre bande
 * (jamais la page) et ramène l'entrée active en vue.
 *
 * entrees : [{ id, titre, icone }] — id : l'id de la section visée. libelle : nom du sommaire.
 */
export default function Sommaire({ entrees, libelle }) {
  const [actif, setActif] = useState(entrees[0]?.id)
  const nav = useRef(null)
  const liste = useRef(null)
  const ids = entrees.map((e) => e.id).join(' ')

  // La section active : la dernière dont le haut a franchi le tiers supérieur de l'écran.
  useEffect(() => {
    const sections = ids.split(' ').map((id) => document.getElementById(id)).filter(Boolean)
    const mettreAJour = () => {
      const seuil = window.innerHeight / 3
      let courant = sections[0]?.id
      for (const s of sections) if (s.getBoundingClientRect().top <= seuil) courant = s.id
      // En bas de page, la dernière section est active même si elle est courte.
      if (window.innerHeight + window.scrollY >= document.documentElement.scrollHeight - 2) courant = sections.at(-1)?.id
      setActif(courant)
    }
    mettreAJour()
    window.addEventListener('scroll', mettreAJour, { passive: true })
    window.addEventListener('resize', mettreAJour)
    return () => {
      window.removeEventListener('scroll', mettreAJour)
      window.removeEventListener('resize', mettreAJour)
    }
  }, [ids])

  // Mobile : la pilule active reste visible dans la bande (on fait défiler la bande seule, jamais la page).
  useEffect(() => {
    const bande = liste.current
    const pilule = document.getElementById(`sommaire-${actif}`)
    if (!bande || !pilule || bande.scrollWidth <= bande.clientWidth) return
    const gauche = pilule.offsetLeft - bande.offsetLeft
    if (gauche < bande.scrollLeft || gauche + pilule.offsetWidth > bande.scrollLeft + bande.clientWidth) {
      bande.scrollTo({ left: gauche - 16, behavior: 'smooth' })
    }
  }, [actif])

  function aller(evenement, id) {
    const cible = document.getElementById(id)
    if (!cible) return
    evenement.preventDefault()
    const doux = window.matchMedia?.('(prefers-reduced-motion: reduce)').matches ? 'auto' : 'smooth'
    // Le titre arrive juste sous l'en-tête (et sous la bande du sommaire sur mobile).
    const entete = document.querySelector('header')?.getBoundingClientRect().bottom ?? 0
    // Sur mobile, la bande sera collée sous l'en-tête une fois arrivé : on compte sa hauteur.
    const bande = window.innerWidth < 1024 ? (nav.current?.offsetHeight ?? 0) : 0
    const haut = cible.getBoundingClientRect().top + window.scrollY - entete - bande - 24
    window.scrollTo({ top: haut, behavior: doux })
    history.replaceState(history.state, '', `#${id}`)
    setActif(id)
  }

  return (
    <nav
      ref={nav}
      aria-label={libelle}
      className="sticky top-16 z-20 -mx-4 mb-8 lg:mb-0 border-b border-bordure/70 bg-fond/90 px-4 py-2 backdrop-blur-xl sm:-mx-6 sm:px-6 lg:top-24 lg:mx-0 lg:self-start lg:border-0 lg:bg-transparent lg:p-0 lg:backdrop-blur-none"
    >
      <p className="mb-3 hidden text-xs font-semibold tracking-wide text-texte-2 uppercase lg:block">{libelle}</p>
      <ul ref={liste} className="flex gap-2 overflow-x-auto [scrollbar-width:none] max-lg:[mask-image:linear-gradient(to_right,black_88%,transparent)] lg:flex-col lg:gap-0.5 lg:border-l lg:border-bordure [&::-webkit-scrollbar]:hidden">
        {entrees.map(({ id, titre, icone: Icone }) => {
          const estActif = id === actif
          return (
            <li key={id} className="shrink-0">
              <a
                id={`sommaire-${id}`}
                href={`#${id}`}
                onClick={(e) => aller(e, id)}
                aria-current={estActif ? 'location' : undefined}
                className={[
                  'flex items-center gap-2 text-sm font-medium transition-colors duration-150 ease-douce focus-visible:ring-2 focus-visible:ring-action focus-visible:outline-none',
                  // Mobile : pilule
                  'h-9 rounded-full border px-3.5 whitespace-nowrap',
                  // Grand écran : ligne, filet à gauche
                  'lg:-ml-px lg:h-auto lg:whitespace-normal lg:leading-5 lg:rounded-none lg:rounded-r-xl lg:border-0 lg:border-l-2 lg:py-2 lg:pr-3 lg:pl-4',
                  estActif
                    ? 'border-texte bg-texte text-fond lg:border-action lg:bg-action/8 lg:text-action'
                    : 'border-bordure bg-surface text-texte-2 hover:text-texte lg:border-transparent lg:bg-transparent lg:hover:border-bordure-champ',
                ].join(' ')}
              >
                <Icone aria-hidden="true" className="size-4 shrink-0" strokeWidth={1.75} />
                {titre}
              </a>
            </li>
          )
        })}
      </ul>
    </nav>
  )
}
