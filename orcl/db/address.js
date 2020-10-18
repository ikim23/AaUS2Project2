const _ = require('lodash');

async function getCountries(conn) {
  const result = await conn.execute(`
  SELECT id_krajiny, nazov, ROW_NUMBER() OVER (ORDER BY id_krajiny) as rn
  FROM krajina`);
  return _.map(result.rows, ([id, name, rowNumber]) => ({ rowNumber, id, name }));
}

async function insertCountry(conn, country) {
  const result = await conn.execute(`
  INSERT INTO krajina(id_krajiny, nazov)
  VALUES(:id, :name)`, country);
  return result;
}

async function deleteCountry(conn, id) {
  const result = await conn.execute(`
  DELETE FROM krajina
  WHERE id_krajiny = :id`, { id });
  return result;
}

async function getRegions(conn) {
  const result = await conn.execute(`
  SELECT id_krajiny, id_regionu, r.nazov, ROW_NUMBER() OVER (ORDER BY id_krajiny, id_regionu) as rn
  FROM region r
  JOIN krajina USING (id_krajiny)`);
  return _.map(result.rows, ([countryId, regionId, name, rowNumber]) => ({
    rowNumber,
    countryId,
    regionId,
    name,
  }));
}

async function insertRegion(conn, country) {
  const result = await conn.execute(`
  INSERT INTO region(id_krajiny, id_regionu, nazov)
  VALUES(:countryId, :regionId, :name)`, country);
  return result;
}

async function deleteRegion(conn, id) {
  const result = await conn.execute(`
  DELETE FROM region
  WHERE id_regionu = :id`, { id });
  return result;
}

async function getCities(conn) {
  const result = await conn.execute(`
  SELECT nazov, psc, id_regionu, ROW_NUMBER() OVER (ORDER BY nazov) as rn
  FROM mesto`);
  const cities = _.map(result.rows, ([city, cityCode, regionId, rowNumber]) => ({
    city,
    cityCode,
    regionId,
    rowNumber,
  }));
  return cities;
}

async function insertCity(conn, city) {
  const result = await conn.execute(`
  INSERT INTO mesto(id_regionu, psc, nazov)
  VALUES(:regionId, :cityCode, :city)`, city);
  return result;
}

async function deleteCity(conn, id) {
  const result = await conn.execute(`
  DELETE FROM mesto
  WHERE psc = :id`, { id });
  return result;
}

async function getDistricts(conn) {
  const result = await conn.execute(`
  SELECT id_obvodu, nazov, psc, ROW_NUMBER() OVER (order by id_obvodu) as rn
  FROM obvod`);
  return _.map(result.rows, ([districtId, district, cityCode, rowNumber]) => ({ rowNumber, districtId, district, cityCode }));
}

async function insertDistrict(conn, district) {
  const result = await conn.execute(`
  INSERT INTO obvod(psc, nazov, id_obvodu)
  VALUES(:cityCode, :district, :districtId)`, district);
  return result;
}

async function deleteDistrict(conn, districtId) {
  const result = await conn.execute(`
  DELETE FROM obvod
  WHERE id_obvodu = :districtId`, { districtId });
  return result;
}

module.exports = {
  getCountries,
  insertCountry,
  deleteCountry,
  getRegions,
  insertRegion,
  deleteRegion,
  getCities,
  insertCity,
  deleteCity,
  getDistricts,
  insertDistrict,
  deleteDistrict,
};
