import { BookOpen, LayoutList } from 'lucide-react'

/** Les pages du site, partagées par l'en-tête et le pied. pages : les routes où le lien est « actif ». */
export const LIENS = [
  { vers: '/', cle: 'nav.titres', icone: LayoutList, pages: ['accueil', 'fiche'] },
  { vers: '/methodes', cle: 'nav.methodes', icone: BookOpen, pages: ['methodes'] },
]
