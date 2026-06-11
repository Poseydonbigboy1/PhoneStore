// Разовый скрипт: подтягивает реальные фото телефонов из Wikipedia (pageimages API)
// и записывает их в phones-dataset.json (поле "images"). Запуск: node fetch-images.mjs
import fs from 'fs';

const DATASET = new URL('./phones-dataset.json', import.meta.url);

// dataset title -> Wikipedia article title
const wikiMap = {
  'iPhone 15 Pro Max': 'iPhone 15 Pro', 'iPhone 15 Pro': 'iPhone 15 Pro',
  'iPhone 15 Plus': 'iPhone 15', 'iPhone 15': 'iPhone 15',
  'iPhone 14 Pro Max': 'iPhone 14 Pro', 'iPhone 14': 'iPhone 14', 'iPhone 14 Plus': 'iPhone 14',
  'iPhone 13': 'iPhone 13', 'iPhone 13 Pro Max': 'iPhone 13 Pro', 'iPhone 13 Pro': 'iPhone 13 Pro',
  'iPhone 12': 'iPhone 12', 'iPhone SE 2022': 'iPhone SE (3rd generation)',

  'Galaxy S24 Ultra': 'Samsung Galaxy S24 Ultra', 'Galaxy S24+': 'Samsung Galaxy S24', 'Galaxy S24': 'Samsung Galaxy S24',
  'Galaxy S23 Ultra': 'Samsung Galaxy S23 Ultra', 'Galaxy S23': 'Samsung Galaxy S23', 'Galaxy S23 FE': 'Samsung Galaxy S23 FE',
  'Galaxy A55': 'Samsung Galaxy A55', 'Galaxy A35': 'Samsung Galaxy A35', 'Galaxy A25': 'Samsung Galaxy A25', 'Galaxy A15': 'Samsung Galaxy A15',
  'Galaxy S22 Ultra': 'Samsung Galaxy S22 Ultra', 'Galaxy Z Fold5': 'Samsung Galaxy Z Fold 5', 'Galaxy Z Flip5': 'Samsung Galaxy Z Flip 5',

  'Xiaomi 14 Ultra': 'Xiaomi 14 Ultra', 'Xiaomi 14': 'Xiaomi 14', 'Xiaomi 13T Pro': 'Xiaomi 13T',
  'Redmi Note 13 Pro+': 'Redmi Note 13', 'Redmi Note 13': 'Redmi Note 13',
  'Poco X6 Pro': 'Poco X6 Pro', 'Poco F6': 'Poco F6', 'Xiaomi 13': 'Xiaomi 13', 'Redmi 13C': 'Redmi 13C',

  'Pixel 8 Pro': 'Google Pixel 8 Pro', 'Pixel 8': 'Google Pixel 8', 'Pixel 8a': 'Google Pixel 8a', 'Pixel 7a': 'Google Pixel 7a',

  'OnePlus 12': 'OnePlus 12', 'OnePlus 12R': 'OnePlus 12R', 'OnePlus 11': 'OnePlus 11', 'OnePlus Nord 3': 'OnePlus Nord 3',

  'Magic6 Pro': 'Honor Magic 6 Pro', 'Honor 90': 'Honor 90', 'Honor X9b': 'Honor X9b',
  'Realme GT 5 Pro': 'Realme GT 5 Pro', 'Realme 12 Pro+': 'Realme 12 Pro', 'Realme 12+': 'Realme 12',
  'Find X7 Ultra': 'Oppo Find X7', 'Oppo Reno11': 'Oppo Reno11',
  'Vivo X100 Pro': 'Vivo X100', 'Vivo V30': 'Vivo V30',
  'Nothing Phone (2)': 'Nothing Phone (2)', 'Nothing Phone (2a)': 'Nothing Phone (2a)',
  'Motorola Edge 50 Pro': 'Motorola Edge 50 Pro', 'Motorola Razr 40 Ultra': 'Motorola Razr 40 Ultra',
  'Sony Xperia 1 V': 'Sony Xperia 1 V',
  'Asus ROG Phone 8 Pro': 'Asus ROG Phone 8', 'Asus Zenfone 10': 'Asus Zenfone 10',
  'Tecno Camon 30': 'Tecno Camon 30', 'Infinix Note 40': 'Infinix Note 40', 'Infinix Zero 30': 'Infinix Zero 30',
  'Nubia Z60 Ultra': 'Nubia Z60 Ultra',
};

const data = JSON.parse(fs.readFileSync(DATASET, 'utf8'));

const wikiTitles = [...new Set(Object.values(wikiMap))];

async function fetchBatch(titles) {
  const params = new URLSearchParams({
    action: 'query', format: 'json', prop: 'pageimages', piprop: 'thumbnail',
    pithumbsize: '800', redirects: '1', titles: titles.join('|'),
  });
  const url = 'https://en.wikipedia.org/w/api.php?' + params.toString();
  const res = await fetch(url, { headers: { 'User-Agent': 'PhoneStoreImport/1.0 (demo)' } });
  return res.json();
}

const titleToThumb = {}; // resolved title -> url
const normMap = {};      // input -> normalized
const redirMap = {};     // normalized -> redirect target

for (let i = 0; i < wikiTitles.length; i += 40) {
  const batch = wikiTitles.slice(i, i + 40);
  const j = await fetchBatch(batch);
  const q = j.query || {};
  (q.normalized || []).forEach(n => normMap[n.from] = n.to);
  (q.redirects || []).forEach(r => redirMap[r.from] = r.to);
  for (const k in (q.pages || {})) {
    const p = q.pages[k];
    if (p.thumbnail && p.thumbnail.source) titleToThumb[p.title] = p.thumbnail.source;
  }
}

function resolve(t) {
  if (normMap[t]) t = normMap[t];
  if (redirMap[t]) t = redirMap[t];
  return titleToThumb[t] || null;
}

let found = 0, missing = [];
for (const model of data) {
  const wt = wikiMap[model.title];
  const url = wt ? resolve(wt) : null;
  if (url) { model.images = [url]; found++; }
  else { delete model.images; missing.push(model.title); }
}

fs.writeFileSync(DATASET, JSON.stringify(data, null, 2) + '\n', 'utf8');
console.log(`Найдено фото: ${found} / ${data.length}`);
console.log(`Без фото (плейсхолдер): ${missing.length ? missing.join(', ') : '—'}`);
