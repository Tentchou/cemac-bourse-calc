// Génère la carte de l'Afrique utilisée par le site : src/components/carte/afrique.json.
// À lancer une seule fois (ou après un changement de projection) : `npm run carte`.
//
// Source : Natural Earth 1:50m (domaine public), via le paquet world-atlas (ISC).
// Le site ne reçoit qu'un fichier figé de tracés SVG : aucune bibliothèque de cartographie
// n'est chargée dans le navigateur.

import { geoMercator, geoPath, geoArea } from 'd3-geo'
import { readFileSync, writeFileSync } from 'node:fs'
import { feature } from 'topojson-client'

const LARGEUR = 600

// Pays d'Afrique continentale et îles proches (codes ISO 3166 numériques de Natural Earth).
// Les îles lointaines (Cap-Vert, Maurice, Seychelles) sont exclues : elles écraseraient l'échelle.
const AFRIQUE = new Set([
  '012', '024', '204', '072', '854', '108', '120', '140', '148', '174', '178', '180', '262', '818', '226', '232',
  '748', '231', '266', '270', '288', '324', '624', '384', '404', '426', '430', '434', '450', '454', '466', '478',
  '504', '508', '516', '562', '566', '646', '678', '686', '694', '706', '710', '728', '729', '834', '768', '788',
  '800', '732', '894', '716',
])
const SANS_CODE = new Set(['Somaliland']) // territoires sans code ISO dans Natural Earth

// Les 6 pays de la CEMAC, et où poser leur badge (en longitude / latitude) pour qu'aucun ne se
// chevauche : à gauche dans le golfe de Guinée, à droite vers l'est. Un trait relie badge et pays.
const CEMAC = {
  '120': { code: 'CM', ancre: [3.5, 11.5], cote: 'gauche' },
  '226': { code: 'GQ', ancre: [-3.5, 3.5], cote: 'gauche' },
  '266': { code: 'GA', ancre: [-3.5, -3.5], cote: 'gauche' },
  '178': { code: 'CG', ancre: [-3.5, -10.5], cote: 'gauche' },
  '148': { code: 'TD', ancre: [27.5, 19.5], cote: 'droite' },
  '140': { code: 'CF', ancre: [30, 7], cote: 'droite' },
}

const monde = JSON.parse(readFileSync(new URL('../node_modules/world-atlas/countries-50m.json', import.meta.url)))
const pays = feature(monde, monde.objects.countries).features.filter(
  (f) => AFRIQUE.has(f.id) || SANS_CODE.has(f.properties.name),
)
const collection = { type: 'FeatureCollection', features: pays }

const projection = geoMercator().fitWidth(LARGEUR, collection)
const trace = geoPath(projection).digits(1)
const [[, haut], [, bas]] = trace.bounds(collection)
const HAUTEUR = Math.ceil(bas - haut)
projection.translate([projection.translate()[0], projection.translate()[1] - haut])

const arrondi = ([x, y]) => [Math.round(x * 10) / 10, Math.round(y * 10) / 10]

/** Le centre de la plus grande partie du pays (Guinée équatoriale : le continent, pas Bioko). */
function centre(f) {
  if (f.geometry.type !== 'MultiPolygon') return arrondi(trace.centroid(f))
  const parties = f.geometry.coordinates.map((coordinates) => ({ type: 'Polygon', coordinates }))
  const plusGrande = parties.reduce((a, b) => (geoArea(b) > geoArea(a) ? b : a))
  return arrondi(trace.centroid(plusGrande))
}

const autres = pays.filter((f) => !CEMAC[f.id]).map((f) => trace(f)).join('')
const cemac = Object.fromEntries(
  pays
    .filter((f) => CEMAC[f.id])
    .map((f) => {
      const { code, ancre, cote } = CEMAC[f.id]
      return [code, { d: trace(f), centre: centre(f), ancre: arrondi(projection(ancre)), cote }]
    }),
)

const sortie = new URL('../src/components/carte/afrique.json', import.meta.url)
writeFileSync(sortie, JSON.stringify({ source: 'Natural Earth 1:50m (domaine public)', largeur: LARGEUR, hauteur: HAUTEUR, autres, cemac }))
console.log(`Carte écrite : ${LARGEUR} × ${HAUTEUR}, ${pays.length} pays, ${Object.keys(cemac).length} pays CEMAC.`)
