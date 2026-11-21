import assert from 'node:assert/strict'
import { test } from 'node:test'
import { parametresListe } from './admin.js'

test('paramètres de liste : valeurs vides retirées, texte encodé', () => {
  assert.equal(parametresListe({ page: 2, taille: 20, recherche: '', pays: null, type: undefined }), 'page=2&taille=20')
  assert.equal(parametresListe({ recherche: ' 6,25% & co ' }), 'recherche=6%2C25%25+%26+co')
})
