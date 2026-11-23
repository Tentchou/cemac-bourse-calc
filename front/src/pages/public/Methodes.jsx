import { ArrowRight, BookOpen, Calculator, CalendarDays, Clock, Coins, Info, Scale, Sigma, TrendingUp, Variable, Wallet } from 'lucide-react'
import { Exemple, Notes, TitreBloc, Variables } from '../../components/methodes/BlocsMethode'
import Formule from '../../components/methodes/Formule'
import Sommaire from '../../components/methodes/Sommaire'
import {
  ACTION,
  CONVENTIONS,
  OBLIGATION,
  PERIODE_CONVENTIONS,
  exempleCouponCouru,
  exempleDividende,
  exempleEcheancier,
  exemplePrixPlein,
  exempleRendement,
  joursConvention,
} from '../../components/methodes/exemples'
import { Apparition, Page, Section } from '../../components/mise-en-page/MiseEnPage'
import Bouton from '../../components/ui/Bouton'
import Carte from '../../components/ui/Carte'
import Pastille from '../../components/ui/Pastille'
import Tableau from '../../components/ui/Tableau'
import { usePreferences } from '../../contexte/Preferences'

/** Une méthode : formule(s) et variables à gauche, exemple à droite, points d'attention dessous. */
function CarteMethode({ formules, variables, exemple, notes }) {
  const { t } = usePreferences()
  return (
    <Carte sansMarge className="overflow-hidden">
      <div className="grid xl:grid-cols-2">
        <div className="flex min-w-0 flex-col gap-6 p-6">
          <div>
            <TitreBloc icone={Sigma}>{t('meth.formule')}</TitreBloc>
            <div className="flex flex-col gap-4">
              {formules.map((f) => (
                <Formule key={f.libelle ?? 'formule'} {...f} />
              ))}
            </div>
          </div>
          <div>
            <TitreBloc icone={Variable}>{t('meth.variables')}</TitreBloc>
            <Variables lignes={variables} />
          </div>
        </div>
        <div className="min-w-0 border-t border-bordure bg-rayure/60 p-6 xl:border-t-0 xl:border-l">
          <TitreBloc icone={Calculator}>{t('meth.exemple')}</TitreBloc>
          {exemple}
        </div>
      </div>
      <div className="border-t border-bordure p-6">
        <TitreBloc icone={Info}>{t('meth.attention')}</TitreBloc>
        <Notes notes={notes} />
      </div>
    </Carte>
  )
}

/** Toutes les formules du site, expliquées pas à pas, avec un sommaire qui suit la lecture. */
export default function Methodes() {
  const { t, nombre, montant, pourcent, date } = usePreferences()
  const o = OBLIGATION
  const n = (v, d = 2) => nombre(v, d)
  const fcfa = (v, d = 2) => montant(v, d)
  const pct = (v) => pourcent(v, 2)
  const couru = exempleCouponCouru()
  const plein = exemplePrixPlein()
  const rend = exempleRendement()
  const ech = exempleEcheancier()
  const div = exempleDividende()
  const valeurs = {
    titre: o.titre,
    bulletin: o.bulletin,
    nominal: fcfa(couru.nominal, 0),
    taux: pct(o.taux),
    jours: couru.jours,
    quantite: o.quantite,
    cours: pct(o.cours),
    date: date(o.dateNegociation),
    reglement: date(o.dateReglement),
    coupon: date(o.dernierCoupon),
    publie: fcfa(o.couruPublie),
  }
  const somme = (cle) => ech.reduce((s, l) => s + l[cle], 0)

  const entrees = [
    { id: 'coupon-couru', titre: t('meth.couru.titre'), icone: Clock },
    { id: 'prix-plein', titre: t('meth.plein.titre'), icone: Wallet },
    { id: 'rendement', titre: t('meth.rend.titre'), icone: TrendingUp },
    { id: 'echeancier', titre: t('meth.ech.titre'), icone: CalendarDays },
    { id: 'dividende', titre: t('meth.div.titre'), icone: Coins },
    { id: 'regles', titre: t('meth.regles.titre'), icone: Scale },
  ]

  const regles = [
    { icone: Clock, titre: t('meth.regles.reglement'), texte: t('meth.regles.reglement.t') },
    { icone: CalendarDays, titre: t('meth.regles.seance'), texte: t('meth.regles.seance.t') },
    { icone: Calculator, titre: t('meth.regles.arrondi'), texte: t('meth.regles.arrondi.t') },
    { icone: TrendingUp, titre: t('meth.regles.cours'), texte: t('meth.regles.cours.t') },
  ]

  const conventions = CONVENTIONS.map((c) => {
    const jours = joursConvention(c.code, PERIODE_CONVENTIONS.debut, PERIODE_CONVENTIONS.fin)
    return { ...c, jours: `${jours} ÷ ${c.base}`, fraction: n(jours / c.base, 4) }
  })

  return (
    <Page>
      <Apparition as="header" className="max-w-3xl">
        <div className="flex items-center gap-4">
          <Pastille icone={BookOpen} taille="grande" />
          <h1 className="text-[1.75rem] leading-tight font-extrabold sm:text-4xl">{t('meth.titre')}</h1>
        </div>
        <p className="mt-4 text-base text-texte-2 sm:text-lg">{t('meth.intro')}</p>
      </Apparition>

      {/* Bloc (pas grille) sur mobile : la bande du sommaire reste collée sur toute la hauteur du contenu. */}
      <div className="lg:grid lg:grid-cols-[13rem_minmax(0,1fr)] lg:gap-x-12">
        <Sommaire entrees={entrees} libelle={t('meth.sommaire')} />

        <div className="flex min-w-0 flex-col gap-12 sm:gap-16">
          <Section id="coupon-couru" titre={t('meth.couru.titre')} icone={Clock} aide={t('meth.couru.intro')}>
            <CarteMethode
              formules={[{ jetons: [{ v: 'couru' }, '=', { v: 'nominal' }, '×', { v: 'taux' }, '×', { v: 'jours' }, '÷', { v: 'base' }] }]}
              variables={[
                { v: 'nominal', libelle: t('meth.couru.var.nominal'), valeur: fcfa(couru.nominal, 0) },
                { v: 'taux', libelle: t('meth.couru.var.taux'), valeur: pct(o.taux) },
                { v: 'jours', libelle: t('meth.couru.var.jours'), valeur: couru.jours },
                { v: 'base', libelle: t('meth.couru.var.base'), valeur: o.base },
              ]}
              exemple={
                <Exemple
                  contexte={t('meth.couru.contexte', valeurs)}
                  etapes={[
                    { libelle: t('meth.couru.etape1'), calcul: `${n(couru.nominal, 0)} × ${pct(o.taux)} × ${couru.jours} ÷ ${o.base}`, resultat: `${n(couru.parTitre, 4)}…` },
                    { libelle: t('meth.couru.etape2', valeurs), calcul: `${n(couru.parTitre, 4)}… × ${o.quantite}`, resultat: fcfa(couru.total) },
                  ]}
                />
              }
              notes={[t('meth.couru.note1'), t('meth.couru.note2'), t('meth.couru.note3')]}
            />
          </Section>

          <Section id="prix-plein" titre={t('meth.plein.titre')} icone={Wallet} aide={t('meth.plein.intro')}>
            <CarteMethode
              formules={[{ jetons: [{ v: 'plein' }, '=', '(', { v: 'nominal' }, '×', { v: 'cours' }, '÷', '100', '+', { v: 'couru' }, ')', '×', { v: 'quantite' }] }]}
              variables={[
                { v: 'nominal', libelle: t('meth.plein.var.nominal'), valeur: fcfa(couru.nominal, 0) },
                { v: 'cours', libelle: t('meth.plein.var.cours'), valeur: pct(o.cours) },
                { v: 'couru', libelle: t('meth.plein.var.couru'), valeur: n(plein.couru) },
                { v: 'quantite', libelle: t('meth.plein.var.quantite'), valeur: o.quantite },
              ]}
              exemple={
                <Exemple
                  contexte={t('meth.plein.contexte', valeurs)}
                  etapes={[
                    { libelle: t('meth.plein.etape1'), calcul: `${n(couru.nominal, 0)} × ${n(o.cours)} ÷ 100`, resultat: n(plein.piedParTitre) },
                    { libelle: t('meth.plein.etape2'), calcul: `${n(plein.piedParTitre)} + ${n(plein.couru, 4)}…`, resultat: `${n(plein.pleinParTitre, 4)}…` },
                    { libelle: t('meth.plein.etape3', valeurs), calcul: `${n(plein.pleinParTitre, 4)}… × ${o.quantite}`, resultat: fcfa(plein.total) },
                  ]}
                />
              }
              notes={[t('meth.plein.note1'), t('meth.plein.note2')]}
            />
          </Section>

          <Section id="rendement" titre={t('meth.rend.titre')} icone={TrendingUp} aide={t('meth.rend.intro')}>
            <CarteMethode
              formules={[
                { libelle: t('meth.rend.formule.courant'), jetons: [{ v: 'courant' }, '=', { v: 'taux' }, '÷', { v: 'cours' }, '×', '100'] },
                {
                  libelle: t('meth.rend.formule.actuariel'),
                  jetons: [{ v: 'prix' }, '=', 'Σ', { v: 'flux', k: true }, '÷', '(', '1', '+', { v: 'rendement' }, ')', { exp: [{ v: 'joursFlux', k: true }, '÷', '365'] }],
                },
              ]}
              variables={[
                { v: 'taux', libelle: t('meth.rend.var.taux'), valeur: pct(o.taux) },
                { v: 'cours', libelle: t('meth.rend.var.cours'), valeur: pct(o.cours) },
                { v: 'prix', libelle: t('meth.rend.var.prix'), valeur: n(rend.prix) },
                { v: 'flux', k: true, libelle: t('meth.rend.var.flux') },
                { v: 'joursFlux', k: true, libelle: t('meth.rend.var.joursFlux') },
                { v: 'rendement', libelle: t('meth.rend.var.rendement') },
              ]}
              exemple={
                <Exemple
                  contexte={t('meth.rend.contexte', { n: rend.flux.length })}
                  etapes={[
                    { libelle: t('meth.rend.etape1'), calcul: `${n(o.taux)} ÷ ${n(o.cours)} × 100`, resultat: pct(rend.courant) },
                    ...rend.flux.map((f) => ({
                      libelle: t('meth.rend.etapeFlux', { date: date(f.date), jours: f.jours }),
                      calcul: `${n(f.montant)} ÷ (1 + ${t('meth.sym.rendement')})^(${f.jours} ÷ 365)`,
                      resultat: `${n(f.valeurActuelle)} (${t('meth.rend.valeurActuelle')})`,
                    })),
                    {
                      libelle: t('meth.rend.etape4', { taux: pct(rend.actuariel) }),
                      calcul: rend.flux.map((f) => n(f.valeurActuelle)).join(' + '),
                      resultat: n(rend.prix),
                    },
                    { libelle: t('meth.rend.etape5'), resultat: `${t('meth.sym.rendement')} = ${pct(rend.actuariel)}` },
                  ]}
                />
              }
              notes={[t('meth.rend.note1'), t('meth.rend.note2'), t('meth.rend.note3')]}
            />
          </Section>

          <Section id="echeancier" titre={t('meth.ech.titre')} icone={CalendarDays} aide={t('meth.ech.intro')}>
            <CarteMethode
              formules={[
                { libelle: t('meth.ech.formule.reguliere'), jetons: [{ v: 'coupon' }, '=', { v: 'nominal' }, '×', { v: 'taux' }, '÷', { v: 'frequence' }] },
                { libelle: t('meth.ech.formule.irreguliere'), jetons: [{ v: 'coupon' }, '=', { v: 'nominal' }, '×', { v: 'taux' }, '×', { v: 'jours' }, '÷', { v: 'base' }] },
              ]}
              variables={[
                { v: 'coupon', libelle: t('meth.ech.var.coupon') },
                { v: 'nominal', libelle: t('meth.ech.var.nominal') },
                { v: 'taux', libelle: t('meth.ech.var.taux') },
                { v: 'frequence', libelle: t('meth.ech.var.frequence') },
                { v: 'jours', libelle: t('meth.ech.var.jours') },
                { v: 'base', libelle: t('meth.ech.var.base') },
              ]}
              exemple={
                <div>
                  <p className="mb-5 text-sm leading-6 text-texte-2">{t('meth.ech.contexte', { ...valeurs, nominal: fcfa(o.nominal, 0), n: ech.length, differe: o.differe })}</p>
                  <Tableau
                    legende={t('meth.ech.titre')}
                    cleLigne={(l) => l.rang}
                    colonnes={[
                      { cle: 'rang', titre: t('meth.ech.col.rang'), rendu: (l) => <span className="chiffres">{date(l.date)}</span> },
                      { cle: 'debut', titre: t('meth.ech.col.debut'), droite: true, rendu: (l) => (l.debut === null ? '' : n(l.debut)) },
                      { cle: 'coupon', titre: t('meth.ech.col.coupon'), droite: true, rendu: (l) => n(l.coupon) },
                      { cle: 'capital', titre: t('meth.ech.col.capital'), droite: true, rendu: (l) => n(l.capital) },
                      { cle: 'total', titre: t('meth.ech.col.total'), droite: true, rendu: (l) => n(l.total) },
                    ]}
                    lignes={ech}
                    libellePied={t('meth.ech.total')}
                    pied={{ debut: null, coupon: somme('coupon'), capital: somme('capital'), total: somme('total') }}
                  />
                </div>
              }
              notes={[t('meth.ech.note1'), t('meth.ech.note2'), t('meth.ech.note3'), t('meth.ech.note4')]}
            />
          </Section>

          <Section id="dividende" titre={t('meth.div.titre')} icone={Coins} aide={t('meth.div.intro')}>
            <CarteMethode
              formules={[{ jetons: [{ v: 'rendDiv' }, '=', { v: 'dividende' }, '÷', { v: 'cours' }, '×', '100'] }]}
              variables={[
                { v: 'dividende', libelle: t('meth.div.var.dividende'), valeur: fcfa(div.dividende, 0) },
                { v: 'cours', libelle: t('meth.div.var.cours'), valeur: fcfa(ACTION.cours, 0) },
              ]}
              exemple={
                <Exemple
                  contexte={t('meth.div.contexte', {
                    titre: ACTION.titre,
                    bulletin: o.bulletin,
                    cours: fcfa(ACTION.cours, 0),
                    date: date(ACTION.date),
                    dividende: fcfa(div.dividende, 0),
                    exercice: ACTION.exercice,
                  })}
                  etapes={[
                    { libelle: t('meth.div.etape2'), calcul: `${n(div.dividende, 0)} ÷ ${n(ACTION.cours, 0)} × 100`, resultat: pct(div.rendement) },
                  ]}
                />
              }
              notes={[t('meth.div.note1'), t('meth.div.note2')]}
            />
          </Section>

          <Section id="regles" titre={t('meth.regles.titre')} icone={Scale} aide={t('meth.regles.intro')}>
            <div className="flex flex-col gap-6">
              <div className="grid gap-4 sm:grid-cols-2">
                {regles.map(({ icone, titre, texte }) => (
                  <Carte key={titre} as="article" className="flex gap-4">
                    <Pastille icone={icone} ton="neutre" />
                    <div className="min-w-0">
                      <h3 className="font-semibold">{titre}</h3>
                      <p className="mt-1 text-sm leading-6 text-texte-2">{texte}</p>
                    </div>
                  </Carte>
                ))}
              </div>

              <div>
                <h3 className="text-lg font-bold">{t('meth.conv.titre')}</h3>
                <p className="mt-2 mb-4 text-sm text-texte-2">
                  {t('meth.conv.aide', { debut: date(PERIODE_CONVENTIONS.debut), fin: date(PERIODE_CONVENTIONS.fin) })}
                </p>
                <Tableau
                  legende={t('meth.conv.titre')}
                  cleLigne={(c) => c.code}
                  colonnes={[
                    { cle: 'code', titre: t('meth.conv.col.convention'), rendu: (c) => <span className="font-semibold whitespace-nowrap">{t(`meth.conv.nom.${c.code}`)}</span> },
                    { cle: 'regle', titre: t('meth.conv.col.regle'), rendu: (c) => <span className="text-texte-2">{t(`meth.conv.${c.code}`)}</span> },
                    { cle: 'jours', titre: t('meth.conv.col.jours'), droite: true, rendu: (c) => <span className="chiffres">{c.jours}</span> },
                    { cle: 'fraction', titre: t('meth.conv.col.fraction'), droite: true, rendu: (c) => <span className="chiffres whitespace-nowrap">{c.fraction}</span> },
                  ]}
                  lignes={conventions}
                />
              </div>
            </div>
          </Section>

          <Carte accent className="flex flex-col items-start gap-4 sm:flex-row sm:items-center sm:justify-between">
            <p className="text-sm text-texte-2">{t('meth.essayer.aide')}</p>
            <Bouton vers="/" variante="action" icone={ArrowRight} className="shrink-0">
              {t('meth.essayer')}
            </Bouton>
          </Carte>
        </div>
      </div>
    </Page>
  )
}
