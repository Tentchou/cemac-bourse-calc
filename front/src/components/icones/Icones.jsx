// Icônes propres au site (les autres viennent de Lucide, même trait de 1,75).

/** Logo provisoire : trois barres montantes dans un carré bleu marque. À remplacer par le logo définitif. */
export function Logo({ className = 'size-9' }) {
  return (
    <svg viewBox="0 0 36 36" className={className} aria-hidden="true">
      <rect width="36" height="36" rx="11" fill="var(--cbc-marque)" />
      <rect x="0.5" y="0.5" width="35" height="35" rx="10.5" fill="none" stroke="var(--cbc-bordure)" opacity="0.5" />
      <rect x="9" y="19" width="4.5" height="8" rx="2.25" fill="var(--cbc-sur-marque)" opacity="0.7" />
      <rect x="15.75" y="14" width="4.5" height="13" rx="2.25" fill="var(--cbc-sur-marque)" opacity="0.85" />
      <rect x="22.5" y="9" width="4.5" height="18" rx="2.25" fill="var(--cbc-orange)" />
    </svg>
  )
}

/** Logo WhatsApp (marque), dessiné en plein pour rester lisible à petite taille. */
export function IconeWhatsApp(props) {
  return (
    <svg viewBox="0 0 24 24" fill="currentColor" {...props}>
      <path d="M12 2a10 10 0 0 0-8.6 15.1L2 22l5-1.3A10 10 0 1 0 12 2zm0 18.2a8.2 8.2 0 0 1-4.2-1.1l-.3-.2-3 .8.8-2.9-.2-.3A8.2 8.2 0 1 1 12 20.2zm4.5-6.1c-.2-.1-1.5-.7-1.7-.8s-.4-.1-.6.1-.7.8-.8 1-.3.2-.5.1a6.7 6.7 0 0 1-3.3-2.9c-.3-.4.2-.4.7-1.3.1-.2 0-.3 0-.4l-.8-1.8c-.2-.5-.4-.4-.6-.4h-.5a1 1 0 0 0-.7.3 3 3 0 0 0-.9 2.2 5.2 5.2 0 0 0 1.1 2.7 11.8 11.8 0 0 0 4.5 4c1.7.7 2.3.8 3.2.6a2.7 2.7 0 0 0 1.8-1.2 2.2 2.2 0 0 0 .1-1.3c0-.1-.2-.2-.4-.3z" />
    </svg>
  )
}
