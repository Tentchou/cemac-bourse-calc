namespace CemacBourseCalc.Domain.Communs;

// Les listes de valeurs du domaine. En base, chacune est stockée sous son nom
// (texte) et limitée par une contrainte CHECK générée à partir de ces enums.

public enum TypeTitre { Obligation, Action }

public enum StatutTitre { Actif, Suspendu, Echu, Radie }

public enum TypeEmetteur { Etat, Entreprise, Supranational }

/// <summary>NET : coupon versé sans retenue d'impôt. BRUT : l'impôt sur le revenu des capitaux mobiliers est retenu à la source.</summary>
public enum RegimeFiscal { Net, Brut }

public enum TypeDocument { Prospectus, Avis, Bulletin }

/// <summary>ADMIN : tout, y compris les comptes. ÉDITEUR : saisie des titres, des cours et des règles de la place.</summary>
public enum RoleAdministrateur { Admin, Editeur }
