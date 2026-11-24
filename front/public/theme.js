// Thème appliqué avant le premier affichage (pas de flash clair → sombre).
try {
  var t = localStorage.getItem('cbc-theme')
  if (t !== 'clair' && t !== 'sombre') t = matchMedia('(prefers-color-scheme: dark)').matches ? 'sombre' : 'clair'
  document.documentElement.dataset.theme = t
} catch (e) {}
