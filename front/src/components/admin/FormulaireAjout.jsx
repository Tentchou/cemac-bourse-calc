import { useEffect, useMemo, useState } from 'react'
import { creer, listerEmetteurs } from '../../api/admin'
import { listerTitres } from '../../api/titres'
import { lireLigne, preparerReferences } from '../../admin/import/conversion'
import { MODELES } from '../../admin/import/modeles'
import { libelleChoix } from '../../admin/rubriques'
import { usePreferences } from '../../contexte/Preferences'
import Bouton from '../ui/Bouton'
import Champ from '../ui/Champ'
import ChampChoix from '../ui/ChampChoix'
import ChampDate from '../ui/date/ChampDate'
import Panneau from '../ui/Panneau'

const UNITES = { nominal: 'FCFA', tauxFacial: '%', couponCouruPublie: 'FCFA', montantBrut: 'FCFA', montantNet: 'FCFA' }

/** « TauxFacial » (nom de propriété C# renvoyé par l'API) → « tauxFacial » (champ du modèle). */
const champApi = (cle) => cle.charAt(0).toLowerCase() + cle.slice(1)

/**
 * Ajout d'une seule ligne, dans le panneau latéral. Les champs viennent du modèle d'import et la saisie est
 * lue par la même fonction qu'une ligne de fichier (lireLigne) : mêmes formats, mêmes contrôles.
 * Puis l'API contrôle à nouveau ; ses refus sont affichés sous le champ concerné.
 */
export default function FormulaireAjout({ rubrique, onFermer, onCree, onSessionExpiree }) {
  const p = usePreferences()
  const { t } = p
  const modele = MODELES[rubrique.modele]
  const besoinEmetteurs = modele.colonnes.some((c) => c.type === 'emetteur')
  const besoinTitres = modele.colonnes.some((c) => c.type === 'isin')

  const [valeurs, setValeurs] = useState({})
  const [erreurs, setErreurs] = useState({})
  const [message, setMessage] = useState(null)
  const [references, setReferences] = useState(null)
  const [listes, setListes] = useState({ emetteurs: [], titres: [] })
  const [envoi, setEnvoi] = useState(false)

  // Les listes de choix sont relues à chaque ouverture (le formulaire est monté à neuf) : un émetteur saisi
  // juste avant y figure.
  useEffect(() => {
    let actif = true
    Promise.all([besoinTitres ? listerTitres() : [], besoinEmetteurs ? listerEmetteurs() : []])
      .then(([titres, emetteurs]) => {
        if (!actif) return
        setListes({ titres, emetteurs })
        setReferences(preparerReferences(titres, emetteurs))
      })
      .catch((e) => {
        if (!actif) return
        if (e.statut === 401) onSessionExpiree()
        setMessage(e.message)
      })
    return () => {
      actif = false
    }
  }, [besoinTitres, besoinEmetteurs, onSessionExpiree])

  const optionsTitres = useMemo(
    () =>
      listes.titres
        .filter((x) => rubrique.modele !== 'dividendes' || x.typeTitre === 'Action')
        .map((x) => ({ valeur: x.isin, libelle: `${x.nom} · ${x.isin}` })),
    [listes.titres, rubrique.modele],
  )
  const optionsEmetteurs = useMemo(
    () => listes.emetteurs.map((e) => ({ valeur: e.id, libelle: e.sigle ? `${e.sigle} — ${e.nom}` : e.nom })),
    [listes.emetteurs],
  )

  function changer(champ, valeur) {
    setValeurs((v) => ({ ...v, [champ]: valeur }))
    setErreurs((e) => (e[champ] ? { ...e, [champ]: null } : e))
  }

  async function enregistrer(evenement) {
    evenement.preventDefault()
    if (!references || envoi) return
    setMessage(null)

    const { commande, erreurs: lues } = lireLigne(modele, (champ) => valeurs[champ], references)
    if (lues.length) {
      setErreurs(Object.fromEntries(lues.map((e) => [e.champ, e.message === 'obligatoire' ? t('admin.ajout.obligatoire') : e.message])))
      setMessage(t('admin.ajout.corriger'))
      return
    }

    setEnvoi(true)
    try {
      await creer(modele.route, commande)
      onCree()
    } catch (e) {
      if (e.statut === 401) onSessionExpiree()
      const parChamp = Object.entries(e.details ?? {}).map(([cle, messages]) => [champApi(cle), messages.join(' ')])
      const connus = parChamp.filter(([champ]) => modele.colonnes.some((c) => c.champ === champ))
      setErreurs(Object.fromEntries(connus))
      // Un refus sans champ précis (titre déjà saisi, règle métier) s'affiche en tête du formulaire.
      setMessage(connus.length === parChamp.length && connus.length ? t('admin.ajout.corriger') : e.message)
    } finally {
      setEnvoi(false)
    }
  }

  function champ(colonne) {
    const { champ: nom, type, obligatoire } = colonne
    const libelle = `${t(`admin.champ.${nom}`)}${obligatoire ? ' *' : ''}`
    const cleAide = nom === 'titreId' && rubrique.modele === 'cours' ? 'admin.aide.titreId.cours' : `admin.aide.${nom}`
    const aide = t(cleAide) === cleAide ? undefined : t(cleAide)
    const commun = { key: nom, libelle, aide, erreur: erreurs[nom] ?? undefined }
    const large = nom === 'nom' || nom === 'libelle' || nom === 'siteWeb' || type === 'isin' ? 'sm:col-span-2' : ''

    if (type === 'date')
      return <ChampDate {...commun} className={large} valeur={valeurs[nom] ?? ''} onChange={(iso) => changer(nom, iso)} required={obligatoire} />

    if (type === 'choix' || type === 'emetteur' || type === 'isin') {
      const options =
        type === 'emetteur'
          ? optionsEmetteurs
          : type === 'isin'
            ? optionsTitres
            : colonne.choix.map((c) => ({ valeur: c.valeur, libelle: nom === 'paysCode' ? t(`pays.${c.valeur}`) : libelleChoix(t, c.valeur, colonne.choix) }))
      return (
        <ChampChoix
          {...commun}
          className={large}
          options={options}
          vide={t('admin.ajout.choisir')}
          value={valeurs[nom] ?? ''}
          onChange={(e) => changer(nom, e.target.value)}
          required={obligatoire}
        />
      )
    }

    return (
      <Champ
        {...commun}
        className={large}
        unite={UNITES[nom]}
        value={valeurs[nom] ?? ''}
        onChange={(e) => changer(nom, e.target.value)}
        maxLength={colonne.longueurMax ?? (type === 'texte' ? 200 : 30)}
        inputMode={type === 'nombre' ? 'decimal' : type === 'entier' ? 'numeric' : undefined}
        autoCapitalize={nom === 'isin' ? 'characters' : undefined}
        spellCheck={type === 'texte' && nom !== 'isin' ? undefined : false}
        required={obligatoire}
      />
    )
  }

  const groupes = rubrique.groupesFormulaire ?? [{ cle: null, champs: modele.colonnes.map((c) => c.champ) }]
  const colonne = (nom) => modele.colonnes.find((c) => c.champ === nom)
  const idFormulaire = `ajout-${rubrique.cle}`

  return (
    <Panneau
      ouvert
      onFermer={onFermer}
      occupe={envoi}
      titre={t(`admin.ajout.titre.${rubrique.cle}`)}
      aide={t('admin.ajout.aide')}
      pied={
        <>
          <Bouton taille="petite" onClick={onFermer} disabled={envoi}>
            {t('admin.annuler')}
          </Bouton>
          <Bouton taille="petite" variante="action" type="submit" form={idFormulaire} chargement={envoi} disabled={!references}>
            {envoi ? t('admin.enregistrement') : t('admin.enregistrer')}
          </Bouton>
        </>
      }
    >
      <form id={idFormulaire} onSubmit={enregistrer} noValidate className="flex flex-col gap-7">
        {message && (
          <p role="alert" className="rounded-2xl border border-erreur/30 bg-erreur/8 px-4 py-3 text-sm text-erreur">
            {message}
          </p>
        )}
        {!references && !message && (
          <p role="status" className="text-sm text-texte-2">
            {t('admin.ajout.chargement')}
          </p>
        )}
        {references &&
          groupes.map((g) => (
            <fieldset key={g.cle ?? 'tous'} className="grid min-w-0 grid-cols-1 gap-x-4 gap-y-5 sm:grid-cols-2">
              {g.cle && (
                <legend className="mb-3 text-xs font-semibold tracking-wider text-texte-2 uppercase">{t(`admin.groupe.${g.cle}`)}</legend>
              )}
              {g.champs.map((nom) => champ(colonne(nom)))}
            </fieldset>
          ))}
      </form>
    </Panneau>
  )
}
