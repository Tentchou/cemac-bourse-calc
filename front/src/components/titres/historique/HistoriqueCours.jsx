import { ChartSpline, Minus, TrendingDown, TrendingUp } from 'lucide-react'
import { useMemo, useState } from 'react'
import { titresService, useRequete } from '../../../services'
import { usePreferences } from '../../../contexte/Preferences'
import { Section } from '../../mise-en-page/MiseEnPage'
import Bascule from '../../ui/Bascule'
import Carte from '../../ui/Carte'
import { EtatVide, MessageErreur } from '../../ui/Etats'
import Pagination from '../../ui/pagination/Pagination'
import { paginer, TAILLES_PAGE } from '../../ui/pagination/paginer'
import { Forme, SqueletteStatistiques, SqueletteTableau } from '../../ui/Squelettes'
import Statistique from '../../ui/Statistique'
import Tableau from '../../ui/Tableau'
import GraphiqueCours from './GraphiqueCours'
import { filtrerPeriode, lignesTableau, lirePeriode, PERIODES, resumer } from './historique'

// Période choisie par le visiteur : simple confort, mémorisée si le stockage le permet.
const CLE_PERIODE = 'cbc-periode-cours'
function periodeMemorisee() {
  try {
    return lirePeriode(localStorage.getItem(CLE_PERIODE))
  } catch {
    return lirePeriode(null)
  }
}

/** Une variation en %, avec son sens écrit (flèche et signe), pas seulement sa couleur. */
function Variation({ valeur }) {
  const { pourcent } = usePreferences()
  if (valeur === null || valeur === undefined) return <span className="text-texte-2">—</span>
  const arrondi = Math.round(valeur * 100) / 100
  const [Icone, ton, signe] =
    arrondi > 0 ? [TrendingUp, 'text-whatsapp', '+'] : arrondi < 0 ? [TrendingDown, 'text-erreur', '−'] : [Minus, 'text-texte-2', '']
  return (
    <span className={`chiffres inline-flex items-center gap-1.5 font-semibold ${ton}`}>
      <Icone aria-hidden="true" className="size-4 shrink-0" strokeWidth={2} />
      {signe}
      {pourcent(Math.abs(arrondi), 2)}
    </span>
  )
}

/**
 * L'historique des cours d'un titre, sur la fiche : choix de la période, quatre chiffres clés,
 * la courbe (avec les volumes), puis le tableau des séances. Une seule requête : changer de
 * période ne rappelle pas l'API. Ce bloc charge ses données à part, avec ses propres skeletons.
 */
export default function HistoriqueCours({ titre }) {
  const { t, montant, nombre, pourcent, date } = usePreferences()
  const { donnees, erreur, recharger } = useRequete((signal) => titresService.historique(titre.id, signal), [titre.id])
  const [periode, setPeriode] = useState(periodeMemorisee)
  const [page, setPage] = useState(1)
  const [taille, setTaille] = useState(TAILLES_PAGE[0])

  const obligation = titre.typeTitre === 'Obligation'
  const formatCours = (v) => (obligation ? pourcent(v, 2) : montant(v, 0))

  const points = useMemo(() => (donnees ? filtrerPeriode(donnees, periode) : []), [donnees, periode])
  const resume = useMemo(() => resumer(points), [points])
  const lignes = useMemo(() => lignesTableau(points), [points])
  const pagination = useMemo(() => paginer(lignes, page, taille), [lignes, page, taille])

  const changerPeriode = (cle) => {
    setPeriode(cle)
    setPage(1)
    try {
      localStorage.setItem(CLE_PERIODE, cle)
    } catch {
      /* stockage indisponible : le choix vaut pour la visite */
    }
  }

  // Le choix de période n'a de sens qu'avec au moins deux séances.
  const avecPeriodes = donnees && donnees.length > 1
  const actions = avecPeriodes && (
    <Bascule
      libelle={t('hist.periode')}
      valeur={periode}
      onChange={changerPeriode}
      options={PERIODES.map((p) => ({ valeur: p.cle, libelle: t(`hist.p.${p.cle}`), libelleCache: t(`hist.p.${p.cle}.long`) }))}
    />
  )

  const section = (contenu) => (
    <Section id="historique" titre={t('hist.titre')} icone={ChartSpline} aide={t(obligation ? 'hist.aide.obligation' : 'hist.aide.action')} actions={actions}>
      {contenu}
    </Section>
  )

  if (erreur) return section(<MessageErreur erreur={erreur} onReessayer={recharger} />)

  if (!donnees) {
    return section(
      <div className="space-y-4">
        <SqueletteStatistiques nombre={4} />
        <Carte>
          <Forme className="h-60 w-full" />
        </Carte>
        <SqueletteTableau colonnes={4} lignes={5} />
      </div>,
    )
  }

  if (donnees.length === 0) {
    return section(<EtatVide icone={ChartSpline} titre={t('hist.vide.titre')} texte={t('hist.vide.texte')} />)
  }

  const colonnes = [
    { cle: 'seance', titre: t('hist.col.seance'), rendu: (l) => date(l.dateSeance) },
    { cle: 'cours', titre: t('hist.col.cours'), droite: true, rendu: (l) => <span className="font-semibold">{formatCours(l.cours)}</span> },
    { cle: 'variation', titre: t('hist.col.variation'), droite: true, rendu: (l) => <Variation valeur={l.variationPourcent} /> },
    {
      cle: 'volume',
      titre: t('hist.col.volume'),
      droite: true,
      rendu: (l) => (l.volume > 0 ? nombre(l.volume, 0) : <span className="text-texte-2">{t('hist.sansEchange')}</span>),
    },
  ]

  return section(
    <div className="space-y-4">
      <div className="apparition grid grid-cols-2 gap-4 lg:grid-cols-4">
        <Statistique className="max-sm:p-4" libelle={t('hist.dernier')} valeur={formatCours(resume.dernier.cours)} point="orange" detail={t('hist.seanceDu', { date: date(resume.dernier.dateSeance) })} />
        <Statistique
          className="max-sm:p-4"
          libelle={t('hist.variation')}
          valeur={<Variation valeur={resume.seances > 1 ? resume.variationPourcent : null} />}
          detail={resume.seances > 1 ? t('hist.depuis', { date: date(resume.premier.dateSeance) }) : undefined}
        />
        <Statistique className="max-sm:p-4" libelle={t('hist.haut')} valeur={formatCours(resume.haut.cours)} detail={date(resume.haut.dateSeance)} />
        <Statistique className="max-sm:p-4" libelle={t('hist.bas')} valeur={formatCours(resume.bas.cours)} detail={date(resume.bas.dateSeance)} />
      </div>

      <Carte className="apparition" style={{ '--rang': 1 }}>
        {points.length > 1 ? (
          <>
            <GraphiqueCours
              key={periode}
              points={points}
              formatCours={formatCours}
              libelle={t('hist.graphique', { debut: date(resume.premier.dateSeance), fin: date(resume.dernier.dateSeance) })}
            />
            <p className="chiffres mt-4 border-t border-bordure pt-4 text-sm text-texte-2">
              {t('hist.bilan', { n: nombre(resume.seances, 0), e: nombre(resume.seancesEchange, 0), v: nombre(resume.volume, 0) })}
            </p>
          </>
        ) : (
          <p className="py-6 text-center text-sm text-texte-2">{t('hist.unSeul')}</p>
        )}
      </Carte>

      <div className="apparition" style={{ '--rang': 2 }}>
        <Tableau legende={t('hist.tableau')} colonnes={colonnes} lignes={pagination.lignes} cleLigne={(l) => l.dateSeance} />
        <Pagination
          etat={pagination}
          taille={taille}
          onPage={setPage}
          onTaille={(n) => {
            setTaille(n)
            setPage(1)
          }}
          libelle={t('hist.tableau')}
        />
      </div>
    </div>,
  )
}
