import { loadState, saveState, resetState, loadPresets, savePresets } from './state.js';
import { buildPrompt, buildNegativePrompt, randomizeState } from './promptEngine.js';
import {
  generateImage,
  loadApiKey,
  saveApiKey,
  MODELS,
  RESOLUTIONS,
  QUALITIES,
  suggestModel,
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
  const modelSelect = $('#imageModel');
  modelSelect.innerHTML = '';
  for (const item of MODELS) {
    const opt = document.createElement('option');
    opt.value = item.value;
    opt.textContent = item.label;
    if (item.value === state.imageModel) opt.selected = true;
    modelSelect.appendChild(opt);
  }

  const resolutionSelect = $('#imageResolution');
  resolutionSelect.innerHTML = '';
  for (const item of RESOLUTIONS) {
    const opt = document.createElement('option');
    opt.value = item.value;
    opt.textContent = item.label;
    if (item.value === state.imageResolution) opt.selected = true;
    resolutionSelect.appendChild(opt);
  }

  const qualitySelect = $('#imageQuality');
  qualitySelect.innerHTML = '';
  for (const item of QUALITIES) {
    const opt = document.createElement('option');
    opt.value = item.value;
    opt.textContent = item.label;
    if (item.value === state.imageQuality) opt.selected = true;
    qualitySelect.appendChild(opt);
  }

  $('#apiKey').value = loadApiKey();
  updateApiKeyBadge();
}

function updateApiKeyBadge() {
  const hasKey = Boolean(loadApiKey());
  $('#apiKeySaved').hidden = !hasKey;
}

function setImageGenBusy(busy) {
  $('#btnGenerateImage').disabled = busy;
  $('#btnSuggestModel').disabled = busy;
  $('#btnSuggestResolution').disabled = busy;
  $('#apiKey').disabled = busy;
  $('#imageModel').disabled = busy;
  $('#imageResolution').disabled = busy;
  $('#imageQuality').disabled = busy;

  const btn = $('#btnGenerateImage');
  if (busy) {
    btn.dataset.originalLabel = btn.textContent;
    btn.textContent = 'Generating…';
  } else if (btn.dataset.originalLabel) {
    btn.textContent = btn.dataset.originalLabel;
  }
}

function setImageProgress({ phase, message, progress, active }) {
  const wrap = $('#imageProgress');
  const bar = $('#imageProgressBar');
  const label = $('#imageProgressLabel');
  const percent = $('#imageProgressPercent');
  const track = wrap.querySelector('.image-progress-track');

  wrap.hidden = !active;
  if (!active) return;

  const rounded = Math.round(progress);
  label.textContent = message;
  percent.textContent = `${rounded}%`;
  bar.style.width = `${rounded}%`;
  wrap.dataset.phase = phase;
  track.setAttribute('aria-valuenow', String(rounded));
}

function hideImageProgress() {
  $('#imageProgress').hidden = true;
}

function setImageStatus(message, isError = false) {
  const el = $('#imageStatus');
  el.hidden = !message;
  el.textContent = message || '';
  el.classList.toggle('is-error', isError);
  el.classList.toggle('is-success', !isError && Boolean(message));
}

function showGeneratedImage(url) {
  lastImageUrl = url;
  const img = $('#generatedImage');
  img.src = url;
  $('#btnDownloadImage').href = url;
  $('#imageResult').hidden = false;
}

function bindImageGeneration() {
  if ($('#panel-output').dataset.imageGenBound) return;
  $('#panel-output').dataset.imageGenBound = 'true';

  renderImageSettings();

  let apiKeySaveTimer;

  const persistApiKey = (value) => {
    saveApiKey(value);
    updateApiKeyBadge();
  };

  $('#apiKey').addEventListener('input', (e) => {
    clearTimeout(apiKeySaveTimer);
    apiKeySaveTimer = setTimeout(() => persistApiKey(e.target.value), 300);
  });

  $('#apiKey').addEventListener('change', (e) => {
    persistApiKey(e.target.value);
  });

  $('#apiKey').addEventListener('blur', (e) => {
    persistApiKey(e.target.value);
  });

  $('#imageModel').addEventListener('change', (e) => {
    state.imageModel = e.target.value;
    saveState(state);
  });

  $('#imageResolution').addEventListener('change', (e) => {
    state.imageResolution = e.target.value;
    saveState(state);
  });

  $('#imageQuality').addEventListener('change', (e) => {
    state.imageQuality = e.target.value;
    saveState(state);
  });

  $('#btnSuggestModel').addEventListener('click', () => {
    const suggested = suggestModel(state.imageStyle);
    state.imageModel = suggested;
    $('#imageModel').value = suggested;
    saveState(state);
    setImageStatus(`Model set to ${suggested}.`);
  });

  $('#btnSuggestResolution').addEventListener('click', () => {
    const suggested = suggestResolution(state.cameraAngle);
    state.imageResolution = suggested;
    $('#imageResolution').value = suggested;
    saveState(state);
    setImageStatus(`Resolution set to ${suggested}.`);
  });

  $('#btnOpenImage').addEventListener('click', () => {
    if (lastImageUrl) window.open(lastImageUrl, '_blank', 'noopener');
  });

  $('#btnGenerateImage').addEventListener('click', async () => {
    const apiKey = $('#apiKey').value;
    persistApiKey(apiKey);

    const prompt = $('#promptOutput').value.trim();
    const negativePrompt = $('#negativeOutput').value.trim();
    const modelId = $('#imageModel').value;
    const resolution = $('#imageResolution').value;
    const quality = $('#imageQuality').value;

    setImageGenBusy(true);
    $('#imageResult').hidden = true;
    setImageStatus('');
    setImageProgress({ phase: 'submitting', message: 'Starting…', progress: 0, active: true });

    try {
      const { url } = await generateImage({
        apiKey,
        prompt,
        negativePrompt,
        modelId,
        resolution,
        quality,
        onProgress: setImageProgress
      });
      showGeneratedImage(url);
      setImageProgress({ phase: 'complete', message: 'Complete', progress: 100, active: true });
      setImageStatus('Image generated successfully.');
      setTimeout(hideImageProgress, 1200);
    } catch (err) {
      hideImageProgress();
      setImageStatus(err.message, true);
    } finally {
      setImageGenBusy(false);
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
