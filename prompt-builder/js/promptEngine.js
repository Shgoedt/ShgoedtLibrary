const NUDE_OUTFITS = new Set(['fully_nude', 'topless', 'bottomless', 'artistic_nude']);

function lookup(templates, category, key) {
  return templates[category]?.[key] ?? '';
}

function lookupNested(templates, category, group, key) {
  return templates[category]?.[group]?.[key] ?? '';
}

function shouldIncludeShaved(state) {
  if (!state.includeShaved) return false;
  const isNude = state.outfitCategory === 'nude' ||
    NUDE_OUTFITS.has(state.outfitDetail);
  return isNude || state.includeShaved;
}

export function buildPrompt(state, templates, features) {
  const parts = [];

  const agePhrase = lookup(templates, 'age', state.age);
  if (agePhrase) parts.push(agePhrase);

  const racePhrase = lookup(templates, 'race', state.race);
  if (racePhrase) parts.push(racePhrase);

  const physiquePhrase = lookup(templates, 'physique', state.physique);
  if (physiquePhrase) parts.push(physiquePhrase);

  if (state.includeBodyDetails) {
    const breastPhrase = lookup(templates, 'breastSize', state.breastSize);
    if (breastPhrase) parts.push(breastPhrase);

    const buttPhrase = lookup(templates, 'buttSize', state.buttSize);
    if (buttPhrase) parts.push(buttPhrase);
  }

  const hairColor = lookup(templates, 'hairColor', state.hairColor);
  const hairLength = lookup(templates, 'hairLength', state.hairLength);
  if (state.hairLength === 'bald') {
    parts.push('bald head');
  } else if (hairLength && hairColor) {
    parts.push(`${hairLength} ${hairColor} hair`);
  }

  const eyePhrase = lookup(templates, 'eyeColor', state.eyeColor);
  if (eyePhrase) parts.push(eyePhrase);

  if (shouldIncludeShaved(state)) {
    const shavedPhrase = lookup(templates, 'shaved', state.shaved);
    if (shavedPhrase) parts.push(shavedPhrase);
  }

  if (state.customOutfit?.trim()) {
    parts.push(state.customOutfit.trim());
  } else {
    const outfitPhrase = lookupNested(templates, 'outfits', state.outfitCategory, state.outfitDetail);
    if (outfitPhrase) parts.push(outfitPhrase);
  }

  if (state.customPose?.trim()) {
    parts.push(state.customPose.trim());
  } else {
    const posePhrase = lookupNested(templates, 'poses', state.poseCategory, state.poseDetail);
    if (posePhrase) parts.push(posePhrase);
  }

  const backdropPhrase = lookup(templates, 'backdrop', state.backdrop);
  if (backdropPhrase) parts.push(backdropPhrase);

  const lightingPhrase = lookup(templates, 'lighting', state.lighting);
  if (lightingPhrase) parts.push(lightingPhrase);

  const cameraPhrase = lookup(templates, 'cameraAngle', state.cameraAngle);
  if (cameraPhrase) parts.push(cameraPhrase);

  const stylePhrase = lookup(templates, 'imageStyle', state.imageStyle);
  if (stylePhrase) parts.push(stylePhrase);

  if (features.qualityTags?.length) {
    parts.push(features.qualityTags.join(', '));
  }

  return parts.filter(Boolean).join(', ');
}

export function buildNegativePrompt(features, extras = []) {
  const defaults = features.negativeDefaults ?? [];
  return [...defaults, ...extras].join(', ');
}

export function randomizeState(state, features, { nsfw = false } = {}) {
  const pick = (arr) => arr[Math.floor(Math.random() * arr.length)].value;
  const pickKey = (obj, filter) => {
    const keys = Object.keys(obj).filter(filter ?? (() => true));
    return keys[Math.floor(Math.random() * keys.length)];
  };

  const outfitCategory = pickKey(features.outfits);
  const outfitItems = features.outfits[outfitCategory].items;

  const poseFilter = nsfw ? () => true : (k) => !features.poses[k].nsfw;
  const poseCategory = pickKey(features.poses, poseFilter);
  const poseItems = features.poses[poseCategory].items;

  return {
    ...state,
    age: pick(features.age),
    race: pick(features.race),
    physique: pick(features.physique),
    breastSize: pick(features.breastSize),
    buttSize: pick(features.buttSize),
    hairColor: pick(features.hairColor),
    hairLength: pick(features.hairLength),
    eyeColor: pick(features.eyeColor),
    shaved: pick(features.shaved),
    imageStyle: pick(features.imageStyle),
    lighting: pick(features.lighting),
    backdrop: pick(features.backdrop),
    cameraAngle: pick(features.cameraAngle),
    outfitCategory,
    outfitDetail: pick(outfitItems),
    poseCategory,
    poseDetail: pick(poseItems),
    customOutfit: '',
    customPose: ''
  };
}
