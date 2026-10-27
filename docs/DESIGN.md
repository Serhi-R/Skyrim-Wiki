# SkyrimWIKI design brief

## Concept

Cold Nordic stone in late winter. Snow on dark rock, a sliver of green
northern light, weathered iron and shallow-cut runes. The site should read as a
carved place, not a dashboard: quiet surfaces, one loud moment, warmth used only
where the eye must land.

Subject: a lore wiki for the five holds and the quests, races and places of
Skyrim. Audience: players reading mid-game, often on a phone at night. Job of
the design: make a page scannable in seconds and pleasant to stay in.

## Palette

Six named values. Contrast checked against WCAG AA (4.5:1 for body text).

| Name        | Hex       | Role                                  |
| ----------- | --------- | ------------------------------------- |
| Night slate | `#0F1318` | Page background                       |
| Stone       | `#1B232C` | Raised surfaces, cards, header        |
| Slate blue  | `#2E3A46` | Borders, dividers, secondary surface  |
| Frost text  | `#D9E2EA` | Body and heading text                 |
| Aged gold   | `#C9A24A` | Accent, rules, links, one hero        |
| Blood red   | `#8C2F2B` | Destructive fills only                |

Derived tints (documented because they are required for contrast):

| Name        | Hex       | Role                                  |
| ----------- | --------- | ------------------------------------- |
| Ice muted   | `#9FB0C0` | Metadata, captions                    |
| Blood light | `#E8A9A3` | Destructive text on dark              |

Measured ratios (sRGB, WCAG 2.1 formula):

- Frost on night slate: 14.4:1
- Frost on stone: 12.1:1
- Frost on slate blue: 8.9:1
- Aged gold on night slate: 7.9:1
- Aged gold on stone: 6.6:1
- Ice muted on stone: 7.1:1
- Blood light on stone: 8.0:1
- Blood red `#8C2F2B` on night slate is 2.3:1, so it is never used as text;
  it is only a button fill carrying frost text.

## Typography

Two faces, clearly distinct:

- Display: Cinzel. Roman inscriptional capitals. Used for the wordmark, page
  titles and the one hero line. Never for whole paragraphs.
- Body: EB Garamond. Warm, readable old-style serif. Used for all body copy,
  at 17px minimum, line-height 1.7, measure 60 to 75 characters.

Both are self-hosted (woff2 in `wwwroot/fonts`, `@font-face` in the CSS, no
external requests). No all-caps tracked labels, no eyebrow label above every
heading, no single-word decorative accent inside a headline. Sentence case
throughout.

## Signature element

The home hero. One orchestrated moment: three mountain layers (SVG, original)
rise slowly on load, a faint aurora band drifts behind them, and light snow
falls. This is the only place boldness is spent. Every other surface stays flat
and disciplined.

## Layout vocabulary

Each section has its own shape, so no two pages are grids of equal boxes:

- Home: full-bleed hero, then one wide featured quest with its image, then a
  compact list of recent quests.
- Quest list: filter bar, then one featured row plus list rows. Each row shows a
  level badge, the title, the start location and a one-line summary. Not a grid.
- Quest detail: two columns. Main column has a wide image header, prose, key
  steps. Sidebar has reward, level, quest giver, and related quests nearby.
- Location detail: wide image header (hold photo), then prose, then the list of
  quests that start there as rows, plus a small crest.
- Race detail: crest banner at the left, prose at the right, stat list for
  skill bonuses and the racial power.

## Wireframes

Home

```
+--------------------------------------------------------------+
| logo        Quests  Races  Locations  About        [sign in]  |
+--------------------------------------------------------------+
|                                                              |
|              ~ aurora band (drifts) ~                        |
|            /\/\ mountains rise on load /\/\                  |
|                                                              |
|                 S K Y R I M W I K I                          |
|          lore of the holds, quests and races                 |
|                                                              |
+--------------------------------------------------------------+
| FEATURED QUEST  [                        image            ]  |
|                 Bleak Falls Barrow                           |
|                 level 4  |  Whiterun Hold   [read]           |
+--------------------------------------------------------------+
| Recent quests                                                |
|  lvl   The Man Who Cried Wolf        Solitude        ->      |
|  lvl   Missing In Action             Windhelm        ->      |
|  lvl   The Pursuit                   Riften          ->      |
+--------------------------------------------------------------+
| credits | about                        copyright             |
+--------------------------------------------------------------+
```

Quests index

```
+--------------------------------------------------------------+
| Quests                                       [New quest]     |
+--------------------------------------------------------------+
| [ search ] [ location v ] [ lvl - lvl ] [ sort v ] [reset]   |
| 8 quests found                                  [Export CSV] |
+--------------------------------------------------------------+
| [ FEATURED ROW: image | title | level | summary ]            |
+--------------------------------------------------------------+
| lvl 6   The Man Who Cried Wolf   Solitude   summary...   ->  |
| lvl 6   Missing In Action        Windhelm   summary...   ->  |
| lvl 36  Deathbrand               Dawnstar   summary...   ->  |
+--------------------------------------------------------------+
|                         prev  page 1/2  next                 |
+--------------------------------------------------------------+
```

Quest detail

```
+--------------------------------------------------------------+
| <- All quests                                                |
+------------------------------------+-------------------------+
| [        wide image header       ] |  Reward   500 gold      |
|                                    |  Level    4             |
| Bleak Falls Barrow                 |  Giver    Farengar      |
| Whiterun Hold                      |  Hold     Whiterun      |
|                                    |-------------------------|
| prose description ...              |  Related quests         |
|                                    |  - The Way of the Voice |
| Key steps                          |  - ...                  |
|  1. ...                            |                         |
|  2. ...                            |                         |
|------------------------------------+-------------------------|
| Comments (island)                                            |
+--------------------------------------------------------------+
```

Location detail

```
+--------------------------------------------------------------+
| <- All locations                                             |
+--------------------------------------------------------------+
| [              wide hold photo header                       ]|
|  [crest]  Whiterun   (Whiterun Hold)                         |
+--------------------------------------------------------------+
| prose description ...                                       |
+--------------------------------------------------------------+
| Quests that start here                                      |
|  lvl   Bleak Falls Barrow                       ->           |
|  lvl   ...                                                   |
+--------------------------------------------------------------+
```

## Review against the anti-default list

Checked each item the skill flags as a generated-default tell:

- Warm cream + serif + terracotta: not used. Base is cold near-black slate.
- Near-black + single acid accent: the base is near-black, but the accent is
  aged gold and it is deliberately used sparingly, not as an acid highlight.
- Broadsheet hairline columns: not used. Serif is used for warmth, not to fake
  a newspaper.
- SaaS card kit (identical rounded cards, one radius, one soft shadow): dropped.
  Surfaces are flat with a single low border and no drop shadow; shapes differ
  between pages.
- Tracked ALL-CAPS eyebrow above every heading: removed. Sentence case.
- Meta joined with middle dots (`A · B · C`): removed, rebuilt as separate
  labelled facts.
- Tinted near-black `#0B0B0B`/`#111` as "black": the base is a blue-leaning
  slate `#0F1318`, chosen for the cold read, not a flat tint.
- Monospace for small data: not used.
- `->` appended to every link: book arrows used once, on list rows, where a
  row genuinely navigates.
- Fade-and-slide-up on every section / hover lift on every card: not used. One
  load sequence in the hero, and small action-driven motion only.

Kept because the brief asks for them: a dark base, a serif identity, a single
gold accent, and one bold hero.
