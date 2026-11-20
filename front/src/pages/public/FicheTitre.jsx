import { Banknote, CalendarCheck, CalendarClock, CalendarRange, Coins, FileText, Flag, Layers, Percent, Receipt, Ruler, Undo2 } from 'lucide-react'
import { TYPES_ERREUR, titresService, useRequete } from '../../services'
import { Page, Section } from '../../components/mise-en-page/MiseEnPage'
import CalculateurDividende from '../../components/titres/CalculateurDividende'
import CalculateurObligation from '../../components/titres/CalculateurObligation'
import Caracteristiques from '../../components/titres/Caracteristiques'
import EnTeteFiche from '../../components/titres/EnTeteFiche'
import { MessageErreur } from '../../components/ui/Etats'
import {
  Forme,
  SqueletteEnTeteFiche,
  SqueletteFicheTechnique,
  SqueletteFormulaire,
  SqueletteResultat,
} from '../../components/ui/Squelettes'
import { usePreferences } from '../../contexte/Preferences'

/**
 * La fiche d'un titre. Pendant le chargement, chaque bloc a son skeleton à sa place (en-tête,
 * caractéristiques, calculateur) ; ensuite, chaque bloc du calculateur charge ses propres données.
 */
export default function FicheTitre({ id }) {
  const { t, montant, nombre, pourcent, date } = usePreferences()
  const { donnees: titre, erreur, recharger } = useRequete((signal) => titresService.obtenir(id, signal), [id])

  if (erreur) {
    return (
      <Page>
        <MessageErreur erreur={erreur} cles={{ introuvable: 'fiche.introuvable' }} onReessayer={erreur.type === TYPES_ERREUR.introuvable ? undefined : recharger} />
      </Page>
    )
  }

  if (!titre) {
    return (
      <Page>
        <SqueletteEnTeteFiche />
        <Section titre={t('fiche.caracteristiques')} icone={FileText}>
          <SqueletteFicheTechnique />
        </Section>
        <section className="min-w-0">
          <div className="mb-6 flex items-center gap-3">
            <Forme rond className="size-9" />
            <Forme className="h-7 w-40" />
          </div>
          <div className="grid grid-cols-[minmax(0,1fr)] items-start gap-4 lg:grid-cols-[360px_minmax(0,1fr)]">
            <SqueletteFormulaire />
            <SqueletteResultat principal composition />
          </div>
        </section>
      </Page>
    )
  }

  const o = titre.obligation
  const a = titre.action
  const elements = o
    ? [
        { icone: Banknote, libelle: t('carac.nominal'), valeur: montant(titre.nominal, 0) },
        { icone: Percent, libelle: t('carac.taux'), valeur: pourcent(o.tauxFacial) },
        { icone: CalendarClock, libelle: t('carac.coupon'), valeur: t(`periodicite.${o.periodicite}`) },
        { icone: Undo2, libelle: t('carac.remboursement'), valeur: t(`remboursement.${o.modeRemboursement}`) },
        { icone: CalendarCheck, libelle: t('carac.jouissance'), valeur: date(o.dateJouissance) },
        { icone: Flag, libelle: t('carac.echeance'), valeur: date(o.dateEcheance) },
        { icone: Ruler, libelle: t('carac.convention'), valeur: o.convention },
        { icone: Receipt, libelle: t('carac.fiscal'), valeur: t(`fiscal.${o.regimeFiscal}`) },
      ]
    : [
        { icone: Banknote, libelle: t('carac.nominal'), valeur: montant(titre.nominal, 0) },
        { icone: Layers, libelle: t('carac.nbactions'), valeur: a?.nombreTitres ? nombre(a.nombreTitres, 0) : '—' },
        {
          icone: Coins,
          libelle: t('carac.capitalisation'),
          valeur: a?.nombreTitres && titre.dernierCours ? montant(a.nombreTitres * titre.dernierCours, 0) : '—',
        },
        { icone: CalendarRange, libelle: t('col.exercice'), valeur: a?.dividendes?.[0]?.exercice ?? '—' },
      ]

  return (
    <Page>
      <EnTeteFiche titre={titre} />
      <Section titre={t('fiche.caracteristiques')} icone={FileText}>
        <Caracteristiques elements={elements} />
      </Section>
      {o ? <CalculateurObligation key={titre.id} titre={titre} /> : <CalculateurDividende key={titre.id} titre={titre} />}
    </Page>
  )
}
