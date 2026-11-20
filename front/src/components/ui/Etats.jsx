import { CircleAlert, RotateCw } from 'lucide-react'
import { usePreferences } from '../../contexte/Preferences'
import Bouton from './Bouton'
import Carte from './Carte'
import Pastille from './Pastille'

/**
 * Erreur traduite, dans une carte, avec « Réessayer » quand c'est utile.
 * erreur : une ErreurApi (services) : son message d'API s'il existe, sinon le texte de son type.
 * cles : textes propres à l'écran par type d'erreur, ex. { introuvable: 'fiche.introuvable' }.
 */
export function MessageErreur({ erreur, cles = {}, onReessayer, className = '' }) {
  const { t } = usePreferences()
  const cle = cles[erreur?.type] ?? erreur?.cle
  const texte = erreur?.message || (cle ? t(cle) : t('erreur.reseau'))
  return (
    <Carte role="alert" className={`apparition flex flex-wrap items-center gap-4 ${className}`}>
      <Pastille icone={CircleAlert} ton="erreur" />
      <p className="min-w-0 flex-1 text-sm text-texte">{texte}</p>
      {onReessayer && (
        <Bouton taille="petite" icone={RotateCw} onClick={onReessayer}>
          {t('erreur.reessayer')}
        </Bouton>
      )}
    </Carte>
  )
}

/** Rien à montrer (recherche sans résultat…) : grande icône, titre, phrase, action facultative. */
export function EtatVide({ icone, titre, texte, action, className = '' }) {
  return (
    <Carte className={`apparition flex flex-col items-center gap-4 py-12 text-center ${className}`}>
      <Pastille icone={icone} ton="neutre" taille="grande" />
      <div className="max-w-sm">
        <p className="font-titre text-lg font-semibold">{titre}</p>
        {texte && <p className="mt-1 text-sm text-texte-2">{texte}</p>}
      </div>
      {action}
    </Carte>
  )
}
