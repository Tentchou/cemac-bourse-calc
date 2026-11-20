import { usePreferences } from '../../contexte/Preferences'
import Carte from './Carte'

/**
 * Skeletons : chacun reprend la forme EXACTE du bloc qu'il remplace (mêmes hauteurs, mêmes
 * marges, mêmes rayures). Chaque bloc a le sien et est remplacé dès que SES données arrivent :
 * la page se construit pas à pas, jamais d'un seul bloc. Un reflet traverse les formes.
 */

/** Forme de base. Donner la taille par className (ex. « h-4 w-32 »). rond : cercle. */
export function Forme({ rond = false, className = '' }) {
  return <span aria-hidden="true" className={`squelette block ${rond ? 'rounded-full' : 'rounded-lg'} ${className}`} />
}

/** Annonce le chargement aux lecteurs d'écran ; les formes elles-mêmes sont masquées. */
function Zone({ className = '', children }) {
  const { t } = usePreferences()
  return (
    <div role="status" aria-live="polite" className={`apparition ${className}`}>
      <span className="sr-only">{t('etat.chargement')}</span>
      {children}
    </div>
  )
}

// Largeurs variées : un skeleton aux barres toutes égales paraît artificiel.
const LARGEURS = ['w-40', 'w-24', 'w-16', 'w-20', 'w-28', 'w-14', 'w-24']

/** Remplace un <Tableau> : même cadre, même en-tête, mêmes lignes rayées de 56 px, total éventuel. */
export function SqueletteTableau({ colonnes = 5, lignes = 5, lienLigne = false, pied = false, className = '' }) {
  const n = Array.isArray(colonnes) ? colonnes.length : colonnes
  const droite = (i) => (Array.isArray(colonnes) ? colonnes[i]?.droite : i >= n - 2)
  return (
    <Zone className={`min-w-0 overflow-hidden rounded-2xl border border-bordure bg-surface ${className}`}>
      <table className="tableau-raye">
        <thead>
          <tr>
            {Array.from({ length: n }, (_, i) => (
              <th key={i} className="h-11 px-4 sm:px-5">
                <Forme className={`h-2.5 w-14 ${droite(i) ? 'ml-auto' : ''}`} />
              </th>
            ))}
            {lienLigne && <th className="w-12" />}
          </tr>
        </thead>
        <tbody>
          {Array.from({ length: lignes }, (_, ligne) => (
            <tr key={ligne}>
              {Array.from({ length: n }, (_, i) => (
                <td key={i} className="h-14 px-4 sm:px-5">
                  <Forme className={`h-3.5 ${i === 0 ? LARGEURS[ligne % 3] : LARGEURS[(i + ligne) % LARGEURS.length]} max-w-full ${droite(i) ? 'ml-auto' : ''}`} />
                </td>
              ))}
              {lienLigne && (
                <td className="w-12 pr-4">
                  <Forme className="size-4" />
                </td>
              )}
            </tr>
          ))}
        </tbody>
        {pied && (
          <tfoot>
            <tr>
              {Array.from({ length: n }, (_, i) => (
                <td key={i} className="h-16 px-4 sm:px-5">
                  <Forme className={`h-4 ${i === 0 ? 'w-20' : 'w-24'} max-w-full ${droite(i) ? 'ml-auto' : ''}`} />
                </td>
              ))}
              {lienLigne && <td />}
            </tr>
          </tfoot>
        )}
      </table>
    </Zone>
  )
}

/** Remplace une carte <Resultat> : pastille + libellé, grand chiffre, aide, lien « Comment… ». */
export function SqueletteResultat({ principal = false, composition = false, className = '' }) {
  return (
    <Zone className={className}>
      <Carte accent={principal}>
        <div className="flex items-center gap-3">
          <Forme rond className="size-8" />
          <Forme className="h-3.5 w-28" />
        </div>
        <Forme className={`mt-4 ${principal ? 'h-10 w-64' : 'h-8 w-40'} max-w-full`} />
        <Forme className="mt-2 h-3.5 w-44 max-w-full" />
        {composition && (
          <div className="mt-6 space-y-3">
            <Forme className="h-3 w-full rounded-full" />
            <div className="flex justify-between gap-4">
              <Forme className="h-3.5 w-32" />
              <Forme className="h-3.5 w-24" />
            </div>
          </div>
        )}
        <div className="mt-6 border-t border-bordure pt-4">
          <Forme className="h-3.5 w-36" />
        </div>
      </Carte>
    </Zone>
  )
}

/** Remplace un chiffre clé : pastille, nombre, libellé (sur une ligne). */
export function SqueletteChiffreCle({ className = '' }) {
  return (
    <Zone className={className}>
      <Carte className="flex items-center gap-4">
        <Forme rond className="size-10" />
        <div className="space-y-2">
          <Forme className="h-7 w-10" />
          <Forme className="h-3 w-20" />
        </div>
      </Carte>
    </Zone>
  )
}

/** Remplace l'en-tête d'une fiche : retour, nom, étiquettes, émetteur ; carte du dernier cours. */
export function SqueletteEnTeteFiche() {
  return (
    <Zone className="grid gap-6 lg:grid-cols-[minmax(0,1fr)_280px] lg:items-end">
      <div className="min-w-0 space-y-4">
        <Forme className="h-10 w-36 rounded-full" />
        <Forme className="h-10 w-4/5 max-w-xl" />
        <div className="flex flex-wrap gap-2">
          {['w-28', 'w-32', 'w-24', 'w-20'].map((l, i) => (
            <Forme key={i} className={`h-7 rounded-full ${l}`} />
          ))}
        </div>
        <Forme className="h-4 w-32" />
      </div>
      <Carte className="space-y-3">
        <Forme className="h-3.5 w-36" />
        <Forme className="h-10 w-40" />
      </Carte>
    </Zone>
  )
}

/** Remplace la fiche technique (grille de caractéristiques à filets fins). */
export function SqueletteFicheTechnique({ elements = 8 }) {
  return (
    <Zone>
      <div className="grid grid-cols-2 gap-px overflow-hidden rounded-2xl border border-bordure bg-bordure lg:grid-cols-4">
        {Array.from({ length: elements }, (_, i) => (
          <div key={i} className="flex items-center gap-3 bg-surface p-4 sm:p-5">
            <Forme rond className="size-8 max-[399px]:hidden" />
            <div className="min-w-0 flex-1 space-y-2">
              <Forme className="h-3 w-16" />
              <Forme className={`h-4 ${LARGEURS[i % LARGEURS.length]} max-w-full`} />
            </div>
          </div>
        ))}
      </div>
    </Zone>
  )
}

/** Remplace le formulaire du calculateur : libellés, champs de 48 px, bouton. */
export function SqueletteFormulaire({ champs = 3 }) {
  return (
    <Zone>
      <Carte className="grid gap-4">
        <Forme className="h-5 w-36" />
        {Array.from({ length: champs }, (_, i) => (
          <div key={i} className="space-y-2">
            <Forme className="h-3.5 w-32" />
            <Forme className="h-12 w-full rounded-2xl" />
          </div>
        ))}
        <Forme className="mt-2 h-12 w-full rounded-full" />
      </Carte>
    </Zone>
  )
}

/** Remplace une rangée de statistiques (libellé + montant). */
export function SqueletteStatistiques({ nombre = 3 }) {
  return (
    <Zone className="grid grid-cols-1 gap-4 sm:grid-cols-3">
      {Array.from({ length: nombre }, (_, i) => (
        <Carte key={i} className="space-y-3">
          <Forme className="h-3 w-24" />
          <Forme className="h-6 w-32 max-w-full" />
        </Carte>
      ))}
    </Zone>
  )
}

const HAUTEURS = ['h-[12%]', 'h-[12%]', 'h-[12%]', 'h-[12%]', 'h-[88%]']

/** Remplace le graphique de l'échéancier : axe, barres de hauteurs variées, légende. */
export function SqueletteGraphique({ barres = 5 }) {
  return (
    <Zone>
      <Carte>
        <div className="mb-6 flex justify-end gap-4">
          <Forme className="h-3 w-16" />
          <Forme className="h-3 w-16" />
        </div>
        <div className="flex h-48 items-end justify-around gap-4 border-b border-bordure pb-px">
          {Array.from({ length: barres }, (_, i) => (
            <Forme key={i} className={`w-7 rounded-t-lg rounded-b-none sm:w-10 ${HAUTEURS[i % HAUTEURS.length]}`} />
          ))}
        </div>
        <div className="mt-3 flex justify-around gap-4">
          {Array.from({ length: barres }, (_, i) => (
            <Forme key={i} className="h-3 w-9" />
          ))}
        </div>
      </Carte>
    </Zone>
  )
}
