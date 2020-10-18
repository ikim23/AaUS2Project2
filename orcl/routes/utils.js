const _ = require('lodash');

function nullIfEmpty(str) {
  if (_.isString(str) && str.length === 0) return null;
  return str;
}

function getPages(urlBase, cur, last, recordsPerPage, query) {
  const numbers = [1, cur - 2, cur - 1, cur, cur + 1, cur + 2, last];
  const pageNums = _.uniq(_.filter(numbers, p => p > 0 && p <= last));
  if (pageNums.length === 1) return [];
  const q = query ? `&q=${query}` : '';
  const pages = _.map(pageNums, page => ({
    page,
    actual: page === cur,
    url: `${urlBase}?r=${recordsPerPage}&p=${page}${q}`,
  }));
  pages[0] = _.assign(pages[0], { page: 'Prvá' });
  const lastIdx = pages.length - 1;
  pages[lastIdx] = _.assign(pages[lastIdx], { page: 'Posledná' });
  return pages;
}

function getCrimeNames() {
  return ['kradez', 'vrazda', 'lupez', 'podvod', 'sprenevera', 'zabitie', 'uzera', 'pytliactvo', 'vlastizrada', 'vydieranie', 'unos', 'podplacanie', 'vytrznictvo', 'prevadzacstvo', 'kupliarstvo'];
}

module.exports = {
  nullIfEmpty,
  getPages,
  getCrimeNames,
};
