import { X } from 'lucide-react'
import { useEffect, useId, useRef } from 'react'
import { createPortal } from 'react-dom'
import { usePreferences } from '../../contexte/Preferences'

const FOCALISABLES = 'a[href], button:not([disabled]), input:not([disabled]), select:not([disabled]), textarea:not([disabled]), [tabindex]:not([tabindex="-1"])'

/**
 * Panneau latéral (formulaire d'ajout, import) : glisse depuis la droite, 520 px sur grand écran,
 * tout l'écran sur mobile. Fenêtre modale accessible : le focus y entre et y reste (Tab boucle),
 * Échap ou un clic sur le voile le ferme, le focus revient ensuite à l'élément qui l'a ouvert.
 * La page derrière ne défile plus tant qu'il est ouvert.
 *
 * large : 1000 px au lieu de 520 (un tableau à lire). titre, aide : en-tête. pied : les boutons du bas (toujours visibles). occupe : bloque la fermeture
 * pendant un enregistrement.
 */
export default function Panneau({ ouvert, onFermer, titre, aide, pied, occupe = false, large = false, children }) {
  const { t } = usePreferences()
  const id = useId()
  const boite = useRef(null)
  const fermer = useRef(onFermer)
  useEffect(() => {
    fermer.current = occupe ? () => {} : onFermer
  }, [occupe, onFermer])

  useEffect(() => {
    if (!ouvert) return undefined
    const precedent = document.activeElement
    const defilement = document.body.style.overflow
    document.body.style.overflow = 'hidden'

    // Le premier champ reçoit le focus (sinon le bouton Fermer).
    const premier = boite.current?.querySelector(`.corps ${FOCALISABLES}`) ?? boite.current?.querySelector(FOCALISABLES)
    premier?.focus()

    function clavier(e) {
      if (e.key === 'Escape') {
        e.stopPropagation()
        fermer.current()
        return
      }
      if (e.key !== 'Tab' || !boite.current) return
      const elements = [...boite.current.querySelectorAll(FOCALISABLES)].filter((el) => el.offsetParent !== null)
      if (!elements.length) return
      const [debut, fin] = [elements[0], elements.at(-1)]
      if (e.shiftKey && document.activeElement === debut) {
        e.preventDefault()
        fin.focus()
      } else if (!e.shiftKey && document.activeElement === fin) {
        e.preventDefault()
        debut.focus()
      }
    }
    document.addEventListener('keydown', clavier)
    return () => {
      document.removeEventListener('keydown', clavier)
      document.body.style.overflow = defilement
      if (precedent instanceof HTMLElement) precedent.focus()
    }
  }, [ouvert])

  if (!ouvert) return null

  return createPortal(
    <div className="fixed inset-0 z-50">
      <div aria-hidden="true" className="voile-entree absolute inset-0 bg-[#0a1020]/45" onClick={() => fermer.current()} />
      <section
        ref={boite}
        role="dialog"
        aria-modal="true"
        aria-labelledby={`${id}-titre`}
        aria-describedby={aide ? `${id}-aide` : undefined}
        className={`panneau-entree absolute inset-y-0 right-0 flex w-full flex-col bg-surface shadow-haute sm:border-l sm:border-bordure ${
          large ? 'sm:w-[min(1000px,100%)]' : 'sm:w-[min(520px,100%)]'
        }`}
      >
        <header className="flex items-start gap-3 border-b border-bordure px-5 pt-5 pb-4">
          <div className="min-w-0 flex-1">
            <h2 id={`${id}-titre`} className="text-xl font-semibold">
              {titre}
            </h2>
            {aide && (
              <p id={`${id}-aide`} className="mt-1 text-sm text-texte-2">
                {aide}
              </p>
            )}
          </div>
          <button
            type="button"
            onClick={() => fermer.current()}
            disabled={occupe}
            aria-label={t('admin.fermer')}
            className="grid size-10 shrink-0 place-items-center rounded-full text-texte-2 transition-colors hover:bg-voile hover:text-texte disabled:opacity-40"
          >
            <X aria-hidden="true" className="size-5" strokeWidth={1.75} />
          </button>
        </header>
        <div className="corps min-h-0 flex-1 overflow-y-auto overscroll-contain px-5 py-5">{children}</div>
        {pied && <footer className="flex flex-wrap justify-end gap-2 border-t border-bordure px-5 py-4 max-sm:[&>*]:flex-1">{pied}</footer>}
      </section>
    </div>,
    document.body,
  )
}
