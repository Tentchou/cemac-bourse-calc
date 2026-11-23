import { ArrowRight, CalendarDays, Clock, Hash, Tag, TrendingUp, Wallet } from 'lucide-react'
import { useEffect, useRef, useState } from 'react'
import { usePreferences } from '../../contexte/Preferences'
import Drapeau from '../carte/Drapeau'
import { OBLIGATION, exemplePrixPlein, exempleRendement } from '../methodes/exemples'
import Composition from '../titres/Composition'
import Bouton from '../ui/Bouton'
import Carte from '../ui/Carte'
import Etiquette from '../ui/Etiquette'
import Pastille from '../ui/Pastille'

/**
 * Aperçu du calcul (bandeau d'accueil) : une vraie carte de résultat qui se joue toute seule,
 * pour montrer le produit avant le moindre clic. Le titre et les montants sont réels : l'ECMR
 * 6,25 % 2022-2029 à la séance du 20/11/2026, calculé par les mêmes fonctions que la page
 * Méthodes (vérifiées contre l'API et le bulletin de la BVMAC).
 *
 * La scène rejoue toutes les 8 s tant qu'elle est visible ; immobile (valeurs finales) si le
 * visiteur a demandé moins d'animations.
 */
const PLEIN = exemplePrixPlein()
const EXEMPLE = {
  titre: OBLIGATION.titre,
  pays: OBLIGATION.pays,
  date: OBLIGATION.dateNegociation,
  quantite: OBLIGATION.quantite,
  cours: OBLIGATION.cours,
  prixPlein: PLEIN.total,
  piedDeCoupon: PLEIN.pied,
  couru: PLEIN.couruTotal,
  actuariel: exempleRendement().actuariel,
}

const DUREE_COMPTEUR = 1200
const PERIODE = 8000

const moinsDAnimations = () => window.matchMedia?.('(prefers-reduced-motion: reduce)').matches

/** Compte de 0 à `cible` (accélération douce) à chaque nouveau `tour`. */
function useCompteur(cible, tour) {
  const [valeur, setValeur] = useState(() => (moinsDAnimations() ? cible : 0))
  useEffect(() => {
    if (moinsDAnimations()) return
    let image
    const debut = performance.now()
    const avancer = (maintenant) => {
      const p = Math.min((maintenant - debut) / DUREE_COMPTEUR, 1)
      setValeur(cible * (1 - (1 - p) ** 3))
      if (p < 1) image = requestAnimationFrame(avancer)
    }
    image = requestAnimationFrame(avancer)
    return () => cancelAnimationFrame(image)
  }, [cible, tour])
  return valeur
}

/** Rejoue la scène tant qu'elle est visible. */
function useTours() {
  const ref = useRef(null)
  const [tour, setTour] = useState(0)
  useEffect(() => {
    if (moinsDAnimations()) return
    let minuterie
    const observateur = new IntersectionObserver(([entree]) => {
      clearInterval(minuterie)
      if (entree.isIntersecting) minuterie = setInterval(() => setTour((n) => n + 1), PERIODE)
    })
    observateur.observe(ref.current)
    return () => {
      observateur.disconnect()
      clearInterval(minuterie)
    }
  }, [])
  return [ref, tour]
}

export default function ApercuCalcul({ lienExemple }) {
  const { t, montant, nombre, pourcent, date } = usePreferences()
  const [ref, tour] = useTours()
  const prixPlein = useCompteur(EXEMPLE.prixPlein, tour)

  const saisies = [
    { icone: CalendarDays, texte: date(EXEMPLE.date) },
    { icone: Hash, texte: `${EXEMPLE.quantite} ${t('unite.titres')}` },
    { icone: Tag, texte: pourcent(EXEMPLE.cours) },
  ]

  return (
    <div ref={ref} className="relative">
      {/* Halo bleu discret derrière la carte (la « grande zone » du bleu de la charte, en douceur) */}
      <div
        aria-hidden="true"
        className="pointer-events-none absolute -inset-10 rounded-full opacity-90 blur-2xl"
        style={{ background: 'radial-gradient(closest-side, color-mix(in srgb, var(--cbc-marque) 28%, transparent), transparent)' }}
      />

      {/* key={tour} : la scène entière rejoue ses apparitions à chaque tour */}
      <Carte key={tour} className="relative shadow-haute">
        <div className="flex items-center justify-between gap-3">
          <p className="flex min-w-0 items-center gap-2 text-sm font-semibold">
            <Drapeau code={EXEMPLE.pays} />
            <span className="truncate">{EXEMPLE.titre}</span>
          </p>
          <Etiquette ton="orange" point>
            {t('apercu.exemple')}
          </Etiquette>
        </div>

        {/* Les trois saisies apparaissent l'une après l'autre */}
        <div className="mt-4 flex flex-wrap gap-2">
          {saisies.map(({ icone: Icone, texte }, i) => (
            <span
              key={i}
              className="apparition chiffres inline-flex h-8 items-center gap-1.5 rounded-full border border-bordure-champ bg-surface px-3 text-xs font-medium"
              style={{ '--rang': i * 3 }}
            >
              <Icone aria-hidden="true" className="size-3.5 text-action" strokeWidth={2} />
              {texte}
            </span>
          ))}
        </div>

        <div className="mt-6 border-t border-bordure pt-6">
          <p className="flex items-center gap-2 text-sm text-texte-2">
            <Pastille icone={Wallet} taille="petite" />
            {t('res.plein')}
          </p>
          <p className="chiffres mt-3 font-titre text-3xl leading-none font-bold tracking-tight sm:text-4xl" aria-label={montant(EXEMPLE.prixPlein)}>
            {montant(prixPlein)}
          </p>
          <Composition
            parts={[
              { libelle: t('comp.pied'), valeur: EXEMPLE.piedDeCoupon, texte: nombre(EXEMPLE.piedDeCoupon), couleur: 'action' },
              { libelle: t('comp.couru'), valeur: EXEMPLE.couru, texte: nombre(EXEMPLE.couru), couleur: 'orange' },
            ]}
          />
        </div>

        <div className="mt-6 grid grid-cols-2 gap-px overflow-hidden rounded-2xl border border-bordure bg-bordure">
          {[
            { icone: TrendingUp, libelle: t('res.actuariel'), valeur: pourcent(EXEMPLE.actuariel), ton: 'action' },
            { icone: Clock, libelle: t('res.couru'), valeur: nombre(EXEMPLE.couru), ton: 'orange' },
          ].map(({ icone: Icone, libelle, valeur, ton }, i) => (
            <div key={libelle} className="apparition bg-surface p-4" style={{ '--rang': 10 + i * 2 }}>
              <p className="flex items-start gap-1.5 text-xs leading-snug text-texte-2">
                <Icone aria-hidden="true" className={`mt-px size-3.5 shrink-0 ${ton === 'orange' ? 'text-orange-texte' : 'text-action'}`} strokeWidth={2} />
                {libelle}
              </p>
              <p className="chiffres mt-1 font-titre text-lg font-bold">{valeur}</p>
            </div>
          ))}
        </div>

        {lienExemple && (
          <Bouton vers={lienExemple} variante="fantome" taille="petite" icone={ArrowRight} className="mt-4 -mb-2 w-full flex-row-reverse">
            {t('apercu.voir')}
          </Bouton>
        )}
      </Carte>
    </div>
  )
}
