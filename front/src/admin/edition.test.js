import assert from 'node:assert/strict'
import { test } from 'node:test'
import { lireLigne, preparerReferences } from './import/conversion.js'
import { MODELES } from './import/modeles.js'
import { aUnHistorique, commandeModification, valeursFormulaire } from './edition.js'

const EMETTEUR = '01a1fecc-0000-7000-8000-000000000001'
const titre = {
  id: '01a1fecc-27cd-7e87-8dcd-621937963201',
  typeTitre: 'Obligation',
  isin: 'CM0000000011',
  nom: 'ECMR 6,25 % 2022-2029',
  emetteurId: EMETTEUR,
  nominal: 10000,
  dateAdmission: null,
  statut: 'Actif',
  tauxFacial: 6.25,
  periodicite: 'Annuelle',
  dateJouissance: '2022-05-27',
  datePremierCoupon: null,
  dateEcheance: '2029-05-27',
  modeRemboursement: 'AmortissementConstant',
  differeEcheances: 2,
  codeConvention: 'EXACT_365',
  regimeFiscal: 'Net',
  nombreTitres: null,
  nombreCours: 1,
  nombreDividendes: 0,
  nombreDocuments: 0,
}

test('le formulaire est prérempli comme une saisie : décimales à la française, champs vides pour null', () => {
  const v = valeursFormulaire(MODELES.obligations, titre, 'fr')
  assert.equal(v.tauxFacial, '6,25')
  assert.equal(v.nominal, '10000')
  assert.equal(v.differeEcheances, '2')
  assert.equal(v.dateAdmission, '')
  assert.equal(v.datePremierCoupon, '')
  assert.equal(v.emetteurId, EMETTEUR)
  assert.equal(v.statut, 'Actif')
  assert.equal(valeursFormulaire(MODELES.obligations, titre, 'en').tauxFacial, '6.25')
})

test('relu par lireLigne, le formulaire prérempli redonne exactement le titre (aller-retour sans perte)', () => {
  const references = preparerReferences([], [{ id: EMETTEUR, nom: 'ECMR', sigle: null }])
  const valeurs = valeursFormulaire(MODELES.obligations, titre, 'fr')
  const { commande, erreurs } = lireLigne(MODELES.obligations, (champ) => valeurs[champ], references)
  assert.deepEqual(erreurs, [])
  const m = commandeModification(commande, valeurs.statut)
  assert.equal(m.isin, undefined) // l'ISIN ne part jamais dans une modification
  assert.deepEqual(
    [m.tauxFacial, m.nominal, m.differeEcheances, m.dateEcheance, m.emetteurId, m.codeConvention, m.statut],
    [6.25, 10000, 2, '2029-05-27', EMETTEUR, 'EXACT_365', 'Actif'],
  )
})

test('un statut inconnu ne donne pas de commande', () => {
  assert.equal(commandeModification({ nom: 'x' }, 'Supprime'), null)
  assert.deepEqual(commandeModification({ isin: 'CM0000000011', nom: 'x' }, 'Radie'), { nom: 'x', statut: 'Radie' })
})

test('historique : un cours, un dividende ou un document suffit', () => {
  assert.equal(aUnHistorique(titre), true)
  assert.equal(aUnHistorique({ ...titre, nombreCours: 0 }), false)
  assert.equal(aUnHistorique({ ...titre, nombreCours: 0, nombreDocuments: 1 }), true)
  assert.equal(aUnHistorique(null), false)
})
