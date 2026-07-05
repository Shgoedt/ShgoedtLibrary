const API_BASE = 'https://kwikiai.com/api/v1';
const GENERATE_PATH = '/generate';
const LEGACY_API_KEY_STORAGE = 'prompt-builder-sinancode-api-key';
const API_KEY_STORAGE = 'prompt-builder-kwiki-api-key';

export const STYLES = [
  { value: 'realistic', label: 'Realistic' },
  { value: 'anime', label: 'Anime' },
  { value: 'nude', label: 'Nude' }
];

export const QUALITIES = [
  { value: 'fast', label: 'Fast (1 credit)' },
  { value: 'hires', label: 'Hi-res (2 credits)' }
];

export function loadApiKey() {
  return localStorage.getItem(API_KEY_STORAGE)
    || localStorage.getItem(LEGACY_API_KEY_STORAGE)
    || '';
}

export function saveApiKey(key) {
  if (key?.trim()) {
    localStorage.setItem(API_KEY_STORAGE, key.trim());
  } else {
    localStorage.removeItem(API_KEY_STORAGE);
  }
}

async function parseError(response) {
  try {
    const data = await response.json();
    return data.error || data.message || data.detail || `HTTP ${response.status}`;
  } catch {
    return `HTTP ${response.status}`;
  }
}

export function suggestStyle(imageStyle, outfitCategory) {
  if (imageStyle === 'anime') return 'anime';
  if (outfitCategory === 'nude') return 'nude';
  return 'realistic';
}

function startProgressTicker(onProgress, start = 12, cap = 92) {
  let progress = start;
  onProgress?.({
    phase: 'processing',
    message: 'Generating image… this can take up to 90 seconds',
    progress,
    active: true
  });

  return setInterval(() => {
    progress = Math.min(cap, progress + (progress < 50 ? 2 : 1));
    onProgress?.({
      phase: 'processing',
      message: 'Generating image… this can take up to 90 seconds',
      progress,
      active: true
    });
  }, 1200);
}

export async function generateImage({
  apiKey,
  prompt,
  style = 'realistic',
  quality = 'fast',
  onProgress
}) {
  if (!apiKey?.trim()) {
    throw new Error('Enter your Kwiki AI API key first (free at kwikiai.com).');
  }
  if (!prompt?.trim()) {
    throw new Error('Prompt is empty — build a prompt first.');
  }

  onProgress?.({
    phase: 'submitting',
    message: 'Submitting to Kwiki AI…',
    progress: 8,
    active: true
  });

  const ticker = startProgressTicker(onProgress);

  try {
    const response = await fetch(`${API_BASE}${GENERATE_PATH}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${apiKey.trim()}`
      },
      body: JSON.stringify({
        prompt: prompt.trim(),
        style,
        quality
      })
    });

    clearInterval(ticker);

    if (!response.ok) {
      throw new Error(await parseError(response));
    }

    const data = await response.json();
    const imageUrl = data.image_url || data.url || data.result?.[0]?.url;

    if (data.status && data.status !== 'completed' && !imageUrl) {
      throw new Error(data.message || `Generation ended with status: ${data.status}`);
    }

    if (!imageUrl) {
      throw new Error('Generation finished but no image URL was returned.');
    }

    onProgress?.({ phase: 'complete', message: 'Image ready', progress: 100, active: true });
    return { url: imageUrl, id: data.id };
  } catch (err) {
    clearInterval(ticker);
    throw err;
  }
}
