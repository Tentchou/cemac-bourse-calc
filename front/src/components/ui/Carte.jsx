/**
 * Surface de base : fond blanc (bleu nuit en sombre), bordure fine, rayon de 16 px,
 * marge intérieure de 24 px partout (sauf `sansMarge`, pour un tableau ou une image bord à bord).
 *
 * accent : met la carte en avant (bordure bleue) — pour le résultat principal d'un calcul.
 * interactive : légère élévation au survol, pour une carte cliquable.
 */
export default function Carte({ as: Balise = 'div', accent = false, interactive = false, sansMarge = false, className = '', children, ...autres }) {
  return (
    <Balise
      className={[
        'min-w-0 rounded-2xl border bg-surface',
        accent ? 'border-action/60 shadow-douce' : 'border-bordure',
        interactive ? 'transition-[box-shadow,transform,border-color] duration-150 ease-douce hover:-translate-y-0.5 hover:border-bordure-champ hover:shadow-haute' : '',
        sansMarge ? '' : 'p-6',
        className,
      ].join(' ')}
      {...autres}
    >
      {children}
    </Balise>
  )
}
