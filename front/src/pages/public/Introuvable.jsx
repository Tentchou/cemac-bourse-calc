import { ArrowLeft, Compass } from 'lucide-react'
import Bouton from '../../components/ui/Bouton'
import Pastille from '../../components/ui/Pastille'
import { usePreferences } from '../../contexte/Preferences'

export default function Introuvable() {
  const { t } = usePreferences()
  return (
    <div className="mx-auto flex max-w-6xl flex-col items-center gap-5 px-4 pt-24 text-center">
      <Pastille icone={Compass} ton="neutre" taille="grande" />
      <h1 className="text-2xl font-bold">404</h1>
      <Bouton vers="/" variante="action" icone={ArrowLeft}>
        {t('fiche.retour')}
      </Bouton>
    </div>
  )
}
