import { CalendarDays, CircleAlert } from 'lucide-react'
import { useEffect, useId, useRef, useState } from 'react'
import { createPortal } from 'react-dom'
import { usePreferences } from '../../../contexte/Preferences'
import Calendrier from './Calendrier'
import { dansBornes, estWeekend, formaterSaisie, lireIso, lireSaisie, masquerSaisie } from './dates'

const HAUTEUR_CALENDRIER = 440
const LARGEUR_CALENDRIER = 336
const LARGEUR_FEUILLE = 640 // en dessous : feuille en bas de l'écran
const MARGE = 8

/** Coordonnées (fixes, en px) du calendrier collé au champ `el`, sans sortir de l'écran. */
function placeDe(el, position) {
  const r = el.getBoundingClientRect()
  return {
    position,
    haut: r.bottom + MARGE,
    bas: window.innerHeight - r.top + MARGE,
    gauche: Math.max(16, Math.min(r.left, window.innerWidth - LARGEUR_CALENDRIER - 16)),
  }
}

/**
 * Champ de date : on peut TAPER la date (les barres se posent seules : « 12012026 » → 12/01/2026)
 * ou la CHOISIR dans le calendrier (bouton à droite, ou Alt + ↓ au clavier).
 * Même gabarit que Champ : libellé au-dessus, 48 px, bordure marquée, focus bleu, erreur rouge.
 *
 * valeur : date ISO « AAAA-MM-JJ » ou ''. onChange(iso | '') : appelé seulement avec une date
 * valide et dans les bornes (ou '' si le champ est vidé / invalide).
 * min, max : bornes ISO incluses (ex. jouissance et échéance d'une obligation).
 * joursOuvres : refuse samedis et dimanches (pas de séance BVMAC), au calendrier comme à la frappe.
 * aide : phrase sous le champ. erreur : message venu du formulaire (une erreur de frappe passe avant).
 */
export default function ChampDate({ libelle, valeur, onChange, min, max, joursOuvres = false, aide, erreur, required, className = '' }) {
  const { t, date, langue } = usePreferences()
  const id = useId()
  const idAide = `${id}-aide`
  const idCalendrier = `${id}-calendrier`
  const racine = useRef(null)
  const bouton = useRef(null)
  const boite = useRef(null)
  const fenetre = useRef(null)

  const [texte, setTexte] = useState(() => formaterSaisie(valeur))
  const [valeurVue, setValeurVue] = useState(valeur)
  const [erreurFrappe, setErreurFrappe] = useState(null)
  const [ouvert, setOuvert] = useState(false)
  const [place, setPlace] = useState({ position: 'bas', haut: 0, bas: 0, gauche: 0 })

  // La valeur change de l'extérieur (calendrier, lien partagé) : la zone de saisie suit.
  // Exception : une date refusée reste écrite, sous son message, pour être corrigée.
  if (valeur !== valeurVue) {
    setValeurVue(valeur)
    if (valeur ? lireSaisie(texte) !== valeur : !erreurFrappe) setTexte(formaterSaisie(valeur))
  }

  /** Vérifie une date lue : rend le message d'erreur, ou null. */
  function controler(iso) {
    if (!iso) return t('date.invalide')
    if (min && iso < min) return t('date.avant', { date: date(min) })
    if (max && iso > max) return t('date.apres', { date: date(max) })
    if (joursOuvres && estWeekend(iso)) return t('date.weekendInterdit')
    return null
  }

  function frappe(e) {
    const masque = masquerSaisie(e.target.value)
    setTexte(masque)
    // Dès que la date est complète et juste, le calcul peut la prendre (pas besoin de quitter le champ).
    if (masque.length === 10) {
      const iso = lireSaisie(masque)
      const probleme = controler(iso)
      setErreurFrappe(probleme)
      if (!probleme) onChange(iso)
    } else if (erreurFrappe) {
      setErreurFrappe(null)
    }
  }

  function quitter() {
    if (!texte) {
      setErreurFrappe(null)
      onChange('')
      return
    }
    const iso = lireSaisie(texte)
    const probleme = controler(iso)
    setErreurFrappe(probleme)
    if (probleme) onChange('')
    else {
      setTexte(formaterSaisie(iso))
      onChange(iso)
    }
  }

  /**
   * Ouvre le calendrier SOUS le champ ; s'il n'y a pas la place, la page défile juste ce qu'il
   * faut (sans cacher le champ sous l'en-tête) ; sinon, au-dessus si là il tient en entier.
   */
  function ouvrir() {
    let position = 'bas'
    if (window.innerWidth >= LARGEUR_FEUILLE) {
      const r = boite.current.getBoundingClientRect()
      const plafond = (document.querySelector('header')?.getBoundingClientRect().bottom ?? 0) + MARGE
      const manque = r.bottom + MARGE + HAUTEUR_CALENDRIER + MARGE * 2 - window.innerHeight
      const doux = window.matchMedia?.('(prefers-reduced-motion: reduce)').matches ? 'auto' : 'smooth'
      if (manque > 0 && r.top - manque >= plafond) window.scrollBy({ top: manque, behavior: doux })
      else if (manque > 0 && r.top - plafond >= HAUTEUR_CALENDRIER + MARGE) position = 'haut'
      else if (manque > 0) window.scrollBy({ top: r.top - plafond, behavior: doux })
    }
    setPlace(placeDe(boite.current, position))
    setOuvert(true)
  }

  function fermer({ rendreFocus } = {}) {
    setOuvert(false)
    if (rendreFocus) bouton.current?.focus()
  }

  function choisir(iso) {
    setErreurFrappe(null)
    setTexte(formaterSaisie(iso))
    onChange(iso)
    fermer({ rendreFocus: true })
  }

  // Ouvert : un clic hors du champ et du calendrier le referme ; il suit le champ au défilement.
  useEffect(() => {
    if (!ouvert) return undefined
    const dehors = (e) => {
      if (!racine.current?.contains(e.target) && !fenetre.current?.contains(e.target)) setOuvert(false)
    }
    const suivre = () => setPlace((p) => placeDe(boite.current, p.position))
    document.addEventListener('pointerdown', dehors)
    window.addEventListener('scroll', suivre, { capture: true, passive: true })
    window.addEventListener('resize', suivre)
    return () => {
      document.removeEventListener('pointerdown', dehors)
      window.removeEventListener('scroll', suivre, { capture: true })
      window.removeEventListener('resize', suivre)
    }
  }, [ouvert])

  const message = erreurFrappe ?? erreur
  const valide = lireIso(valeur) && dansBornes(valeur, min, max)
  const etat = message
    ? 'border-erreur ring-4 ring-erreur/12'
    : ouvert
      ? 'border-action ring-4 ring-action/15'
      : 'border-bordure-champ hover:border-texte-2 focus-within:border-action focus-within:ring-4 focus-within:ring-action/15'
  const jourCourt = valide
    ? new Date(`${valeur}T00:00:00Z`).toLocaleDateString(langue === 'fr' ? 'fr-FR' : 'en-GB', { weekday: 'short', timeZone: 'UTC' })
    : null

  return (
    <div ref={racine} className={`relative min-w-0 ${className}`}>
      <label htmlFor={id} className="mb-2 block text-sm font-medium text-texte">
        {libelle}
      </label>
      <div ref={boite} className={`group flex h-12 items-center gap-3 rounded-2xl border bg-surface pr-1.5 pl-4 transition-[border-color,box-shadow] duration-150 ease-douce ${etat}`}>
        <CalendarDays
          aria-hidden="true"
          strokeWidth={1.75}
          className={`size-5 shrink-0 transition-colors duration-150 ${message ? 'text-erreur' : ouvert ? 'text-action' : 'text-texte-2 group-focus-within:text-action'}`}
        />
        <input
          id={id}
          type="text"
          inputMode="numeric"
          autoComplete="off"
          maxLength={10}
          placeholder={t('date.format')}
          value={texte}
          onChange={frappe}
          onBlur={quitter}
          onKeyDown={(e) => {
            if (e.key === 'ArrowDown' && e.altKey) {
              e.preventDefault()
              ouvrir()
            }
            if (e.key === 'Enter') quitter()
          }}
          required={required}
          aria-invalid={message ? true : undefined}
          aria-describedby={aide || message ? idAide : undefined}
          className="chiffres h-full min-w-0 flex-1 bg-transparent text-base text-texte outline-none placeholder:text-texte-2/70"
        />
        {jourCourt && (
          <span aria-hidden="true" className="hidden shrink-0 text-sm text-texte-2 capitalize min-[400px]:inline">
            {jourCourt.replace('.', '')}
          </span>
        )}
        <button
          ref={bouton}
          type="button"
          onClick={() => (ouvert ? fermer() : ouvrir())}
          aria-label={t('date.ouvrir')}
          title={t('date.ouvrir')}
          aria-haspopup="dialog"
          aria-expanded={ouvert}
          aria-controls={ouvert ? idCalendrier : undefined}
          className={`grid size-9 shrink-0 place-items-center rounded-full transition-colors duration-150 focus-visible:ring-2 focus-visible:ring-action focus-visible:outline-none ${
            ouvert ? 'bg-action text-sur-action' : 'bg-voile text-texte-2 hover:bg-action/12 hover:text-action'
          }`}
        >
          <CalendarDays aria-hidden="true" className="size-4.5" strokeWidth={2} />
        </button>
      </div>

      {(message || aide) && (
        <p id={idAide} aria-live="polite" className={`mt-2 flex items-start gap-1.5 text-xs leading-5 ${message ? 'text-erreur' : 'text-texte-2'}`}>
          {message && <CircleAlert aria-hidden="true" className="mt-0.5 size-3.5 shrink-0" strokeWidth={2} />}
          {message || aide}
        </p>
      )}

      {ouvert &&
        createPortal(
          <Calendrier
            id={idCalendrier}
            refFenetre={fenetre}
            valeur={valide ? valeur : ''}
            min={min}
            max={max}
            joursOuvres={joursOuvres}
            libelle={libelle}
            place={place}
            onChoisir={choisir}
            onFermer={fermer}
          />,
          document.body,
        )}
    </div>
  )
}
