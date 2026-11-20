import { ArrowLeft, Banknote, Building2, CalendarDays, ChartNoAxesCombined, MapPin } from 'lucide-react'
import { usePreferences } from '../../contexte/Preferences'
import Bouton from '../ui/Bouton'
import Carte from '../ui/Carte'
import Etiquette from '../ui/Etiquette'

/** Le haut d'une fiche : retour, nom, étiquettes, émetteur ; à droite, la carte du dernier cours. */
export default function EnTeteFiche({ titre }) {
  const { t, pourcent, montant, date } = usePreferences()
  const obligation = titre.typeTitre === 'Obligation'
  const cours = titre.dernierCours == null ? '—' : obligation ? pourcent(titre.dernierCours) : montant(titre.dernierCours, 0)

  return (
    <header className="apparition grid gap-6 lg:grid-cols-[minmax(0,1fr)_280px] lg:items-end">
      <div className="min-w-0">
        <Bouton vers="/" taille="petite" icone={ArrowLeft}>
          {t('fiche.retour')}
        </Bouton>
        <h1 className="mt-6 text-[1.75rem] leading-tight font-bold break-words sm:text-4xl lg:text-[2.75rem]">{titre.nom}</h1>
        <div className="mt-4 flex flex-wrap gap-2">
          {obligation ? (
            <Etiquette ton="action" icone={Banknote}>{t('type.Obligation')}</Etiquette>
          ) : (
            <Etiquette ton="orange" icone={ChartNoAxesCombined}>{t('type.Action')}</Etiquette>
          )}
          <Etiquette className="chiffres">{titre.isin}</Etiquette>
          <Etiquette icone={MapPin}>{t(`pays.${titre.pays}`)}</Etiquette>
          <Etiquette ton={titre.statut === 'Actif' ? 'orange' : 'erreur'} point>
            {t(`statut.${titre.statut}`)}
          </Etiquette>
        </div>
        <p className="mt-4 flex items-center gap-2 text-sm text-texte-2">
          <Building2 aria-hidden="true" className="size-4 shrink-0" strokeWidth={1.75} />
          <span className="sr-only">{t('fiche.emetteur')} :</span>
          {titre.emetteur}
        </p>
      </div>

      <Carte className="lg:text-right">
        <p className="flex items-center gap-2 text-sm text-texte-2 lg:justify-end">
          <CalendarDays aria-hidden="true" className="size-4 shrink-0" strokeWidth={1.75} />
          {t('fiche.dernier')}
          {titre.dateDernierCours && <span className="chiffres">· {date(titre.dateDernierCours)}</span>}
        </p>
        <p className="chiffres mt-2 font-titre text-4xl leading-none font-bold">{cours}</p>
      </Carte>
    </header>
  )
}
