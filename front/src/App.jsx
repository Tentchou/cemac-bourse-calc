import { lazy, Suspense } from 'react'
import Disposition from './components/disposition/Disposition'
import { PreferencesProvider } from './contexte/Preferences'
import Accueil from './pages/public/Accueil'
import FicheTitre from './pages/public/FicheTitre'
import Introuvable from './pages/public/Introuvable'
import Methodes from './pages/public/Methodes'
import { RouteurProvider, useRoute } from './routeur/Routeur'

// L'administration est un module à part, chargé seulement sous /admin (protégée par l'API).
const EspaceAdmin = lazy(() => import('./pages/admin/EspaceAdmin'))
// La vitrine des composants n'existe qu'en développement : absente du build de production.
const Vitrine = import.meta.env.DEV ? lazy(() => import('./pages/vitrine/Vitrine')) : null

function PagePublique() {
  const { route } = useRoute()
  switch (route.page) {
    case 'accueil':
      return <Accueil />
    case 'fiche':
      return <FicheTitre key={route.id} id={route.id} />
    case 'methodes':
      return <Methodes />
    case 'vitrine':
      return Vitrine ? <Vitrine /> : <Introuvable />
    default:
      return <Introuvable />
  }
}

export default function App() {
  if (window.location.pathname.startsWith('/admin')) {
    return (
      <Suspense fallback={null}>
        <EspaceAdmin />
      </Suspense>
    )
  }
  return (
    <PreferencesProvider>
      <RouteurProvider>
        <Disposition>
          <Suspense fallback={null}>
            <PagePublique />
          </Suspense>
        </Disposition>
      </RouteurProvider>
    </PreferencesProvider>
  )
}
