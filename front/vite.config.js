import tailwindcss from '@tailwindcss/vite'
import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'
import serveurExemple from './exemple/serveurExemple.js'

// En développement, /api est relayé vers l'API ASP.NET Core (profil http de launchSettings.json).
// `npm run dev:exemple` : des réponses fixes à la place de l'API, pour voir l'interface sans base.
export default defineConfig(({ mode }) => ({
  plugins: [react(), tailwindcss(), ...(mode === 'exemple' ? [serveurExemple()] : [])],
  server: {
    proxy: mode === 'exemple' ? undefined : { '/api': 'http://localhost:5034' },
  },
}))
