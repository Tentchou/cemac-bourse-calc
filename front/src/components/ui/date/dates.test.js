import assert from 'node:assert/strict'
import { test } from 'node:test'
import {
  ajouterJours,
  ajouterMois,
  aujourdhui,
  borner,
  dansBornes,
  estWeekend,
  formaterSaisie,
  grilleMois,
  jourSemaine,
  joursDansMois,
  lireIso,
  lireSaisie,
  masquerSaisie,
  versIso,
} from './dates.js'

test('jours dans le mois, années bissextiles comprises', () => {
  assert.equal(joursDansMois(2026, 2), 28)
  assert.equal(joursDansMois(2028, 2), 29)
  assert.equal(joursDansMois(2100, 2), 28)
  assert.equal(joursDansMois(2026, 12), 31)
})

test('une date qui n’existe pas est refusée', () => {
  assert.equal(versIso(2026, 2, 30), null)
  assert.equal(versIso(2026, 13, 1), null)
  assert.equal(lireIso('2026-02-29'), null)
  assert.deepEqual(lireIso('2028-02-29'), { a: 2028, m: 2, j: 29 })
})

test('ajout de jours et de mois, fin de mois ramenée', () => {
  assert.equal(ajouterJours('2026-12-31', 1), '2027-01-01')
  assert.equal(ajouterJours('2026-03-01', -1), '2026-02-28')
  assert.equal(ajouterMois('2026-01-31', 1), '2026-02-28')
  assert.equal(ajouterMois('2026-01-15', -1), '2025-12-15')
  assert.equal(ajouterMois('2026-06-30', 12), '2027-06-30')
})

test('la semaine commence le lundi ; samedi et dimanche sont des week-ends', () => {
  assert.equal(jourSemaine('2026-01-12'), 0) // lundi
  assert.equal(jourSemaine('2026-01-18'), 6) // dimanche
  assert.equal(estWeekend('2026-01-17'), true)
  assert.equal(estWeekend('2026-01-16'), false)
})

test('grille d’un mois : 42 cases, lundi en tête', () => {
  const g = grilleMois(2026, 1)
  assert.equal(g.length, 42)
  assert.equal(g[0].iso, '2025-12-29') // le 1er janvier 2026 est un jeudi
  assert.equal(g[0].horsMois, true)
  assert.equal(g[3].iso, '2026-01-01')
  assert.equal(g[3].horsMois, false)
  assert.ok(g.every((c, i) => jourSemaine(c.iso) === i % 7))
})

test('bornes', () => {
  assert.equal(dansBornes('2026-01-12', '2025-06-30', '2030-06-30'), true)
  assert.equal(dansBornes('2031-01-01', null, '2030-06-30'), false)
  assert.equal(borner('2020-01-01', '2025-06-30', null), '2025-06-30')
  assert.equal(borner('2026-01-12', undefined, undefined), '2026-01-12')
})

test('aujourd’hui dans le fuseau du visiteur', () => {
  assert.equal(aujourdhui(new Date(2026, 0, 5, 23, 30)), '2026-01-05')
})

test('saisie : les barres se posent toutes seules', () => {
  assert.equal(masquerSaisie('12'), '12')
  assert.equal(masquerSaisie('120'), '12/0')
  assert.equal(masquerSaisie('12012026'), '12/01/2026')
  assert.equal(masquerSaisie('120120261'), '12/01/2026')
  assert.equal(masquerSaisie('1/'), '01/')
  assert.equal(masquerSaisie('12/1/'), '12/01/')
  assert.equal(masquerSaisie('12.01.2026'), '12/01/2026')
  assert.equal(masquerSaisie('ab'), '')
})

test('saisie : lecture des formats courants', () => {
  assert.equal(lireSaisie('12/01/2026'), '2026-01-12')
  assert.equal(lireSaisie('1/2/2026'), '2026-02-01')
  assert.equal(lireSaisie('12012026'), '2026-01-12')
  assert.equal(lireSaisie('2026-01-12'), '2026-01-12')
  assert.equal(lireSaisie('31/02/2026'), null)
  assert.equal(lireSaisie('12/01/26'), null)
  assert.equal(formaterSaisie('2026-01-12'), '12/01/2026')
  assert.equal(formaterSaisie(''), '')
})

test('jour de séance : le week-end recule au vendredi, ou avance au lundi si besoin', async () => {
  const { jourDeSeance } = await import('./dates.js')
  assert.equal(jourDeSeance('2026-01-17'), '2026-01-16') // samedi → vendredi
  assert.equal(jourDeSeance('2026-01-18'), '2026-01-16') // dimanche → vendredi
  assert.equal(jourDeSeance('2026-01-14'), '2026-01-14')
  assert.equal(jourDeSeance('2026-01-18', '2026-01-17'), '2026-01-19') // vendredi avant la jouissance → lundi
  assert.equal(jourDeSeance('2031-01-01', null, '2030-06-30'), '2030-06-28') // échéance un dimanche → vendredi
})
