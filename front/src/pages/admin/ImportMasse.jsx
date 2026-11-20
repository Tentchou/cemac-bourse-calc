import { useRef, useState } from 'react'
import { envoyerEnMasse, listerEmetteurs } from '../../api/admin'
import { listerTitres } from '../../api/titres'
import { convertir, erreursServeurVersFichier, preparerReferences } from '../../admin/import/conversion'
import { lireFichier } from '../../admin/import/lectureFichier'
import { telechargerModeleCsv, telechargerModeleXlsx } from '../../admin/import/modeleFichier'
import { MODELES } from '../../admin/import/modeles'
import { normaliser } from '../../admin/import/valeurs'

const LIGNES_APERCU = 5

function afficherCellule(cellule) {
  if (cellule instanceof Date) return cellule.toLocaleDateString('fr-FR', { timeZone: 'UTC' })
  if (typeof cellule === 'number') return cellule.toLocaleString('fr-FR')
  return cellule ?? ''
}

/**
 * Import d'un fichier .xlsx ou .csv. Règle validée : s'il reste la moindre ligne à corriger, rien n'est
 * envoyé ; on corrige le fichier et on le recharge. Le serveur applique ensuite le même tout-ou-rien.
 */
export default function ImportMasse({ onSessionExpiree }) {
  const [type, setType] = useState('cours')
  const [nomFichier, setNomFichier] = useState(null)
  const [tableau, setTableau] = useState(null)
  const [analyse, setAnalyse] = useState(null)
  const [etat, setEtat] = useState('attente') // attente · lecture · pret · envoi · termine
  const [message, setMessage] = useState(null)
  const champFichier = useRef(null)

  const modele = MODELES[type]

  function reinitialiser() {
    setNomFichier(null)
    setTableau(null)
    setAnalyse(null)
    setMessage(null)
    setEtat('attente')
    if (champFichier.current) champFichier.current.value = ''
  }

  function changerType(nouveau) {
    setType(nouveau)
    reinitialiser()
  }

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
      setMessage({ type: 'succes', texte: `${reponse.bilan.enregistrees} ligne(s) enregistrée(s) : ${modele.titre.toLowerCase()} à jour.` })
      setEtat('termine')
    } catch (e) {
      erreurApi(e)
      setEtat('pret')
    }
  }

  const erreurs = analyse?.erreurs ?? []
  const pretes = analyse?.commandes.length ?? 0
  // Erreurs du navigateur : lignes écartées des commandes. Erreurs du serveur : parmi les commandes envoyées.
  const lues = analyse?.serveur ? pretes : pretes + erreurs.length
  const peutEnvoyer = analyse && !analyse.erreurFichier && erreurs.length === 0 && pretes > 0

  // Aperçu : les cellules du fichier telles quelles, pour les colonnes connues.
  const colonnesApercu = tableau && analyse && !analyse.erreurFichier
    ? modele.colonnes
      .map((c) => ({ entete: c.entete, position: tableau[0].findIndex((e) => normaliser(e) === normaliser(c.entete)) }))
      .filter((c) => c.position >= 0)
    : []

  return (
    <section className="carte">
      <h2>Import en masse</h2>

      <p className="etape">1. Que voulez-vous importer ?</p>
      <div className="pastilles" role="group" aria-label="Type de données">
        {Object.entries(MODELES).map(([cle, m]) => (
          <button key={cle} type="button" className="pastille" aria-pressed={cle === type} onClick={() => changerType(cle)}>
            {m.titre}
          </button>
        ))}
      </div>
      <div className="actions">
        <button type="button" className="bouton" onClick={() => telechargerModeleXlsx(modele)}>Modèle Excel</button>
        <button type="button" className="bouton" onClick={() => telechargerModeleCsv(modele)}>Modèle CSV</button>
      </div>

      <p className="etape">2. Votre fichier (.xlsx ou .csv, 2 Mo et 500 lignes au plus)</p>
      <label className="zone-fichier">
        <input
          ref={champFichier}
          type="file"
          accept=".xlsx,.csv,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,text/csv"
          onChange={(e) => analyser(e.target.files?.[0])}
        />
        <span>{nomFichier ?? 'Choisir un fichier'}</span>
        {nomFichier && <span className="doux">Changer de fichier</span>}
      </label>

      {etat === 'lecture' && <p role="status">Lecture et vérification du fichier…</p>}
      {message && <p className={message.type} role={message.type === 'erreur' ? 'alert' : 'status'}>{message.texte}</p>}

      {analyse && (
        <>
          <p className="etape">3. Vérification, avant tout envoi</p>

          {analyse.erreurFichier ? (
            <p className="bloc-erreurs" role="alert">{analyse.erreurFichier}</p>
          ) : (
            <>
              <div className="metriques">
                <div className="metrique"><span>Lignes lues</span><strong>{lues}</strong></div>
                <div className="metrique"><span>Prêtes</span><strong className="succes">{lues - erreurs.length}</strong></div>
                <div className="metrique"><span>À corriger</span><strong className={erreurs.length ? 'erreur' : ''}>{erreurs.length}</strong></div>
              </div>

              {analyse.colonnesIgnorees.length > 0 && (
                <p className="doux">Colonnes ignorées : {analyse.colonnesIgnorees.join(', ')}.</p>
              )}

              {erreurs.length > 0 && (
                <div className="bloc-erreurs" role="alert">
                  <p><strong>
                    {analyse.serveur ? 'Le serveur a refusé ces lignes : rien n’a été enregistré. ' : ''}
                    Corrigez ces lignes dans le fichier, puis rechargez-le.
                  </strong></p>
                  <ul>
                    {erreurs.map((e) => (
                      <li key={e.ligne}>Ligne {e.ligne} · {e.messages.join(' · ')}</li>
                    ))}
                  </ul>
                </div>
              )}

              {pretes > 0 && (
                <>
                  <p className="doux">Aperçu des {Math.min(LIGNES_APERCU, pretes)} premières lignes prêtes</p>
                  <div className="defilement">
                    <table className="titres apercu">
                      <thead>
                        <tr><th>Ligne</th>{colonnesApercu.map((c) => <th key={c.entete}>{c.entete}</th>)}</tr>
                      </thead>
                      <tbody>
                        {analyse.lignesFichier.slice(0, LIGNES_APERCU).map((numero) => (
                          <tr key={numero}>
                            <td>{numero}</td>
                            {colonnesApercu.map((c) => <td key={c.entete}>{afficherCellule(tableau[numero - 1][c.position])}</td>)}
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                </>
              )}

              {peutEnvoyer && etat !== 'termine' && (
                <div className="actions">
                  <button type="button" className="bouton bouton-principal" onClick={enregistrer} disabled={etat === 'envoi'}>
                    {etat === 'envoi' ? 'Enregistrement…' : `Enregistrer les ${pretes} lignes`}
                  </button>
                  <span className="doux">Tout ou rien : si le serveur refuse une ligne, rien n'est enregistré.</span>
                </div>
              )}
            </>
          )}
        </>
      )}

      {etat === 'termine' && (
        <div className="actions">
          <button type="button" className="bouton" onClick={reinitialiser}>Importer un autre fichier</button>
        </div>
      )}
    </section>
  )
}
