// Composants de mise en page. Ce sont eux qui portent les espacements : une page ne
// choisit jamais ses marges elle-même, elle assemble ces blocs. D'où un rythme identique partout.

/** Largeur maximale du site et marges latérales (16 px sur mobile, 24 px au-delà). */
export function Conteneur({ as: Balise = 'div', className = '', children, ...autres }) {
  return (
    <Balise className={`mx-auto w-full max-w-6xl px-4 sm:px-6 ${className}`} {...autres}>
      {children}
    </Balise>
  )
}

/** Une page : marge haute de 32 px, puis 48 px (mobile) ou 64 px entre ses sections. */
export function Page({ className = '', children }) {
  return <Conteneur className={`flex flex-col gap-12 pt-8 pb-16 sm:gap-16 sm:pb-24 ${className}`}>{children}</Conteneur>
}

/**
 * Une section : titre (avec son icône), phrase d'aide facultative, actions à droite,
 * puis le contenu 24 px plus bas.
 */
export function Section({ id, titre, icone: Icone, aide, actions, niveau = 2, className = '', children }) {
  const Titre = `h${niveau}`
  const idTitre = id ? `${id}-titre` : undefined
  return (
    <section id={id} aria-labelledby={idTitre} className={`min-w-0 scroll-mt-24 ${className}`}>
      {(titre || actions) && (
        <header className="mb-6 flex flex-wrap items-end justify-between gap-4">
          <div className="min-w-0">
            {titre && (
              <Titre id={idTitre} className={`flex items-center gap-3 font-bold ${niveau === 2 ? 'text-2xl sm:text-[1.75rem]' : 'text-lg'}`}>
                {Icone && (
                  <span aria-hidden="true" className="inline-grid size-9 shrink-0 place-items-center rounded-full bg-action/10 text-action">
                    <Icone className="size-5" strokeWidth={1.75} />
                  </span>
                )}
                {titre}
              </Titre>
            )}
            {aide && <p className="mt-2 text-sm text-texte-2">{aide}</p>}
          </div>
          {actions && <div className="flex flex-wrap items-center gap-2">{actions}</div>}
        </header>
      )}
      {children}
    </section>
  )
}

const COLONNES = {
  1: 'grid-cols-1',
  2: 'grid-cols-1 sm:grid-cols-2',
  3: 'grid-cols-1 sm:grid-cols-2 lg:grid-cols-3',
  4: 'grid-cols-2 lg:grid-cols-4',
}

/** Grille responsive avec l'écart unique de 16 px. `colonnes` : 1, 2, 3 ou 4 (sur grand écran). */
export function Grille({ colonnes = 2, as: Balise = 'div', className = '', children, ...autres }) {
  return (
    <Balise className={`grid gap-4 [&>*]:min-w-0 ${COLONNES[colonnes]} ${className}`} {...autres}>
      {children}
    </Balise>
  )
}

/** Fait apparaître son contenu (fondu + montée). `rang` décale l'animation de 40 ms par élément. */
export function Apparition({ rang = 0, as: Balise = 'div', className = '', style, children, ...autres }) {
  return (
    <Balise className={`apparition ${className}`} style={{ '--rang': rang, ...style }} {...autres}>
      {children}
    </Balise>
  )
}
