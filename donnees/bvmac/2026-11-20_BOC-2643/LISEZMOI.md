# Bulletin officiel de la cote BVMAC n° 2643 du 20/11/2026

Données réelles, relevées dans le bulletin publié par la BVMAC (BOC-20261120.pdf, 27 pages).
Les fiches des émetteurs (p. 14 à 27) sont celles du bulletin n° 2638 du 13/11/2026, reprises
dans ce numéro. Chaque ligne des fichiers indique sa page dans la colonne « Source ».

## Fichiers (à importer dans cet ordre, écran Administration → Import)

| Fichier | Contenu |
|---|---|
| `1_emetteurs.csv` | 14 émetteurs : 3 États, la BDEAC, 3 entreprises émettrices d'obligations, 7 sociétés cotées |
| `2_obligations.csv` | les 33 lignes obligataires (p. 6 et 7) |
| `3_actions.csv` | les 7 actions (p. 5) ; nominal = capital social de la fiche / nombre de titres |
| `4_cours.csv` | la séance du 20/11/2026 ; pour les obligations, le coupon couru publié (J+3, au 25/11/2026) |
| `5_dividendes.csv` | le dernier dividende de chaque action |
| `6_echeanciers_fiches.csv` | les échéanciers des fiches émetteurs (comparaison seulement, pas d'import) |

La colonne « Source » n'est pas lue par l'import (elle apparaît dans les colonnes ignorées).

## Comparaison automatique avec la BVMAC

`tests/CemacBourseCalc.Domain.Tests/Bvmac` relit ces mêmes fichiers :

- **coupon couru** : pour les 33 obligations, notre calcul au 25/11/2026 doit tomber au centime
  sur le coupon couru publié — **33 / 33 identiques** ;
- **échéanciers** : nos échéances (capital et intérêts, pour le nombre de titres cotés) face aux
  fiches des émetteurs, à moins d'1 FCFA près (les fiches sont arrondies au franc).

## Valeurs déduites (le bulletin ne les imprime pas telles quelles)

- **Date de jouissance** : la date anniversaire du coupon (p. 6-7), moins la maturité en années
  (colonne « Mat »). Exemple : ECMR 6,25 % 2022-2029, coupon au 27/05, 7 ans → 27/05/2022.
- **Date d'échéance** : la dernière date des échéanciers des fiches ; à défaut, l'anniversaire du
  coupon l'année de fin du nom du titre.
- **Différé** : nombre d'échéances sans remboursement, déduit du nominal restant (p. 3-4) et des
  amortissements restant dus dans les fiches.
- **Nominal d'origine** : montant levé / nombre de titres = 10 000 FCFA pour les 33 obligations.

Toutes ces déductions sont vérifiées par la comparaison ci-dessus.

## Points ouverts

1. **BDEAC 6,20 % 2024-2031 et 5,95 % 2024-2029** : pas de fiche BDEAC dans le bulletin. Le différé
   est mis à 2 : aucun amortissement n'est encore payé, donc le coupon couru actuel est juste,
   mais le plan d'amortissement est à confirmer par le prospectus.
2. **Arrondi de la part d'amortissement** : pour une part qui ne tombe pas juste (10 000 / 3,
   10 000 / 18), la BVMAC garde la valeur exacte (nominal publié 8 333,333), alors que notre
   calcul arrondit la part au centime et laisse le reste à la dernière échéance. Concerne
   ECMR 6,75 % 2023-2029, EOG 6,5 % 2024-2029 et ALIOS-06.
3. **ALIOS-05 et ALIOS-06 (trimestrielles)** : la fiche ALIOS calcule les intérêts en Exact/360
   (jours réels / 360), alors que le coupon couru du bulletin est en Exact/365. À trancher avec
   le prospectus. Les intitulés des deux tableaux de la fiche sont par ailleurs inversés (le
   montant levé de l'un est affiché sur l'autre).
4. **Dividendes** : la p. 5 donne le brut pour SEMC, SAFACAM, SOCAPALM, BANGE et BGFI HC
   (confirmé par les fiches), mais le net pour La Régionale (894 = 993,334 × 90 %) et SCG-Ré
   (433,16 ≈ 481,3 × 90 %). Les deux montants sont saisis.
5. **Jours fériés** : le bulletin ne les donne pas ; il faut le calendrier officiel de la BVMAC.
