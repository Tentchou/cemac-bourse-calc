// Lecture du fichier choisi dans le navigateur : rien n'est envoyé au serveur avant la vérification à l'écran.
import { readSheet } from 'read-excel-file/browser'
import { analyserCsv, decoderTexte } from './csv.js'

export const TAILLE_MAXIMUM = 2 * 1024 * 1024 // 2 Mo : un bulletin de cote entier en pèse quelques dizaines de Ko

/** @returns {Promise<{ tableau?: unknown[][], erreur?: string }>} */
export async function lireFichier(fichier) {
  if (!fichier) return { erreur: 'Choisissez un fichier.' }
  if (fichier.size > TAILLE_MAXIMUM) return { erreur: 'Le fichier dépasse 2 Mo.' }

  const nom = fichier.name.toLowerCase()
  try {
    if (nom.endsWith('.csv')) return { tableau: analyserCsv(decoderTexte(await fichier.arrayBuffer())) }
    if (nom.endsWith('.xlsx')) return { tableau: await readSheet(fichier) }
    if (nom.endsWith('.xls')) return { erreur: 'Ancien format Excel (.xls) : enregistrez le fichier en .xlsx ou en .csv.' }
    return { erreur: 'Format non pris en charge : choisissez un fichier .xlsx ou .csv.' }
  } catch {
    return { erreur: 'Le fichier est illisible ou endommagé.' }
  }
}
