import { CircleAlert, CircleCheck, FileText, TriangleAlert, Upload } from 'lucide-react'
import { useMemo, useRef, useState } from 'react'
import { analyserBulletin, envoyerEnMasse } from '../../api/admin'
import { commandesCours, FILTRES_BULLETIN, filtrerBulletin, resumerBulletin } from '../../admin/bulletin'
import { libelleChoix } from '../../admin/rubriques'
import { usePreferences } from '../../contexte/Preferences'
import Bouton from '../ui/Bouton'
import Etiquette from '../ui/Etiquette'
import Panneau from '../ui/Panneau'
import Tableau from '../ui/Tableau'
import ControleCoupon from './ControleCoupon'

function Alerte({ ton, icone: Icone, titre, children }) {
  const couleurs = ton === 'erreur' ? 'border-erreur/30 bg-erreur/8' : ton === 'orange' ? 'border-orange/45 bg-orange/10' : 'border-action/30 bg-action/8'
  const icone = ton === 'erreur' ? 'text-erreur' : ton === 'orange' ? 'text-orange-texte' : 'text-action'
  return (
    <div role={ton === 'erreur' ? 'alert' : 'status'} className={`flex items-start gap-2.5 rounded-2xl border px-4 py-3 text-sm ${couleurs}`}>
      <Icone aria-hidden="true" className={`mt-0.5 size-4 shrink-0 ${icone}`} strokeWidth={2} />
      <div className="min-w-0">
        <p className="font-semibold">{titre}</p>
        {children && <div className="mt-0.5 text-texte">{children}</div>}
      </div>
    </div>
  )
}

/**
 * Import du bulletin officiel de la cote (PDF) : l'API lit le PDF, rapproche chaque ligne de la base et
 * contrôle le coupon couru publié ; l'aperçu s'affiche ici, rien n'est enregistré avant « Enregistrer ».
 * L'enregistrement passe par l'import en masse des cours (tout ou rien), comme un fichier CSV.
 * Règles validées : titre inconnu → rien n'est importable ; écart de coupon couru → signalé, pas bloquant.
 */
export default function ImportBulletin({ onFermer, onImporte, onSessionExpiree }) {
  const p = usePreferences()
  const { t } = p
  const champ = useRef(null)
  const [fichier, setFichier] = useState(null)
  const [analyse, setAnalyse] = useState(null)
  const [etat, setEtat] = useState('attente') // attente · lecture · pret · envoi · termine
  const [erreur, setErreur] = useState(null)
  const [refus, setRefus] = useState([])
  const [filtre, setFiltre] = useState('tout')

  async function lire(nouveau) {
    if (!nouveau) return
    setFichier(nouveau.name)
    setAnalyse(null)
    setErreur(null)
    setRefus([])
    setFiltre('tout')
    setEtat('lecture')
    try {
      setAnalyse(await analyserBulletin(nouveau))
      setEtat('pret')
    } catch (e) {
      if (e.statut === 401) onSessionExpiree()
      setErreur(e.message)
      setEtat('attente')
    } finally {
      if (champ.current) champ.current.value = ''
    }
  }

  const commandes = useMemo(() => commandesCours(analyse), [analyse])

  async function enregistrer() {
    if (!commandes) return
    setEtat('envoi')
    setErreur(null)
    try {
      const reponse = await envoyerEnMasse('cours', commandes)
      if (reponse.bilan.erreurs.length) {
        // Les numéros de ligne de l'API suivent l'ordre d'envoi : ils désignent les lignes de l'aperçu.
        setRefus(reponse.bilan.erreurs.map((e) => ({ isin: analyse.lignes[e.ligne - 1]?.isin ?? `#${e.ligne}`, messages: e.messages })))
        setEtat('pret')
        return
      }
      setEtat('termine')
      onImporte(t('admin.bulletin.termine', { n: reponse.bilan.enregistrees, date: p.date(analyse.dateSeance) }))
    } catch (e) {
      if (e.statut === 401) onSessionExpiree()
      setErreur(e.message)
      setEtat('pret')
    }
  }

  const resume = resumerBulletin(analyse)
  const lignes = analyse ? filtrerBulletin(analyse.lignes, filtre) : []

  const colonnes = [
    {
      cle: 'nom',
      titre: t('admin.col.titre'),
      rendu: (l) => (
        <span className="flex flex-col gap-0.5 py-2">
          <span className="line-clamp-2 leading-snug font-semibold max-sm:text-[0.8125rem]">{l.nom}</span>
          <span className="text-xs text-texte-2">{l.isin}</span>
        </span>
      ),
    },
    {
      cle: 'type',
      titre: t('admin.col.type'),
      rendu: (l) => <Etiquette ton={l.typeTitre === 'Action' ? 'orange' : 'action'}>{libelleChoix(t, l.typeTitre)}</Etiquette>,
    },
    {
      cle: 'cloture',
      titre: t('admin.col.cloture'),
      droite: true,
      rendu: (l) => <span className="font-semibold">{l.typeTitre === 'Obligation' ? p.pourcent(l.coursCloture) : p.montant(l.coursCloture, 0)}</span>,
    },
    { cle: 'volume', titre: t('admin.col.volume'), droite: true, rendu: (l) => p.nombre(l.volume, 0) },
    { cle: 'cc', titre: t('admin.col.ccPublie'), droite: true, rendu: (l) => (l.couponCouruPublie == null ? '—' : p.montant(l.couponCouruPublie, 2)) },
    {
      cle: 'controle',
      titre: t('admin.col.controle'),
      rendu: (l) =>
        l.probleme ? (
          <Etiquette ton="erreur" point title={l.probleme}>
            {l.probleme}
          </Etiquette>
        ) : l.typeTitre === 'Obligation' ? (
          <ControleCoupon controle={l.controle} />
        ) : (
          '—'
        ),
    },
  ]

  const comptes = { tout: resume.lus, ecarts: resume.ecarts, problemes: resume.problemes, transactions: resume.transactions }
  const inconnus = analyse?.lignes.filter((l) => l.probleme) ?? []

  return (
    <Panneau
      ouvert
      large
      onFermer={onFermer}
      occupe={etat === 'envoi'}
      titre={t('admin.bulletin.titre')}
      aide={t('admin.bulletin.aide')}
      pied={
        <>
          <Bouton taille="petite" onClick={onFermer} disabled={etat === 'envoi'}>
            {etat === 'termine' ? t('admin.fermer') : t('admin.annuler')}
          </Bouton>
          {etat !== 'termine' && (
            <Bouton taille="petite" variante="action" onClick={enregistrer} disabled={!commandes || etat !== 'pret'} chargement={etat === 'envoi'}>
              {etat === 'envoi' ? t('admin.enregistrement') : t('admin.bulletin.enregistrer', { n: commandes?.length ?? resume.lus })}
            </Bouton>
          )}
        </>
      }
    >
      <div className="flex flex-col gap-5">
        <input
          ref={champ}
          type="file"
          className="sr-only"
          tabIndex={-1}
          accept="application/pdf,.pdf"
          onChange={(e) => lire(e.target.files?.[0])}
        />

        {!analyse && (
          <button
            type="button"
            onClick={() => champ.current?.click()}
            disabled={etat === 'lecture'}
            className="flex flex-col items-center gap-1 rounded-2xl border-[1.5px] border-dashed border-bordure-champ px-4 py-8 text-center text-sm text-texte-2 transition-colors hover:border-action hover:bg-voile disabled:opacity-60"
          >
            <Upload aria-hidden="true" className="mb-1 size-6 text-action" strokeWidth={1.75} />
            <span className="font-semibold text-texte">{fichier ?? t('admin.bulletin.choisir')}</span>
            <span>{etat === 'lecture' ? t('admin.bulletin.lecture') : t('admin.bulletin.choisir.aide')}</span>
          </button>
        )}

        {erreur && (
          <Alerte ton="erreur" icone={CircleAlert} titre={erreur} />
        )}

        {analyse && (
          <>
            <div className="flex flex-wrap items-center gap-3 rounded-2xl border border-bordure bg-voile p-3.5">
              <span aria-hidden="true" className="grid size-10 shrink-0 place-items-center rounded-xl border border-bordure bg-surface text-action">
                <FileText className="size-5" strokeWidth={1.75} />
              </span>
              <div className="min-w-0 flex-1">
                <p className="font-semibold">{t('admin.bulletin.entete', { numero: analyse.numero, date: p.date(analyse.dateSeance) })}</p>
                <p className="truncate text-sm text-texte-2">{t('admin.bulletin.fichier', { fichier, date: p.dateLongue(analyse.dateSeance) })}</p>
              </div>
              {etat !== 'termine' && (
                <Bouton taille="petite" onClick={() => champ.current?.click()} disabled={etat === 'envoi'}>
                  {t('admin.bulletin.changer')}
                </Bouton>
              )}
            </div>

            <dl className="chiffres grid grid-cols-2 gap-2.5 sm:grid-cols-4">
              {[
                ['admin.bulletin.lus', resume.lus, 'text-texte'],
                ['admin.bulletin.identiques', `${resume.identiques} / ${resume.controles}`, 'text-action'],
                ['admin.bulletin.ecarts', resume.ecarts, resume.ecarts ? 'text-orange-texte' : 'text-texte'],
                ['admin.bulletin.problemes', resume.problemes, resume.problemes ? 'text-erreur' : 'text-texte'],
              ].map(([cle, valeur, ton]) => (
                <div key={cle} className="rounded-2xl border border-bordure px-3.5 py-3">
                  <dt className="text-xs text-texte-2">{t(cle)}</dt>
                  <dd className={`mt-1 font-titre text-2xl font-bold ${ton}`}>{valeur}</dd>
                </div>
              ))}
            </dl>

            {etat === 'termine' && (
              <Alerte ton="action" icone={CircleCheck} titre={t('admin.bulletin.termine', { n: commandes.length, date: p.date(analyse.dateSeance) })} />
            )}
            {analyse.coursDejaEnregistres > 0 && etat !== 'termine' && (
              <Alerte ton="erreur" icone={CircleAlert} titre={t('admin.bulletin.deja', { date: p.date(analyse.dateSeance), n: analyse.coursDejaEnregistres })} />
            )}
            {analyse.anomalies.length > 0 && (
              <Alerte ton="erreur" icone={CircleAlert} titre={t('admin.bulletin.anomalies')}>
                <ul className="list-inside list-disc">
                  {analyse.anomalies.map((a) => (
                    <li key={a}>{a}</li>
                  ))}
                </ul>
              </Alerte>
            )}
            {inconnus.length > 0 && (
              <Alerte ton="erreur" icone={CircleAlert} titre={`${t('admin.bulletin.inconnus', { n: inconnus.length })} : ${inconnus.map((l) => `${l.nom} (${l.isin})`).join(', ')}`}>
                {t('admin.bulletin.inconnus.aide')}
              </Alerte>
            )}
            {refus.length > 0 && (
              <Alerte ton="erreur" icone={CircleAlert} titre={t('admin.bulletin.refus')}>
                <ul className="flex flex-col gap-0.5">
                  {refus.map((r) => (
                    <li key={r.isin}>
                      <span className="font-semibold">{r.isin}</span> · {r.messages.join(' · ')}
                    </li>
                  ))}
                </ul>
              </Alerte>
            )}
            {resume.ecarts > 0 && (
              <Alerte ton="orange" icone={TriangleAlert} titre={t('admin.bulletin.ecarts.titre', { n: resume.ecarts })}>
                {t('admin.bulletin.ecarts.aide')}
              </Alerte>
            )}
            {analyse.absents.length > 0 && (
              <p className="text-sm text-texte-2">
                {t('admin.bulletin.absents', { n: analyse.absents.length, liste: analyse.absents.map((a) => a.nom).join(', ') })}
              </p>
            )}

            <div className="flex flex-wrap gap-1.5" role="group" aria-label={t('admin.bulletin.filtres')}>
              {FILTRES_BULLETIN.map((f) => (
                <button
                  key={f}
                  type="button"
                  aria-pressed={filtre === f}
                  onClick={() => setFiltre(f)}
                  className={`h-9 rounded-full border px-3.5 text-sm transition-colors ${
                    filtre === f ? 'border-action bg-action text-sur-action' : 'border-bordure bg-surface text-texte-2 hover:text-texte'
                  }`}
                >
                  {t(`admin.bulletin.filtre.${f}`, { n: comptes[f] })}
                </button>
              ))}
            </div>

            {lignes.length ? (
              <Tableau legende={t('admin.bulletin.titre')} colonnes={colonnes} lignes={lignes} cleLigne={(l) => l.isin} />
            ) : (
              <p className="rounded-2xl border border-bordure px-4 py-6 text-center text-sm text-texte-2">{t('admin.bulletin.vide')}</p>
            )}
          </>
        )}
      </div>
    </Panneau>
  )
}
