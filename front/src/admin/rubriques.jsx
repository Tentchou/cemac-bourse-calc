import { Activity, Banknote, CalendarX, ChartNoAxesCombined, HandCoins, Landmark } from 'lucide-react'
import Drapeau from '../components/carte/Drapeau'
import { PAYS_CEMAC } from '../components/carte/drapeaux'
import Etiquette from '../components/ui/Etiquette'
import ControleCoupon from '../components/admin/ControleCoupon'
import { MODELES } from './import/modeles'

// Les six rubriques de l'administration. Chacune : son modèle (mêmes champs pour la saisie à l'unité et
// l'import), sa route d'API (la même pour la liste paginée, l'ajout et l'import), ses filtres et ses colonnes.

/** Les choix d'un champ d'un modèle (périodicités, conventions…). */
export function choixDe(modele, champ) {
  return MODELES[modele].colonnes.find((c) => c.champ === champ)?.choix ?? []
}

/** Le libellé traduit d'un choix ; à défaut, celui du modèle (les conventions s'écrivent pareil partout). */
export function libelleChoix(t, valeur, choix = []) {
  if (valeur === null || valeur === undefined || valeur === '') return '—'
  const cle = `admin.choix.${valeur}`
  const traduit = t(cle)
  if (traduit !== cle) return traduit
  return choix.find((c) => c.valeur === valeur)?.libelle ?? String(valeur)
}

const cellTitre = (ligne) => (
  <span className="flex flex-col gap-0.5 py-2">
    <span className="line-clamp-2 leading-snug font-semibold max-sm:text-[0.8125rem]">{ligne.nom}</span>
    <span className="text-xs text-texte-2">{ligne.isin}</span>
  </span>
)

const cellPays = (t, code) => (
  <span className="inline-flex items-center gap-2">
    <Drapeau code={code} />
    {t(`pays.${code}`)}
  </span>
)

/** Le cours d'une obligation est en % du nominal ; celui d'une action en FCFA. */
const cours = (p, valeur, typeTitre) =>
  valeur === null || valeur === undefined ? '—' : typeTitre === 'Obligation' ? p.pourcent(valeur, 2) : p.montant(valeur, 0)

const optionsPays = (t) => PAYS_CEMAC.map((code) => ({ valeur: code, libelle: t(`pays.${code}`) }))
const optionsChoix = (t, modele, champ) => choixDe(modele, champ).map((c) => ({ valeur: c.valeur, libelle: libelleChoix(t, c.valeur) }))

export const RUBRIQUES = [
  {
    cle: 'obligations',
    groupe: 'titres',
    icone: Banknote,
    modele: 'obligations',
    compteur: 'obligations',
    recherche: 'admin.filtre.recherche.titres',
    filtres: (t) => [
      { cle: 'pays', tous: t('admin.filtre.pays'), options: optionsPays(t) },
      { cle: 'remboursement', tous: t('admin.filtre.remboursement'), options: optionsChoix(t, 'obligations', 'modeRemboursement') },
    ],
    groupesFormulaire: [
      { cle: 'identification', champs: ['isin', 'emetteurId', 'nom'] },
      { cle: 'caracteristiques', champs: ['nominal', 'tauxFacial', 'periodicite', 'codeConvention', 'regimeFiscal', 'dateAdmission'] },
      { cle: 'calendrier', champs: ['dateJouissance', 'dateEcheance', 'datePremierCoupon', 'modeRemboursement', 'differeEcheances'] },
    ],
    colonnes: (p) => [
      { cle: 'nom', titre: p.t('admin.col.titre'), rendu: cellTitre },
      { cle: 'emetteur', titre: p.t('admin.col.emetteur'), rendu: (o) => <span className="text-texte-2">{o.emetteur}</span> },
      { cle: 'taux', titre: p.t('admin.col.taux'), droite: true, rendu: (o) => p.pourcent(o.tauxFacial) },
      { cle: 'periodicite', titre: p.t('admin.col.periodicite'), rendu: (o) => libelleChoix(p.t, o.periodicite) },
      {
        cle: 'remboursement',
        titre: p.t('admin.col.remboursement'),
        rendu: (o) =>
          o.modeRemboursement === 'InFine'
            ? libelleChoix(p.t, o.modeRemboursement)
            : `${libelleChoix(p.t, o.modeRemboursement)} · ${p.t('admin.differe', { n: o.differeEcheances })}`,
      },
      { cle: 'jouissance', titre: p.t('admin.col.jouissance'), rendu: (o) => p.date(o.dateJouissance) },
      { cle: 'echeance', titre: p.t('admin.col.echeance'), rendu: (o) => p.date(o.dateEcheance) },
      { cle: 'convention', titre: p.t('admin.col.convention'), rendu: (o) => libelleChoix(p.t, o.codeConvention, choixDe('obligations', 'codeConvention')) },
      { cle: 'controle', titre: p.t('admin.col.controle'), rendu: (o) => <ControleCoupon controle={o.controle} /> },
    ],
  },
  {
    cle: 'actions',
    groupe: 'titres',
    icone: ChartNoAxesCombined,
    modele: 'actions',
    compteur: 'actions',
    recherche: 'admin.filtre.recherche.titres',
    filtres: (t) => [{ cle: 'pays', tous: t('admin.filtre.pays'), options: optionsPays(t) }],
    colonnes: (p) => [
      { cle: 'nom', titre: p.t('admin.col.titre'), rendu: cellTitre },
      { cle: 'emetteur', titre: p.t('admin.col.emetteur'), rendu: (a) => <span className="text-texte-2">{a.emetteur}</span> },
      { cle: 'pays', titre: p.t('admin.col.pays'), rendu: (a) => cellPays(p.t, a.pays) },
      { cle: 'nominal', titre: p.t('admin.col.nominal'), droite: true, rendu: (a) => p.montant(a.nominal, 0) },
      { cle: 'nombreTitres', titre: p.t('admin.col.nombreTitres'), droite: true, rendu: (a) => p.nombre(a.nombreTitres, 0) },
      { cle: 'admission', titre: p.t('admin.col.admission'), rendu: (a) => p.date(a.dateAdmission) },
      { cle: 'dernierCours', titre: p.t('admin.col.dernierCours'), droite: true, rendu: (a) => <span className="font-semibold">{cours(p, a.dernierCours, 'Action')}</span> },
    ],
  },
  {
    cle: 'cours',
    groupe: 'marche',
    icone: Activity,
    modele: 'cours',
    compteur: 'cours',
    recherche: 'admin.filtre.recherche.cours',
    seances: true,
    colonnes: (p) => [
      { cle: 'nom', titre: p.t('admin.col.titre'), rendu: cellTitre },
      {
        cle: 'type',
        titre: p.t('admin.col.type'),
        rendu: (c) => <Etiquette ton={c.typeTitre === 'Action' ? 'orange' : 'action'}>{libelleChoix(p.t, c.typeTitre)}</Etiquette>,
      },
      { cle: 'seance', titre: p.t('admin.col.seance'), rendu: (c) => p.date(c.dateSeance) },
      { cle: 'reference', titre: p.t('admin.col.reference'), droite: true, rendu: (c) => cours(p, c.coursReference, c.typeTitre) },
      { cle: 'cloture', titre: p.t('admin.col.cloture'), droite: true, rendu: (c) => <span className="font-semibold">{cours(p, c.coursCloture, c.typeTitre)}</span> },
      { cle: 'volume', titre: p.t('admin.col.volume'), droite: true, rendu: (c) => p.nombre(c.volume, 0) },
      { cle: 'ccPublie', titre: p.t('admin.col.ccPublie'), droite: true, rendu: (c) => (c.couponCouruPublie == null ? '—' : p.montant(c.couponCouruPublie, 2)) },
      { cle: 'controle', titre: p.t('admin.col.controle'), rendu: (c) => (c.typeTitre === 'Obligation' ? <ControleCoupon controle={c.controle} /> : '—') },
    ],
  },
  {
    cle: 'dividendes',
    groupe: 'marche',
    icone: HandCoins,
    modele: 'dividendes',
    compteur: 'dividendes',
    recherche: 'admin.filtre.recherche.cours',
    colonnes: (p) => [
      { cle: 'nom', titre: p.t('admin.col.titre'), rendu: cellTitre },
      { cle: 'exercice', titre: p.t('admin.col.exercice'), droite: true, rendu: (d) => d.exercice },
      { cle: 'brut', titre: p.t('admin.col.brut'), droite: true, rendu: (d) => <span className="font-semibold">{p.montant(d.montantBrut, 2)}</span> },
      { cle: 'net', titre: p.t('admin.col.net'), droite: true, rendu: (d) => p.montant(d.montantNet, 2) },
      { cle: 'detachement', titre: p.t('admin.col.detachement'), rendu: (d) => p.date(d.dateDetachement) },
      { cle: 'paiement', titre: p.t('admin.col.paiement'), rendu: (d) => p.date(d.datePaiement) },
    ],
  },
  {
    cle: 'emetteurs',
    groupe: 'referentiel',
    icone: Landmark,
    modele: 'emetteurs',
    compteur: 'emetteurs',
    recherche: 'admin.filtre.recherche.emetteurs',
    filtres: (t) => [
      { cle: 'pays', tous: t('admin.filtre.pays'), options: optionsPays(t) },
      { cle: 'type', tous: t('admin.filtre.type'), options: optionsChoix(t, 'emetteurs', 'type') },
    ],
    colonnes: (p) => [
      { cle: 'nom', titre: p.t('admin.col.nom'), rendu: (e) => <span className="line-clamp-2 py-2 leading-snug font-semibold max-sm:text-[0.8125rem]">{e.nom}</span> },
      { cle: 'sigle', titre: p.t('admin.col.sigle'), rendu: (e) => e.sigle ?? '—' },
      { cle: 'type', titre: p.t('admin.col.type'), rendu: (e) => libelleChoix(p.t, e.type) },
      { cle: 'pays', titre: p.t('admin.col.pays'), rendu: (e) => cellPays(p.t, e.pays) },
      { cle: 'titres', titre: p.t('admin.col.titres'), droite: true, rendu: (e) => e.nombreTitres },
    ],
  },
  {
    cle: 'jours-non-ouvres',
    groupe: 'referentiel',
    icone: CalendarX,
    modele: 'joursNonOuvres',
    compteur: 'joursNonOuvres',
    recherche: 'admin.filtre.recherche.jours',
    filtres: (t) => {
      const annee = new Date().getFullYear()
      return [
        {
          cle: 'annee',
          tous: t('admin.filtre.annee'),
          options: Array.from({ length: 8 }, (_, i) => annee + 1 - i).map((a) => ({ valeur: String(a), libelle: String(a) })),
        },
      ]
    },
    colonnes: (p) => [
      { cle: 'date', titre: p.t('admin.col.date'), rendu: (j) => <span className="font-semibold">{p.dateLongue(j.date)}</span> },
      { cle: 'libelle', titre: p.t('admin.col.libelle'), rendu: (j) => j.libelle },
    ],
  },
]

export const GROUPES = ['titres', 'marche', 'referentiel']

/** La rubrique d'une adresse « /admin/cours » ; la première par défaut. */
export function rubriqueDuChemin(chemin) {
  const cle = /^\/admin\/([a-z-]+)\/?$/.exec(chemin)?.[1]
  return RUBRIQUES.find((r) => r.cle === cle) ?? null
}
