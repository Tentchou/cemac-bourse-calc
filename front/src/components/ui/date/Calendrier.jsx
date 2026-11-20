import { ChevronDown, ChevronLeft, ChevronRight, X } from 'lucide-react'
import { useEffect, useMemo, useRef, useState } from 'react'
import { usePreferences } from '../../../contexte/Preferences'
import { ajouterJours, ajouterMois, aujourdhui, borner, dansBornes, estWeekend, grilleMois, jourSemaine, joursDansMois, lireIso, versIso } from './dates'

const LOCALES = { fr: 'fr-FR', en: 'en-GB' }
const enUtc = (iso) => {
  const { a, m, j } = lireIso(iso)
  return new Date(Date.UTC(a, m - 1, j))
}

/** Noms des mois et des jours dans la langue du visiteur (lundi en tête). */
function useNoms(langue) {
  return useMemo(() => {
    const locale = LOCALES[langue]
    const mois = (style) => Array.from({ length: 12 }, (_, i) => new Date(Date.UTC(2026, i, 1)).toLocaleDateString(locale, { month: style, timeZone: 'UTC' }))
    // 2026-01-12 est un lundi.
    const jours = (style) => Array.from({ length: 7 }, (_, i) => new Date(Date.UTC(2026, 0, 12 + i)).toLocaleDateString(locale, { weekday: style, timeZone: 'UTC' }))
    const majuscule = (s) => s.charAt(0).toUpperCase() + s.slice(1)
    return {
      moisLongs: mois('long').map(majuscule),
      moisCourts: mois('short').map((s) => majuscule(s.replace('.', ''))),
      joursCourts: jours('short').map((s) => majuscule(s.replace('.', ''))),
      joursLongs: jours('long'),
      dateLongue: (iso) => enUtc(iso).toLocaleDateString(locale, { weekday: 'long', day: 'numeric', month: 'long', year: 'numeric', timeZone: 'UTC' }),
    }
  }, [langue])
}

/** Bouton rond de navigation (mois / années précédents et suivants). */
function BoutonNav({ icone: Icone, libelle, onClick, disabled }) {
  return (
    <button
      type="button"
      onClick={onClick}
      disabled={disabled}
      aria-label={libelle}
      title={libelle}
      className="grid size-9 place-items-center rounded-full text-texte-2 transition-colors duration-150 hover:bg-voile hover:text-texte focus-visible:ring-2 focus-visible:ring-action focus-visible:outline-none disabled:pointer-events-none disabled:opacity-30"
    >
      <Icone aria-hidden="true" className="size-4.5" strokeWidth={2} />
    </button>
  )
}

/**
 * Le calendrier d'un ChampDate, en trois vues : jours, mois, années.
 *
 *  - semaine du lundi au dimanche, week-ends grisés (pas de séance BVMAC), aujourd'hui cerclé ;
 *  - dates hors bornes (min / max) inactives, mois et années hors bornes aussi ;
 *  - clavier complet : flèches (jour / semaine), Début / Fin (semaine), Pg préc. / Pg suiv.
 *    (mois, + Maj : année), Entrée pour choisir, Échap pour fermer ;
 *  - rendu dans un portail (au-dessus de tout) : fenêtre collée au champ sur grand écran,
 *    feuille montant du bas sur mobile.
 *
 * valeur : date ISO choisie (ou vide). onChoisir(iso). onFermer({ rendreFocus }).
 * joursOuvres : samedis et dimanches non choisissables (pas de séance BVMAC).
 * libelle : titre de la feuille mobile. place : { position: 'bas' | 'haut', haut, bas, gauche } en px.
 * refFenetre : reçoit l'élément de la fenêtre (pour détecter un clic à l'extérieur).
 */
export default function Calendrier({ id, valeur, min, max, joursOuvres = false, onChoisir, onFermer, libelle, place, refFenetre }) {
  const { t, langue } = usePreferences()
  const noms = useNoms(langue)
  const jour = aujourdhui()
  const depart = borner(lireIso(valeur) ? valeur : jour, min, max)

  const [vue, setVue] = useState('jours')
  const [focus, setFocus] = useState(depart)
  const [sens, setSens] = useState(1)
  const [debutAnnees, setDebutAnnees] = useState(() => lireIso(depart).a - 5)
  const grilleRef = useRef(null)
  const doitFocaliser = useRef(true)

  const { a, m } = lireIso(focus)
  const cases = useMemo(() => grilleMois(a, m), [a, m])
  const anneeMin = min ? lireIso(min).a : 1900
  const anneeMax = max ? lireIso(max).a : 2199
  // Un mois est disponible s'il contient au moins un jour dans les bornes.
  const moisDispo = (an, mo) => (!min || versIso(an, mo, joursDansMois(an, mo)) >= min) && (!max || versIso(an, mo, 1) <= max)
  const voisin = (n) => lireIso(ajouterMois(versIso(a, m, 1), n))
  const precedent = voisin(-1)
  const suivant = voisin(1)

  // Le jour qui a le focus clavier reçoit vraiment le focus (ouverture, flèches, changement de mois).
  useEffect(() => {
    if (vue !== 'jours' || !doitFocaliser.current) return
    grilleRef.current?.querySelector(`[data-iso="${focus}"]`)?.focus({ preventScroll: true })
  }, [focus, vue])

  function deplacer(cible, focaliser = true) {
    const bornee = borner(cible, min, max)
    if (bornee.slice(0, 7) !== focus.slice(0, 7)) setSens(bornee > focus ? 1 : -1)
    doitFocaliser.current = focaliser
    setFocus(bornee)
  }

  function changerMois(n) {
    deplacer(ajouterMois(focus, n), false)
  }

  function clavierGrille(e) {
    const actions = {
      ArrowLeft: () => ajouterJours(focus, -1),
      ArrowRight: () => ajouterJours(focus, 1),
      ArrowUp: () => ajouterJours(focus, -7),
      ArrowDown: () => ajouterJours(focus, 7),
      Home: () => ajouterJours(focus, -jourSemaine(focus)),
      End: () => ajouterJours(focus, 6 - jourSemaine(focus)),
      PageUp: () => ajouterMois(focus, e.shiftKey ? -12 : -1),
      PageDown: () => ajouterMois(focus, e.shiftKey ? 12 : 1),
    }
    if (!actions[e.key]) return
    e.preventDefault()
    deplacer(actions[e.key]())
  }

  function clavierFenetre(e) {
    if (e.key !== 'Escape') return
    e.stopPropagation()
    if (vue !== 'jours') setVue('jours')
    else onFermer({ rendreFocus: true })
  }

  const placement = place.position === 'haut' ? 'sm:top-auto sm:bottom-(--bas)' : 'sm:top-(--haut) sm:bottom-auto'
  const titreVue =
    vue === 'annees' ? `${debutAnnees} – ${debutAnnees + 11}` : vue === 'mois' ? String(a) : `${noms.moisLongs[m - 1]} ${a}`

  return (
    <>
      {/* Fond assombri, sur mobile seulement (la feuille couvre le bas de l'écran) */}
      <div aria-hidden="true" className="fondu fixed inset-0 z-40 bg-[#14213D]/40 sm:hidden" onClick={() => onFermer({ rendreFocus: false })} />

      <div
        id={id}
        role="dialog"
        aria-modal="false"
        aria-label={libelle}
        ref={refFenetre}
        onKeyDown={clavierFenetre}
        style={{ '--haut': `${place.haut}px`, '--bas': `${place.bas}px`, '--gauche': `${place.gauche}px` }}
        className={`surgit fixed inset-x-0 bottom-0 z-50 rounded-t-3xl border border-bordure bg-surface p-4 pb-[max(1rem,env(safe-area-inset-bottom))] text-texte shadow-haute sm:right-auto sm:left-(--gauche) sm:w-[336px] sm:rounded-2xl sm:pb-4 ${placement}`}
      >
        {/* En-tête de la feuille mobile */}
        <div className="mb-3 flex items-center justify-between sm:hidden">
          <span aria-hidden="true" className="absolute top-2 left-1/2 h-1 w-10 -translate-x-1/2 rounded-full bg-bordure" />
          <p className="pt-2 text-sm font-semibold">{libelle}</p>
          <BoutonNav icone={X} libelle={t('date.fermer')} onClick={() => onFermer({ rendreFocus: true })} />
        </div>

        {/* Navigation : vue courante + précédent / suivant */}
        <div className="flex items-center justify-between gap-2">
          <button
            type="button"
            onClick={() => setVue(vue === 'jours' ? 'mois' : vue === 'mois' ? 'annees' : 'jours')}
            aria-label={vue === 'jours' ? t('date.choisirMois') : t('date.choisirAnnee')}
            aria-live="polite"
            className="inline-flex h-9 items-center gap-1.5 rounded-full px-3 font-titre text-[0.9375rem] font-semibold transition-colors duration-150 hover:bg-voile focus-visible:ring-2 focus-visible:ring-action focus-visible:outline-none"
          >
            {titreVue}
            <ChevronDown aria-hidden="true" className={`size-4 text-texte-2 transition-transform duration-150 ${vue !== 'jours' ? 'rotate-180' : ''}`} strokeWidth={2} />
          </button>
          <div className="flex items-center">
            {vue === 'jours' && (
              <>
                <BoutonNav icone={ChevronLeft} libelle={t('date.moisPrecedent')} onClick={() => changerMois(-1)} disabled={!moisDispo(precedent.a, precedent.m)} />
                <BoutonNav icone={ChevronRight} libelle={t('date.moisSuivant')} onClick={() => changerMois(1)} disabled={!moisDispo(suivant.a, suivant.m)} />
              </>
            )}
            {vue === 'mois' && (
              <>
                <BoutonNav icone={ChevronLeft} libelle={t('date.periodePrecedente')} onClick={() => changerMois(-12)} disabled={a - 1 < anneeMin} />
                <BoutonNav icone={ChevronRight} libelle={t('date.periodeSuivante')} onClick={() => changerMois(12)} disabled={a + 1 > anneeMax} />
              </>
            )}
            {vue === 'annees' && (
              <>
                <BoutonNav icone={ChevronLeft} libelle={t('date.periodePrecedente')} onClick={() => setDebutAnnees((d) => d - 12)} disabled={debutAnnees - 1 < anneeMin} />
                <BoutonNav icone={ChevronRight} libelle={t('date.periodeSuivante')} onClick={() => setDebutAnnees((d) => d + 12)} disabled={debutAnnees + 12 > anneeMax} />
              </>
            )}
          </div>
        </div>

        {/* Vue jours */}
        {vue === 'jours' && (
          <div key={`${a}-${m}`} className="glisse mt-3" style={{ '--sens': sens }}>
            <div role="grid" ref={grilleRef} onKeyDown={clavierGrille} aria-label={`${noms.moisLongs[m - 1]} ${a}`}>
              <div role="row" className="grid grid-cols-7">
                {noms.joursCourts.map((nom, i) => (
                  <span
                    key={nom}
                    role="columnheader"
                    aria-label={noms.joursLongs[i]}
                    className={`grid h-8 place-items-center text-[0.6875rem] font-semibold tracking-wide uppercase ${i >= 5 ? 'text-texte-2/60' : 'text-texte-2'}`}
                  >
                    {nom.slice(0, langue === 'fr' ? 3 : 2)}
                  </span>
                ))}
              </div>
              {Array.from({ length: 6 }, (_, ligne) => (
                <div role="row" key={ligne} className="grid grid-cols-7">
                  {cases.slice(ligne * 7, ligne * 7 + 7).map(({ iso, jour: numero, horsMois }) => {
                    const choisi = iso === valeur
                    const actif = dansBornes(iso, min, max)
                    const weekend = estWeekend(iso)
                    // Week-end bloqué : il garde le focus clavier (on le traverse aux flèches) mais ne se choisit pas.
                    const bloque = joursOuvres && weekend
                    const estAujourdhui = iso === jour
                    const teinte = choisi
                      ? 'bg-action font-semibold text-sur-action shadow-douce'
                      : bloque
                        ? `cursor-not-allowed ${horsMois ? 'text-texte-2/20' : 'text-texte-2/40'}`
                        : horsMois
                          ? 'text-texte-2/35 hover:bg-voile'
                          : weekend
                            ? 'text-texte-2/70 hover:bg-voile'
                            : 'text-texte hover:bg-voile'
                    return (
                      <div role="gridcell" key={iso} aria-selected={choisi} className="grid place-items-center py-0.5">
                        <button
                          type="button"
                          data-iso={iso}
                          tabIndex={iso === focus ? 0 : -1}
                          disabled={!actif}
                          aria-disabled={bloque || undefined}
                          aria-current={estAujourdhui ? 'date' : undefined}
                          aria-label={`${noms.dateLongue(iso)}${weekend ? ` (${t('date.weekend')})` : ''}`}
                          title={bloque ? t('date.weekendInterdit') : undefined}
                          onClick={() => !bloque && onChoisir(iso)}
                          className={`chiffres relative grid size-10 place-items-center rounded-full text-sm transition-[background-color,color,box-shadow] duration-150 focus-visible:ring-2 focus-visible:ring-action focus-visible:ring-offset-2 focus-visible:ring-offset-surface focus-visible:outline-none disabled:cursor-not-allowed disabled:bg-transparent disabled:text-texte-2/25 disabled:line-through ${teinte} ${
                            estAujourdhui && !choisi ? 'font-semibold text-action ring-1 ring-action/60 ring-inset' : ''
                          }`}
                        >
                          {numero}
                          {estAujourdhui && <span aria-hidden="true" className={`absolute bottom-1.5 size-1 rounded-full ${choisi ? 'bg-sur-action' : 'bg-orange'}`} />}
                        </button>
                      </div>
                    )
                  })}
                </div>
              ))}
            </div>
          </div>
        )}

        {/* Vue mois */}
        {vue === 'mois' && (
          <div key={`mois-${a}`} className="glisse mt-3 grid grid-cols-3 gap-2" style={{ '--sens': sens }}>
            {noms.moisCourts.map((nom, i) => {
              const choisi = valeur && lireIso(valeur)?.a === a && lireIso(valeur)?.m === i + 1
              const courant = m === i + 1
              return (
                <button
                  key={nom}
                  type="button"
                  disabled={!moisDispo(a, i + 1)}
                  aria-label={`${noms.moisLongs[i]} ${a}`}
                  onClick={() => {
                    deplacer(versIso(a, i + 1, Math.min(lireIso(focus).j, 28)))
                    setVue('jours')
                  }}
                  className={`h-12 rounded-full text-sm font-medium transition-colors duration-150 focus-visible:ring-2 focus-visible:ring-action focus-visible:outline-none disabled:cursor-not-allowed disabled:opacity-30 ${
                    choisi ? 'bg-action text-sur-action' : courant ? 'bordure-fine text-action' : 'hover:bg-voile'
                  }`}
                >
                  {nom}
                </button>
              )
            })}
          </div>
        )}

        {/* Vue années */}
        {vue === 'annees' && (
          <div key={`annees-${debutAnnees}`} className="glisse mt-3 grid grid-cols-3 gap-2">
            {Array.from({ length: 12 }, (_, i) => debutAnnees + i).map((annee) => (
              <button
                key={annee}
                type="button"
                disabled={annee < anneeMin || annee > anneeMax}
                onClick={() => {
                  deplacer(versIso(annee, m, Math.min(lireIso(focus).j, 28)), false)
                  setVue('mois')
                }}
                className={`chiffres h-12 rounded-full text-sm font-medium transition-colors duration-150 focus-visible:ring-2 focus-visible:ring-action focus-visible:outline-none disabled:cursor-not-allowed disabled:opacity-30 ${
                  annee === a ? 'bg-action text-sur-action' : annee === Number(jour.slice(0, 4)) ? 'bordure-fine text-action' : 'hover:bg-voile'
                }`}
              >
                {annee}
              </button>
            ))}
          </div>
        )}

        {/* Pied : légende + aujourd'hui */}
        <div className="mt-3 flex items-center justify-between gap-3 border-t border-bordure pt-3">
          <p className="flex min-w-0 items-center gap-2 text-xs text-texte-2">
            <span aria-hidden="true" className="chiffres grid size-6 shrink-0 place-items-center rounded-full bg-voile text-[0.6875rem] text-texte-2/70">
              S
            </span>
            <span className="truncate">{t('date.legende.weekend')}</span>
          </p>
          <button
            type="button"
            disabled={!dansBornes(jour, min, max) || (joursOuvres && estWeekend(jour))}
            onClick={() => onChoisir(jour)}
            className="inline-flex h-9 shrink-0 items-center rounded-full border border-bordure-champ px-4 text-sm font-medium text-action transition-colors duration-150 hover:border-action hover:bg-action/8 focus-visible:ring-2 focus-visible:ring-action focus-visible:outline-none disabled:pointer-events-none disabled:opacity-40"
          >
            {t('date.aujourdhui')}
          </button>
        </div>
        <p className="sr-only">{t('date.aide.clavier')}</p>
      </div>
    </>
  )
}
