import { usePreferences } from '../../contexte/Preferences'
import Etiquette from '../ui/Etiquette'

/**
 * Le contrôle du coupon couru : notre calcul face à celui publié par la BVMAC pour la dernière séance.
 * Identique (bleu), écart (orange, avec son montant), non calculé (rouge, la raison au survol),
 * rien de publié (texte discret). Le détail complet est dans l'infobulle et lu par les lecteurs d'écran.
 */
export default function ControleCoupon({ controle }) {
  const p = usePreferences()
  const { t } = p
  if (!controle) return <span className="text-xs text-texte-2">{t('admin.controle.aucun')}</span>

  const detail = controle.erreur
    ? controle.erreur
    : t('admin.controle.detail', {
        seance: p.date(controle.dateSeance),
        reglement: p.date(controle.dateReglement),
        publie: p.montant(controle.publie, 2),
        calcule: p.montant(controle.calcule, 2),
      })

  if (controle.erreur)
    return (
      <Etiquette ton="erreur" point title={detail} aria-label={`${t('admin.controle.indisponible')} : ${detail}`}>
        {t('admin.controle.indisponible')}
      </Etiquette>
    )

  if (controle.identique)
    return (
      <Etiquette ton="action" point title={detail} aria-label={`${t('admin.controle.identique')} : ${detail}`}>
        {t('admin.controle.identique')}
      </Etiquette>
    )

  const ecart = `${controle.ecart > 0 ? '+' : ''}${p.nombre(controle.ecart, 2)}`
  return (
    <Etiquette ton="orange" point title={detail} aria-label={`${t('admin.controle.ecart', { ecart })} : ${detail}`}>
      {t('admin.controle.ecart', { ecart })}
    </Etiquette>
  )
}
