import { usePreferences } from '../../contexte/Preferences'
import { IllustrationChoisir, IllustrationLire, IllustrationSaisir } from '../illustrations/Illustrations'
import { Grille } from '../mise-en-page/MiseEnPage'
import Carte from '../ui/Carte'

const ETAPES = [
  { Illustration: IllustrationChoisir, titre: 'etapes.1', texte: 'etapes.1.texte' },
  { Illustration: IllustrationSaisir, titre: 'etapes.2', texte: 'etapes.2.texte' },
  { Illustration: IllustrationLire, titre: 'etapes.3', texte: 'etapes.3.texte' },
]

/** « En 3 étapes » : une scène animée par étape, son numéro, un titre court et une phrase. */
export default function EtapesIllustrees() {
  const { t } = usePreferences()
  return (
    <Grille colonnes={3} as="ol">
      {ETAPES.map(({ Illustration, titre, texte }, i) => (
        <Carte as="li" key={titre} sansMarge className="apparition overflow-hidden" style={{ '--rang': i }}>
          <div className="border-b border-bordure bg-fond px-6 pt-6 pb-4">
            <Illustration />
          </div>
          <div className="flex gap-4 p-6">
            <span
              aria-hidden="true"
              className="chiffres grid size-8 shrink-0 place-items-center rounded-full bg-orange font-titre text-sm font-bold text-[#14213D]"
            >
              {i + 1}
            </span>
            <div className="min-w-0">
              <h3 className="font-semibold">
                <span className="sr-only">{i + 1}. </span>
                {t(titre)}
              </h3>
              <p className="mt-1 text-sm text-texte-2">{t(texte)}</p>
            </div>
          </div>
        </Carte>
      ))}
    </Grille>
  )
}
