import { ChevronRight, Pencil, Sigma } from 'lucide-react'
import { useEffect, useRef, useState } from 'react'
import { Lien, useRoute } from '../../routeur/Routeur'

/**
 * Tableau de données rayé (une ligne sur deux teintée), le même sur tous les écrans.
 *
 * Choix : sur mobile, on garde un vrai tableau (on compare des colonnes, une liste de cartes
 * casserait cette lecture). Il défile horizontalement DANS son cadre, jamais la page, et sa
 * première colonne reste fixe à gauche pour qu'on sache toujours de quelle ligne il s'agit.
 *
 * colonnes : [{ cle, titre, droite?, rendu?(ligne), classe? }] — la première est la colonne fixe.
 * lignes, cleLigne(ligne, i) : les données et leur clé React.
 * lienLigne(ligne) : rend la ligne cliquable (adresse interne) ; le titre de la ligne est un vrai lien.
 * actionLigne : { surClic(ligne), libelle(ligne) } — rend la ligne cliquable pour une action sur place
 *   (ouvrir la modification) ; le titre de la ligne est un vrai bouton, atteignable au clavier.
 * pied : la ligne de total (même forme qu'une ligne) ; libellePied : son libellé, précédé d'un Σ.
 * Le total est nettement distinct : filet bleu au-dessus, fond teinté, chiffres en gras.
 * legende : titre lu par les lecteurs d'écran.
 */
export default function Tableau({ colonnes, lignes, cleLigne, lienLigne, actionLigne, pied, libellePied, legende, className = '' }) {
  const { naviguer } = useRoute()
  const cadre = useRef(null)
  const [deborde, setDeborde] = useState(false)

  // Le tableau dépasse-t-il de son cadre ? Le filet sous la colonne fixe n'apparaît que dans ce cas.
  useEffect(() => {
    const element = cadre.current
    const mesurer = () => setDeborde(element.scrollWidth > element.clientWidth + 1)
    mesurer()
    const observateur = new ResizeObserver(mesurer)
    observateur.observe(element)
    return () => observateur.disconnect()
  }, [lignes, colonnes.length])
  const valeur = (c, ligne) => (c.rendu ? c.rendu(ligne) : ligne[c.cle])
  const alignement = (c) => (c.droite ? 'text-right' : 'text-left')
  // Sur mobile, la colonne fixe est bornée (le texte passe à la ligne) pour laisser voir les autres.
  const fixe = (i) => (i === 0 ? 'colonne-fixe max-sm:max-w-[11rem] max-sm:min-w-[8.5rem] max-sm:whitespace-normal' : '')

  return (
    <div className={`min-w-0 overflow-hidden rounded-2xl border border-bordure bg-surface ${className}`}>
      <div ref={cadre} data-deborde={deborde} className="overflow-x-auto overscroll-x-contain" role="region" aria-label={legende} tabIndex={0}>
        <table className="tableau-raye chiffres text-sm sm:text-[0.9375rem]">
          {legende && <caption className="sr-only">{legende}</caption>}
          <thead>
            <tr>
              {colonnes.map((c, i) => (
                <th
                  key={c.cle}
                  scope="col"
                  className={`h-11 px-4 text-xs font-semibold tracking-wide whitespace-nowrap text-texte-2 uppercase sm:px-5 ${alignement(c)} ${fixe(i)} ${c.classe ?? ''}`}
                >
                  {c.titre}
                </th>
              ))}
              {(lienLigne || actionLigne) && <th aria-hidden="true" className="w-12" />}
            </tr>
          </thead>

          <tbody>
            {lignes.map((ligne, rang) => {
              const lien = lienLigne?.(ligne)
              const action = actionLigne ? () => actionLigne.surClic(ligne) : null
              const surClic = lien ? () => naviguer(lien) : action
              return (
                <tr
                  key={cleLigne(ligne, rang)}
                  onClick={surClic ? (e) => !e.target.closest('a, button') && surClic() : undefined}
                  className={`group ${surClic ? 'cursor-pointer' : ''}`}
                >
                  {colonnes.map((c, i) => (
                    <td key={c.cle} className={`h-14 px-4 whitespace-nowrap sm:px-5 ${alignement(c)} ${fixe(i)} ${c.classe ?? ''}`}>
                      {i === 0 && lien ? (
                        <Lien vers={lien} className="rounded-md font-semibold text-texte transition-colors duration-150 group-hover:text-action">
                          {valeur(c, ligne)}
                        </Lien>
                      ) : i === 0 && action ? (
                        <button
                          type="button"
                          onClick={action}
                          aria-label={actionLigne.libelle(ligne)}
                          className="cursor-pointer rounded-md text-left text-texte transition-colors duration-150 group-hover:text-action"
                        >
                          {valeur(c, ligne)}
                        </button>
                      ) : (
                        valeur(c, ligne)
                      )}
                    </td>
                  ))}
                  {action && (
                    <td className="w-12 pr-4 text-texte-2">
                      <Pencil aria-hidden="true" strokeWidth={1.75} className="size-4 transition-colors duration-150 group-hover:text-action" />
                    </td>
                  )}
                  {lien && (
                    <td className="w-12 pr-4 text-texte-2">
                      <ChevronRight
                        aria-hidden="true"
                        strokeWidth={1.75}
                        className="size-4 transition-transform duration-150 ease-douce group-hover:translate-x-1 group-hover:text-action"
                      />
                    </td>
                  )}
                </tr>
              )
            })}
          </tbody>

          {pied && (
            <tfoot>
              <tr>
                {colonnes.map((c, i) => (
                  <td key={c.cle} className={`h-16 px-4 whitespace-nowrap sm:px-5 ${alignement(c)} ${fixe(i)} ${c.classe ?? ''}`}>
                    {i === 0 && libellePied ? (
                      <span className="inline-flex items-center gap-2">
                        <span aria-hidden="true" className="grid size-6 place-items-center rounded-full bg-action text-sur-action">
                          <Sigma className="size-3.5" strokeWidth={2.5} />
                        </span>
                        {libellePied}
                      </span>
                    ) : (
                      valeur(c, pied)
                    )}
                  </td>
                ))}
                {(lienLigne || actionLigne) && <td />}
              </tr>
            </tfoot>
          )}
        </table>
      </div>
    </div>
  )
}
