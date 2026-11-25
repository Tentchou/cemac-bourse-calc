// Relais Cloudflare Pages → API (Render). Le site et l'API restent sur une seule adresse pour le
// navigateur : le cookie de session (SameSite=Strict, Path=/api/admin) et la protection CSRF marchent
// tels quels, sans ouvrir le CORS. Logique pure, testée dans relais.test.js ; branchée par
// functions/api/[[chemin]].js.

/** En-tête qui porte l'adresse du visiteur jusqu'à l'API (lu par ProxyInverse côté API). */
export const EN_TETE_IP = 'X-CBC-IP-Visiteur'

/** En-tête qui prouve à l'API que la requête vient du relais (secret partagé, jamais côté navigateur). */
export const EN_TETE_SECRET = 'X-CBC-Relais'

/** En-têtes du visiteur à ne jamais transmettre : ceux que lui seul pourrait inventer pour tromper l'API. */
const RETIRES = ['host', 'x-forwarded-for', 'x-forwarded-host', 'x-forwarded-proto', 'x-real-ip', EN_TETE_IP.toLowerCase(), EN_TETE_SECRET.toLowerCase()]

/**
 * Construit la requête vers l'API à partir de celle du visiteur.
 * @param {Request} requete la requête reçue par Cloudflare
 * @param {{ API_ORIGINE?: string, RELAIS_SECRET?: string }} env variables du projet Pages
 * @returns {Request | null} null si le relais n'est pas configuré
 */
export function requeteVersApi(requete, env) {
  if (!env.API_ORIGINE || !env.RELAIS_SECRET) return null

  const adresse = new URL(requete.url)
  const cible = new URL(adresse.pathname + adresse.search, env.API_ORIGINE)

  const enTetes = new Headers(requete.headers)
  for (const nom of RETIRES) enTetes.delete(nom)
  // Adresse du visiteur vue par Cloudflare (impossible à falsifier par le navigateur).
  const ip = requete.headers.get('CF-Connecting-IP')
  if (ip) enTetes.set(EN_TETE_IP, ip)
  enTetes.set(EN_TETE_SECRET, env.RELAIS_SECRET)

  const sansCorps = requete.method === 'GET' || requete.method === 'HEAD'
  return new Request(cible, {
    method: requete.method,
    headers: enTetes,
    body: sansCorps ? null : requete.body,
    redirect: 'manual',
    // Corps transmis en flux (envoi du PDF du bulletin) : exigé par fetch hors navigateur.
    ...(sansCorps ? {} : { duplex: 'half' }),
  })
}

/** Réponse du relais : celle de l'API, telle quelle (statut, cookies, corps). */
export async function relayer(requete, env, envoyer = fetch) {
  const versApi = requeteVersApi(requete, env)
  if (!versApi) {
    return Response.json({ title: 'Le service est mal configuré.', status: 503 }, { status: 503 })
  }
  try {
    return await envoyer(versApi)
  } catch {
    return Response.json({ title: 'Le service est indisponible pour le moment.', status: 502 }, { status: 502 })
  }
}
