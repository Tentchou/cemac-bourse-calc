import assert from 'node:assert/strict'
import { describe, test } from 'node:test'
import { readSheet as readXlsxSheet } from 'read-excel-file/node'
import writeExcelFile from 'write-excel-file/node'
import { analyserCsv, decoderTexte } from './csv.js'
import { convertir, erreursServeurVersFichier, LIGNES_MAXIMUM, preparerReferences } from './conversion.js'
import { MODELES } from './modeles.js'

const references = preparerReferences(
  [{ id: 'id-titre-1', isin: 'CM0000000011' }],
  [{ id: 'id-etat-cm', nom: 'État du Cameroun', sigle: 'ECMR' }],
)

describe('conversion', () => {
  test('les en-têtes sont reconnus dans n’importe quel ordre, sans accents ni majuscules', () => {
    const r = convertir(MODELES.joursNonOuvres, [['LIBELLE', 'date'], ['Noël', '25/12/2026']], references)
    assert.deepEqual(r.commandes, [{ date: '2026-12-25', libelle: 'Noël' }])
    assert.deepEqual(r.lignesFichier, [2])
  })

  test('une colonne obligatoire absente arrête tout, avec son nom', () => {
    const r = convertir(MODELES.joursNonOuvres, [['Date'], ['25/12/2026']], references)
    assert.match(r.erreurFichier, /Libellé/)
    assert.equal(r.commandes.length, 0)
  })

  test('les colonnes inconnues sont signalées et ignorées', () => {
    const r = convertir(MODELES.joursNonOuvres, [['Date', 'Libellé', 'Commentaire'], ['25/12/2026', 'Noël', 'x']], references)
    assert.deepEqual(r.colonnesIgnorees, ['Commentaire'])
    assert.equal(r.commandes.length, 1)
  })

  test('toutes les erreurs d’une ligne sont listées, avec le numéro de ligne du fichier', () => {
    const r = convertir(MODELES.cours, [
      ['ISIN', 'Date de séance', 'Cours de clôture'],
      ['CM0000000011', '12/01/2026', '98,50'],
      [],                                       // ligne blanche, ignorée mais comptée
      ['CM9999999999', '31/02/2026', 'abc'],
    ], references)

    assert.deepEqual(r.commandes, [{
      titreId: 'id-titre-1', dateSeance: '2026-01-12', coursReference: null, coursCloture: 98.5, volume: null, couponCouruPublie: null,
    }])
    assert.equal(r.erreurs.length, 1)
    assert.equal(r.erreurs[0].ligne, 4)
    assert.equal(r.erreurs[0].messages.length, 3)
  })

  test('l’émetteur se reconnaît par son nom ou son sigle', () => {
    const entetes = ['ISIN', 'Nom', 'Émetteur', 'Nominal', 'Nombre de titres']
    const r = convertir(MODELES.actions, [entetes, ['CM0000000029', 'Société A', 'ecmr', '10 000', '1 000 000']], references)
    assert.equal(r.commandes[0].emetteurId, 'id-etat-cm')
    assert.equal(r.commandes[0].nombreTitres, 1000000)
  })

  test('une valeur par défaut remplace une cellule vide (différé = 0)', () => {
    const colonnes = MODELES.obligations.colonnes.map((c) => c.entete)
    const ligne = {
      ISIN: 'CM0000000037', Nom: 'ECMR 6,50 % 2025-2030', 'Émetteur': 'ECMR', Nominal: '10000', 'Taux facial': '6,50',
      'Périodicité': 'Annuelle', 'Date de jouissance': '30/06/2025', "Date d'échéance": '30/06/2030',
      Remboursement: 'In fine', Convention: 'Exact/365', 'Régime fiscal': 'Net',
    }
    const r = convertir(MODELES.obligations, [colonnes, colonnes.map((c) => ligne[c] ?? '')], references)
    assert.deepEqual(r.erreurs, [])
    assert.equal(r.commandes[0].differeEcheances, 0)
    assert.equal(r.commandes[0].modeRemboursement, 'InFine')
    assert.equal(r.commandes[0].codeConvention, 'EXACT_365')
  })

  test(`plus de ${LIGNES_MAXIMUM} lignes : refusé avant tout envoi`, () => {
    const lignes = Array.from({ length: LIGNES_MAXIMUM + 1 }, () => ['02/01/2026', 'Test'])
    assert.match(convertir(MODELES.joursNonOuvres, [['Date', 'Libellé'], ...lignes], references).erreurFichier, /500/)
  })

  test('les erreurs de l’API sont ramenées aux lignes du fichier', () => {
    assert.deepEqual(
      erreursServeurVersFichier([{ ligne: 2, messages: ['déjà saisi'] }], [2, 5, 6]),
      [{ ligne: 5, messages: ['déjà saisi'] }])
  })
})

describe('CSV', () => {
  test('séparateur « ; » d’Excel en français, avec BOM UTF-8', () => {
    const octets = new TextEncoder().encode('﻿Date;Libellé\r\n25/12/2026;Noël\r\n')
    assert.deepEqual(analyserCsv(decoderTexte(octets)), [['Date', 'Libellé'], ['25/12/2026', 'Noël']])
  })

  test('un CSV en Windows-1252 garde ses accents', () => {
    const octets = new Uint8Array([0x4e, 0x6f, 0xeb, 0x6c]) // « Noël » en Windows-1252
    assert.equal(decoderTexte(octets), 'Noël')
  })

  test('séparateur « , » et valeurs entre guillemets', () => {
    assert.deepEqual(analyserCsv('Date,Libellé\n25/12/2026,"Noël, jour férié"'), [['Date', 'Libellé'], ['25/12/2026', 'Noël, jour férié']])
  })
})

describe('Excel (.xlsx)', () => {
  test('un fichier écrit puis relu donne les mêmes commandes (dates et nombres d’Excel)', async () => {
    const tampon = await writeExcelFile([
      ['ISIN', 'Date de séance', 'Cours de référence', 'Volume'],
      ['CM0000000011', { value: new Date(Date.UTC(2026, 0, 12)), type: Date, format: 'dd/mm/yyyy' }, 98.5, 1200],
    ]).toBuffer()

    const tableau = await readXlsxSheet(tampon)
    const r = convertir(MODELES.cours, tableau, references)

    assert.deepEqual(r.erreurs, [])
    assert.deepEqual(r.commandes, [{
      titreId: 'id-titre-1', dateSeance: '2026-01-12', coursReference: 98.5, coursCloture: null, volume: 1200, couponCouruPublie: null,
    }])
  })
})

test('formulaire : même lecture qu’une ligne de fichier, erreurs par champ', async () => {
  const { lireLigne, preparerReferences } = await import('./conversion.js')
  const { MODELES } = await import('./modeles.js')
  const refs = preparerReferences([], [{ id: 'e-1', nom: 'État du Cameroun', sigle: 'ECMR' }])
  const saisie = {
    isin: 'CM0000020305', nom: 'ECMR 6,25% NET 2022-2029', emetteurId: 'e-1', nominal: '10 000', tauxFacial: '6,25',
    periodicite: 'Annuelle', dateJouissance: '2022-05-27', dateEcheance: '27/05/2029', modeRemboursement: 'AmortissementConstant',
    differeEcheances: '', codeConvention: 'EXACT_365', regimeFiscal: 'Net',
  }
  const { commande, erreurs } = lireLigne(MODELES.obligations, (c) => saisie[c], refs)
  assert.deepEqual(erreurs, [])
  assert.equal(commande.emetteurId, 'e-1')
  assert.equal(commande.nominal, 10000)
  assert.equal(commande.tauxFacial, 6.25)
  assert.equal(commande.dateEcheance, '2029-05-27')
  assert.equal(commande.differeEcheances, 0)
  assert.equal(commande.datePremierCoupon, null)

  const faux = lireLigne(MODELES.obligations, (c) => ({ ...saisie, tauxFacial: 'six', emetteurId: 'inconnu', nom: '' })[c], refs)
  assert.deepEqual(faux.erreurs.map((e) => e.champ).sort(), ['emetteurId', 'nom', 'tauxFacial'])
})
