import assert from 'node:assert/strict'
import { test } from 'node:test'
import { EN_TETE_IP, EN_TETE_SECRET, relayer, requeteVersApi } from './relais.js'

const ENV = { API_ORIGINE: 'https://api.exemple.test', RELAIS_SECRET: 'secret-de-test' }

test("la requête part vers l'API, même chemin et mêmes paramètres", () => {
  const r = requeteVersApi(new Request('https://site.pages.dev/api/titres/abc/cours?periode=1a'), ENV)
  assert.equal(r.url, 'https://api.exemple.test/api/titres/abc/cours?periode=1a')
  assert.equal(r.method, 'GET')
})

test("l'IP vient de Cloudflare et le secret est ajouté ; ce que le visiteur invente est retiré", () => {
  const r = requeteVersApi(
    new Request('https://site.pages.dev/api/titres', {
      headers: {
        'CF-Connecting-IP': '41.202.1.2',
        'X-Forwarded-For': '1.2.3.4',
        [EN_TETE_IP]: '5.6.7.8',
        [EN_TETE_SECRET]: 'devine',
        Cookie: 'cbc_session=jeton',
      },
    }),
    ENV,
  )
  assert.equal(r.headers.get(EN_TETE_IP), '41.202.1.2')
  assert.equal(r.headers.get(EN_TETE_SECRET), 'secret-de-test')
  assert.equal(r.headers.get('X-Forwarded-For'), null)
  assert.equal(r.headers.get('Cookie'), 'cbc_session=jeton')
})

test("une écriture garde sa méthode, son corps et l'en-tête CSRF", async () => {
  const r = requeteVersApi(
    new Request('https://site.pages.dev/api/admin/session', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json', 'X-CBC-Admin': '1' },
      body: JSON.stringify({ email: 'a@b.c' }),
    }),
    ENV,
  )
  assert.equal(r.method, 'POST')
  assert.equal(r.headers.get('X-CBC-Admin'), '1')
  assert.equal(await r.text(), '{"email":"a@b.c"}')
})

test('sans configuration : 503, rien ne part', async () => {
  let appele = false
  const reponse = await relayer(new Request('https://site.pages.dev/api/titres'), {}, () => {
    appele = true
  })
  assert.equal(reponse.status, 503)
  assert.equal(appele, false)
})

test("la réponse de l'API revient telle quelle, cookie compris ; une API injoignable donne 502", async () => {
  const apiRepond = async () =>
    new Response('{}', { status: 200, headers: { 'Set-Cookie': 'cbc_session=x; Path=/api/admin; Secure; HttpOnly; SameSite=Strict' } })
  const ok = await relayer(new Request('https://site.pages.dev/api/admin/session'), ENV, apiRepond)
  assert.equal(ok.status, 200)
  assert.match(ok.headers.get('Set-Cookie'), /SameSite=Strict/)

  const panne = await relayer(new Request('https://site.pages.dev/api/titres'), ENV, async () => {
    throw new Error('réseau')
  })
  assert.equal(panne.status, 502)
})
