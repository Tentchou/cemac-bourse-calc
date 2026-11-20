import { Banknote, ChartNoAxesCombined } from 'lucide-react'
import { usePreferences } from '../../contexte/Preferences'
import Drapeau from '../carte/Drapeau'
import Etiquette from '../ui/Etiquette'
import Tableau from '../ui/Tableau'

/** Colonnes de la liste des titres (servent aussi au skeleton, pour qu'il ait la même forme). */
export function colonnesTitres({ t, pourcent, montant, date }) {
  return [
    {
      cle: 'nom',
      titre: t('col.titre'),
      rendu: (titre) => (
        <span className="flex flex-col gap-0.5 py-2">
          <span className="line-clamp-2 leading-snug max-sm:text-[0.8125rem]">{titre.nom}</span>
          <span className="text-xs font-normal text-texte-2">{titre.isin}</span>
        </span>
      ),
    },
    { cle: 'emetteur', titre: t('col.emetteur'), rendu: (titre) => <span className="text-texte-2">{titre.emetteur}</span> },
    {
      cle: 'type',
      titre: t('col.type'),
      rendu: (titre) =>
        titre.typeTitre === 'Action' ? (
          <Etiquette ton="orange" icone={ChartNoAxesCombined}>{t('type.Action')}</Etiquette>
        ) : (
          <Etiquette ton="action" icone={Banknote}>{t('type.Obligation')}</Etiquette>
        ),
    },
    {
      cle: 'pays',
      titre: t('filtre.pays'),
      rendu: (titre) => (
        <span className="inline-flex items-center gap-2">
          <Drapeau code={titre.pays} />
          {t(`pays.${titre.pays}`)}
        </span>
      ),
    },
    { cle: 'taux', titre: t('col.taux'), droite: true, rendu: (titre) => (titre.tauxFacial == null ? '—' : pourcent(titre.tauxFacial)) },
    { cle: 'echeance', titre: t('col.echeance'), rendu: (titre) => (titre.dateEcheance ? date(titre.dateEcheance) : '—') },
    {
      cle: 'cours',
      titre: t('col.cours'),
      droite: true,
      rendu: (titre) => (
        <span className="font-semibold">
          {titre.dernierCours == null ? '—' : titre.typeTitre === 'Obligation' ? pourcent(titre.dernierCours) : montant(titre.dernierCours, 0)}
        </span>
      ),
    },
  ]
}

/** La liste des titres : tableau rayé, chaque ligne mène à la fiche du titre. */
export default function ListeTitres({ titres, className = '' }) {
  const preferences = usePreferences()
  return (
    <Tableau
      className={className}
      legende={preferences.t('liste.titre')}
      colonnes={colonnesTitres(preferences)}
      lignes={titres}
      cleLigne={(titre) => titre.id}
      lienLigne={(titre) => `/titres/${titre.id}`}
    />
  )
}
