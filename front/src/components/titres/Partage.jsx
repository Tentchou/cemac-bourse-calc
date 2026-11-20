import { Check, Link2 } from 'lucide-react'
import { useState } from 'react'
import { usePreferences } from '../../contexte/Preferences'
import { IconeWhatsApp } from '../icones/Icones'
import Bouton from '../ui/Bouton'

/** Partager le calcul : WhatsApp (le canal le plus utilisé dans la CEMAC) et copie du lien. */
export default function Partage({ nomTitre }) {
  const { t } = usePreferences()
  const [copie, setCopie] = useState(false)
  const url = window.location.href
  const message = `${t('partage.message', { titre: nomTitre })}\n${url}`

  async function copier() {
    try {
      await navigator.clipboard.writeText(url)
      setCopie(true)
      setTimeout(() => setCopie(false), 2000)
    } catch {
      /* presse-papiers refusé : rien à faire, le lien reste dans la barre d'adresse */
    }
  }

  return (
    <div className="flex flex-wrap gap-2">
      <Bouton
        variante="whatsapp"
        href={`https://wa.me/?text=${encodeURIComponent(message)}`}
        target="_blank"
        rel="noopener noreferrer"
        icone={IconeWhatsApp}
      >
        {t('partage.whatsapp')}
      </Bouton>
      <Bouton icone={copie ? Check : Link2} onClick={copier} aria-live="polite">
        {copie ? t('partage.copie') : t('partage.copier')}
      </Bouton>
    </div>
  )
}
