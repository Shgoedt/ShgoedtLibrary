# Character Prompt Builder

A single-page HTML tool for building image-generation prompts from human feature selectors. Runs entirely in the browser with no backend.

## Features

- **Body & face**: age, race, physique, breast/butt size, hair color/length, eye color, shaved status
- **Style & scene**: image style, lighting, backdrop/location, camera angle
- **Outfit & pose**: categorized outfits (lingerie, sports, nude, etc.) and poses (SFW + optional NSFW)
- **Live prompt generation** with editable output and negative prompt
- **Presets** saved to `localStorage`
- **Randomize** button for quick inspiration
- **Image generation** via uncensored Kwiki AI API (5 free credits/day — API key stored locally)

## Run locally

### Development (multi-file, hot reload friendly)

```bash
cd prompt-builder
python3 -m http.server 8080
```

Open [http://localhost:8080](http://localhost:8080)

> ES modules require a local HTTP server — opening `index.html` via `file://` won't work.

### Standalone build (same as GitHub Pages deploy)

```bash
cd prompt-builder
node scripts/build-standalone.mjs
python3 -m http.server 8080 --directory dist
```

## Use on iPhone / iPad

### Instant preview (works now)

Open this link in **Safari**:

**https://raw.githack.com/Shgoedt/ShgoedtLibrary/master/prompt-builder/dist/index.html**

Add to Home Screen: Safari → Share → **Add to Home Screen**.

> Do **not** use the jsDelivr or raw GitHub links — they serve HTML as plain text and Safari will show source code instead of the app.

### GitHub Pages (recommended, permanent URL)

One-time setup:

1. Open [Settings → Pages](https://github.com/Shgoedt/ShgoedtLibrary/settings/pages)
2. Set **Source** to **Deploy from a branch**
3. Choose branch **`gh-pages`**, folder **`/ (root)`**, click **Save**

After ~1 minute, open:

**https://shgoedt.github.io/ShgoedtLibrary/**

## Image generation (Kwiki AI — uncensored)

The app generates images from your prompt using the **uncensored** [Kwiki AI API](https://kwikiai.com/guides/nsfw-ai-api) — no content filters on legal adult prompts.

1. Create a free account at [kwikiai.com/account](https://kwikiai.com/account) (5 free credits per day)
2. Paste your API key into the app (saved in your browser only)
3. Choose style (realistic / anime / nude) and quality, then click **Generate image**

Generation typically takes 30–90 seconds. Image URLs are temporary — download to keep them.

## Deploy on GitHub

Pushing to `master` runs the **Deploy to gh-pages branch** workflow, which builds the standalone app and publishes it to the `gh-pages` branch.

## Project structure

```
prompt-builder/
├── index.html              # dev entry (multi-file)
├── dist/index.html         # standalone bundle (deployed to gh-pages)
├── scripts/build-standalone.mjs
├── styles.css
├── data/
│   ├── features.json
│   └── templates.json
└── js/
    ├── app.js
    ├── state.js
    └── promptEngine.js
```

## Customization

1. Edit `data/features.json` and `data/templates.json`
2. Rebuild: `node scripts/build-standalone.mjs`
3. Commit source changes (and `dist/index.html` if you want the githack preview updated immediately)
