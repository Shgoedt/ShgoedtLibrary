const API_BASE = 'https://modelslab.com/api/v6/images';
const LEGACY_KEY_STORAGES = [
  'prompt-builder-kwiki-api-key',
  'prompt-builder-sinancode-api-key'
];
const API_KEY_STORAGE = 'prompt-builder-modelslab-api-key';

const POLL_INTERVAL_MS = 3000;
const MAX_POLL_ATTEMPTS = 60;

export const MODELS = [
  { value: 'NSFW-flux-lora', label: 'NSFW Flux (realistic)' },
  { value: 'omnigenxl-nsfw-sfw', label: 'OmniGenXL (versatile)' }
];

export const RESOLUTIONS = [
  { value: '1024x1024', label: 'Square 1024×1024', width: 1024, height: 1024 },
  { value: '768x1024', label: 'Portrait 768×1024', width: 768, height: 1024 },
  { value: '1024x768', label: 'Landscape 1024×768', width: 1024, height: 768 },
  { value: '512x768', label: 'Portrait 512×768 (faster)', width: 512, height: 768 }
];

export const QUALITIES = [
  { value: 'fast', label: 'Fast (25 steps)' },
  { value: 'hires', label: 'Hi-res (40 steps)' }
];

const QUALITY_STEPS = { fast: 25, hires: 40 };

export function loadApiKey() {
  const current = localStorage.getItem(API_KEY_STORAGE);
  if (current) return current;
  for (const legacy of LEGACY_KEY_STORAGES) {
    const key = localStorage.getItem(legacy);
    if (key) return key;
  }
  return '';
}

export function saveApiKey(key) {
  if (key?.trim()) {
    localStorage.setItem(API_KEY_STORAGE, key.trim());
  } else {
    localStorage.removeItem(API_KEY_STORAGE);
  }
}

function sleep(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

function parseResolution(value) {
  const preset = RESOLUTIONS.find((r) => r.value === value);
  if (preset) return { width: preset.width, height: preset.height };
  const [w, h] = value.split('x').map(Number);
  if (w && h) return { width: w, height: h };
  return { width: 1024, height: 1024 };
}

async function parseError(response, data) {
  if (data) {
    return data.message || data.error || data.status || `HTTP ${response.status}`;
  }
  try {
    const json = await response.json();
    return json.message || json.error || `HTTP ${response.status}`;
  } catch {
    return `HTTP ${response.status}`;
  }
}

export function suggestModel(imageStyle) {
  if (imageStyle === 'anime') return 'omnigenxl-nsfw-sfw';
  return 'NSFW-flux-lora';
}

export function suggestResolution(cameraAngle) {
  const portrait = new Set(['full_body', 'three_quarter', 'close_up', 'eye_level', 'from_behind']);
  const landscape = new Set(['low_angle', 'high_angle']);
  if (landscape.has(cameraAngle)) return '1024x768';
  if (portrait.has(cameraAngle)) return '768x1024';
  return '1024x1024';
}

async function pollForResult(fetchUrl, apiKey, onProgress) {
  for (let attempt = 0; attempt < MAX_POLL_ATTEMPTS; attempt++) {
    await sleep(POLL_INTERVAL_MS);

    const response = await fetch(fetchUrl, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ key: apiKey.trim() })
    });

    const data = await response.json();

    if (data.status === 'success' && data.output?.[0]) {
      return data.output[0];
    }

    if (data.status === 'error' || data.status === 'failed') {
      throw new Error(await parseError(response, data));
    }

    const progress = 22 + ((attempt + 1) / MAX_POLL_ATTEMPTS) * 73;
    const eta = data.eta ? ` ~${data.eta}s left` : '';
    onProgress?.({
      phase: data.status === 'processing' ? 'processing' : 'queued',
      message: `Generating image… (${attempt + 1}/${MAX_POLL_ATTEMPTS})${eta}`,
      progress,
      active: true
    });
  }

  throw new Error('Timed out — try again in a moment.');
}

export async function generateImage({
  apiKey,
  prompt,
  negativePrompt = '',
  modelId = 'NSFW-flux-lora',
  resolution = '1024x1024',
  quality = 'fast',
  onProgress
}) {
  if (!apiKey?.trim()) {
    throw new Error('Enter your ModelsLab API key first (free at modelslab.com).');
  }
  if (!prompt?.trim()) {
    throw new Error('Prompt is empty — build a prompt first.');
  }

  const { width, height } = parseResolution(resolution);
  const steps = QUALITY_STEPS[quality] ?? 25;

  onProgress?.({
    phase: 'submitting',
    message: 'Submitting to ModelsLab…',
    progress: 8,
    active: true
  });

  const response = await fetch(`${API_BASE}/text2img`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      key: apiKey.trim(),
      model_id: modelId,
      prompt: prompt.trim(),
      negative_prompt: negativePrompt.trim() || undefined,
      width: String(width),
      height: String(height),
      samples: '1',
      num_inference_steps: String(steps),
      safety_checker: 'no',
      enhance_prompt: 'yes',
      guidance_scale: 7.5
    })
  });

  const data = await response.json();

  if (!response.ok && data.status !== 'processing') {
    throw new Error(await parseError(response, data));
  }

  if (data.status === 'success' && data.output?.[0]) {
    onProgress?.({ phase: 'complete', message: 'Image ready', progress: 100, active: true });
    return { url: data.output[0], id: data.id };
  }

  if (data.status === 'processing') {
    onProgress?.({
      phase: 'queued',
      message: data.message || 'Queued — generating…',
      progress: 18,
      active: true
    });

    const fetchUrl = data.fetch_result
      || `${API_BASE}/fetch/${data.id}`;

    const url = await pollForResult(fetchUrl, apiKey, onProgress);
    onProgress?.({ phase: 'complete', message: 'Image ready', progress: 100, active: true });
    return { url, id: data.id };
  }

  throw new Error(await parseError(response, data));
}
