import assert from 'node:assert/strict'
import { test } from 'node:test'
import { estPassagere, normaliserErreur, TYPES_ERREUR } from './ErreurApi.js'
import { parametresUtiles } from './parametres.js'

const reponse = (status, data = null) => ({ response: { status, data } })

test('annulation : reconnue, jamais affichée', () => {
  assert.equal(normaliserErreur({ code: 'ERR_CANCELED', name: 'CanceledError' }).estAnnulee, true)
  assert.equal(normaliserErreur(new DOMException('x', 'AbortError')).estAnnulee, true)
})

test('sans réponse : réseau ou délai dépassé', () => {
  assert.equal(normaliserErreur({ code: 'ERR_NETWORK' }).type, TYPES_ERREUR.reseau)
  assert.equal(normaliserErreur({ code: 'ECONNABORTED' }).type, TYPES_ERREUR.delai)
  assert.equal(normaliserErreur({ code: 'ECONNABORTED' }).cle, 'erreur.delai')
})

test('400 : le message de l’API est gardé, avec les erreurs par champ', () => {
  const e = normaliserErreur(reponse(400, { title: 'La date est avant la jouissance.', errors: { cours: ['> 0'] } }))
  assert.equal(e.type, TYPES_ERREUR.refus)
  assert.equal(e.message, 'La date est avant la jouissance.')
  assert.equal(e.cle, null)
  assert.deepEqual(e.details, { cours: ['> 0'] })
})

test('400 sans message : texte générique traduit', () => {
  assert.equal(normaliserErreur(reponse(400, '<html>')).cle, 'erreur.refus')
})

test('404, 429, 500', () => {
  assert.equal(normaliserErreur(reponse(404, { title: 'Introuvable' })).type, TYPES_ERREUR.introuvable)
  assert.equal(normaliserErreur(reponse(429)).cle, 'erreur.limite')
  assert.equal(normaliserErreur(reponse(500)).cle, 'erreur.serveur')
})

test('seules les pannes passagères sont réessayées', () => {
  assert.equal(estPassagere(normaliserErreur({ code: 'ERR_NETWORK' })), true)
  assert.equal(estPassagere(normaliserErreur(reponse(503))), true)
  assert.equal(estPassagere(normaliserErreur(reponse(500))), false)
  assert.equal(estPassagere(normaliserErreur(reponse(400))), false)
  assert.equal(estPassagere(normaliserErreur(reponse(429))), false)
})

test('paramètres vides retirés de l’adresse', () => {
  assert.deepEqual(parametresUtiles({ a: 1, b: '', c: null, d: undefined, e: 0 }), { a: 1, e: 0 })
})
