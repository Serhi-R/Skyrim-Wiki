const STAR_PATH =
  "M10.868 2.884c-.321-.772-1.415-.772-1.736 0l-1.83 4.401-4.753.381c-.833.067-1.171 1.107-.536 1.651l3.62 3.102-1.106 4.637c-.194.813.691 1.456 1.405 1.02L10 15.591l4.069 2.485c.713.436 1.598-.207 1.404-1.02l-1.106-4.637 3.62-3.102c.635-.544.297-1.584-.536-1.65l-4.752-.382-1.831-4.401Z";

// One star, filled or empty. The picker draws five of these as buttons.
export function Star({ filled, size = "h-4 w-4" }: { filled: boolean; size?: string }) {
  return (
    <svg
      className={`${size} ${filled ? "text-gold" : "text-brass/30"}`}
      viewBox="0 0 20 20"
      fill="currentColor"
      aria-hidden="true"
    >
      <path fillRule="evenodd" clipRule="evenodd" d={STAR_PATH} />
    </svg>
  );
}

// Five stars, filled up to the rounded rating. Used by the comment list.
export function Stars({ rating, size = "h-4 w-4" }: { rating: number; size?: string }) {
  const filled = Math.round(rating);

  return (
    <span className="inline-flex items-center gap-0.5" title={`${rating.toFixed(1)} / 5`}>
      {[1, 2, 3, 4, 5].map((star) => (
        <Star key={star} filled={star <= filled} size={size} />
      ))}
    </span>
  );
}
