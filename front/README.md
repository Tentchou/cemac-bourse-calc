# CEMAC Bourse Calc — front

Site public et administration, en React 19 (JSX), Vite et Tailwind CSS 4.
Présentation du projet, installation et architecture : voir le [README principal](../README.md).

```bash
npm install
npm run dev          # http://localhost:5173, /api relayé vers l'API locale
npm run dev:exemple  # sans API ni base, avec des données d'exemple
npm test             # tests (node --test)
npm run lint         # oxlint
npm run build        # dist/
```

| Dossier | Contenu |
|---|---|
| `src/pages/public` | Accueil, fiche d'un titre, méthodes |
| `src/pages/admin`, `src/components/admin`, `src/admin` | Administration (chargée à part, sous `/admin`) |
| `src/components/ui` | Composants de la charte : boutons, champs, tableau, pagination, panneau… |
| `src/i18n` | Textes français et anglais, formats des nombres et des dates |
