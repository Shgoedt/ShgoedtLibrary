import { loadState, saveState, resetState, loadPresets, savePresets } from './state.js';
import { buildPrompt, buildNegativePrompt, randomizeState } from './promptEngine.js';
import {
  generateImage,
  loadApiKey,
  saveApiKey,
  RESOLUTIONS,
  suggestResolution
} from './imageGenerator.js';

let features = null;
let templates = null;
let state = loadState();
let promptEdited = false;
let negativeEdited = false;
let lastImageUrl = null;

const FIELD_LABELS = {
  age: 'Age',
  race: 'Race / ethnicity',
  physique: 'Physique',
  breastSize: 'Breast size',
  buttSize: 'Butt size',
  hairColor: 'Hair color',
  hairLength: 'Hair length',
  eyeColor: 'Eye color',
  shaved: 'Shaved status',
  imageStyle: 'Image style',
  lighting: 'Lighting',
  backdrop: 'Backdrop / location',
  cameraAngle: 'Camera angle'
};

const $ = (sel) => document.querySelector(sel);

async function loadData() {
  const [fRes, tRes] = await Promise.all([
    fetch('data/features.json'),
    fetch('data/templates.json')
  ]);
  if (!fRes.ok || !tRes.ok) throw new Error('Could not load data files');
  features = await fRes.json();
  templates = await tRes.json();
}

function createSelect(id, options, value, onChange) {
  const field = document.createElement('div');
  field.className = 'field';

  const label = document.createElement('label');
  label.htmlFor = id;
  label.textContent = FIELD_LABELS[id] ?? id;

  const select = document.createElement('select');
  select.id = id;
  select.name = id;

  for (const opt of options) {
    const option = document.createElement('option');
    option.value = opt.value;
    option.textContent = opt.label;
    if (opt.value === value) option.selected = true;
    select.appendChild(option);
  }

  select.addEventListener('change', () => onChange(select.value));
  field.append(label, select);
  return field;
}

function renderSimpleFields(container, fieldNames) {
  container.innerHTML = '';
  for (const name of fieldNames) {
    const options = features[name];
    if (!options) continue;
    container.appendChild(createSelect(name, options, state[name], (val) => {
      state[name] = val;
      persistAndUpdate();
    }));
  }
}

function renderOutfitFields() {
  const categorySelect = $('#outfitCategory');
  const detailSelect = $('#outfitDetail');

  categorySelect.innerHTML = '';
  for (const [key, group] of Object.entries(features.outfits)) {
    const opt = document.createElement('option');
    opt.value = key;
    opt.textContent = group.label;
    if (key === state.outfitCategory) opt.selected = true;
    categorySelect.appendChild(opt);
  }

  populateOutfitDetails(detailSelect);

  categorySelect.onchange = () => {
    state.outfitCategory = categorySelect.value;
    const items = features.outfits[state.outfitCategory].items;
    state.outfitDetail = items[0].value;
    populateOutfitDetails(detailSelect);
    persistAndUpdate();
  };

  detailSelect.onchange = () => {
    state.outfitDetail = detailSelect.value;
    persistAndUpdate();
  };
}

function populateOutfitDetails(detailSelect) {
  detailSelect.innerHTML = '';
  const items = features.outfits[state.outfitCategory].items;
  for (const item of items) {
    const opt = document.createElement('option');
    opt.value = item.value;
    opt.textContent = item.label;
    if (item.value === state.outfitDetail) opt.selected = true;
    detailSelect.appendChild(opt);
  }
  if (!items.find((i) => i.value === state.outfitDetail)) {
    state.outfitDetail = items[0].value;
  }
}

function renderPoseFields() {
  const categorySelect = $('#poseCategory');
  const detailSelect = $('#poseDetail');

  categorySelect.innerHTML = '';
  for (const [key, group] of Object.entries(features.poses)) {
    if (group.nsfw && !state.showNsfw) continue;
    const opt = document.createElement('option');
    opt.value = key;
    opt.textContent = group.label + (group.nsfw ? ' (NSFW)' : '');
    if (key === state.poseCategory) opt.selected = true;
    categorySelect.appendChild(opt);
  }

  if (!categorySelect.querySelector(`option[value="${state.poseCategory}"]`)) {
    const first = categorySelect.options[0];
    if (first) state.poseCategory = first.value;
  }

  populatePoseDetails(detailSelect);

  categorySelect.onchange = () => {
    state.poseCategory = categorySelect.value;
    const items = features.poses[state.poseCategory].items;
    state.poseDetail = items[0].value;
    populatePoseDetails(detailSelect);
    persistAndUpdate();
  };

  detailSelect.onchange = () => {
    state.poseDetail = detailSelect.value;
    persistAndUpdate();
  };
}

function populatePoseDetails(detailSelect) {
  detailSelect.innerHTML = '';
  const items = features.poses[state.poseCategory].items;
  for (const item of items) {
    const opt = document.createElement('option');
    opt.value = item.value;
    opt.textContent = item.label;
    if (item.value === state.poseDetail) opt.selected = true;
    detailSelect.appendChild(opt);
  }
  if (!items.find((i) => i.value === state.poseDetail)) {
    state.poseDetail = items[0].value;
  }
}

function renderToggles() {
  $('#showNsfw').checked = state.showNsfw;
  $('#includeShaved').checked = state.includeShaved;
  $('#includeBodyDetails').checked = state.includeBodyDetails;
  $('#customOutfit').value = state.customOutfit || '';
  $('#customPose').value = state.customPose || '';
}

function updatePromptOutput() {
  if (!promptEdited) {
    $('#promptOutput').value = buildPrompt(state, templates, features);
  }
  if (!negativeEdited) {
    $('#negativeOutput').value = buildNegativePrompt(features);
  }
}

function persistAndUpdate() {
  saveState(state);
  updatePromptOutput();
}

function bindGlobalControls() {
  $('#showNsfw').addEventListener('change', (e) => {
    state.showNsfw = e.target.checked;
    renderPoseFields();
    persistAndUpdate();
  });

  $('#includeShaved').addEventListener('change', (e) => {
    state.includeShaved = e.target.checked;
    persistAndUpdate();
  });

  $('#includeBodyDetails').addEventListener('change', (e) => {
    state.includeBodyDetails = e.target.checked;
    persistAndUpdate();
  });

  $('#customOutfit').addEventListener('input', (e) => {
    state.customOutfit = e.target.value;
    persistAndUpdate();
  });

  $('#customPose').addEventListener('input', (e) => {
    state.customPose = e.target.value;
    persistAndUpdate();
  });

  $('#promptOutput').addEventListener('input', () => {
    promptEdited = true;
  });

  $('#negativeOutput').addEventListener('input', () => {
    negativeEdited = true;
  });

  $('#btnRegenerate').addEventListener('click', () => {
    promptEdited = false;
    negativeEdited = false;
    updatePromptOutput();
  });

  $('#btnCopy').addEventListener('click', () => copyText($('#promptOutput').value, '#btnCopy'));
  $('#btnCopyNegative').addEventListener('click', () => copyText($('#negativeOutput').value, '#btnCopyNegative'));

  $('#btnReset').addEventListener('click', () => {
    if (!confirm('Reset all selectors to defaults?')) return;
    state = resetState();
    promptEdited = false;
    negativeEdited = false;
    initUI();
    persistAndUpdate();
  });

  $('#btnRandom').addEventListener('click', () => {
    state = randomizeState(state, features, { nsfw: state.showNsfw });
    promptEdited = false;
    negativeEdited = false;
    initUI();
    persistAndUpdate();
  });

  $('#btnSavePreset').addEventListener('click', savePreset);
  $('#btnLoadPreset').addEventListener('click', loadPreset);
  $('#btnDeletePreset').addEventListener('click', deletePreset);

  refreshPresetList();
  bindImageGeneration();
}

function renderImageSettings() {
  const resolutionSelect = $('#imageResolution');
  resolutionSelect.innerHTML = '';
  for (const res of RESOLUTIONS) {
    const opt = document.createElement('option');
    opt.value = res.value;
    opt.textContent = res.label;
    if (res.value === state.imageResolution) opt.selected = true;
    resolutionSelect.appendChild(opt);
  }

  $('#apiKey').value = loadApiKey();
  $('#imageSeed').value = state.imageSeed ?? -1;
}

function setImageStatus(message, isError = false) {
  const el = $('#imageStatus');
  el.hidden = !message;
  el.textContent = message || '';
  el.classList.toggle('is-error', isError);
}

function showGeneratedImage(url) {
  lastImageUrl = url;
  const img = $('#generatedImage');
  img.src = url;
  $('#btnDownloadImage').href = url;
  $('#imageResult').hidden = false;
}

function bindImageGeneration() {
  renderImageSettings();

  $('#apiKey').addEventListener('change', (e) => {
    saveApiKey(e.target.value);
  });

  $('#apiKey').addEventListener('blur', (e) => {
    saveApiKey(e.target.value);
  });

  $('#imageResolution').addEventListener('change', (e) => {
    state.imageResolution = e.target.value;
    saveState(state);
  });

  $('#btnSuggestResolution').addEventListener('click', () => {
    const suggested = suggestResolution(state.cameraAngle);
    state.imageResolution = suggested;
    $('#imageResolution').value = suggested;
    saveState(state);
    setImageStatus(`Resolution set to ${suggested} for ${state.cameraAngle.replace(/_/g, ' ')}.`);
  });

  $('#btnOpenImage').addEventListener('click', () => {
    if (lastImageUrl) window.open(lastImageUrl, '_blank', 'noopener');
  });

  $('#btnGenerateImage').addEventListener('click', async () => {
    const btn = $('#btnGenerateImage');
    const apiKey = $('#apiKey').value;
    saveApiKey(apiKey);

    const prompt = $('#promptOutput').value.trim();
    const resolution = $('#imageResolution').value;
    const seed = Number.parseInt($('#imageSeed').value, 10);

    btn.disabled = true;
    $('#imageResult').hidden = true;
    setImageStatus('Starting…');

    try {
      const { url } = await generateImage({
        apiKey,
        prompt,
        resolution,
        seed: Number.isFinite(seed) ? seed : -1,
        nsfwThreshold: state.nsfwThreshold ?? 0.5,
        onStatus: setImageStatus
      });
      showGeneratedImage(url);
      setImageStatus('Image generated successfully.');
    } catch (err) {
      setImageStatus(err.message, true);
    } finally {
      btn.disabled = false;
    }
  });
}

async function copyText(text, btnSelector) {
  try {
    await navigator.clipboard.writeText(text);
    const btn = $(btnSelector);
    const original = btn.textContent;
    btn.textContent = 'Copied!';
    setTimeout(() => { btn.textContent = original; }, 1500);
  } catch {
    alert('Copy failed — select the text manually.');
  }
}

function refreshPresetList() {
  const presets = loadPresets();
  const select = $('#presetList');
  select.innerHTML = '<option value="">— Select preset —</option>';
  for (const name of Object.keys(presets).sort()) {
    const opt = document.createElement('option');
    opt.value = name;
    opt.textContent = name;
    select.appendChild(opt);
  }
}

function savePreset() {
  const name = $('#presetName').value.trim();
  if (!name) {
    alert('Enter a preset name first.');
    return;
  }
  const presets = loadPresets();
  presets[name] = { ...state };
  savePresets(presets);
  refreshPresetList();
  $('#presetList').value = name;
  $('#presetName').value = '';
}

function loadPreset() {
  const name = $('#presetList').value;
  if (!name) return;
  const presets = loadPresets();
  if (!presets[name]) return;
  state = { ...state, ...presets[name] };
  promptEdited = false;
  negativeEdited = false;
  initUI();
  persistAndUpdate();
}

function deletePreset() {
  const name = $('#presetList').value;
  if (!name) return;
  if (!confirm(`Delete preset "${name}"?`)) return;
  const presets = loadPresets();
  delete presets[name];
  savePresets(presets);
  refreshPresetList();
}

function initUI() {
  renderSimpleFields($('#bodyFields'), [
    'age', 'race', 'physique', 'breastSize', 'buttSize',
    'hairColor', 'hairLength', 'eyeColor', 'shaved'
  ]);
  renderSimpleFields($('#styleFields'), [
    'imageStyle', 'lighting', 'backdrop', 'cameraAngle'
  ]);
  renderOutfitFields();
  renderPoseFields();
  renderToggles();
  renderImageSettings();
}

async function main() {
  try {
    await loadData();
    initUI();
    bindGlobalControls();
    updatePromptOutput();
  } catch (err) {
    document.body.innerHTML = `<div class="error-banner">
      <h2>Failed to load app data</h2>
      <p>${err.message}</p>
      <p>Try running via a local server: <code>cd prompt-builder && python3 -m http.server 8080</code></p>
    </div>`;
  }
}

main();
