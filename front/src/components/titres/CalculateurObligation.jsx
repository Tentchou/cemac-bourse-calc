import { CalendarDays, Calculator, Clock, Hash, Percent, SlidersHorizontal, Tag, TrendingUp, Wallet } from 'lucide-react'
import { useState } from 'react'
import { calculsService, useRequete } from '../../services'
import { usePreferences } from '../../contexte/Preferences'
import { lireNombreSaisi } from '../../i18n/format'
import { useRoute } from '../../routeur/Routeur'
import { Grille, Section } from '../mise-en-page/MiseEnPage'
import Bascule from '../ui/Bascule'
import Bouton from '../ui/Bouton'
import Carte from '../ui/Carte'
import Champ from '../ui/Champ'
import { aujourdhui, jourDeSeance, lireIso } from '../ui/date/dates'
import ChampDate from '../ui/date/ChampDate'
import { MessageErreur } from '../ui/Etats'
import { SqueletteGraphique, SqueletteResultat, SqueletteStatistiques, SqueletteTableau } from '../ui/Squelettes'
import Tableau from '../ui/Tableau'
import Composition from './Composition'
import Echeancier from './Echeancier'
import Partage from './Partage'
import Resultat from './Resultat'

/** Les dates de négociation possibles : de la jouissance à l'échéance du titre. */
const bornes = (titre) => ({ min: titre.obligation?.dateJouissance ?? null, max: titre.obligation?.dateEcheance ?? null })

/** Les paramètres du calcul viennent de l'adresse (lien partagé) ou des valeurs par défaut. */
function saisieInitiale(parametres, titre, langue) {
  const cours = parametres.get('cours') ?? (titre.dernierCours != null ? String(titre.dernierCours) : '100')
  const { min, max } = bornes(titre)
  const datePartagee = parametres.get('date')
  return {
    // Toujours un jour de séance : un lien partagé tombant un week-end recule au vendredi.
    date: jourDeSeance(lireIso(datePartagee) ? datePartagee : aujourdhui(), min, max),
    quantite: parametres.get('quantite') ?? '1',
    cours: langue === 'fr' ? cours.replace('.', ',') : cours,
  }
}

/** Vérifie la saisie ; rend { demande } (valeurs numériques) ou { erreurs } par champ. */
function verifier(saisie) {
  const quantite = lireNombreSaisi(saisie.quantite)
  const cours = lireNombreSaisi(saisie.cours)
  const erreurs = {}
  if (!saisie.date) erreurs.date = true
  if (!Number.isInteger(quantite) || quantite < 1) erreurs.quantite = true
  if (cours === null || cours <= 0) erreurs.cours = true
  return Object.keys(erreurs).length ? { erreurs } : { demande: { date: saisie.date, quantite, cours } }
}

/**
 * Le calculateur d'une obligation. Chaque bloc (prix et coupon couru, rendements, échéancier)
 * a SA requête et SON skeleton : il s'affiche dès que ses données arrivent, sans attendre les autres.
 */
export default function CalculateurObligation({ titre }) {
  const { t, langue, nombre, montant, pourcent, date, dateLongue } = usePreferences()
  const { parametres, naviguer, chemin } = useRoute()
  const [saisie, setSaisie] = useState(() => saisieInitiale(parametres, titre, langue))
  const [demande, setDemande] = useState(() => verifier(saisieInitiale(parametres, titre, langue)).demande ?? { date: jourDeSeance(aujourdhui(), bornes(titre).min, bornes(titre).max), quantite: 1, cours: 100 })
  const [erreurs, setErreurs] = useState({})
  const [mode, setMode] = useState(parametres.get('mode') === 'expert' ? 'expert' : 'simple')
  const expert = mode === 'expert'

  const couru = useRequete(
    (signal) => calculsService.couponCouru(titre.id, { dateNegociation: demande.date, quantite: demande.quantite, cours: demande.cours }, signal),
    [demande],
  )
  const rendement = useRequete((signal) => calculsService.rendement(titre.id, { dateNegociation: demande.date, cours: demande.cours }, signal), [demande])
  const echeancier = useRequete((signal) => calculsService.echeancier(titre.id, { quantite: demande.quantite, dateNegociation: demande.date }, signal), [demande])

  function mettreAJourAdresse(valeurs, modeChoisi) {
    const requete = new URLSearchParams({ date: valeurs.date, quantite: String(valeurs.quantite), cours: String(valeurs.cours) })
    if (modeChoisi === 'expert') requete.set('mode', 'expert')
    naviguer(`${chemin}?${requete}`, { remplacer: true })
  }

  function calculer(evenement) {
    evenement.preventDefault()
    const resultat = verifier(saisie)
    setErreurs(resultat.erreurs ?? {})
    if (!resultat.demande) return
    setDemande(resultat.demande)
    mettreAJourAdresse(resultat.demande, mode)
  }

  function changerMode(m) {
    setMode(m)
    mettreAJourAdresse(demande, m)
  }

  const maj = (cle) => (e) => setSaisie((s) => ({ ...s, [cle]: e.target.value }))
  // Une seule erreur affichée, même si les trois requêtes échouent pour la même raison.
  const erreur = couru.erreur ?? rendement.erreur ?? echeancier.erreur
  const recalculer = () => setDemande((d) => ({ ...d }))
  const c = couru.donnees
  const r = rendement.donnees
  const e = echeancier.donnees
  // Pendant un nouveau calcul, l'ancien résultat reste affiché, estompé.
  const estompe = (bloc) => `transition-opacity duration-250 ${bloc.chargement && bloc.donnees ? 'opacity-50' : ''}`

  return (
    <>
      <Section
        id="calcul"
        titre={t('calc.titre')}
        icone={Calculator}
        actions={
          <Bascule
            libelle={t('mode.groupe')}
            valeur={mode}
            onChange={changerMode}
            options={[
              { valeur: 'simple', libelle: t('mode.simple') },
              { valeur: 'expert', libelle: t('mode.expert') },
            ]}
          />
        }
      >
        <div className="grid grid-cols-[minmax(0,1fr)] items-start gap-4 lg:grid-cols-[360px_minmax(0,1fr)]">
          {/* Formulaire */}
          <Carte as="form" noValidate onSubmit={calculer} className="apparition grid gap-4 lg:sticky lg:top-24">
            <h3 className="flex items-center gap-2 text-base font-semibold">
              <SlidersHorizontal aria-hidden="true" className="size-4 text-action" strokeWidth={2} />
              {t('calc.operation')}
            </h3>
            <ChampDate
              libelle={t('calc.date')}
              valeur={saisie.date}
              onChange={(iso) => setSaisie((s) => ({ ...s, date: iso }))}
              min={bornes(titre).min}
              max={bornes(titre).max}
              joursOuvres
              erreur={erreurs.date ? t('date.invalide') : null}
              aide={c ? t('calc.reglement', { date: dateLongue(c.dateReglement), n: c.delaiReglementJoursOuvres }) : null}
              required
            />
            <Champ
              libelle={t('calc.quantite')}
              icone={Hash}
              inputMode="numeric"
              value={saisie.quantite}
              onChange={maj('quantite')}
              unite={t('unite.titres')}
              erreur={erreurs.quantite ? '≥ 1' : null}
              required
            />
            <Champ
              libelle={t('calc.cours')}
              icone={Tag}
              inputMode="decimal"
              value={saisie.cours}
              onChange={maj('cours')}
              unite="%"
              erreur={erreurs.cours ? '> 0' : null}
              required
            />
            <Bouton type="submit" variante="action" icone={Calculator} chargement={couru.chargement || rendement.chargement} className="mt-2 w-full">
              {t('calc.lancer')}
            </Bouton>
          </Carte>

          {/* Résultats : chaque carte arrive avec sa propre requête */}
          <div className="min-w-0 space-y-4">
            {erreur && <MessageErreur erreur={erreur} onReessayer={recalculer} />}

            {!erreur &&
              (c ? (
                <Resultat
                  principal
                  className={`apparition ${estompe(couru)}`}
                  icone={Wallet}
                  libelle={t('res.plein')}
                  valeur={montant(c.prixPleinCouponTotal)}
                  aide={t('res.plein.aide')}
                  explication={
                    <>
                      {nombre(c.nominalRestant, 0)} × {pourcent(c.coursPiedCoupon)} = <b>{nombre(c.prixPiedCouponParTitre)}</b>
                      <br />({nombre(c.prixPiedCouponParTitre)} + {nombre(c.couponCouruParTitre, expert ? 4 : 2)}) × {nombre(c.quantite, 0)} ={' '}
                      <b>{montant(c.prixPleinCouponTotal)}</b>
                    </>
                  }
                >
                  <Composition
                    parts={[
                      { libelle: t('comp.pied'), valeur: c.prixPiedCouponTotal, texte: nombre(c.prixPiedCouponTotal), couleur: 'action' },
                      { libelle: t('comp.couru'), valeur: c.couponCouruTotal, texte: nombre(c.couponCouruTotal), couleur: 'orange' },
                    ]}
                  />
                </Resultat>
              ) : (
                <SqueletteResultat principal composition />
              ))}

            {!erreur && (
              <Grille colonnes={2} className="xl:grid-cols-3">
                {c ? (
                  <Resultat
                    className={`apparition ${estompe(couru)}`}
                    icone={Clock}
                    ton="orange"
                    libelle={t('res.couru')}
                    valeur={montant(c.couponCouruTotal)}
                    aide={t('res.couru.aide', { montant: montant(c.couponCouruParTitre) })}
                    explication={
                      <>
                        {nombre(c.nominalRestant, 0)} × {pourcent(c.tauxFacial)} × {c.jours} ÷ {c.joursAnnee} × {nombre(c.quantite, 0)} ={' '}
                        <b>{montant(c.couponCouruTotal)}</b>
                        {expert && (
                          <span className="mt-2 block text-texte-2">
                            {t('expl.periode', { debut: date(c.debutPeriode), fin: date(c.dateReglement), jours: c.jours, convention: c.convention })} ({t('expl.arrondi')})
                          </span>
                        )}
                      </>
                    }
                  />
                ) : (
                  <SqueletteResultat />
                )}
                {r ? (
                  <>
                    <Resultat
                      className={`apparition ${estompe(rendement)}`}
                      style={{ '--rang': 1 }}
                      icone={TrendingUp}
                      libelle={t('res.actuariel')}
                      valeur={pourcent(r.rendementActuariel, expert ? 4 : 2)}
                      aide={t('res.actuariel.aide')}
                      explication={
                        <>
                          {nombre(r.prixPleinCouponParTitre)} = Σ flux ÷ (1 + r)<sup>{langue === 'fr' ? 'jours' : 'days'} ÷ {r.joursAnneeActuarielle}</sup>
                          <br />→ r = <b>{pourcent(r.rendementActuariel, 4)}</b>
                        </>
                      }
                    />
                    <Resultat
                      className={`apparition ${estompe(rendement)} sm:col-span-2 xl:col-span-1`}
                      style={{ '--rang': 2 }}
                      icone={Percent}
                      ton="neutre"
                      libelle={t('res.courant')}
                      valeur={pourcent(r.rendementCourant, expert ? 4 : 2)}
                      aide={t('res.courant.aide')}
                      explication={
                        <>
                          {nombre(r.tauxFacial)} ÷ {nombre(r.coursPiedCoupon)} × 100 = <b>{pourcent(r.rendementCourant, 4)}</b>
                        </>
                      }
                    />
                  </>
                ) : (
                  <>
                    <SqueletteResultat />
                    <SqueletteResultat className="sm:col-span-2 xl:col-span-1" />
                  </>
                )}
              </Grille>
            )}

            {c && r && <Partage nomTitre={titre.nom} />}
          </div>
        </div>
      </Section>

      {/* Mode expert : le détail de l'actualisation */}
      {expert && !erreur && (
        <Section id="flux" titre={t('expert.flux')} icone={TrendingUp} aide={t('expert.flux.aide')}>
          {r ? (
            <Tableau
              className="apparition"
              legende={t('expert.flux')}
              colonnes={[
                { cle: 'date', titre: t('col.date'), rendu: (f) => date(f.date) },
                { cle: 'jours', titre: t('col.jours'), droite: true, rendu: (f) => (f.date === 'total' ? '' : nombre(f.jours, 0)) },
                { cle: 'annees', titre: t('col.annees'), droite: true, rendu: (f) => (f.date === 'total' ? '' : nombre(f.fractionAnnee, 4)) },
                { cle: 'flux', titre: t('col.flux'), droite: true, rendu: (f) => (f.date === 'total' ? '' : nombre(f.flux)) },
                { cle: 'va', titre: t('col.va'), droite: true, rendu: (f) => <span className="font-semibold">{nombre(f.valeurActuelle)}</span> },
              ]}
              lignes={r.flux}
              cleLigne={(f) => f.date}
              pied={{ date: 'total', valeurActuelle: r.flux.reduce((s, f) => s + f.valeurActuelle, 0) }}
              libellePied={t('expert.total')}
            />
          ) : (
            <SqueletteTableau colonnes={5} lignes={5} pied />
          )}
        </Section>
      )}

      {/* Échéancier */}
      {!erreur && (!e || e.lignes.length > 0) && (
        <Section id="echeancier" titre={t('ech.titre')} icone={CalendarDays} aide={t('ech.aide')}>
          {e ? (
            <div className={estompe(echeancier)}>
              <Echeancier echeancier={e} expert={expert} />
            </div>
          ) : (
            <div className="space-y-4">
              <SqueletteStatistiques />
              <SqueletteGraphique />
              <SqueletteTableau colonnes={expert ? 5 : 4} lignes={5} pied />
            </div>
          )}
        </Section>
      )}
    </>
  )
}
