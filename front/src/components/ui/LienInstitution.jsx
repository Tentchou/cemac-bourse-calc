import { ArrowUpRight } from 'lucide-react'
import { usePreferences } from '../../contexte/Preferences'

/**
 * Les institutions du marché, avec leur site officiel et leurs couleurs (relevées sur leurs sites).
 * Les couleurs vivent dans styles/index.css (--cbc-bvmac, --cbc-cosumaf…), clair et sombre.
 */
const INSTITUTIONS = {
  bvmac: {
    sigle: 'BVMAC',
    url: 'https://www.bvm-ac.org/',
    teinte: 'border-bvmac/45 bg-bvmac/10 text-bvmac-texte hover:bg-bvmac/20 hover:border-bvmac',
    point: 'bg-bvmac',
  },
  cosumaf: {
    sigle: 'COSUMAF',
    url: 'https://www.cosumaf.org/',
    teinte: 'border-cosumaf/45 bg-cosumaf/10 text-cosumaf-texte hover:bg-cosumaf/20 hover:border-cosumaf',
    point: 'bg-cosumaf',
  },
}

/**
 * Lien pilule vers le site officiel de la BVMAC ou de la COSUMAF, dans leurs couleurs.
 * code : 'bvmac' | 'cosumaf'. role : affiche le rôle après le sigle (« Bourse », « Régulateur »).
 * S'ouvre dans un nouvel onglet ; le nom complet est donné en info-bulle et aux lecteurs d'écran.
 */
export default function LienInstitution({ code, role = false, className = '' }) {
  const { t } = usePreferences()
  const { sigle, url, teinte, point } = INSTITUTIONS[code]
  const nom = t(`institution.${code}`)

  return (
    <a
      href={url}
      target="_blank"
      rel="noopener noreferrer"
      title={`${nom} — ${t('institution.site')}`}
      className={`group inline-flex h-7 max-w-full items-center gap-1.5 rounded-full border px-2.5 text-xs font-semibold whitespace-nowrap transition-colors duration-150 ease-douce focus-visible:ring-2 focus-visible:ring-action focus-visible:ring-offset-2 focus-visible:ring-offset-fond focus-visible:outline-none ${teinte} ${className}`}
    >
      <span aria-hidden="true" className={`size-2 shrink-0 rounded-full ${point}`} />
      <span className="tracking-wide">{sigle}</span>
      {/* Rôle masqué sous 400 px pour garder les deux liens sur une ligne (le nom complet reste en info-bulle). */}
      {role && <span className="hidden font-medium text-texte-2 min-[400px]:inline">· {t(`institution.${code}.role`)}</span>}
      <span className="sr-only">
        {nom}, {t('institution.nouvelOnglet')}
      </span>
      <ArrowUpRight
        aria-hidden="true"
        className="size-3.5 shrink-0 transition-transform duration-150 ease-douce group-hover:translate-x-0.5 group-hover:-translate-y-0.5 motion-reduce:transition-none"
        strokeWidth={2}
      />
    </a>
  )
}
