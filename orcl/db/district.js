const _ = require('lodash');

async function getDistricts(conn) {
  const result = await conn.execute(`
  SELECT id_obvodu, nazov
  FROM obvod`);
  const districts = _.map(result.rows, ([districtId, district]) => ({ districtId, district }));
  return districts;
}

async function insertDistrict(conn, district) {
  const result = await conn.execute(`
  INSERT INTO obvod(id_obvodu, nazov, psc)
  VALUES(:name, :name, :cityCode)`, district);
  return result;
}

module.exports = {
  getDistricts,
  insertDistrict,
};
