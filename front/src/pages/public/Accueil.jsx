import { Banknote, ChartNoAxesCombined, LayoutList, Lightbulb, Map as IconeCarte, Search, X } from 'lucide-react'
import { useMemo, useState } from 'react'
import { titresService, useRequete } from '../../services'
import BandeauAccueil from '../../components/accueil/BandeauAccueil'
import EtapesIllustrees from '../../components/accueil/EtapesIllustrees'
import CarteCemac from '../../components/carte/CarteCemac'
import Drapeau from '../../components/carte/Drapeau'
import { PAYS_CEMAC } from '../../components/carte/drapeaux'
import { Page, Section } from '../../components/mise-en-page/MiseEnPage'
import ListeTitres, { colonnesTitres } from '../../components/titres/ListeTitres'
import { filtrerTitres } from '../../components/titres/filtrerTitres'
import Bascule from '../../components/ui/Bascule'
import Bouton from '../../components/ui/Bouton'
import { EtatVide, MessageErreur } from '../../components/ui/Etats'
import Pilules from '../../components/ui/Pilules'
import { SqueletteTableau } from '../../components/ui/Squelettes'
import { usePreferences } from '../../contexte/Preferences'

/**
 * L'accueil : bandeau (recherche, chiffres clés, aperçu d'un calcul), carte des 6 pays, liste filtrable, 3 étapes.
 * Un seul appel (la liste des titres) alimente les chiffres, la carte et le tableau ; chacun a
 * son skeleton à sa place pendant le chargement.
 */
export default function Accueil() {
  const preferences = usePreferences()
  const { t } = preferences
  const { donnees: titres, erreur, chargement, recharger } = useRequete((signal) => titresService.lister(signal), [])
  const [texte, setTexte] = useState('')
  const [type, setType] = useState('tous')
  const [pays, setPays] = useState(null)

  const liste = useMemo(() => titres ?? [], [titres])
  const comptes = useMemo(() => Object.fromEntries(PAYS_CEMAC.map((code) => [code, liste.filter((x) => x.pays === code).length])), [liste])
  const visibles = useMemo(() => filtrerTitres(liste, { texte, type, pays }), [liste, texte, type, pays])
  const filtre = texte || type !== 'tous' || pays

  const allerALaListe = () => document.getElementById('liste')?.scrollIntoView({ behavior: 'smooth', block: 'start' })
  const effacer = () => {
    setTexte('')
    setType('tous')
    setPays(null)
  }

  const chiffres = [
    { cle: 'obligations', valeur: liste.filter((x) => x.typeTitre === 'Obligation').length },
    { cle: 'actions', valeur: liste.filter((x) => x.typeTitre === 'Action').length },
    { cle: 'pays', valeur: PAYS_CEMAC.length },
  ]
  const premiereObligation = liste.find((x) => x.typeTitre === 'Obligation')

  return (
    <Page>
      <BandeauAccueil
        texte={texte}
        onTexte={setTexte}
        onChercher={allerALaListe}
        chiffres={chiffres}
        chargement={chargement}
        lienExemple={premiereObligation ? `/titres/${premiereObligation.id}?date=2026-01-12&quantite=10&cours=98.5` : null}
      />

      {/* Carte des 6 pays */}
      <Section id="carte" titre={t('carte.titre')} icone={IconeCarte} aide={t('carte.aide')}>
        <CarteCemac
          comptes={comptes}
          chargement={chargement}
          valeur={pays}
          onChange={(code) => {
            setPays(code)
            if (code) allerALaListe()
          }}
        />
      </Section>

      {/* Liste des titres */}
      <Section
        id="liste"
        titre={t('liste.titre')}
        icone={LayoutList}
        aide={chargement ? t('liste.chargement') : visibles.length === 1 ? t('liste.nombre.1') : t('liste.nombre', { n: visibles.length })}
        actions={
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
        }
      >
        <Pilules
          className="mb-4"
          libelle={t('filtre.pays')}
          valeur={pays}
          onChange={setPays}
          options={[null, ...PAYS_CEMAC].map((code) => ({
            valeur: code,
            libelle: code ? t(`pays.${code}`) : t('filtre.tous'),
            prefixe: code ? <Drapeau code={code} /> : null,
          }))}
        />
        {erreur ? (
          <MessageErreur erreur={erreur} onReessayer={recharger} />
        ) : chargement && !titres ? (
          <SqueletteTableau colonnes={colonnesTitres(preferences)} lignes={5} lienLigne />
        ) : visibles.length === 0 ? (
          <EtatVide
            icone={Search}
            titre={t('liste.vide')}
            texte={t('liste.vide.aide')}
            action={
              filtre && (
                <Bouton taille="petite" icone={X} onClick={effacer}>
                  {t('liste.effacer')}
                </Bouton>
              )
            }
          />
        ) : (
          <ListeTitres titres={visibles} className="apparition" />
        )}
      </Section>

      {/* En 3 étapes */}
      <Section id="etapes" titre={t('etapes.titre')} icone={Lightbulb}>
        <EtapesIllustrees />
      </Section>
    </Page>
  )
}
