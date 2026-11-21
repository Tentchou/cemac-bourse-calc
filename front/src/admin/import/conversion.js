// Tableau lu dans le fichier → lignes de commande pour l'API, avec les erreurs par numéro de ligne du fichier.
import { estVide, lireChoix, lireDate, lireEntier, lireNombre, lireTexte, normaliser } from './valeurs.js'

export const LIGNES_MAXIMUM = 500

/**
 * Les références déjà en base, pour traduire un ISIN ou un nom d'émetteur en identifiant.
 * @param {{id: string, isin: string}[]} titres
 * @param {{id: string, nom: string, sigle?: string|null}[]} emetteurs
 */
export function preparerReferences(titres, emetteurs) {
  const titresParIsin = new Map(titres.map((t) => [t.isin.trim().toUpperCase(), t.id]))
  const emetteursParNom = new Map()
  for (const e of emetteurs) {
    emetteursParNom.set(normaliser(e.nom), e.id)
    if (e.sigle) emetteursParNom.set(normaliser(e.sigle), e.id)
  }
  // Le formulaire de saisie choisit l'émetteur dans une liste : il donne directement son identifiant.
  const emetteursIds = new Set(emetteurs.map((e) => e.id))
  return { titresParIsin, emetteursParNom, emetteursIds }
}

function lireCellule(colonne, cellule, references) {
  switch (colonne.type) {
    case 'texte': return lireTexte(cellule, colonne)
    case 'nombre': return lireNombre(cellule)
    case 'entier': return lireEntier(cellule)
    case 'date': return lireDate(cellule)
    case 'choix': return lireChoix(cellule, colonne.choix)
    case 'emetteur': {
      if (references.emetteursIds?.has(String(cellule))) return { valeur: String(cellule) }
      const id = references.emetteursParNom.get(normaliser(cellule))
      return id ? { valeur: id } : { erreur: `l'émetteur « ${String(cellule).trim()} » n'existe pas : saisissez-le d'abord` }
    }
    case 'isin': {
      const id = references.titresParIsin.get(String(cellule).trim().toUpperCase())
      return id ? { valeur: id } : { erreur: `le titre ${String(cellule).trim()} n'existe pas : saisissez-le d'abord` }
    }
    default: throw new Error(`Type de colonne inconnu : ${colonne.type}`)
  }
}

/**
 * Une ligne, lue champ par champ : la même règle pour une ligne de fichier et pour le formulaire de saisie.
 * @param {object} modele un modèle de modeles.js
 * @param {(champ: string) => unknown} cellule la valeur saisie pour un champ (vide : null, '' ou absent)
 * @returns {{ commande: object, erreurs: {champ: string, entete: string, message: string}[] }}
 */
export function lireLigne(modele, cellule, references) {
  const commande = {}
  const erreurs = []
  for (const colonne of modele.colonnes) {
    const valeur = cellule(colonne.champ)
    if (estVide(valeur)) {
      if (colonne.obligatoire) erreurs.push({ champ: colonne.champ, entete: colonne.entete, message: 'obligatoire' })
      commande[colonne.champ] = colonne.defaut ?? null
      continue
    }
    const lu = lireCellule(colonne, valeur, references)
    if (lu.erreur) erreurs.push({ champ: colonne.champ, entete: colonne.entete, message: lu.erreur })
    else commande[colonne.champ] = lu.valeur
  }
  return { commande, erreurs }
}

/**
 * @param {object} modele un modèle de modeles.js
 * @param {unknown[][]} tableau les lignes du fichier, la première est celle des en-têtes
 * @returns {{
 *   erreurFichier?: string,
 *   colonnesIgnorees: string[],
 *   commandes: object[],
 *   lignesFichier: number[],
 *   erreurs: {ligne: number, messages: string[]}[]
 * }}
 * lignesFichier[i] est le numéro, dans le fichier, de la ligne qui a donné commandes[i] (1 = les en-têtes).
 */
export function convertir(modele, tableau, references) {
  const vide = { colonnesIgnorees: [], commandes: [], lignesFichier: [], erreurs: [] }
  if (!tableau?.length) return { ...vide, erreurFichier: 'Le fichier est vide.' }

  // Les en-têtes peuvent être dans n'importe quel ordre, avec ou sans accents ni majuscules.
  const entetes = tableau[0].map((e) => normaliser(e))
  const positions = new Map()
  const manquantes = []
  for (const colonne of modele.colonnes) {
    const position = entetes.indexOf(normaliser(colonne.entete))
    if (position >= 0) positions.set(colonne.champ, position)
    else if (colonne.obligatoire) manquantes.push(colonne.entete)
  }
  if (manquantes.length)
    return { ...vide, erreurFichier: `Colonnes obligatoires absentes : ${manquantes.join(', ')}. Partez du modèle à télécharger.` }

  const connues = new Set(modele.colonnes.map((c) => normaliser(c.entete)))
  const colonnesIgnorees = tableau[0].filter((e, i) => !estVide(e) && !connues.has(entetes[i])).map((e) => String(e).trim())

  const commandes = []
  const lignesFichier = []
  const erreurs = []

  tableau.slice(1).forEach((ligne, index) => {
    const numero = index + 2
    if (!ligne || ligne.every(estVide)) return // ligne blanche : ignorée

    const { commande, erreurs: erreursLigne } = lireLigne(modele, (champ) => {
      const position = positions.get(champ)
      return position === undefined ? null : ligne[position]
    }, references)

    if (erreursLigne.length) erreurs.push({ ligne: numero, messages: erreursLigne.map((e) => `${e.entete} : ${e.message}`) })
    else {
      commandes.push(commande)
      lignesFichier.push(numero)
    }
  })

  if (!commandes.length && !erreurs.length) return { ...vide, colonnesIgnorees, erreurFichier: 'Le fichier ne contient aucune ligne à importer.' }
  if (commandes.length + erreurs.length > LIGNES_MAXIMUM)
    return { ...vide, colonnesIgnorees, erreurFichier: `Le fichier contient plus de ${LIGNES_MAXIMUM} lignes : découpez-le en plusieurs fichiers.` }

  return { colonnesIgnorees, commandes, lignesFichier, erreurs }
}

/** Les erreurs de l'API (numérotées dans l'ordre d'envoi) ramenées aux numéros de ligne du fichier. */
export function erreursServeurVersFichier(erreursServeur, lignesFichier) {
  return erreursServeur.map((e) => ({ ligne: lignesFichier[e.ligne - 1] ?? e.ligne, messages: e.messages }))
}
