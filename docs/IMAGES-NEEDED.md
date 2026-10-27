# Images

Game images are installed in `wwwroot/img/game/`. The resolver
(`Services/GameImages.cs`) prefers the game file and otherwise falls back to the
previous Wikimedia picture, so no page ever links a missing file.

## Rules

- Folder layout and names are fixed, lowercase with hyphens:
  - `wwwroot/img/game/hero/home.jpg`
  - `wwwroot/img/game/races/<race>.jpg`
  - `wwwroot/img/game/holds/<hold>.jpg`
  - `wwwroot/img/game/quests/<quest>.jpg`
- The slug is the name lowercased, every run of characters that is not `a-z` or
  `0-9` replaced by one hyphen, trimmed. Example: `The Woodsman's Friend` ->
  `the-woodsman-s-friend`.
- Each file ships as `-1600.webp` (heroes, banners) and `-800.webp` (cards),
  every one under 200 KB. The resolver prefers `-800` for cards and `-1600` for
  banners, and serves the raw file if the sized ones do not exist.

## Installed

| Group | Count | Notes |
| --- | --- | --- |
| Home hero | 1 | `hero/hero-*.webp` |
| Races | 10 | one per playable race |
| Holds | 10 | nine hold capitals plus Ivarstead |
| Quests | 12 | one per seeded quest |

The hero uses the High Hrothgar picture. All other files match the seeded
entities one to one, so every card and banner on the site now shows a game
image.

## Fallbacks

`Services/HoldImages.cs` still provides the older Wikimedia photos. They are
only used if a game file is later removed; with the current set, none are
displayed.
