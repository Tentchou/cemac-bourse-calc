import assert from 'node:assert/strict'
import { test } from 'node:test'
import {
  axeCours,
  filtrerPeriode,
  indiceProche,
  indicesDates,
  lignesTableau,
  lirePeriode,
  moinsMois,
  PERIODE_DEFAUT,
  reperes,
  resumer,
  variationPourcent,
} from './historique.js'

const p = (dateSeance, cours, volume = 0) => ({ dateSeance, cours, volume })

test('moins n mois : jour ramené à la fin du mois, changement d’année juste', () => {
  assert.equal(moinsMois('2026-11-20', 1), '2026-10-20')
  assert.equal(moinsMois('2026-05-31', 3), '2026-02-28')
  assert.equal(moinsMois('2024-05-31', 3), '2024-02-29')
  assert.equal(moinsMois('2026-01-15', 1), '2025-12-15')
  assert.equal(moinsMois('2026-11-20', 12), '2025-11-20')
})

test('la période est comptée depuis la dernière séance, bornes incluses', () => {
  const points = [p('2025-11-19', 1), p('2025-11-20', 2), p('2026-08-20', 3), p('2026-10-20', 4), p('2026-11-20', 5)]
  assert.deepEqual(filtrerPeriode(points, '1m').map((x) => x.cours), [4, 5])
  assert.deepEqual(filtrerPeriode(points, '3m').map((x) => x.cours), [3, 4, 5])
  assert.deepEqual(filtrerPeriode(points, '1a').map((x) => x.cours), [2, 3, 4, 5])
  assert.equal(filtrerPeriode(points, 'tout'), points)
  assert.deepEqual(filtrerPeriode([], '1m'), [])
})

test('une période inconnue (stockage modifié) retombe sur la période par défaut', () => {
  assert.equal(lirePeriode('6m'), '6m')
  assert.equal(lirePeriode('<script>'), PERIODE_DEFAUT)
  assert.equal(lirePeriode(null), PERIODE_DEFAUT)
})

test('le résumé : variation, plus haut et plus bas datés, volume des séances avec échanges', () => {
  const r = resumer([p('2026-11-17', 100, 0), p('2026-11-18', 101.5, 20), p('2026-11-19', 99, 5), p('2026-11-20', 100.5, 0)])
  assert.equal(r.variation, 0.5)
  assert.equal(r.variationPourcent, 0.5)
  assert.deepEqual([r.haut.dateSeance, r.bas.dateSeance], ['2026-11-18', '2026-11-19'])
  assert.deepEqual([r.volume, r.seances, r.seancesEchange], [25, 4, 2])
  assert.equal(resumer([]), null)
})

test('variation : null sans cours de départ valable', () => {
  assert.equal(variationPourcent(0, 10), null)
  assert.equal(variationPourcent(100, null), null)
  assert.equal(variationPourcent(32000, 32800), 2.5)
})

test('le tableau : la plus récente en premier, comparée à la séance précédente', () => {
  const lignes = lignesTableau([p('2026-11-18', 100), p('2026-11-19', 102), p('2026-11-20', 102)])
  assert.deepEqual(lignes.map((l) => l.dateSeance), ['2026-11-20', '2026-11-19', '2026-11-18'])
  assert.deepEqual(lignes.map((l) => l.variationPourcent), [0, 2, null])
})

test('l’axe des cours encadre les valeurs avec un pas rond', () => {
  assert.deepEqual(axeCours(98.4, 101.2), { bas: 98, haut: 102, pas: 1 })
  assert.deepEqual(axeCours(31250, 33900), { bas: 31000, haut: 34000, pas: 1000 })
  const constant = axeCours(100, 100)
  assert.ok(constant.bas < 100 && constant.haut > 100)
  assert.deepEqual(reperes({ bas: 99.6, haut: 100.4, pas: 0.2 }), [99.6, 99.8, 100, 100.2, 100.4])
})

test('les dates sous l’axe : la première et la dernière toujours écrites', () => {
  assert.deepEqual(indicesDates(1, 5), [0])
  assert.deepEqual(indicesDates(3, 5), [0, 1, 2])
  assert.deepEqual(indicesDates(250, 5), [0, 62, 125, 187, 249])
})

test('le point le plus proche du pointeur, borné aux séances', () => {
  assert.equal(indiceProche(56, 56, 10, 30), 0)
  assert.equal(indiceProche(84, 56, 10, 30), 3)
  assert.equal(indiceProche(-50, 56, 10, 30), 0)
  assert.equal(indiceProche(9999, 56, 10, 30), 29)
})
