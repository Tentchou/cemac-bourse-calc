import { Languages, Moon, Sun, SunMoon } from 'lucide-react'
import { usePreferences } from '../../contexte/Preferences'
import Bascule from '../ui/Bascule'

/**
 * Langue (FR / EN) et thème (soleil / lune).
 * Par défaut : compact, côte à côte, pour l'en-tête sur grand écran.
 * lignes : une ligne par réglage, son nom à gauche (menu mobile).
 */
export default function Reglages({ lignes = false, className = '' }) {
  const { langue, setLangue, theme, setTheme, t } = usePreferences()

  const reglages = [
    {
      cle: 'langue',
      icone: Languages,
      bascule: (
        <Bascule
          taille="petite"
          libelle={t('reglage.langue')}
          valeur={langue}
          onChange={setLangue}
          options={[
            { valeur: 'fr', libelle: 'FR', libelleCache: 'Français' },
            { valeur: 'en', libelle: 'EN', libelleCache: 'English' },
          ]}
        />
      ),
    },
    {
      cle: 'theme',
      icone: SunMoon,
      bascule: (
        <Bascule
          taille="petite"
          libelle={t('reglage.theme')}
          valeur={theme}
          onChange={setTheme}
          options={[
            { valeur: 'clair', icone: Sun, libelleCache: t('reglage.clair') },
            { valeur: 'sombre', icone: Moon, libelleCache: t('reglage.sombre') },
          ]}
        />
      ),
    },
  ]

  if (!lignes) {
    return (
      <div className={`flex items-center gap-2 ${className}`}>
        {reglages.map((r) => (
          <div key={r.cle}>{r.bascule}</div>
        ))}
      </div>
    )
  }

  return (
    <div className={`flex flex-col ${className}`}>
      {reglages.map(({ cle, icone: Icone, bascule }) => (
        <div key={cle} className="flex h-12 items-center justify-between gap-4 px-4">
          <span className="flex items-center gap-3 text-sm font-medium text-texte-2">
            <Icone aria-hidden="true" className="size-5" strokeWidth={1.75} />
            {t(`reglage.${cle}`)}
          </span>
          {bascule}
        </div>
      ))}
    </div>
  )
}
