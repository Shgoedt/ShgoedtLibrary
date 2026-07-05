#!/usr/bin/env node
/**
 * Bundles the prompt builder into a single self-contained HTML file.
 * Works on iOS Safari via GitHub Pages and raw.githack.com (correct text/html).
 */
import { readFileSync, writeFileSync, mkdirSync } from 'fs';
import { dirname, join } from 'path';
import { fileURLToPath } from 'url';

const root = join(dirname(fileURLToPath(import.meta.url)), '..');
const distDir = join(root, 'dist');

function read(name) {
  return readFileSync(join(root, name), 'utf8');
}

function stripExports(code) {
  return code
    .replace(/^export /gm, '')
    .replace(/^export\{[^}]+\};?\s*$/gm, '');
}

const features = read('data/features.json');
const templates = read('data/templates.json');
const css = read('styles.css');
const stateJs = stripExports(read('js/state.js'));
const engineJs = stripExports(read('js/promptEngine.js'));

const appJs = read('js/app.js')
  .replace(/^import .+;\n/gm, '')
  .replace(
    /async function loadData\(\) \{[\s\S]*?\}\n/,
    `function loadData() {
  features = FEATURES;
  templates = TEMPLATES;
  return Promise.resolve();
}
`
  );

const html = read('index.html')
  .replace('<link rel="stylesheet" href="styles.css">', `<style>\n${css}\n</style>`)
  .replace(
    '<script type="module" src="js/app.js"></script>',
    `<script>
const FEATURES = ${features};
const TEMPLATES = ${templates};
</script>
<script>
${stateJs}
${engineJs}
${appJs}
</script>`
  );

mkdirSync(distDir, { recursive: true });
writeFileSync(join(distDir, 'index.html'), html);
console.log('Built dist/index.html (' + Math.round(html.length / 1024) + ' KB)');
