// Compte les requêtes en cours, pour la fine barre de progression en haut de page.
// Petit magasin observable, lu par React avec useSyncExternalStore.

let enCours = 0
const abonnes = new Set()

const prevenir = () => abonnes.forEach((f) => f())

export const suiviChargement = {
  debut() {
    enCours += 1
    prevenir()
  },
  fin() {
    enCours = Math.max(0, enCours - 1)
    prevenir()
  },
  lire: () => enCours,
  abonner(f) {
    abonnes.add(f)
    return () => abonnes.delete(f)
  },
}
