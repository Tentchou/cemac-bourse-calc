import assert from 'node:assert/strict'
import { test } from 'node:test'
import { commandesCours, filtrerBulletin, resumerBulletin } from './bulletin.js'

const identique = { identique: true, ecart: 0 }
const ecart = { identique: false, ecart: 0.01 }
const lignes = [
  { isin: 'CM0000020305', titreId: 'a', coursCloture: 100, volume: 0, couponCouruPublie: 186.99, controle: identique, probleme: null },
  { isin: 'GA0000020503', titreId: 'b', coursCloture: 100, volume: 0, couponCouruPublie: 495.21, controle: ecart, probleme: null },
  { isin: 'CM0000010017', titreId: 'c', coursCloture: 38500, volume: 11, couponCouruPublie: null, controle: null, probleme: null },
  { isin: 'GA0000020677', titreId: null, coursCloture: 100, volume: 0, couponCouruPublie: 12.33, controle: null, probleme: 'Titre inconnu' },
]

test('résumé : lus, contrôlés, identiques, écarts, problèmes, transactions', () => {
  assert.deepEqual(resumerBulletin({ lignes }), { lus: 4, controles: 2, identiques: 1, ecarts: 1, problemes: 1, transactions: 1 })
  assert.deepEqual(resumerBulletin(null), { lus: 0, controles: 0, identiques: 0, ecarts: 0, problemes: 0, transactions: 0 })
})

test('filtres du tableau', () => {
  assert.equal(filtrerBulletin(lignes, 'tout').length, 4)
  assert.deepEqual(filtrerBulletin(lignes, 'ecarts').map((l) => l.isin), ['GA0000020503'])
  assert.deepEqual(filtrerBulletin(lignes, 'problemes').map((l) => l.isin), ['GA0000020677'])
  assert.deepEqual(filtrerBulletin(lignes, 'transactions').map((l) => l.isin), ['CM0000010017'])
})

test('commandes : seulement si le bulletin est importable, au format de l’import en masse', () => {
  assert.equal(commandesCours({ importable: false, lignes }), null)
  const ok = lignes.slice(0, 3)
  assert.deepEqual(commandesCours({ importable: true, dateSeance: '2026-11-20', lignes: ok })[2], {
    titreId: 'c',
    dateSeance: '2026-11-20',
    coursReference: null,
    coursCloture: 38500,
    volume: 11,
    couponCouruPublie: null,
  })
})
