import AvertissementEducatif from './components/AvertissementEducatif'
import ListeTitres from './pages/ListeTitres'

export default function App() {
  return (
    <main className="page">
      <header>
        <h1>CEMAC Bourse Calc</h1>
        <p className="sous-titre">Les titres cotés à la BVMAC, et les calculs expliqués.</p>
      </header>
      <AvertissementEducatif />
      <section>
        <h2>Titres cotés</h2>
        <ListeTitres />
      </section>
    </main>
  )
}
