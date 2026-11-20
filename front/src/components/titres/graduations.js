/**
 * Graduations « rondes » d'un axe (pas de 1, 2, 2,5 ou 5 × 10^n, environ 5 graduations).
 * Rend le haut de l'axe (multiple du pas, ≥ maximum) et le pas. Fonction pure, testée.
 */
export function graduations(maximum) {
  if (!(maximum > 0)) return { haut: 1, pas: 1 }
  const brut = maximum / 5
  const puissance = 10 ** Math.floor(Math.log10(brut))
  const norme = brut / puissance
  const pas = (norme <= 1 ? 1 : norme <= 2 ? 2 : norme <= 2.5 ? 2.5 : norme <= 5 ? 5 : 10) * puissance
  return { haut: Math.ceil(maximum / pas) * pas, pas }
}
