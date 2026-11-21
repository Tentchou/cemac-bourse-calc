import { CircleCheck, FileSpreadsheet, FileText, Upload } from 'lucide-react'
import { useRef, useState } from 'react'
import { envoyerEnMasse, listerEmetteurs } from '../../api/admin'
import { listerTitres } from '../../api/titres'
import { convertir, erreursServeurVersFichier, preparerReferences } from '../../admin/import/conversion'
import { lireFichier } from '../../admin/import/lectureFichier'
import { telechargerModeleCsv, telechargerModeleXlsx } from '../../admin/import/modeleFichier'
import { MODELES } from '../../admin/import/modeles'
import { normaliser } from '../../admin/import/valeurs'
import { usePreferences } from '../../contexte/Preferences'
import Bouton from '../ui/Bouton'
import Panneau from '../ui/Panneau'

const LIGNES_APERCU = 5

function Etape({ numero, faite, titre, aide, children }) {
  return (
    <div className="grid grid-cols-[28px_minmax(0,1fr)] gap-3">
      <span
        aria-hidden="true"
        className={`grid size-7 place-items-center rounded-full border text-xs font-semibold ${
          faite ? 'border-action bg-action text-sur-action' : 'border-bordure bg-voile text-texte-2'
        }`}
      >
        {numero}
      </span>
      <div className="min-w-0">
        <h3 className="text-[0.9375rem] font-semibold">{titre}</h3>
        {aide && <p className="mt-0.5 mb-3 text-sm text-texte-2">{aide}</p>}
        {children}
      </div>
    </div>
  )
}

/**
 * Import d'un fichier .xlsx ou .csv pour une rubrique, dans le panneau latéral. Règle validée : s'il reste
 * la moindre ligne à corriger, rien n'est envoyé ; on corrige le fichier et on le recharge. L'API applique
 * ensuite le même tout-ou-rien.
 */
export default function ImportFichier({ rubrique, onFermer, onImporte, onSessionExpiree }) {
  const p = usePreferences()
  const { t } = p
  const modele = MODELES[rubrique.modele]
  const [nomFichier, setNomFichier] = useState(null)
  const [tableau, setTableau] = useState(null)
  const [analyse, setAnalyse] = useState(null)
  const [etat, setEtat] = useState('attente') // attente · lecture · pret · envoi · termine
  const [message, setMessage] = useState(null)
  const champFichier = useRef(null)

  function erreurApi(e) {
    if (e.statut === 401) onSessionExpiree()
    setMessage({ type: 'erreur', texte: e.message })
  }

  async function analyser(fichier) {
    if (!fichier) return
    setNomFichier(fichier.name)
    setAnalyse(null)
    setMessage(null)
    setEtat('lecture')
    try {
      // Les références sont relues à chaque fichier : un émetteur importé juste avant est reconnu.
      const [lu, titres, emetteurs] = await Promise.all([lireFichier(fichier), listerTitres(), listerEmetteurs()])
      if (lu.erreur) {
        setMessage({ type: 'erreur', texte: lu.erreur })
        setEtat('attente')
        return
      }
      setTableau(lu.tableau)
      setAnalyse(convertir(modele, lu.tableau, preparerReferences(titres, emetteurs)))
      setEtat('pret')
    } catch (e) {
      erreurApi(e)
      setEtat('attente')
    } finally {
      if (champFichier.current) champFichier.current.value = ''
    }
  }

  async function enregistrer() {
    setEtat('envoi')
    setMessage(null)
    try {
      const reponse = await envoyerEnMasse(modele.route, analyse.commandes)
      if (reponse.bilan.erreurs.length) {
        setAnalyse({ ...analyse, erreurs: erreursServeurVersFichier(reponse.bilan.erreurs, analyse.lignesFichier), serveur: true })
        setEtat('pret')
        return
      }
      setMessage({ type: 'succes', texte: t('admin.import.termine', { n: reponse.bilan.enregistrees }) })
      setEtat('termine')
      onImporte()
    } catch (e) {
      erreurApi(e)
      setEtat('pret')
    }
  }

  const erreurs = analyse?.erreurs ?? []
  const pretes = analyse?.commandes.length ?? 0
  // Erreurs du navigateur : lignes écartées des commandes. Erreurs du serveur : parmi les commandes envoyées.
  const lues = analyse?.serveur ? pretes : pretes + erreurs.length
  const peutEnvoyer = analyse && !analyse.erreurFichier && erreurs.length === 0 && pretes > 0 && etat !== 'termine'

  const colonnesApercu =
    tableau && analyse && !analyse.erreurFichier
      ? modele.colonnes
          .map((c) => ({ entete: c.entete, position: tableau[0].findIndex((e) => normaliser(e) === normaliser(c.entete)) }))
          .filter((c) => c.position >= 0)
      : []

  const cellule = (valeur) => {
    if (valeur instanceof Date) return p.date(valeur.toISOString().slice(0, 10))
    if (typeof valeur === 'number') return valeur.toLocaleString(p.langue === 'en' ? 'en-GB' : 'fr-FR')
    return valeur ?? ''
  }

  return (
    <Panneau
      ouvert
      onFermer={onFermer}
      occupe={etat === 'envoi'}
      titre={t(`admin.import.titre.${rubrique.cle}`)}
      aide={t('admin.import.aide')}
      pied={
        etat === 'termine' ? (
          <>
            <Bouton taille="petite" onClick={() => champFichier.current?.click()}>
              {t('admin.import.autre')}
            </Bouton>
            <Bouton taille="petite" variante="action" onClick={onFermer}>
              {t('admin.fermer')}
            </Bouton>
          </>
        ) : (
          <>
            <Bouton taille="petite" onClick={onFermer} disabled={etat === 'envoi'}>
              {t('admin.annuler')}
            </Bouton>
            <Bouton taille="petite" variante="action" onClick={enregistrer} disabled={!peutEnvoyer} chargement={etat === 'envoi'}>
              {etat === 'envoi'
                ? t('admin.enregistrement')
                : pretes === 1
                  ? t('admin.import.enregistrer.1')
                  : t('admin.import.enregistrer', { n: pretes })}
            </Bouton>
          </>
        )
      }
    >
      <div className="flex flex-col gap-7">
        <Etape numero={1} faite titre={t('admin.import.etape1')} aide={t('admin.import.etape1.aide')}>
          <div className="flex flex-wrap gap-2">
            <Bouton taille="petite" icone={FileSpreadsheet} onClick={() => telechargerModeleXlsx(modele)}>
              {t('admin.import.modeleExcel')}
            </Bouton>
            <Bouton taille="petite" icone={FileText} onClick={() => telechargerModeleCsv(modele)}>
              {t('admin.import.modeleCsv')}
            </Bouton>
          </div>
        </Etape>

        <Etape numero={2} faite={Boolean(nomFichier)} titre={t('admin.import.etape2')} aide={t('admin.import.etape2.aide')}>
          <label className="flex cursor-pointer flex-col items-center gap-1 rounded-2xl border-[1.5px] border-dashed border-bordure-champ px-4 py-6 text-center text-sm text-texte-2 transition-colors hover:border-action hover:bg-voile focus-within:border-action">
            <input
              ref={champFichier}
              type="file"
              className="sr-only"
              accept=".xlsx,.csv,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,text/csv"
              onChange={(e) => analyser(e.target.files?.[0])}
              disabled={etat === 'lecture' || etat === 'envoi'}
            />
            <Upload aria-hidden="true" className="mb-1 size-5 text-action" strokeWidth={1.75} />
            <span className="font-semibold break-all text-texte">{nomFichier ?? t('admin.import.choisir')}</span>
            {nomFichier && <span>{t('admin.import.changer')}</span>}
          </label>
          {etat === 'lecture' && (
            <p role="status" className="mt-3 text-sm text-texte-2">
              {t('admin.import.lecture')}
            </p>
          )}
        </Etape>

        {message && (
          <p
            role={message.type === 'erreur' ? 'alert' : 'status'}
            className={`flex items-start gap-2 rounded-2xl border px-4 py-3 text-sm ${
              message.type === 'erreur' ? 'border-erreur/30 bg-erreur/8 text-erreur' : 'border-action/30 bg-action/8 text-texte'
            }`}
          >
            {message.type === 'succes' && <CircleCheck aria-hidden="true" className="mt-0.5 size-4 shrink-0 text-action" strokeWidth={2} />}
            {message.texte}
          </p>
        )}

        {analyse && (
          <Etape numero={3} faite={peutEnvoyer || etat === 'termine'} titre={t('admin.import.etape3')} aide={t('admin.import.etape3.aide')}>
            {analyse.erreurFichier ? (
              <p role="alert" className="rounded-2xl border border-erreur/30 bg-erreur/8 px-4 py-3 text-sm text-erreur">
                {analyse.erreurFichier}
              </p>
            ) : (
              <div className="flex flex-col gap-4">
                <dl className="chiffres grid grid-cols-3 overflow-hidden rounded-2xl border border-bordure text-center">
                  {[
                    ['admin.import.lues', lues, ''],
                    ['admin.import.pretes', lues - erreurs.length, ''],
                    ['admin.import.aCorriger', erreurs.length, erreurs.length ? 'text-erreur' : ''],
                  ].map(([cle, n, ton]) => (
                    <div key={cle} className="border-bordure px-2 py-3 not-first:border-l">
                      <dt className="text-xs text-texte-2">{t(cle)}</dt>
                      <dd className={`mt-1 text-lg font-semibold ${ton}`}>{n}</dd>
                    </div>
                  ))}
                </dl>

                {analyse.colonnesIgnorees.length > 0 && (
                  <p className="text-sm text-texte-2">{t('admin.import.ignorees', { colonnes: analyse.colonnesIgnorees.join(', ') })}</p>
                )}

                {erreurs.length > 0 && (
                  <div role="alert" className="rounded-2xl border border-erreur/30 bg-erreur/8 px-4 py-3 text-sm">
                    <p className="font-semibold text-erreur">
                      {analyse.serveur && `${t('admin.import.refus')} `}
                      {t('admin.import.corriger')}
                    </p>
                    <ul className="mt-2 flex flex-col gap-1 text-texte">
                      {erreurs.map((e) => (
                        <li key={e.ligne}>
                          <span className="font-semibold">{t('admin.import.ligne', { n: e.ligne })}</span> · {e.messages.join(' · ')}
                        </li>
                      ))}
                    </ul>
                  </div>
                )}

                {pretes > 0 && etat !== 'termine' && (
                  <div>
                    <p className="mb-2 text-sm text-texte-2">{t('admin.import.apercu', { n: Math.min(LIGNES_APERCU, pretes) })}</p>
                    <div className="overflow-x-auto overscroll-x-contain rounded-2xl border border-bordure" role="region" aria-label={t('admin.import.apercu', { n: Math.min(LIGNES_APERCU, pretes) })} tabIndex={0}>
                      <table className="tableau-raye chiffres text-xs">
                        <thead>
                          <tr>
                            <th className="h-9 px-3 text-left font-semibold text-texte-2">#</th>
                            {colonnesApercu.map((c) => (
                              <th key={c.entete} className="h-9 px-3 text-left font-semibold whitespace-nowrap text-texte-2">
                                {c.entete}
                              </th>
                            ))}
                          </tr>
                        </thead>
                        <tbody>
                          {analyse.lignesFichier.slice(0, LIGNES_APERCU).map((numero) => (
                            <tr key={numero}>
                              <td className="h-9 px-3 text-texte-2">{numero}</td>
                              {colonnesApercu.map((c) => (
                                <td key={c.entete} className="h-9 px-3 whitespace-nowrap">
                                  {cellule(tableau[numero - 1][c.position])}
                                </td>
                              ))}
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  </div>
                )}
              </div>
            )}
          </Etape>
        )}
      </div>
    </Panneau>
  )
}
