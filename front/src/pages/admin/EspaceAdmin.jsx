import { PreferencesProvider } from '../../contexte/Preferences'
import { enAdmin, frAdmin } from '../../i18n/textesAdmin'
import { RouteurProvider } from '../../routeur/Routeur'
import Administration from './Administration'

// L'administration est chargée à part (sous /admin) : ses textes et ses écrans n'alourdissent pas le site public.
// Elle reprend la charte, la langue et le thème du site.
const TEXTES = { fr: frAdmin, en: enAdmin }

export default function EspaceAdmin() {
  return (
    <PreferencesProvider textes={TEXTES}>
      <RouteurProvider>
        <Administration />
      </RouteurProvider>
    </PreferencesProvider>
  )
}
