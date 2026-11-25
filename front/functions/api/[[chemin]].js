// Cloudflare Pages Function : toute requête /api/... est relayée vers l'API (voir relais/relais.js).
// Variables du projet Pages : API_ORIGINE (ex. https://….onrender.com) et le secret RELAIS_SECRET.
import { relayer } from '../../relais/relais.js'

export const onRequest = ({ request, env }) => relayer(request, env)
