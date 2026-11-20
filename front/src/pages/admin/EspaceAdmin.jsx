import './admin.css'
import Administration from './Administration'

// L'administration garde son ancienne présentation tant que sa charte n'est pas validée
// (la partie publique d'abord). Elle est chargée à part : son CSS ne touche pas le site public.
export default function EspaceAdmin() {
  return (
    <main className="page">
      <header>
        <h1>CEMAC Bourse Calc</h1>
        <p className="sous-titre">Administration : saisie des titres et des données de marché.</p>
      </header>
      <p className="avertissement" role="note">
        Outil éducatif, pas un conseil en investissement.
      </p>
      <Administration />
    </main>
  )
}
