import { Calculator, Coins, HandCoins, History, Receipt, SlidersHorizontal } from 'lucide-react'
import { useState } from 'react'
import { calculsService, useRequete } from '../../services'
import { usePreferences } from '../../contexte/Preferences'
import { lireNombreSaisi } from '../../i18n/format'
import { Grille, Section } from '../mise-en-page/MiseEnPage'
import Bouton from '../ui/Bouton'
import Carte from '../ui/Carte'
import Champ from '../ui/Champ'
import { MessageErreur } from '../ui/Etats'
import { SqueletteResultat } from '../ui/Squelettes'
import Tableau from '../ui/Tableau'
import Resultat from './Resultat'

/** Rendement du dividende d'une action (sa requête, son skeleton), puis l'historique des dividendes. */
export default function CalculateurDividende({ titre }) {
  const { t, nombre, montant, pourcent, date, langue } = usePreferences()
  const coursInitial = titre.dernierCours != null ? String(titre.dernierCours) : ''
  const [cours, setCours] = useState(coursInitial)
  const [demande, setDemande] = useState(() => lireNombreSaisi(coursInitial))
  const [erreurCours, setErreurCours] = useState(false)
  const dividendes = titre.action?.dividendes ?? []

  const calcul = useRequete(
    (signal) => (demande ? calculsService.rendementDividende(titre.id, { cours: demande }, signal) : null),
    [demande],
  )
  const r = calcul.donnees

  function calculer(evenement) {
    evenement.preventDefault()
    const valeur = lireNombreSaisi(cours)
    setErreurCours(valeur === null || valeur <= 0)
    if (valeur > 0) setDemande(valeur)
  }

  return (
    <>
      <Section id="dividende" titre={t('div.titre')} icone={HandCoins} aide={t('div.passe')}>
        <div className="grid grid-cols-[minmax(0,1fr)] items-start gap-4 lg:grid-cols-[360px_minmax(0,1fr)]">
          <Carte as="form" noValidate onSubmit={calculer} className="apparition grid gap-4">
            <h3 className="flex items-center gap-2 text-base font-semibold">
              <SlidersHorizontal aria-hidden="true" className="size-4 text-action" strokeWidth={2} />
              {t('calc.operation')}
            </h3>
            <Champ
              libelle={t('div.cours')}
              icone={Coins}
              inputMode="decimal"
              value={cours}
              onChange={(e) => setCours(e.target.value)}
              unite={langue === 'fr' ? 'FCFA' : 'XAF'}
              erreur={erreurCours ? '> 0' : null}
            />
            <Bouton type="submit" variante="action" icone={Calculator} chargement={calcul.chargement && Boolean(demande)} className="mt-2 w-full">
              {t('calc.lancer')}
            </Bouton>
          </Carte>

          <div className="min-w-0">
            {calcul.erreur ? (
              <MessageErreur erreur={calcul.erreur} onReessayer={calcul.recharger} />
            ) : (
              <Grille colonnes={2}>
                {r ? (
                  <>
                    <Resultat
                      principal
                      className="apparition"
                      icone={Coins}
                      ton="orange"
                      libelle={t('div.brut')}
                      valeur={pourcent(r.rendementBrut)}
                      aide={t('div.exercice', { exercice: r.exercice })}
                      explication={
                        <>
                          {nombre(r.dividendeBrut)} ÷ {nombre(r.cours)} × 100 = <b>{pourcent(r.rendementBrut, 4)}</b>
                        </>
                      }
                    />
                    {r.rendementNet != null && (
                      <Resultat
                        className="apparition"
                        style={{ '--rang': 1 }}
                        icone={Receipt}
                        ton="neutre"
                        libelle={t('div.net')}
                        valeur={pourcent(r.rendementNet)}
                        aide={t('div.net.aide')}
                        explication={
                          <>
                            {nombre(r.dividendeNet)} ÷ {nombre(r.cours)} × 100 = <b>{pourcent(r.rendementNet, 4)}</b>
                          </>
                        }
                      />
                    )}
                  </>
                ) : (
                  demande && (
                    <>
                      <SqueletteResultat principal />
                      <SqueletteResultat />
                    </>
                  )
                )}
              </Grille>
            )}
          </div>
        </div>
      </Section>

      {dividendes.length > 0 && (
        <Section id="historique" titre={t('div.historique')} icone={History}>
          <Tableau
            className="apparition"
            legende={t('div.historique')}
            colonnes={[
              { cle: 'exercice', titre: t('col.exercice') },
              { cle: 'brut', titre: t('col.brut'), droite: true, rendu: (d) => montant(d.brut, 0) },
              { cle: 'net', titre: t('col.net'), droite: true, rendu: (d) => (d.net == null ? '—' : montant(d.net, 0)) },
              { cle: 'detachement', titre: t('col.detachement'), rendu: (d) => (d.dateDetachement ? date(d.dateDetachement) : '—') },
              { cle: 'paiement', titre: t('col.paiement'), rendu: (d) => (d.datePaiement ? date(d.datePaiement) : '—') },
            ]}
            lignes={dividendes}
            cleLigne={(d, i) => `${d.exercice}-${i}`}
          />
        </Section>
      )}
    </>
  )
}
