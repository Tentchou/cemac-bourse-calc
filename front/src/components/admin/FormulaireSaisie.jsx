import { Archive, Lock, Trash2, TriangleAlert } from 'lucide-react'
import { useEffect, useMemo, useState } from 'react'
import { creer, lireTitre, listerEmetteurs, modifier, supprimerTitre } from '../../api/admin'
import { listerTitres } from '../../api/titres'
import { aUnHistorique, commandeModification, STATUTS, valeursFormulaire } from '../../admin/edition'
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
 * Saisie d'une ligne dans le panneau latéral : un ajout, ou la modification d'un titre (titreId).
 * Les champs viennent du modèle d'import et la saisie est lue par la même fonction qu'une ligne de fichier
 * (lireLigne) : mêmes formats, mêmes contrôles. Puis l'API contrôle à nouveau ; ses refus sont affichés
 * sous le champ concerné.
 *
 * En modification : le formulaire est prérempli, l'ISIN est figé (identité du titre), le statut s'ajoute,
 * et « Supprimer » efface un titre sans historique ou radie un titre qui en a un (après confirmation).
 * onEnregistre(message) : appelé après un ajout, une modification ou une suppression réussie.
 */
export default function FormulaireSaisie({ rubrique, titreId = null, onFermer, onEnregistre, onSessionExpiree }) {
  const p = usePreferences()
  const { t, langue } = p
  const edition = titreId !== null
  const modele = MODELES[rubrique.modele]
  const besoinEmetteurs = modele.colonnes.some((c) => c.type === 'emetteur')
  const besoinTitres = modele.colonnes.some((c) => c.type === 'isin')

  const [valeurs, setValeurs] = useState({})
  const [erreurs, setErreurs] = useState({})
  const [message, setMessage] = useState(null)
  const [references, setReferences] = useState(null)
  const [listes, setListes] = useState({ emetteurs: [], titres: [] })
  const [titre, setTitre] = useState(null)
  const [envoi, setEnvoi] = useState(false)
  const [confirmation, setConfirmation] = useState(false)

  // La langue du préremplissage est lue une fois, à l'ouverture : changer de langue ne recharge pas
  // le formulaire (une saisie en cours ne doit jamais être perdue).
  const [langueOuverture] = useState(langue)

  // Les listes de choix (et le titre à modifier) sont relues à chaque ouverture : le formulaire est monté à neuf.
  useEffect(() => {
    const controleur = new AbortController()
    Promise.all([
      besoinTitres ? listerTitres() : [],
      besoinEmetteurs ? listerEmetteurs() : [],
      edition ? lireTitre(titreId, controleur.signal) : null,
    ])
      .then(([titres, emetteurs, detail]) => {
        if (controleur.signal.aborted) return
        setListes({ titres, emetteurs })
        if (detail) {
          setTitre(detail)
          setValeurs(valeursFormulaire(modele, detail, langueOuverture))
        }
        setReferences(preparerReferences(titres, emetteurs))
      })
      .catch((e) => {
        if (controleur.signal.aborted) return
        if (e.statut === 401) onSessionExpiree()
        setMessage(e.message)
      })
    return () => controleur.abort()
  }, [besoinTitres, besoinEmetteurs, edition, titreId, modele, langueOuverture, onSessionExpiree])

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

  function afficherRefus(e) {
    if (e.statut === 401) onSessionExpiree()
    const parChamp = Object.entries(e.details ?? {}).map(([cle, messages]) => [champApi(cle), messages.join(' ')])
    const connus = parChamp.filter(([champ]) => champ === 'statut' || modele.colonnes.some((c) => c.champ === champ))
    setErreurs(Object.fromEntries(connus))
    // Un refus sans champ précis (titre déjà saisi, règle métier) s'affiche en tête du formulaire.
    setMessage(connus.length === parChamp.length && connus.length ? t('admin.ajout.corriger') : e.message)
  }

  async function enregistrer(evenement) {
    evenement.preventDefault()
    if (!references || envoi) return
    setMessage(null)

    const { commande, erreurs: lues } = lireLigne(modele, (champ) => valeurs[champ], references)
    const modification = edition ? commandeModification(commande, valeurs.statut) : null
    if (lues.length || (edition && !modification)) {
      const parChamp = lues.map((e) => [e.champ, e.message === 'obligatoire' ? t('admin.ajout.obligatoire') : e.message])
      if (edition && !modification) parChamp.push(['statut', t('admin.ajout.obligatoire')])
      setErreurs(Object.fromEntries(parChamp))
      setMessage(t('admin.ajout.corriger'))
      return
    }

    setEnvoi(true)
    try {
      const reponse = edition ? await modifier(modele.route, titreId, modification) : await creer(modele.route, commande)
      onEnregistre(edition ? (reponse?.message ?? t('admin.modifie')) : t('admin.cree'))
    } catch (e) {
      afficherRefus(e)
    } finally {
      setEnvoi(false)
    }
  }

  async function supprimer() {
    if (envoi) return
    setEnvoi(true)
    setMessage(null)
    try {
      const reponse = await supprimerTitre(titreId)
      onEnregistre(reponse?.message ?? t('admin.modifie'))
    } catch (e) {
      if (e.statut === 401) onSessionExpiree()
      setConfirmation(false)
      setMessage(e.message)
    } finally {
      setEnvoi(false)
    }
  }

  function champ(colonne) {
    const { champ: nom, type, obligatoire } = colonne
    const libelle = `${t(`admin.champ.${nom}`)}${obligatoire ? ' *' : ''}`
    const cleAide =
      edition && nom === 'isin' ? 'admin.aide.isin.fige' : nom === 'titreId' && rubrique.modele === 'cours' ? 'admin.aide.titreId.cours' : `admin.aide.${nom}`
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

    // En modification, l'ISIN est l'identité du titre : affiché, jamais modifiable.
    const fige = edition && nom === 'isin'
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
        readOnly={fige}
        icone={fige ? Lock : undefined}
      />
    )
  }

  const champStatut = (
    <ChampChoix
      key="statut"
      libelle={`${t('admin.champ.statut')} *`}
      aide={t('admin.aide.statut')}
      erreur={erreurs.statut ?? undefined}
      options={STATUTS.map((s) => ({ valeur: s, libelle: t(`admin.statut.${s}`) }))}
      vide={t('admin.ajout.choisir')}
      value={valeurs.statut ?? ''}
      onChange={(e) => changer('statut', e.target.value)}
      required
    />
  )

  const groupes = rubrique.groupesFormulaire ?? [{ cle: null, champs: modele.colonnes.map((c) => c.champ) }]
  const colonne = (nom) => modele.colonnes.find((c) => c.champ === nom)
  const idFormulaire = `saisie-${rubrique.cle}`
  const historique = aUnHistorique(titre)
  const pret = references && (!edition || titre)

  const piedSaisie = (
    <>
      {edition && titre && (
        <Bouton
          taille="petite"
          variante="fantome"
          icone={historique ? Archive : Trash2}
          onClick={() => setConfirmation(true)}
          disabled={envoi || (historique && titre.statut === 'Radie')}
          className="text-erreur hover:text-erreur sm:mr-auto"
        >
          {historique ? t('admin.radier') : t('admin.supprimer')}
        </Bouton>
      )}
      <Bouton taille="petite" onClick={onFermer} disabled={envoi}>
        {t('admin.annuler')}
      </Bouton>
      <Bouton taille="petite" variante="action" type="submit" form={idFormulaire} chargement={envoi} disabled={!pret}>
        {envoi ? t('admin.enregistrement') : t('admin.enregistrer')}
      </Bouton>
    </>
  )

  const piedConfirmation = (
    <>
      <Bouton taille="petite" onClick={() => setConfirmation(false)} disabled={envoi}>
        {t('admin.annuler')}
      </Bouton>
      <Bouton
        taille="petite"
        icone={historique ? Archive : Trash2}
        chargement={envoi}
        onClick={supprimer}
        className="border-erreur bg-erreur text-surface hover:brightness-110"
      >
        {historique ? t('admin.radier.confirmer') : t('admin.supprimer.confirmer')}
      </Bouton>
    </>
  )

  return (
    <Panneau
      ouvert
      onFermer={onFermer}
      occupe={envoi}
      titre={edition ? t(`admin.modif.titre.${rubrique.cle}`) : t(`admin.ajout.titre.${rubrique.cle}`)}
      aide={edition ? t('admin.modif.aide') : t('admin.ajout.aide')}
      pied={confirmation ? piedConfirmation : piedSaisie}
    >
      {confirmation && titre ? (
        <div role="alert" className="apparition flex items-start gap-3 rounded-2xl border border-erreur/30 bg-erreur/8 px-4 py-4 text-sm">
          <TriangleAlert aria-hidden="true" className="mt-0.5 size-5 shrink-0 text-erreur" strokeWidth={1.75} />
          <div>
            <p className="font-semibold text-texte">
              {historique ? t('admin.radier.question', { nom: titre.nom }) : t('admin.supprimer.question', { nom: titre.nom })}
            </p>
            <p className="mt-1 text-texte-2">
              {historique
                ? t('admin.radier.explication', { cours: p.nombre(titre.nombreCours, 0), dividendes: p.nombre(titre.nombreDividendes, 0) })
                : t('admin.supprimer.explication')}
            </p>
          </div>
        </div>
      ) : (
        <form id={idFormulaire} onSubmit={enregistrer} noValidate className="flex flex-col gap-7">
          {message && (
            <p role="alert" className="rounded-2xl border border-erreur/30 bg-erreur/8 px-4 py-3 text-sm text-erreur">
              {message}
            </p>
          )}
          {!pret && !message && (
            <p role="status" className="text-sm text-texte-2">
              {t('admin.ajout.chargement')}
            </p>
          )}
          {pret &&
            groupes.map((g, rang) => (
              <fieldset key={g.cle ?? 'tous'} className="grid min-w-0 grid-cols-1 gap-x-4 gap-y-5 sm:grid-cols-2">
                {g.cle && (
                  <legend className="mb-3 text-xs font-semibold tracking-wider text-texte-2 uppercase">{t(`admin.groupe.${g.cle}`)}</legend>
                )}
                {g.champs.map((nom) => champ(colonne(nom)))}
                {edition && rang === 0 && champStatut}
              </fieldset>
            ))}
        </form>
      )}
    </Panneau>
  )
}
