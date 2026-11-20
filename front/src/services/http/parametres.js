/** Retire les paramètres vides (undefined, null, '') avant de construire l'adresse. */
export function parametresUtiles(parametres = {}) {
  return Object.fromEntries(Object.entries(parametres ?? {}).filter(([, v]) => v !== undefined && v !== null && v !== ''))
}
