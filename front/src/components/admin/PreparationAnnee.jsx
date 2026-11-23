import { CalendarCheck, CircleCheck, Info } from 'lucide-react'
import { useEffect, useMemo, useState } from 'react'
import { envoyerEnMasse, preparerAnnee } from '../../api/admin'
import { usePreferences } from '../../contexte/Preferences'
import Bouton from '../ui/Bouton'
import ChampChoix from '../ui/ChampChoix'
import Etiquette from '../ui/Etiquette'
import Panneau from '../ui/Panneau'

/** Les années proposées : l'an dernier, l'année en cours et les six suivantes (les échéanciers vont loin). */
function anneesProposees(aujourdhui = new Date()) {
  const courante = aujourdhui.getFullYear()
  return Array.from({ length: 8 }, (_, i) => courante - 1 + i)
}

/** À partir d'octobre, on prépare d'ordinaire l'année suivante. */
const anneeParDefaut = (aujourdhui = new Date()) => aujourdhui.getFullYear() + (aujourdhui.getMonth() >= 9 ? 1 : 0)

/**
 * « Préparer l'année » : les jours fériés certains du Cameroun (dates fixes, Vendredi saint, Ascension),
 * calculés par l'API, croisés avec le calendrier déjà saisi. L'administrateur voit chaque jour et sa source,
 * puis valide : seuls les jours absents sont envoyés, par la saisie en masse (mêmes règles qu'à la main).
 * Les fêtes musulmanes, fixées à la lune, restent à saisir à la publication de l'avis de la BVMAC.
 */
export default function PreparationAnnee({ onFermer, onImporte, onSessionExpiree }) {
  const p = usePreferences()
  const { t } = p
  const [annee, setAnnee] = useState(anneeParDefaut)
  const [apercu, setApercu] = useState(null)
  const [erreur, setErreur] = useState(null)
  const [envoi, setEnvoi] = useState(false)

  useEffect(() => {
    const controleur = new AbortController()
    preparerAnnee(annee, controleur.signal)
      .then((donnees) => {
        setErreur(null)
        setApercu(donnees)
      })
      .catch((e) => {
        if (controleur.signal.aborted) return
        if (e.statut === 401) onSessionExpiree()
        setErreur(e.message)
      })
    return () => controleur.abort()
  }, [annee, onSessionExpiree])

  // L'aperçu affiché est celui de l'année choisie ; pendant le chargement d'une autre, rien n'est envoyable.
  const courant = apercu?.annee === annee ? apercu : null
  const aAjouter = useMemo(() => courant?.proposes.filter((j) => !j.dejaEnregistre) ?? [], [courant])

  async function ajouter() {
    if (!aAjouter.length || envoi) return
    setEnvoi(true)
    setErreur(null)
    try {
      const reponse = await envoyerEnMasse(
        'jours-non-ouvres',
        aAjouter.map((j) => ({ date: j.date, libelle: j.libelle })),
      )
      if (reponse.bilan.erreurs.length) {
        setErreur(reponse.bilan.erreurs.map((e) => `${p.date(aAjouter[e.ligne - 1]?.date)} : ${e.messages.join(' ')}`).join(' · '))
        return
      }
      onImporte(t('admin.preparation.termine', { n: reponse.bilan.enregistrees, annee }))
    } catch (e) {
      if (e.statut === 401) onSessionExpiree()
      setErreur(e.message)
    } finally {
      setEnvoi(false)
    }
  }

  return (
    <Panneau
      ouvert
      onFermer={onFermer}
      occupe={envoi}
      titre={t('admin.preparation.titre')}
      aide={t('admin.preparation.aide')}
      pied={
        <>
          <Bouton taille="petite" onClick={onFermer} disabled={envoi}>
            {t('admin.annuler')}
          </Bouton>
          <Bouton taille="petite" variante="action" icone={CalendarCheck} chargement={envoi} disabled={!aAjouter.length} onClick={ajouter}>
            {aAjouter.length ? t('admin.preparation.ajouter', { n: aAjouter.length }) : t('admin.preparation.rien')}
          </Bouton>
        </>
      }
    >
      <div className="flex flex-col gap-6">
        <ChampChoix
          libelle={t('admin.preparation.annee')}
          options={anneesProposees().map((a) => ({ valeur: String(a), libelle: String(a) }))}
          value={String(annee)}
          onChange={(e) => setAnnee(Number(e.target.value))}
          disabled={envoi}
        />

        {erreur && (
          <p role="alert" className="rounded-2xl border border-erreur/30 bg-erreur/8 px-4 py-3 text-sm text-erreur">
            {erreur}
          </p>
        )}

        {!courant && !erreur && (
          <p role="status" className="text-sm text-texte-2">
            {t('admin.ajout.chargement')}
          </p>
        )}

        {courant && (
          <>
            <section aria-labelledby="preparation-calcules">
              <h3 id="preparation-calcules" className="mb-3 text-xs font-semibold tracking-wider text-texte-2 uppercase">
                {t('admin.preparation.calcules', { annee })}
              </h3>
              <ul className="divide-y divide-bordure overflow-hidden rounded-2xl border border-bordure">
                {courant.proposes.map((j) => (
                  <li key={j.date} className="flex flex-wrap items-center justify-between gap-x-4 gap-y-1.5 px-4 py-3">
                    <span className="min-w-0">
                      <span className="block font-semibold first-letter:uppercase">{p.dateLongue(j.date)}</span>
                      <span className="block text-sm text-texte-2">
                        {j.libelle}
                        {j.dejaEnregistre && j.libelleEnregistre !== j.libelle && ` · ${t('admin.preparation.saisiComme', { libelle: j.libelleEnregistre })}`}
                      </span>
                    </span>
                    {j.dejaEnregistre ? (
                      <Etiquette icone={CircleCheck}>{t('admin.preparation.deja')}</Etiquette>
                    ) : (
                      <Etiquette ton="action" point>
                        {t('admin.preparation.aAjouter')}
                      </Etiquette>
                    )}
                  </li>
                ))}
              </ul>
            </section>

            {courant.autresEnregistres.length > 0 && (
              <section aria-labelledby="preparation-autres">
                <h3 id="preparation-autres" className="mb-3 text-xs font-semibold tracking-wider text-texte-2 uppercase">
                  {t('admin.preparation.autres')}
                </h3>
                <ul className="divide-y divide-bordure overflow-hidden rounded-2xl border border-bordure">
                  {courant.autresEnregistres.map((j) => (
                    <li key={j.date} className="px-4 py-3">
                      <span className="block font-semibold first-letter:uppercase">{p.dateLongue(j.date)}</span>
                      <span className="block text-sm text-texte-2">{j.libelle}</span>
                    </li>
                  ))}
                </ul>
              </section>
            )}

            <p className="flex items-start gap-3 rounded-2xl border border-orange/40 bg-orange/10 px-4 py-3 text-sm">
              <Info aria-hidden="true" className="mt-0.5 size-4 shrink-0 text-orange-texte" strokeWidth={2} />
              <span>{t('admin.preparation.lune')}</span>
            </p>
          </>
        )}
      </div>
    </Panneau>
  )
}
