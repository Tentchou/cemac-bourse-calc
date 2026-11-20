import assert from 'node:assert/strict'
import { test } from 'node:test'
import { graduations } from './graduations.js'

test('graduations rondes au-dessus du maximum', () => {
  assert.deepEqual(graduations(106500), { haut: 125000, pas: 25000 })
  assert.deepEqual(graduations(6500), { haut: 8000, pas: 2000 })
  assert.deepEqual(graduations(100), { haut: 100, pas: 20 })
})

test('maximum nul ou invalide : un axe minimal', () => {
  assert.deepEqual(graduations(0), { haut: 1, pas: 1 })
  assert.deepEqual(graduations(Number.NaN), { haut: 1, pas: 1 })
})
