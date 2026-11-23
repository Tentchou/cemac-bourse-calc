import assert from 'node:assert/strict'
import { test } from 'node:test'
import {
  OBLIGATION,
  arrondir,
  exempleCouponCouru,
  exempleDividende,
  exempleEcheancier,
  exemplePrixPlein,
  exempleRendement,
  joursConvention,
  nominalRestant,
} from './exemples.js'

// Les valeurs attendues sont celles du bulletin n° 2643 et de l'API pour l'ECMR 6,25 % 2022-2029
// au 20/11/2026 (règlement le 25/11/2026), 100 titres au cours de 100 %.

test('arrondir : au centime, 0,005 monte', () => {
  assert.equal(arrondir(1.005), 1.01)
  assert.equal(arrondir(16027.397), 16027.4)
  assert.equal(arrondir(-2.345), -2.35)
})

test("échéancier ECMR : deux coupons seuls, puis 5 × 2 000 ; coupons de la fiche de l'émetteur", () => {
  const lignes = exempleEcheancier()
  assert.deepEqual(lignes.map((l) => l.coupon), [625, 625, 625, 500, 375, 250, 125])
  assert.deepEqual(lignes.map((l) => l.capital), [0, 0, 2000, 2000, 2000, 2000, 2000])
  assert.equal(lignes.at(-1).date, '2029-05-27')
  assert.equal(lignes.reduce((s, l) => s + l.capital, 0), 10000)
  assert.equal(nominalRestant(), 6000)
})

test('coupon couru : 6 000 × 6,25 % × 182 ÷ 365, identique au chiffre publié par la BVMAC', () => {
  const { jours, parTitre, total } = exempleCouponCouru()
  assert.equal(jours, 182)
  assert.equal(arrondir(parTitre), OBLIGATION.couruPublie)
  assert.equal(total, 18698.63)
})

test("prix plein : pied de coupon 600 000, total 618 698,63 (comme l'API)", () => {
  const r = exemplePrixPlein()
  assert.equal(r.piedParTitre, 6000)
  assert.equal(r.pied, 600000)
  assert.equal(r.couruTotal, 18698.63)
  assert.equal(r.total, 618698.63)
})

test("rendements : courant 6,25 %, actuariel 6,208 % (comme l'API), trois flux restants", () => {
  const r = exempleRendement()
  assert.equal(r.prix, 6186.99)
  assert.equal(arrondir(r.courant), 6.25)
  assert.equal(Math.round(r.actuariel * 10000) / 10000, 6.208)
  assert.deepEqual(r.flux.map((f) => f.jours), [183, 549, 914])
  assert.deepEqual(r.flux.map((f) => f.montant), [2375, 2250, 2125])
  const somme = r.flux.reduce((s, f) => s + f.valeurActuelle, 0)
  assert.ok(Math.abs(somme - r.prix) < 1e-6)
})

test('dividende SAFACAM : 2 200 ÷ 38 500 = 5,71 %', () => {
  const r = exempleDividende()
  assert.equal(r.dividende, 2200)
  assert.equal(arrondir(r.rendement), 5.71)
})

test('conventions : 31/01 → 31/03/2026', () => {
  assert.equal(joursConvention('EXACT_365', '2026-01-31', '2026-03-31'), 59)
  assert.equal(joursConvention('30E_360', '2026-01-31', '2026-03-31'), 60)
  assert.equal(joursConvention('30E_360', '2026-02-28', '2026-03-31'), 32)
})
