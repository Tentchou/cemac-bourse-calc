import assert from 'node:assert/strict'
import { test } from 'node:test'
import { etatPage, lirePage, lireTaille, pagesVisibles, paginer } from './paginer.js'

const quarante = Array.from({ length: 40 }, (_, i) => i + 1)

test('découpe la page demandée et donne la plage affichée', () => {
  const p = paginer(quarante, 2, 10)
  assert.deepEqual(p.lignes, [11, 12, 13, 14, 15, 16, 17, 18, 19, 20])
  assert.deepEqual({ page: p.page, pages: p.pages, total: p.total, debut: p.debut, fin: p.fin }, { page: 2, pages: 4, total: 40, debut: 11, fin: 20 })
})

test('dernière page incomplète', () => {
  const p = paginer(quarante, 3, 15)
  assert.equal(p.lignes.length, 10)
  assert.deepEqual([p.debut, p.fin, p.pages], [31, 40, 3])
})

test('page hors limites ramenée dans la liste', () => {
  assert.equal(paginer(quarante, 99, 10).page, 4)
  assert.equal(paginer(quarante, 0, 10).page, 1)
  assert.equal(paginer(quarante, -3, 10).page, 1)
})

test('liste vide : une page, plage 0–0', () => {
  const p = paginer([], 5, 10)
  assert.deepEqual([p.page, p.pages, p.debut, p.fin, p.lignes.length], [1, 1, 0, 0, 0])
})

test('page lue de l’adresse : entier positif seulement', () => {
  assert.equal(lirePage('3'), 3)
  assert.equal(lirePage(' 2 '), 2)
  for (const mauvais of [null, undefined, '', 'abc', '-1', '1.5', '1e3', '0x10', '9999999', {}, '2; DROP']) {
    assert.equal(lirePage(mauvais), 1, `valeur ${String(mauvais)}`)
  }
  assert.equal(lirePage('0'), 1)
})

test('taille lue du stockage : seulement 10, 20 ou 50', () => {
  assert.equal(lireTaille('20'), 20)
  assert.equal(lireTaille(50), 50)
  for (const mauvais of [null, '', '15', '1000000', '-10', 'abc']) assert.equal(lireTaille(mauvais), 10)
})

test('boutons de page : tout afficher jusqu’à 7 pages', () => {
  assert.deepEqual(pagesVisibles(1, 1), [1])
  assert.deepEqual(pagesVisibles(2, 4), [1, 2, 3, 4])
  assert.deepEqual(pagesVisibles(7, 7), [1, 2, 3, 4, 5, 6, 7])
})

test('boutons de page : au-delà, première, dernière, voisines et « … »', () => {
  assert.deepEqual(pagesVisibles(1, 12), [1, 2, 3, 4, 5, '…', 12])
  assert.deepEqual(pagesVisibles(4, 12), [1, 2, 3, 4, 5, '…', 12])
  assert.deepEqual(pagesVisibles(6, 12), [1, '…', 5, 6, 7, '…', 12])
  assert.deepEqual(pagesVisibles(9, 12), [1, '…', 8, 9, 10, 11, 12])
  assert.deepEqual(pagesVisibles(12, 12), [1, '…', 8, 9, 10, 11, 12])
})

test('boutons de page : jamais plus de 7, la courante toujours présente', () => {
  for (let pages = 1; pages <= 60; pages++) {
    for (let page = 1; page <= pages; page++) {
      const v = pagesVisibles(page, pages)
      assert.ok(v.length <= 7, `${page}/${pages}`)
      assert.ok(v.includes(page), `${page}/${pages}`)
      assert.equal(v[0], 1)
      assert.equal(v.at(-1), pages)
    }
  }
})

test('page lue sur le serveur : plage et nombre de pages', () => {
  assert.deepEqual(etatPage(33, 4, 10, 3), { page: 4, pages: 4, total: 33, debut: 31, fin: 33 })
  assert.deepEqual(etatPage(33, 1, 50, 33), { page: 1, pages: 1, total: 33, debut: 1, fin: 33 })
  assert.deepEqual(etatPage(0, 1, 10, 0), { page: 1, pages: 1, total: 0, debut: 0, fin: 0 })
})
