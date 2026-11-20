import {
  Banknote,
  Calculator,
  ChartNoAxesCombined,
  Clock,
  Hash,
  LayoutGrid,
  Loader,
  MapPin,
  MousePointerClick,
  Palette,
  Percent,
  Play,
  RectangleHorizontal,
  Search,
  Share2,
  Sparkles,
  Table2,
  Tag,
  TextCursorInput,
  ToggleLeft,
  TrendingUp,
  Type,
  Wallet,
} from 'lucide-react'
import { useState } from 'react'
import { IconeWhatsApp } from '../../components/icones/Icones'
import { Apparition, Grille, Page, Section } from '../../components/mise-en-page/MiseEnPage'
import Resultat from '../../components/titres/Resultat'
import Bascule from '../../components/ui/Bascule'
import Bouton from '../../components/ui/Bouton'
import Carte from '../../components/ui/Carte'
import Champ from '../../components/ui/Champ'
import ChampDate from '../../components/ui/date/ChampDate'
import Etiquette from '../../components/ui/Etiquette'
import { EtatVide, MessageErreur } from '../../components/ui/Etats'
import LienInstitution from '../../components/ui/LienInstitution'
import Pastille from '../../components/ui/Pastille'
import Pilules from '../../components/ui/Pilules'
import { SqueletteChiffreCle, SqueletteResultat, SqueletteTableau } from '../../components/ui/Squelettes'
import Tableau from '../../components/ui/Tableau'
import { usePreferences } from '../../contexte/Preferences'

/**
 * Vitrine des composants (développement uniquement, /composants) : chaque brique du site,
 * dans tous ses états, pour valider le style AVANT de recomposer les pages.
 */

const COULEURS = [
  ['Fond de page', '--cbc-fond'],
  ['Surface', '--cbc-surface'],
  ['Texte', '--cbc-texte'],
  ['Texte secondaire', '--cbc-texte-2'],
  ['Action', '--cbc-action'],
  ['Bleu marque', '--cbc-marque'],
  ['Orange', '--cbc-orange'],
  ['Orange texte', '--cbc-orange-texte'],
  ['WhatsApp', '--cbc-whatsapp'],
  ['Erreur', '--cbc-erreur'],
  ['Bordure', '--cbc-bordure'],
  ['Bordure de champ', '--cbc-bordure-champ'],
]

const ESPACEMENTS = [
  [8, 'icône ↔ texte, étiquette ↔ étiquette'],
  [16, 'éléments d’une grille ou d’un formulaire'],
  [24, 'intérieur de toutes les cartes, titre de section → contenu'],
  [32, 'haut de page'],
  [48, 'entre deux sections (mobile)'],
  [64, 'entre deux sections (grand écran)'],
]

const TITRES = [
  ['Exemple in fine 6,50 % 2025-2030', 'CM0000000011', 'Émetteur A', 'Obligation', 'CM', 6.5, '2030-06-30', 98.5],
  ['Exemple amortissable 6,00 % 2024-2029', 'GA0000000020', 'Émetteur B', 'Obligation', 'GA', 6, '2029-09-30', 101],
  ['Exemple semestriel 5,00 % 2026-2028', 'CG0000000038', 'Émetteur C', 'Obligation', 'CG', 5, '2028-09-30', 99.75],
  ['Société Exemple SA', 'CM0000000045', 'Société Exemple', 'Action', 'CM', null, null, 32000],
  ['Exemple Tchad 6,25 % 2025-2031', 'TD0000000052', 'Émetteur D', 'Obligation', 'TD', 6.25, '2031-03-31', 100],
  ['Exemple Guinée équatoriale 5,75 %', 'GQ0000000069', 'Émetteur E', 'Obligation', 'GQ', 5.75, '2029-12-31', 97.4],
].map(([nom, isin, emetteur, type, pays, taux, echeance, cours]) => ({ nom, isin, emetteur, type, pays, taux, echeance, cours }))

const ECHEANCES = [
  ['2026-06-30', 6500, 0],
  ['2027-06-30', 6500, 0],
  ['2028-06-30', 6500, 0],
  ['2029-07-02', 6500, 0],
  ['2030-07-01', 6500, 100000],
].map(([date, coupon, capital], i) => ({ rang: i + 1, date, coupon, capital, total: coupon + capital }))

const attendre = (ms) => new Promise((r) => setTimeout(r, ms))

export default function Vitrine() {
  const { t, nombre, montant, pourcent, date } = usePreferences()
  const [mode, setMode] = useState('simple')
  const [type, setType] = useState('tous')
  const [pays, setPays] = useState(null)
  const [cours, setCours] = useState('98,5')
  const [chargement, setChargement] = useState(false)
  const [envoi, setEnvoi] = useState(false)
  const [tour, setTour] = useState(0)
  const [dateVitrine, setDateVitrine] = useState('2026-01-12')

  async function simulerChargement() {
    setChargement(true)
    await attendre(1800)
    setChargement(false)
  }

  async function simulerEnvoi() {
    setEnvoi(true)
    await attendre(1200)
    setEnvoi(false)
  }

  const colonnesTitres = [
    {
      cle: 'nom',
      titre: t('col.titre'),
      rendu: (l) => (
        <span className="flex flex-col gap-0.5 py-2">
          <span className="line-clamp-2 leading-snug max-sm:text-[0.8125rem]">{l.nom}</span>
          <span className="text-xs font-normal text-texte-2">{l.isin}</span>
        </span>
      ),
    },
    { cle: 'emetteur', titre: t('col.emetteur'), rendu: (l) => <span className="text-texte-2">{l.emetteur}</span> },
    {
      cle: 'type',
      titre: t('col.type'),
      rendu: (l) =>
        l.type === 'Action' ? (
          <Etiquette ton="orange" icone={ChartNoAxesCombined}>{t('type.Action')}</Etiquette>
        ) : (
          <Etiquette ton="action" icone={Banknote}>{t('type.Obligation')}</Etiquette>
        ),
    },
    { cle: 'pays', titre: t('filtre.pays'), rendu: (l) => t(`pays.${l.pays}`) },
    { cle: 'taux', titre: t('col.taux'), droite: true, rendu: (l) => (l.taux == null ? '—' : pourcent(l.taux)) },
    { cle: 'echeance', titre: t('col.echeance'), rendu: (l) => (l.echeance ? date(l.echeance) : '—') },
    {
      cle: 'cours',
      titre: t('col.cours'),
      droite: true,
      rendu: (l) => <span className="font-semibold">{l.type === 'Action' ? montant(l.cours, 0) : pourcent(l.cours)}</span>,
    },
  ]

  const erreurCours = cours.trim() === '' ? 'Saisissez un cours, en % du nominal.' : null

  return (
    <Page>
      {/* En-tête de la vitrine */}
      <header className="apparition">
        <Etiquette ton="orange" point>Développement uniquement</Etiquette>
        <h1 className="mt-4 text-[2rem] leading-tight font-bold sm:text-5xl">Vitrine des composants</h1>
        <p className="mt-4 max-w-2xl text-texte-2">
          Chaque brique du site, dans tous ses états. Changez la langue et le thème en haut à droite : tout doit rester lisible
          et rien ne doit déborder, jusqu’à 360 px de large.
        </p>
        <nav aria-label="Sommaire" className="mt-6 flex flex-wrap gap-2">
          {[
            ['couleurs', 'Couleurs'],
            ['typographie', 'Typographie'],
            ['espacements', 'Espacements'],
            ['boutons', 'Boutons'],
            ['champs', 'Champs'],
            ['choix', 'Choix'],
            ['cartes', 'Cartes'],
            ['tableaux', 'Tableaux'],
            ['chargement', 'Chargement'],
            ['etats', 'États'],
            ['animations', 'Animations'],
          ].map(([ancre, texte]) => (
            <Bouton key={ancre} href={`#${ancre}`} taille="petite">
              {texte}
            </Bouton>
          ))}
        </nav>
      </header>

      {/* 1. Couleurs */}
      <Section id="couleurs" titre="Couleurs" icone={Palette} aide="La charte, en clair et en sombre. Aucune autre couleur n’est utilisée.">
        <Grille colonnes={4}>
          {COULEURS.map(([nom, variable], i) => (
            <Apparition key={variable} rang={i}>
              <Carte className="flex items-center gap-3 p-4">
                <span className="size-10 shrink-0 rounded-full border border-bordure" style={{ background: `var(${variable})` }} />
                <span className="min-w-0">
                  <span className="block truncate text-sm font-medium">{nom}</span>
                  <code className="block truncate text-xs text-texte-2">{variable}</code>
                </span>
              </Carte>
            </Apparition>
          ))}
        </Grille>
      </Section>

      {/* 2. Typographie */}
      <Section id="typographie" titre="Typographie" icone={Type} aide="Plus Jakarta Sans pour les titres, Inter pour le texte et les chiffres.">
        <Carte className="space-y-6">
          {[
            ['Titre de page — Plus Jakarta Sans 700, 32 → 48 px', <p key="h1" className="font-titre text-[2rem] leading-tight font-bold sm:text-5xl">Coupon couru</p>],
            ['Titre de section — 700, 24 → 28 px', <p key="h2" className="font-titre text-2xl font-bold sm:text-[1.75rem]">Échéancier</p>],
            ['Titre de carte — 600, 18 px', <p key="h3" className="font-titre text-lg font-semibold">Rendement actuariel</p>],
            ['Texte — Inter 400, 15 px', <p key="p">Le coupon couru revient au vendeur pour les jours où il a détenu le titre.</p>],
            ['Texte secondaire — 14 px', <p key="s" className="text-sm text-texte-2">Prix plein coupon, hors frais de bourse.</p>],
            ['Chiffres alignés — Inter, chiffres de même largeur', <p key="n" className="chiffres font-titre text-3xl font-bold">{montant(102043.84)}</p>],
          ].map(([legende, exemple]) => (
            <div key={legende} className="grid gap-2 border-b border-bordure pb-6 last:border-0 last:pb-0 sm:grid-cols-[220px_1fr] sm:items-baseline sm:gap-6">
              <p className="text-xs text-texte-2">{legende}</p>
              <div className="min-w-0 break-words">{exemple}</div>
            </div>
          ))}
        </Carte>
      </Section>

      {/* 3. Espacements */}
      <Section id="espacements" titre="Espacements" icone={RectangleHorizontal} aide="Six pas seulement, toujours les mêmes : c’est ce qui donne le rythme.">
        <Carte className="space-y-4">
          {ESPACEMENTS.map(([px, usage]) => (
            <div key={px} className="grid grid-cols-[48px_1fr] items-center gap-4 sm:grid-cols-[48px_80px_1fr]">
              <span className="chiffres text-sm font-semibold">{px} px</span>
              <span className="h-3 rounded-full bg-action/70" style={{ width: px }} />
              <span className="col-span-2 text-sm text-texte-2 sm:col-span-1">{usage}</span>
            </div>
          ))}
        </Carte>
      </Section>

      {/* 4. Boutons */}
      <Section id="boutons" titre="Boutons" icone={MousePointerClick} aide="Toujours en pilule. Un seul bouton bleu (l’action principale) par écran.">
        <Carte className="space-y-6">
          <div className="flex flex-wrap gap-4">
            <Bouton variante="action" icone={Calculator}>Calculer</Bouton>
            <Bouton icone={Share2}>Secondaire</Bouton>
            <Bouton variante="fantome" icone={Search}>Fantôme</Bouton>
            <Bouton variante="whatsapp" icone={IconeWhatsApp}>WhatsApp</Bouton>
          </div>
          <div className="flex flex-wrap items-center gap-4">
            <Bouton variante="action" taille="petite">Petit</Bouton>
            <Bouton taille="petite" icone={Hash}>Petit secondaire</Bouton>
            <Bouton taille="icone" aria-label="Rechercher" icone={Search} />
            <Bouton variante="action" chargement={envoi} onClick={simulerEnvoi} icone={Play}>
              {envoi ? t('calc.encours') : 'Essayer le chargement'}
            </Bouton>
            <Bouton disabled>Désactivé</Bouton>
          </div>
        </Carte>
      </Section>

      {/* 5. Champs */}
      <Section id="champs" titre="Zones de saisie" icone={TextCursorInput} aide="Libellé toujours visible, bordure marquée, halo bleu quand on écrit, rouge en cas d’erreur.">
        <Carte>
          <Grille colonnes={2}>
            <ChampDate libelle={t('calc.date')} valeur={dateVitrine} joursOuvres onChange={setDateVitrine} min="2025-06-30" max="2030-06-30" aide="Règlement le jeudi 15 janvier 2026 (T+3)" />
            <Champ libelle={t('calc.quantite')} icone={Hash} inputMode="numeric" defaultValue="10" unite={t('unite.titres')} />
            <Champ
              libelle={`${t('calc.cours')} (videz-le pour voir l’erreur)`}
              icone={Tag}
              inputMode="decimal"
              value={cours}
              onChange={(e) => setCours(e.target.value)}
              unite="%"
              erreur={erreurCours}
            />
            <Champ libelle="Recherche" icone={Search} type="search" placeholder={t('accueil.recherche')} />
            <Champ libelle="Champ désactivé" icone={Hash} defaultValue="1" disabled />
          </Grille>
        </Carte>
      </Section>

      {/* 6. Choix */}
      <Section id="choix" titre="Interrupteurs et filtres" icone={ToggleLeft} aide="Le curseur glisse sous l’option choisie. Les filtres défilent dans leur bande sur mobile.">
        <Carte className="space-y-6">
          <div className="flex flex-wrap items-center gap-4">
            <Bascule
              libelle={t('mode.groupe')}
              valeur={mode}
              onChange={setMode}
              options={[
                { valeur: 'simple', libelle: t('mode.simple') },
                { valeur: 'expert', libelle: t('mode.expert') },
              ]}
            />
            <Bascule
              libelle={t('col.type')}
              valeur={type}
              onChange={setType}
              options={[
                { valeur: 'tous', libelle: t('filtre.tous') },
                { valeur: 'Obligation', libelle: t('filtre.obligations'), icone: Banknote },
                { valeur: 'Action', libelle: t('filtre.actions'), icone: ChartNoAxesCombined },
              ]}
            />
          </div>
          <Pilules
            libelle={t('filtre.pays')}
            valeur={pays}
            onChange={setPays}
            options={[null, 'CM', 'CF', 'CG', 'GA', 'GQ', 'TD'].map((code) => ({
              valeur: code,
              libelle: code ? t(`pays.${code}`) : t('filtre.tous'),
              prefixe: code ? <span className="chiffres text-[0.6875rem] font-semibold opacity-60">{code}</span> : null,
            }))}
          />
          <div className="flex flex-wrap gap-2">
            <Etiquette ton="action" icone={Banknote}>{t('type.Obligation')}</Etiquette>
            <Etiquette ton="orange" icone={ChartNoAxesCombined}>{t('type.Action')}</Etiquette>
            <Etiquette ton="orange" point>{t('statut.Actif')}</Etiquette>
            <Etiquette icone={MapPin}>{t('pays.CM')}</Etiquette>
            <Etiquette className="chiffres">CM0000000011</Etiquette>
            <Etiquette ton="erreur" point>{t('statut.Suspendu')}</Etiquette>
          </div>
          <div className="flex flex-wrap gap-2">
            <LienInstitution code="bvmac" role />
            <LienInstitution code="cosumaf" role />
            <LienInstitution code="bvmac" />
            <LienInstitution code="cosumaf" />
          </div>
        </Carte>
      </Section>

      {/* 7. Cartes */}
      <Section id="cartes" titre="Cartes et résultats" icone={LayoutGrid} aide="24 px à l’intérieur, 16 px entre elles. Le résultat principal est cerclé de bleu.">
        <div className="space-y-4">
          <Grille colonnes={3}>
            {[
              [Banknote, 3, t('cle.obligations'), 'action'],
              [ChartNoAxesCombined, 1, t('cle.actions'), 'orange'],
              [MapPin, 6, t('cle.pays'), 'neutre'],
            ].map(([icone, n, libelle, ton]) => (
              <Carte key={libelle} interactive className="flex items-center gap-4">
                <Pastille icone={icone} ton={ton} />
                <div>
                  <p className="chiffres font-titre text-3xl leading-none font-bold">{n}</p>
                  <p className="mt-1 text-sm text-texte-2">{libelle}</p>
                </div>
              </Carte>
            ))}
          </Grille>
          <Grille colonnes={2}>
            <Resultat
              principal
              className="sm:col-span-2"
              icone={Wallet}
              libelle={t('res.plein')}
              valeur={montant(102043.84)}
              aide={t('res.plein.aide')}
              explication={
                <>
                  10 000 × {pourcent(98.5)} = <b>{nombre(9850)}</b>
                  <br />({nombre(9850)} + {nombre(354.38)}) × 10 = <b>{montant(102043.84)}</b>
                </>
              }
            />
            <Resultat
              icone={Clock}
              ton="orange"
              libelle={t('res.couru')}
              valeur={montant(3543.84)}
              aide={t('res.couru.aide', { montant: montant(354.38) })}
              explication={<>10 000 × {pourcent(6.5)} × 199 ÷ 365 × 10 = <b>{montant(3543.84)}</b></>}
            />
            <Resultat
              icone={TrendingUp}
              libelle={t('res.actuariel')}
              valeur={pourcent(6.8836)}
              aide={t('res.actuariel.aide')}
              explication={<>r = <b>{pourcent(6.8836, 4)}</b></>}
            />
          </Grille>
        </div>
      </Section>

      {/* 8. Tableaux */}
      <Section
        id="tableaux"
        titre="Tableaux rayés"
        icone={Table2}
        aide="Une ligne sur deux teintée, survol bleuté, chiffres à droite. Sur mobile : la première colonne reste fixe, le reste défile dans le cadre."
      >
        <div className="space-y-6">
          <Tableau
            legende={t('liste.titre')}
            colonnes={colonnesTitres}
            lignes={TITRES}
            cleLigne={(l) => l.isin}
            lienLigne={() => '/composants#tableaux'}
          />
          <Tableau
            legende={t('ech.titre')}
            colonnes={[
              { cle: 'date', titre: t('col.paiement'), rendu: (l) => (l.rang === 'total' ? t('col.total') : date(l.date)) },
              { cle: 'coupon', titre: t('col.coupon'), droite: true, rendu: (l) => nombre(l.coupon) },
              { cle: 'capital', titre: t('col.capital'), droite: true, rendu: (l) => nombre(l.capital) },
              { cle: 'total', titre: t('col.total'), droite: true, rendu: (l) => nombre(l.total) },
            ]}
            lignes={ECHEANCES}
            cleLigne={(l) => l.rang}
            pied={{ rang: 'total', coupon: 32500, capital: 100000, total: 132500 }}
          />
        </div>
      </Section>

      {/* 9. Chargement */}
      <Section
        id="chargement"
        titre="Chargement"
        icone={Loader}
        aide="Chaque skeleton a la forme exacte du contenu qu’il remplace : rien ne saute à l’arrivée des données."
        actions={
          <Bouton variante="action" taille="petite" icone={Play} onClick={simulerChargement} disabled={chargement}>
            Simuler un chargement
          </Bouton>
        }
      >
        <div className="space-y-4">
          <Grille colonnes={3}>
            {chargement
              ? [0, 1, 2].map((i) => <SqueletteChiffreCle key={i} />)
              : [
                  [Banknote, 3, t('cle.obligations'), 'action'],
                  [ChartNoAxesCombined, 1, t('cle.actions'), 'orange'],
                  [MapPin, 6, t('cle.pays'), 'neutre'],
                ].map(([icone, n, libelle, ton], i) => (
                  <Apparition key={libelle} rang={i}>
                    <Carte className="flex items-center gap-4">
                      <Pastille icone={icone} ton={ton} />
                      <div>
                        <p className="chiffres font-titre text-3xl leading-none font-bold">{n}</p>
                        <p className="mt-1 text-sm text-texte-2">{libelle}</p>
                      </div>
                    </Carte>
                  </Apparition>
                ))}
          </Grille>
          {chargement ? (
            <SqueletteResultat principal />
          ) : (
            <Apparition>
              <Resultat principal icone={Wallet} libelle={t('res.plein')} valeur={montant(102043.84)} aide={t('res.plein.aide')} explication={<b>{montant(102043.84)}</b>} />
            </Apparition>
          )}
          {chargement ? (
            <SqueletteTableau colonnes={colonnesTitres} lignes={6} lienLigne />
          ) : (
            <Apparition>
              <Tableau legende={t('liste.titre')} colonnes={colonnesTitres} lignes={TITRES} cleLigne={(l) => l.isin} lienLigne={() => '/composants#chargement'} />
            </Apparition>
          )}
        </div>
      </Section>

      {/* 10. États */}
      <Section id="etats" titre="États vides et erreurs" icone={Sparkles}>
        <Grille colonnes={2}>
          <EtatVide icone={Search} titre={t('liste.vide')} texte="Essayez un autre nom, un émetteur ou un code ISIN." action={<Bouton taille="petite">Effacer la recherche</Bouton>} />
          <div className="space-y-4">
            <MessageErreur erreur={{ cle: 'erreur.reseau' }} onReessayer={() => {}} />
            <MessageErreur erreur={{ cle: 'erreur.limite' }} />
            <MessageErreur erreur={{ message: 'La date de négociation est après l’échéance du titre.' }} />
          </div>
        </Grille>
      </Section>

      {/* 11. Animations */}
      <Section
        id="animations"
        titre="Animations"
        icone={Percent}
        aide="Apparition en fondu et montée de 8 px, 40 ms d’écart entre les éléments. Coupées si l’ordinateur demande moins d’animations."
        actions={
          <Bouton taille="petite" icone={Play} onClick={() => setTour((n) => n + 1)}>
            Rejouer
          </Bouton>
        }
      >
        <Grille colonnes={4} key={tour}>
          {Array.from({ length: 8 }, (_, i) => (
            <Apparition key={i} rang={i}>
              <Carte interactive className="flex items-center gap-3 p-4">
                <Pastille icone={Sparkles} ton={i % 2 ? 'orange' : 'action'} taille="petite" />
                <span className="text-sm font-medium">Élément {i + 1}</span>
              </Carte>
            </Apparition>
          ))}
        </Grille>
      </Section>
    </Page>
  )
}
