const STORAGE_KEY = 'prompt-builder-state';
const PRESETS_KEY = 'prompt-builder-presets';

export const defaultState = {
  age: '23-28',
  race: 'unspecified',
  physique: 'athletic',
  breastSize: 'medium',
  buttSize: 'medium',
  hairColor: 'brunette',
  hairLength: 'long',
  eyeColor: 'brown',
  shaved: 'fully_shaved',
  imageStyle: 'photorealistic',
  lighting: 'soft_natural',
  backdrop: 'bedroom',
  cameraAngle: 'full_body',
  outfitCategory: 'lingerie',
  outfitDetail: 'lace_bra_panties',
  poseCategory: 'standing',
  poseDetail: 'hands_on_hips',
  showNsfw: false,
  includeShaved: true,
  includeBodyDetails: true,
  customOutfit: '',
  customPose: '',
  imageModel: 'NSFW-flux-lora',
  imageResolution: '768x1024',
  imageQuality: 'fast'
};

export function loadState() {
  try {
    const saved = localStorage.getItem(STORAGE_KEY);
    if (saved) {
      return { ...defaultState, ...JSON.parse(saved) };
    }
  } catch {
    /* ignore corrupt storage */
  }
  return { ...defaultState };
}

export function saveState(state) {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(state));
}

export function loadPresets() {
  try {
    const saved = localStorage.getItem(PRESETS_KEY);
    if (saved) return JSON.parse(saved);
  } catch {
    /* ignore */
  }
  return {};
}

export function savePresets(presets) {
  localStorage.setItem(PRESETS_KEY, JSON.stringify(presets));
}

export function resetState() {
  localStorage.removeItem(STORAGE_KEY);
  return { ...defaultState };
}
