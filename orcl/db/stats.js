const _ = require('lodash');

async function getCounts(conn) {
  const tables = ['krajina', 'region', 'mesto', 'obvod', 'zamestnanec', 'osoba', 'bm_udaje', 'ucastnik_cinu', 'trestny_cin'];
  const query = _.join(_.map(tables, table => `SELECT '${table}', COUNT(*) as count FROM ${table}`), ' UNION ');
  const result = await conn.execute(`${query} ORDER BY count DESC`);
  return _.map(result.rows, ([table, count]) => ({ table, count }));
}

module.exports = {
  getCounts,
};
