import { useState } from 'react'
import { ouvrirSession } from '../../api/admin'

/** Connexion à l'administration. Le mot de passe ne quitte ce composant que vers l'API, et n'est jamais gardé. */
export default function Connexion({ onConnecte }) {
  const [email, setEmail] = useState('')
  const [motDePasse, setMotDePasse] = useState('')
  const [erreur, setErreur] = useState(null)
  const [enCours, setEnCours] = useState(false)

  async function soumettre(evenement) {
    evenement.preventDefault()
    if (!email.trim() || !motDePasse) {
      setErreur('Saisissez votre e-mail et votre mot de passe.')
      return
    }
    setEnCours(true)
    setErreur(null)
    try {
      await ouvrirSession(email.trim(), motDePasse)
      setMotDePasse('')
      onConnecte()
    } catch (e) {
      setErreur(e.message)
    } finally {
      setEnCours(false)
    }
  }

  return (
    <form className="carte formulaire" onSubmit={soumettre} noValidate>
      <h2>Connexion à l'administration</h2>
      <label className="champ">
        E-mail
        <input type="email" autoComplete="username" value={email} onChange={(e) => { setEmail(e.target.value); setErreur(null) }} />
      </label>
      <label className="champ">
        Mot de passe
        <input type="password" autoComplete="current-password" value={motDePasse} onChange={(e) => { setMotDePasse(e.target.value); setErreur(null) }} />
      </label>
      {erreur && <p className="erreur" role="alert">{erreur}</p>}
      <button type="submit" className="bouton bouton-principal" disabled={enCours}>
        {enCours ? 'Connexion…' : 'Se connecter'}
      </button>
    </form>
  )
}
