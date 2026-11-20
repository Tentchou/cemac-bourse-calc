import { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react'
import * as format from '../i18n/format'
import { definirLangue } from '../services/http/intercepteurs'
import { en, fr } from '../i18n/textes'

// Langue et thème choisis par le visiteur. Mémorisés dans le navigateur (simple confort :
// tout fonctionne si le stockage est bloqué). Par défaut : la langue et le thème du système.

const DICTIONNAIRES = { fr, en: { ...fr, ...en } }
const CLE_LANGUE = 'cbc-langue'
const CLE_THEME = 'cbc-theme'

function lire(cle) {
  try {
    return localStorage.getItem(cle)
  } catch {
    return null
  }
}

function ecrire(cle, valeur) {
  try {
    localStorage.setItem(cle, valeur)
  } catch {
    /* stockage indisponible : le choix vaut pour la visite */
  }
}

function langueInitiale() {
  const memorisee = lire(CLE_LANGUE)
  if (memorisee === 'fr' || memorisee === 'en') return memorisee
  return navigator.language?.toLowerCase().startsWith('en') ? 'en' : 'fr'
}

function themeInitial() {
  const memorise = lire(CLE_THEME)
  if (memorise === 'clair' || memorise === 'sombre') return memorise
  return window.matchMedia?.('(prefers-color-scheme: dark)').matches ? 'sombre' : 'clair'
}

const Contexte = createContext(null)

export function PreferencesProvider({ children }) {
  const [langue, setLangue] = useState(langueInitiale)
  const [theme, setTheme] = useState(themeInitial)

  useEffect(() => {
    document.documentElement.lang = langue
    definirLangue(langue)
    ecrire(CLE_LANGUE, langue)
  }, [langue])

  useEffect(() => {
    document.documentElement.dataset.theme = theme
    ecrire(CLE_THEME, theme)
  }, [theme])

  const t = useCallback(
    (cle, valeurs) => format.remplir(DICTIONNAIRES[langue][cle] ?? cle, valeurs),
    [langue],
  )

  const valeur = useMemo(
    () => ({
      langue,
      setLangue,
      theme,
      setTheme,
      t,
      nombre: (v, d) => format.nombre(langue, v, d),
      montant: (v, d) => format.montant(langue, v, d),
      pourcent: (v, d) => format.pourcent(langue, v, d),
      date: (iso) => format.date(langue, iso),
      dateLongue: (iso) => format.dateLongue(langue, iso),
    }),
    [langue, theme, t],
  )

  return <Contexte.Provider value={valeur}>{children}</Contexte.Provider>
}

/** Textes traduits, formats et réglages d'affichage. */
export function usePreferences() {
  const valeur = useContext(Contexte)
  if (!valeur) throw new Error('usePreferences doit être utilisé dans PreferencesProvider.')
  return valeur
}
