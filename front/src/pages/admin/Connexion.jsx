import { KeyRound, LogIn, Mail } from 'lucide-react'
import { useState } from 'react'
import { ouvrirSession } from '../../api/admin'
import Reglages from '../../components/disposition/Reglages'
import { Logo } from '../../components/icones/Icones'
import Bouton from '../../components/ui/Bouton'
import Carte from '../../components/ui/Carte'
import Champ from '../../components/ui/Champ'
import { usePreferences } from '../../contexte/Preferences'

/** Connexion à l'administration. Le mot de passe ne quitte ce composant que vers l'API, et n'est jamais gardé. */
export default function Connexion({ onConnecte }) {
  const { t } = usePreferences()
  const [email, setEmail] = useState('')
  const [motDePasse, setMotDePasse] = useState('')
  const [erreur, setErreur] = useState(null)
  const [enCours, setEnCours] = useState(false)

  async function soumettre(evenement) {
    evenement.preventDefault()
    if (!email.trim() || !motDePasse) {
      setErreur(t('admin.connexion.manque'))
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
    <div className="flex min-h-dvh flex-col bg-fond px-4 py-6 text-texte">
      <div className="flex items-center justify-between gap-3">
        <span className="flex items-center gap-2.5 font-titre font-bold">
          <Logo className="size-8" />
          <span className="max-[380px]:sr-only">CEMAC Bourse Calc</span>
        </span>
        <Reglages />
      </div>
      <main className="grid flex-1 place-items-center py-10">
        <Carte as="form" onSubmit={soumettre} noValidate className="apparition flex w-full max-w-md flex-col gap-5 sm:p-8">
          <div>
            <h1 className="text-2xl font-bold">{t('admin.connexion.titre')}</h1>
            <p className="mt-1 text-sm text-texte-2">{t('admin.connexion.aide')}</p>
          </div>
          <Champ
            libelle={t('admin.connexion.email')}
            icone={Mail}
            type="email"
            autoComplete="username"
            value={email}
            maxLength={254}
            onChange={(e) => {
              setEmail(e.target.value)
              setErreur(null)
            }}
          />
          <Champ
            libelle={t('admin.connexion.motDePasse')}
            icone={KeyRound}
            type="password"
            autoComplete="current-password"
            value={motDePasse}
            maxLength={200}
            onChange={(e) => {
              setMotDePasse(e.target.value)
              setErreur(null)
            }}
          />
          {erreur && (
            <p role="alert" className="rounded-2xl border border-erreur/30 bg-erreur/8 px-4 py-3 text-sm text-erreur">
              {erreur}
            </p>
          )}
          <Bouton type="submit" variante="action" icone={LogIn} chargement={enCours} disabled={enCours}>
            {enCours ? t('admin.connexion.enCours') : t('admin.connexion.valider')}
          </Bouton>
        </Carte>
      </main>
    </div>
  )
}
