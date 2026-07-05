# Character Prompt Builder

A single-page HTML tool for building image-generation prompts from human feature selectors. Runs entirely in the browser with no backend.

## Features

- **Body & face**: age, race, physique, breast/butt size, hair color/length, eye color, shaved status
- **Style & scene**: image style, lighting, backdrop/location, camera angle
- **Outfit & pose**: categorized outfits (lingerie, sports, nude, etc.) and poses (SFW + optional NSFW)
- **Live prompt generation** with editable output and negative prompt
- **Presets** saved to `localStorage`
- **Randomize** button for quick inspiration

## Run locally

ES modules require a local HTTP server (opening `index.html` directly via `file://` will not work).

### Option 1: Python (built-in)

```bash
cd prompt-builder
python3 -m http.server 8080
```

Open [http://localhost:8080](http://localhost:8080)

## Use on iPhone / iPad

### Instant preview (no setup)

Open this link in **Safari** on your iPhone or iPad:

**https://cdn.jsdelivr.net/gh/Shgoedt/ShgoedtLibrary@master/prompt-builder/index.html**

Add to Home Screen: Safari → Share → **Add to Home Screen**.

### Official GitHub Pages (recommended)

One-time setup in your repo:

1. Go to **Settings → Pages**
2. Under **Build and deployment**, set **Source** to **GitHub Actions**
3. Re-run the **Deploy Prompt Builder** workflow (Actions tab)

Live URL: **https://shgoedt.github.io/ShgoedtLibrary/**

Alternatively, set Source to **Deploy from branch** → `gh-pages` / `/ (root)` after the **Deploy to gh-pages branch** workflow runs.

### Option 2: Node.js

```bash
cd prompt-builder
npx --yes serve -p 8080
```

### Option 3: PHP

```bash
cd prompt-builder
php -S localhost:8080
```

## Deploy on GitHub Pages

This repo includes a GitHub Actions workflow that publishes the `prompt-builder/` folder automatically.

1. Push this branch to GitHub
2. Go to **Settings → Pages** in your repository
3. Under **Build and deployment**, set **Source** to **GitHub Actions**
4. After the workflow runs, the app will be available at:

   `https://<username>.github.io/<repo-name>/`

   (The workflow sets the correct base path for project sites.)

To trigger a deploy manually: **Actions → Deploy Prompt Builder → Run workflow**

## Project structure

```
prompt-builder/
├── index.html
├── styles.css
├── data/
│   ├── features.json    # selector options
│   └── templates.json   # prompt phrase mappings
└── js/
    ├── app.js           # UI wiring
    ├── state.js         # state & localStorage
    └── promptEngine.js  # prompt assembly
```

## Customization

Edit `data/features.json` to add new outfit categories, poses, or attribute options. Add matching phrase mappings in `data/templates.json`.
