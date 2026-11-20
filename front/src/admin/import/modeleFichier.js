// Les modèles à télécharger : les bons en-têtes, et une feuille d'aide pour Excel.
import writeExcelFile from 'write-excel-file/browser'
import { aideColonne } from './modeles.js'

function telecharger(blob, nomFichier) {
  const url = URL.createObjectURL(blob)
  const lien = document.createElement('a')
  lien.href = url
  lien.download = nomFichier
  lien.click()
  URL.revokeObjectURL(url)
}

function nomFichier(modele, extension) {
  return `modele-${modele.route}.${extension}`
}

/** CSV : BOM UTF-8 et « ; », pour qu'Excel en français l'ouvre directement avec les accents. */
export function telechargerModeleCsv(modele) {
  const entetes = modele.colonnes.map((c) => c.entete).join(';')
  telecharger(new Blob([`﻿${entetes}\r\n`], { type: 'text/csv;charset=utf-8' }), nomFichier(modele, 'csv'))
}

/** Excel : la feuille « Saisie » (en-têtes seuls) et la feuille « Aide » (ce que chaque colonne attend). */
export async function telechargerModeleXlsx(modele) {
  const saisie = [modele.colonnes.map((c) => ({ value: c.entete, fontWeight: 'bold' }))]
  const aide = [
    [{ value: 'Colonne', fontWeight: 'bold' }, { value: 'Ce qu’elle attend', fontWeight: 'bold' }],
    ...modele.colonnes.map((c) => [c.entete, aideColonne(c)]),
  ]
  const blob = await writeExcelFile([
    { sheet: 'Saisie', data: saisie, columns: modele.colonnes.map(() => ({ width: 22 })) },
    { sheet: 'Aide', data: aide, columns: [{ width: 26 }, { width: 80 }] },
  ]).toBlob()
  telecharger(blob, nomFichier(modele, 'xlsx'))
}
