import { CircleCheck, Plus, Search, Upload, X } from 'lucide-react'
import { useEffect, useMemo, useState } from 'react'
import { listerSeances } from '../../api/admin'
import { MODELES } from '../../admin/import/modeles'
import useListeAdmin from '../../admin/useListeAdmin'
import { usePreferences } from '../../contexte/Preferences'
import Bouton from '../ui/Bouton'
import { EtatVide, MessageErreur } from '../ui/Etats'
import Pagination from '../ui/pagination/Pagination'
import { etatPage } from '../ui/pagination/paginer'
import { SqueletteTableau } from '../ui/Squelettes'
import Tableau from '../ui/Tableau'
import FormulaireAjout from './FormulaireAjout'
import ImportFichier from './ImportFichier'

const CLASSE_FILTRE =
  'h-10 min-w-0 rounded-full border border-bordure-champ bg-surface px-4 text-sm text-texte outline-none transition-[border-color,box-shadow] hover:border-texte-2 focus:border-action focus:ring-4 focus:ring-action/15'

/**
 * Une rubrique de l'administration : titre et nombre de lignes, « Importer un fichier » et « Ajouter »,
 * filtres, tableau paginé par l'API. Après un ajout ou un import, la liste et les compteurs sont relus.
 */
export default function PageRubrique({ rubrique, onModifie, onSessionExpiree }) {
  const p = usePreferences()
  const { t } = p
  const route = MODELES[rubrique.modele].route
  const liste = useListeAdmin(route, { onSessionExpiree })
  const [panneau, setPanneau] = useState(null) // null · ajouter · importer
  const [succes, setSucces] = useState(null)
  const [seances, setSeances] = useState([])

  // Les séances saisies, pour le filtre des cours (la dernière est la valeur par défaut côté API).
  useEffect(() => {
    if (!rubrique.seances) return undefined
    const controleur = new AbortController()
    listerSeances(controleur.signal).then(setSeances).catch(() => setSeances([]))
    return () => controleur.abort()
  }, [rubrique.seances, liste.donnees])

  // Le message de réussite s'efface seul.
  useEffect(() => {
    if (!succes) return undefined
    const minuteur = setTimeout(() => setSucces(null), 6000)
    return () => clearTimeout(minuteur)
  }, [succes])

  const colonnes = useMemo(() => rubrique.colonnes(p), [rubrique, p])
  const filtres = useMemo(() => rubrique.filtres?.(t) ?? [], [rubrique, t])
  const donnees = liste.donnees
  const total = donnees?.total ?? 0
  const pagination = donnees ? etatPage(total, liste.page, liste.taille, donnees.lignes.length) : null

  function apresEnregistrement(texte) {
    setSucces(texte)
    liste.recharger()
    onModifie()
  }

  const nombre = total === 0 ? t('admin.nombre.0') : total === 1 ? t('admin.nombre.1') : t('admin.nombre', { n: p.nombre(total, 0) })

  return (
    <>
      <div className="mb-5 flex flex-wrap items-end gap-x-4 gap-y-3">
        <div className="min-w-0 flex-1">
          <h1 className="text-2xl font-bold sm:text-[1.75rem]">{t(`admin.rubrique.${rubrique.cle}`)}</h1>
          <p className="mt-1 text-sm text-texte-2">
            {t(`admin.sous-titre.${rubrique.cle}`)}
            {donnees && (
              <>
                {' · '}
                <span className="chiffres font-medium text-texte" aria-live="polite">
                  {nombre}
                </span>
              </>
            )}
          </p>
        </div>
        <div className="flex gap-2 max-sm:w-full max-sm:[&>*]:flex-1">
          <Bouton taille="petite" icone={Upload} onClick={() => setPanneau('importer')}>
            {t('admin.importer')}
          </Bouton>
          <Bouton taille="petite" variante="action" icone={Plus} onClick={() => setPanneau('ajouter')}>
            {t('admin.ajouter')}
          </Bouton>
        </div>
      </div>

      {succes && (
        <p role="status" className="apparition mb-4 flex items-center gap-2 rounded-2xl border border-action/30 bg-action/8 px-4 py-3 text-sm">
          <CircleCheck aria-hidden="true" className="size-4 shrink-0 text-action" strokeWidth={2} />
          {succes}
        </p>
      )}

      <div className="mb-3 flex flex-wrap gap-2" role="search">
        <label className="relative min-w-0 flex-[1_1_16rem]">
          <span className="sr-only">{t('admin.filtre.recherche')}</span>
          <Search aria-hidden="true" strokeWidth={1.75} className="pointer-events-none absolute top-1/2 left-3.5 size-4 -translate-y-1/2 text-texte-2" />
          <input
            type="search"
            value={liste.recherche}
            onChange={(e) => liste.setRecherche(e.target.value)}
            placeholder={t(rubrique.recherche)}
            maxLength={100}
            className={`${CLASSE_FILTRE} w-full pl-10`}
          />
        </label>
        {rubrique.seances && (
          <label className="min-w-0 max-sm:flex-[1_1_calc(50%-4px)]">
            <span className="sr-only">{t('admin.filtre.seance')}</span>
            <select value={liste.filtres.seance ?? ''} onChange={(e) => liste.setFiltre('seance', e.target.value)} className={`${CLASSE_FILTRE} w-full`}>
              <option value="">{t('admin.filtre.seance.derniere')}</option>
              {seances.map((s) => (
                <option key={s} value={s}>
                  {p.date(s)}
                </option>
              ))}
            </select>
          </label>
        )}
        {filtres.map((f) => (
          <label key={f.cle} className="min-w-0 max-sm:flex-[1_1_calc(50%-4px)]">
            <span className="sr-only">{f.tous}</span>
            <select value={liste.filtres[f.cle] ?? ''} onChange={(e) => liste.setFiltre(f.cle, e.target.value)} className={`${CLASSE_FILTRE} w-full`}>
              <option value="">{f.tous}</option>
              {f.options.map((o) => (
                <option key={o.valeur} value={o.valeur}>
                  {o.libelle}
                </option>
              ))}
            </select>
          </label>
        ))}
      </div>

      {liste.erreur ? (
        <MessageErreur erreur={liste.erreur} onReessayer={liste.recharger} />
      ) : !donnees ? (
        <SqueletteTableau colonnes={colonnes} lignes={5} />
      ) : donnees.lignes.length === 0 ? (
        <EtatVide
          icone={liste.filtre ? Search : rubrique.icone}
          titre={liste.filtre ? t('admin.vide.filtre') : t('admin.vide')}
          texte={liste.filtre ? t('admin.vide.filtre.aide') : t('admin.vide.aide')}
          action={
            liste.filtre && (
              <Bouton taille="petite" icone={X} onClick={liste.effacer}>
                {t('admin.effacer')}
              </Bouton>
            )
          }
        />
      ) : (
        <div aria-busy={liste.chargement || undefined} className={`transition-opacity duration-150 ${liste.chargement ? 'opacity-60' : ''}`}>
          <Tableau
            legende={t(`admin.rubrique.${rubrique.cle}`)}
            colonnes={colonnes}
            lignes={donnees.lignes}
            cleLigne={(ligne) => ligne.id ?? ligne.date}
          />
          <Pagination
            etat={pagination}
            taille={liste.taille}
            onPage={(n) => {
              liste.setPage(n)
              document.getElementById('contenu-admin')?.scrollIntoView({ behavior: 'smooth', block: 'start' })
            }}
            onTaille={liste.setTaille}
          />
        </div>
      )}

      {panneau === 'ajouter' && (
        <FormulaireAjout
          rubrique={rubrique}
          onFermer={() => setPanneau(null)}
          onCree={() => {
            setPanneau(null)
            apresEnregistrement(t('admin.cree'))
          }}
          onSessionExpiree={onSessionExpiree}
        />
      )}
      {panneau === 'importer' && (
        <ImportFichier
          rubrique={rubrique}
          onFermer={() => setPanneau(null)}
          onImporte={() => apresEnregistrement(t('admin.cree'))}
          onSessionExpiree={onSessionExpiree}
        />
      )}
    </>
  )
}
