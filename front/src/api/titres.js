// Appels à l'API des titres. Un seul endroit pour les adresses.

export async function listerTitres() {
  const reponse = await fetch('/api/titres')
  if (reponse.status === 429) throw new Error('Trop de requêtes : réessayez dans une minute.')
  if (!reponse.ok) throw new Error('La liste des titres est indisponible pour le moment.')
  return reponse.json()
}
