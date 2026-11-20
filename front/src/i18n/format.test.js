import assert from 'node:assert/strict'
import { test } from 'node:test'
import { date, lireNombreSaisi, montant, pourcent, remplir } from './format.js'
import { en, fr } from './textes.js'

const espaces = (s) => s.replace(/[  ]/g, ' ')

test('montant en FCFA (français) et en XAF (anglais)', () => {
  assert.equal(espaces(montant('fr', 102043.84)), '102 043,84 FCFA')
  assert.equal(espaces(montant('en', 102043.84)), '102,043.84 XAF')
  assert.equal(montant('fr', null), '—')
})

test('séparateur de milliers : espace insécable classique (présente dans toutes les polices)', () => {
  assert.equal(montant('fr', 102043.84), '102 043,84 FCFA')
})

test('pourcentage', () => {
  assert.equal(espaces(pourcent('fr', 6.8836)), '6,88 %')
  assert.equal(pourcent('en', 6.8836, 4), '6.8836%')
})

test('date jj/mm/aaaa dans les deux langues', () => {
  assert.equal(date('fr', '2026-01-15'), '15/01/2026')
  assert.equal(date('en', '2026-01-15'), '15/01/2026')
})

test('lecture d’un nombre saisi', () => {
  assert.equal(lireNombreSaisi('98,50'), 98.5)
  assert.equal(lireNombreSaisi('32 000'), 32000)
  assert.equal(lireNombreSaisi('98.5'), 98.5)
  assert.equal(lireNombreSaisi('-1'), null)
  assert.equal(lireNombreSaisi('abc'), null)
})

test('remplir les variables', () => {
  assert.equal(remplir('T+{n} le {date}', { n: 3, date: 'jeudi' }), 'T+3 le jeudi')
})

test('chaque texte français a sa traduction anglaise, et inversement', () => {
  assert.deepEqual(Object.keys(fr).filter((k) => !(k in en)), [])
  assert.deepEqual(Object.keys(en).filter((k) => !(k in fr)), [])
})
