import assert from 'node:assert/strict'
import { describe, test } from 'node:test'
import { lireChoix, lireDate, lireEntier, lireNombre, normaliser } from './valeurs.js'

describe('nombres', () => {
  for (const [saisie, attendu] of [
    ['98,50', 98.5], ['98.50', 98.5], ['10 000', 10000], ['10 000', 10000], ['1 234 567,89', 1234567.89],
    ['1.234.567,89', 1234567.89], ['1,234,567.89', 1234567.89], ['6,5 %', 6.5], [6.5, 6.5], ['-2', -2],
  ]) {
    test(`« ${saisie} » vaut ${attendu}`, () => assert.equal(lireNombre(saisie).valeur, attendu))
  }

  for (const saisie of ['abc', '12,3,4', '1.2.3', '', '9,85 FCFA']) {
    test(`« ${saisie} » est refusé`, () => assert.ok(lireNombre(saisie).erreur))
  }

  test('un entier refuse les décimales', () => {
    assert.equal(lireEntier('1 500').valeur, 1500)
    assert.ok(lireEntier('2,5').erreur)
  })
})

describe('dates', () => {
  test('jj/mm/aaaa, jj-mm-aaaa, jj.mm.aaaa et aaaa-mm-jj', () => {
    for (const saisie of ['15/01/2026', '15-01-2026', '15.01.2026', '2026-01-15', '2026-1-15'])
      assert.equal(lireDate(saisie).valeur, '2026-01-15')
  })

  test('une cellule Date d’Excel (en UTC)', () => {
    assert.equal(lireDate(new Date(Date.UTC(2026, 0, 15))).valeur, '2026-01-15')
  })

  test('le jour avant le mois, jamais l’inverse', () => {
    assert.equal(lireDate('02/03/2026').valeur, '2026-03-02')
  })

  test('les dates impossibles sont refusées', () => {
    for (const saisie of ['31/02/2026', '29/02/2027', '15/13/2026', '2026/01/15', 'demain'])
      assert.ok(lireDate(saisie).erreur, saisie)
  })
})

describe('choix', () => {
  const choix = [
    { valeur: 'InFine', libelle: 'In fine' },
    { valeur: 'AmortissementConstant', libelle: 'Amortissement constant', alias: ['Amortissable'] },
  ]

  test('la valeur, le libellé ou un alias, sans tenir compte des accents ni de la casse', () => {
    assert.equal(lireChoix('in fine', choix).valeur, 'InFine')
    assert.equal(lireChoix('INFINE', choix).valeur, 'InFine')
    assert.equal(lireChoix('amortissable', choix).valeur, 'AmortissementConstant')
  })

  test('un choix inconnu liste les choix possibles', () => {
    assert.match(lireChoix('mensuel', choix).erreur, /In fine, Amortissement constant/)
  })

  test('normaliser enlève accents, espaces et ponctuation', () => {
    assert.equal(normaliser("Date d'échéance"), 'datedecheance')
  })
})
