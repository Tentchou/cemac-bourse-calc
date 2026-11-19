import { useEffect, useState } from 'react'
import { listerTitres } from '../api/titres'

const fcfa = new Intl.NumberFormat('fr-FR', { maximumFractionDigits: 2 })

const libelleType = { OBLIGATION: 'Obligation', ACTION: 'Action' }

export default function ListeTitres() {
  const [titres, setTitres] = useState(null)
  const [erreur, setErreur] = useState(null)

  useEffect(() => {
    listerTitres().then(setTitres).catch((e) => setErreur(e.message))
  }, [])

  if (erreur) return <p className="erreur">{erreur}</p>
  if (titres === null) return <p>Chargement des titres…</p>
  if (titres.length === 0) return <p>Aucun titre n'est encore enregistré.</p>

  return (
    <table className="titres">
      <thead>
        <tr>
          <th>Titre</th>
          <th>ISIN</th>
          <th>Émetteur</th>
          <th>Pays</th>
          <th>Type</th>
          <th className="nombre">Nominal (FCFA)</th>
        </tr>
      </thead>
      <tbody>
        {titres.map((t) => (
          <tr key={t.id}>
            <td>{t.nom}</td>
            <td>{t.isin}</td>
            <td>{t.emetteur}</td>
            <td>{t.pays}</td>
            <td>{libelleType[t.typeTitre] ?? t.typeTitre}</td>
            <td className="nombre">{fcfa.format(t.nominal)}</td>
          </tr>
        ))}
      </tbody>
    </table>
  )
}
