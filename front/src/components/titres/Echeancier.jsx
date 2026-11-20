import { CalendarClock } from 'lucide-react'
import { usePreferences } from '../../contexte/Preferences'
import Carte from '../ui/Carte'
import Etiquette from '../ui/Etiquette'
import Statistique from '../ui/Statistique'
import Tableau from '../ui/Tableau'
import GraphiqueEcheancier from './GraphiqueEcheancier'

/**
 * L'échéancier rendu par l'API : trois totaux, le graphique, puis le tableau avec sa ligne de total.
 * Le mode expert ajoute le nominal restant après chaque échéance.
 */
export default function Echeancier({ echeancier, expert }) {
  const { t, nombre, date } = usePreferences()
  const lignes = echeancier.lignes
  if (lignes.length === 0) return null

  const colonnes = [
    {
      cle: 'paiement',
      titre: t('col.paiement'),
      rendu: (l) => (
        <span className="inline-flex flex-wrap items-center gap-2">
          <span className="chiffres text-texte-2">{String(l.rang).padStart(2, '0')}</span>
          {date(l.datePaiement)}
          {l.datePaiement !== l.dateTheorique && (
            <Etiquette ton="orange" icone={CalendarClock} title={t('ech.reporte.aide')}>
              {t('ech.reporte')}
            </Etiquette>
          )}
        </span>
      ),
    },
    { cle: 'coupon', titre: t('col.coupon'), droite: true, rendu: (l) => nombre(l.coupon) },
    { cle: 'capital', titre: t('col.capital'), droite: true, rendu: (l) => nombre(l.capital) },
    { cle: 'total', titre: t('col.total'), droite: true, rendu: (l) => <span className="font-semibold">{nombre(l.total)}</span> },
    ...(expert
      ? [{ cle: 'restant', titre: t('col.restant'), droite: true, rendu: (l) => (l.rang === 'total' ? '' : <span className="text-texte-2">{nombre(l.nominalRestantApresParTitre)}</span>) }]
      : []),
  ]

  return (
    <div className="space-y-4">
      <div className="apparition grid grid-cols-1 gap-4 sm:grid-cols-3">
        <Statistique libelle={t('ech.totalCoupons')} valeur={nombre(echeancier.totalCoupons)} point="orange" />
        <Statistique libelle={t('ech.totalCapital')} valeur={nombre(echeancier.totalCapital)} point="action" />
        <Statistique libelle={t('ech.totalFlux')} valeur={nombre(echeancier.totalFlux)} fort />
      </div>
      <Carte className="apparition" style={{ '--rang': 1 }}>
        <GraphiqueEcheancier lignes={lignes} />
      </Carte>
      <Tableau
        className="apparition"
        legende={t('ech.titre')}
        colonnes={colonnes}
        lignes={lignes}
        cleLigne={(l) => l.rang}
        pied={{ rang: 'total', coupon: echeancier.totalCoupons, capital: echeancier.totalCapital, total: echeancier.totalFlux }}
        libellePied={t('col.total')}
      />
    </div>
  )
}
