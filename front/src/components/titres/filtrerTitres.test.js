import assert from 'node:assert/strict'
import { test } from 'node:test'
import { compterTitres, filtrerTitres } from './filtrerTitres.js'

const titres = [
  { nom: 'État du Cameroun 6,50 %', emetteur: 'Émetteur A', isin: 'CM0000000011', typeTitre: 'Obligation', pays: 'CM' },
  { nom: 'Gabon 6 %', emetteur: 'Émetteur B', isin: 'GA0000000020', typeTitre: 'Obligation', pays: 'GA' },
  { nom: 'Société Exemple', emetteur: 'Société Exemple', isin: 'CM0000000045', typeTitre: 'Action', pays: 'CM' },
]

test('recherche sans tenir compte des accents ni de la casse', () => {
  assert.equal(filtrerTitres(titres, { texte: 'etat cameroun' }).length, 1)
  assert.equal(filtrerTitres(titres, { texte: 'ga00' })[0].pays, 'GA')
})

test('filtre par type et par pays', () => {
  assert.equal(filtrerTitres(titres, { type: 'Action' }).length, 1)
  assert.equal(filtrerTitres(titres, { pays: 'CM' }).length, 2)
  assert.equal(filtrerTitres(titres, { type: 'Obligation', pays: 'CM' }).length, 1)
})

test('chiffres clés', () => {
  assert.deepEqual(compterTitres(titres), { obligations: 2, actions: 1, pays: ['CM', 'GA'] })
})
