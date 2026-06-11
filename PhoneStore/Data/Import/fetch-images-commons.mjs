// Добор фото для моделей без изображения — поиск по Wikimedia Commons С ПРОВЕРКОЙ имени файла
// (принимаем результат, только если имя файла содержит название модели — отсекает мусор).
import fs from 'fs';
const DATASET = new URL('./phones-dataset.json', import.meta.url);
const data = JSON.parse(fs.readFileSync(DATASET, 'utf8'));

// модель -> страховочная строка, которая ДОЛЖНА быть в имени файла
const guard = {
  'Poco X6 Pro': 'poco x6 pro', 'Poco F6': 'poco f6', 'Redmi 13C': 'redmi 13c',
  'Pixel 8 Pro': 'pixel 8 pro', 'Pixel 8': 'pixel 8',
  'OnePlus 12': 'oneplus 12', 'OnePlus 11': 'oneplus 11', 'OnePlus Nord 3': 'nord 3',
  'Honor 90': 'honor 90', 'Honor X9b': 'honor x9b',
  'Realme GT 5 Pro': 'gt 5 pro', 'Realme 12 Pro+': 'realme 12 pro', 'Realme 12+': 'realme 12',
  'Oppo Reno11': 'reno11', 'Vivo V30': 'vivo v30',
  'Motorola Edge 50 Pro': 'edge 50 pro', 'Motorola Razr 40 Ultra': 'razr 40 ultra',
  'Asus Zenfone 10': 'zenfone 10', 'Tecno Camon 30': 'camon 30',
  'Infinix Note 40': 'infinix note 40', 'Infinix Zero 30': 'zero 30', 'Nubia Z60 Ultra': 'z60 ultra',
};

const collapse = s => s.toLowerCase().replace(/[^a-z0-9]+/g, ' ').trim();
const nospace  = s => s.toLowerCase().replace(/[^a-z0-9]+/g, '');
const sleep = ms => new Promise(r => setTimeout(r, ms));

async function search(query, guardStr) {
  const p = new URLSearchParams({
    action: 'query', format: 'json', generator: 'search', gsrnamespace: '6', gsrlimit: '6',
    gsrsearch: query, prop: 'imageinfo', iiprop: 'url|mime', iiurlwidth: '800',
  });
  const r = await fetch('https://commons.wikimedia.org/w/api.php?' + p.toString(),
    { headers: { 'User-Agent': 'PhoneStoreImport/1.0 (demo; contact dev)' } });
  const j = await r.json();
  const pages = Object.values((j.query && j.query.pages) || {}).sort((a, b) => a.index - b.index);
  for (const it of pages) {
    const ii = it.imageinfo && it.imageinfo[0];
    if (!ii || !/jpeg|png/.test(ii.mime)) continue;
    const name = it.title.replace(/^File:/, '');
    if (collapse(name).includes(collapse(guardStr)) || nospace(name).includes(nospace(guardStr)))
      return ii.thumburl || ii.url;
  }
  return null;
}

let added = 0; const still = [];
for (const model of data) {
  if (model.images && model.images.length) continue;
  const g = guard[model.title];
  if (!g) { still.push(model.title); continue; }
  let url = null;
  try { url = await search(model.title + ' smartphone', g); } catch {}
  if (!url) { try { url = await search(g, g); } catch {} }
  if (url) { model.images = [url]; added++; console.log('OK  ', model.title, '->', url.split('/').pop()); }
  else { still.push(model.title); }
  await sleep(400);
}

fs.writeFileSync(DATASET, JSON.stringify(data, null, 2) + '\n', 'utf8');
const total = data.filter(m => m.images && m.images.length).length;
console.log(`\nДобавлено: ${added}. Всего с фото: ${total}/${data.length}.`);
console.log(`Остались на плейсхолдере: ${still.length ? still.join(', ') : '—'}`);
