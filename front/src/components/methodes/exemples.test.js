import assert from 'node:assert/strict'
import { test } from 'node:test'
import {
  arrondir,
  exempleCouponCouru,
  exempleDividende,
  exempleEcheancier,
  exemplePrixPlein,
  exempleRendement,
  joursConvention,
} from './exemples.js'

test('arrondir : au centime, 0,005 monte', () => {
  assert.equal(arrondir(1.005), 1.01)
  assert.equal(arrondir(16027.397), 16027.4)
  assert.equal(arrondir(-2.345), -2.35)
})

test('coupon couru : 10 000 × 6,50 % × 90 ÷ 365', () => {
  const { parTitre, total } = exempleCouponCouru()
  assert.equal(arrondir(parTitre), 160.27)
  assert.equal(total, 16027.4)
})

test('prix plein : pied de coupon 9 850, total 1 001 027,40', () => {
  const r = exemplePrixPlein()
  assert.equal(r.piedParTitre, 9850)
  assert.equal(arrondir(r.pleinParTitre), 10010.27)
  assert.equal(r.total, 1001027.4)
})

test('rendements : courant 6,60 %, actuariel 7,42 %, les valeurs actuelles redonnent le prix', () => {
  const r = exempleRendement()
  assert.equal(arrondir(r.courant), 6.6)
  assert.equal(arrondir(r.actuariel), 7.42)
  const somme = r.flux.reduce((s, f) => s + f.valeurActuelle, 0)
  assert.ok(Math.abs(somme - r.prix) < 1e-6)
  assert.deepEqual(r.flux.map((f) => f.jours), [275, 640])
})

test('échéancier : amortissement constant, la somme du capital fait le nominal', () => {
  const lignes = exempleEcheancier()
  assert.deepEqual(lignes.map((l) => l.coupon), [650, 487.5, 325, 162.5])
  assert.equal(lignes.reduce((s, l) => s + l.capital, 0), 10000)
  assert.equal(exempleEcheancier({ nominal: 10000, taux: 6.5 }, 3).at(-1).capital, 3333.34)
})

test('dividende : 1 500 ÷ 25 000 = 6 %', () => {
  assert.deepEqual(exempleDividende(), { dividende: 1500, rendement: 6 })
})

test('conventions : 31/01 → 31/03/2026', () => {
  assert.equal(joursConvention('EXACT_365', '2026-01-31', '2026-03-31'), 59)
  assert.equal(joursConvention('30E_360', '2026-01-31', '2026-03-31'), 60)
  assert.equal(joursConvention('30E_360', '2026-02-28', '2026-03-31'), 32)
})
