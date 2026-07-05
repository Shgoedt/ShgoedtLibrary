const API_BASE = 'https://api.sinancode.com/v1';
const GENERATE_PATH = '/generate/hf/z-image-turbo';
const POLL_INTERVAL_MS = 2000;
const MAX_POLL_ATTEMPTS = 120;

export const RESOLUTIONS = [
  { value: '1024x1024', label: 'Square 1:1 (1024×1024)' },
  { value: '1280x720', label: 'Landscape 16:9 (1280×720)' },
  { value: '720x1280', label: 'Portrait 9:16 (720×1280)' },
  { value: '896x1152', label: 'Portrait 3:4 (896×1152)' },
  { value: '1152x896', label: 'Landscape 4:3 (1152×896)' },
  { value: '1280x1280', label: 'Large square (1280×1280)' },
  { value: '1536x1024', label: 'Wide 3:2 (1536×1024)' }
];

const API_KEY_STORAGE = 'prompt-builder-sinancode-api-key';

export function loadApiKey() {
  return localStorage.getItem(API_KEY_STORAGE) || '';
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

function extractImageUrl(result) {
  if (!result?.length) return null;
  const first = result[0];
  return typeof first === 'string' ? first : first?.url;
}

async function parseError(response) {
  try {
    const data = await response.json();
    return data.message || data.error_msg || data.error_code || `HTTP ${response.status}`;
  } catch {
    return `HTTP ${response.status}`;
  }
}

export function suggestResolution(cameraAngle) {
  const portrait = new Set(['full_body', 'three_quarter', 'close_up', 'eye_level']);
  const landscape = new Set(['low_angle', 'high_angle']);
  if (cameraAngle === 'from_behind') return '896x1152';
  if (portrait.has(cameraAngle)) return '720x1280';
  if (landscape.has(cameraAngle)) return '1280x720';
  return '1024x1024';
}

export async function generateImage({
  apiKey,
  prompt,
  resolution = '1024x1024',
  seed = -1,
  onStatus
}) {
  if (!apiKey?.trim()) {
    throw new Error('Enter your SinanCode API key first (free at sinancode.com).');
  }
  if (!prompt?.trim()) {
    throw new Error('Prompt is empty — build a prompt first.');
  }

  onStatus?.('Submitting to Z-Image Turbo…');

  const createRes = await fetch(`${API_BASE}${GENERATE_PATH}`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${apiKey.trim()}`
    },
    body: JSON.stringify({
      prompt: prompt.trim(),
      resolution,
      num_images: 1,
      seed
    })
  });

  if (!createRes.ok) {
    throw new Error(await parseError(createRes));
  }

  const { task_id: taskId } = await createRes.json();
  if (!taskId) throw new Error('No task ID returned from API.');

  onStatus?.('Queued…');

  for (let attempt = 0; attempt < MAX_POLL_ATTEMPTS; attempt++) {
    await sleep(POLL_INTERVAL_MS);

    const statusRes = await fetch(`${API_BASE}/tasks/${taskId}`, {
      headers: { Authorization: `Bearer ${apiKey.trim()}` }
    });

    if (!statusRes.ok) {
      throw new Error(await parseError(statusRes));
    }

    const task = await statusRes.json();

    if (task.status === 'completed') {
      const url = extractImageUrl(task.result);
      if (!url) throw new Error('Generation finished but no image URL was returned.');
      onStatus?.('Complete');
      return { url, taskId };
    }

    if (task.status === 'failed') {
      throw new Error(task.error_msg || task.error_code || 'Image generation failed.');
    }

    const label = task.status === 'processing' ? 'Generating image…' : 'Waiting in queue…';
    onStatus?.(`${label} (${attempt + 1}/${MAX_POLL_ATTEMPTS})`);
  }

  throw new Error('Timed out — try again in a moment.');
}
