const _ = require('lodash');
const { parseDate, strToDate } = require('./utils');

async function getCrimes(conn, startIdx = 1, endIdx = 50, query = null) {
  let result;
  if (query) {
    result = await conn.execute(`
    SELECT rn, id_cinu, druh, popis, popis_filename, hodnota_skody, objasneny, datum_zapisu, datum_uzavretia, id_obvodu, nazov
    FROM (
      SELECT id_cinu, druh, popis, popis_filename, hodnota_skody, objasneny, datum_zapisu, datum_uzavretia, id_obvodu, nazov,
      ROW_NUMBER() OVER (order by druh) as rn
      FROM trestny_cin
      JOIN obvod USING (id_obvodu)
      WHERE LOWER(druh) LIKE LOWER('%${query}')
    ) WHERE rn BETWEEN :startIdx AND :endIdx`, { startIdx, endIdx });
  } else {
    result = await conn.execute(`
    SELECT rn, id_cinu, druh, popis, popis_filename, hodnota_skody, objasneny, datum_zapisu, datum_uzavretia, id_obvodu, nazov
    FROM (
      SELECT id_cinu, druh, popis, popis_filename, hodnota_skody, objasneny, datum_zapisu, datum_uzavretia, id_obvodu, nazov,
      ROW_NUMBER() OVER (order by druh) as rn
      FROM trestny_cin
      JOIN obvod USING (id_obvodu)
    ) WHERE rn BETWEEN :startIdx AND :endIdx`, { startIdx, endIdx });
  }
  const crimes = _.map(result.rows, ([rowNumber, id, type, description, descFileName, damageValue, explained, openDate, closeDate, districtId, district]) => ({
    rowNumber,
    id,
    type,
    description,
    descFileName,
    damageValue,
    explained,
    openDate: parseDate(openDate),
    closeDate: parseDate(closeDate),
    districtId,
    district,
  }));
  return crimes;
}

async function getCrimesCount(conn, query = null) {
  let result;
  if (query) {
    result = await conn.execute(`
    SELECT COUNT(*)
    FROM trestny_cin
    WHERE LOWER(druh) LIKE LOWER('%${query}')`);
  } else {
    result = await conn.execute(`
    SELECT COUNT(*)
    FROM trestny_cin`);
  }
  return result.rows[0];
}

async function getCrimeIds(conn) {
  const result = await conn.execute(`
  SELECT id_cinu
  FROM trestny_cin`);
  const crimeIds = _.map(result.rows, ([crimeId]) => ({ crimeId }));
  return crimeIds;
}

async function getCrimesSeason(conn, params) {
  const fromD = params.fromDate;
  const toD = params.toDate;
  const typ = params.type;
  const result = await conn.execute(`
  SELECT id_cinu, druh,
  hodnota_skody, objasneny, datum_zapisu, datum_uzavretia
  FROM trestny_cin
  WHERE datum_zapisu BETWEEN to_date(:fromD, 'yyyy-mm-dd') AND to_date(:toD, 'yyyy-mm-dd')
  AND LOWER(druh) LIKE LOWER(:typ)`, { fromD, toD, typ });
  const crimes = _.map(result.rows, ([id, type, damageValue, explained, openDate, closeDate]) => ({
    id,
    type,
    damageValue,
    explained,
    openDate: parseDate(openDate),
    closeDate: parseDate(closeDate),
  }));
  return crimes;
}

async function getCrime(conn, crimeid) {
  const result = await conn.execute(`
  SELECT id_cinu, druh, popis_filename,
  hodnota_skody, objasneny, datum_zapisu, datum_uzavretia,
  id_obvodu, nazov
  FROM trestny_cin
  JOIN obvod USING (id_obvodu)
  WHERE id_cinu = :crimeid`, { crimeid });
  // const [id, type, descFileName, damageValue, explained, openDate,
  //   closeDate, districtId, district] = result.row[0];
  const crime = result.rows[0];
  return {
    id: crime[0],
    type: crime[1],
    descFileName: _.isNull(crime[2]) ? '' : crime[2],
    damageValue: crime[3],
    explained: crime[4],
    openDate: parseDate(crime[5]),
    closeDate: parseDate(crime[6]),
    districtId: crime[7],
    district: crime[8],
  };
}

async function insertCrime(conn, crime) {
  const result = await conn.execute(`
  BEGIN
  insert_crime(
    :districtId,
    :type,
    :description,
    :descFileName,
    :damageValue,
    :explained,
    :openDate,
    :closeDate
  );
  END;`, crime);
  return result;
}

async function updateCrime(conn, crime) {
  const result = await conn.execute(`
  BEGIN
    update_crime(
      :id,
      :districtId,
      :type,
      :description,
      :descFileName,
      :damageValue,
      :explained,
      :openDate,
      :closeDate
    );
  END;`, crime);
  return result;
}

async function deleteCrime(conn, id) {
  const result = await conn.execute(`
  BEGIN
    delete_trestny_cin(:id);
  END;`, { id });
  return result;
}

module.exports = {
  getCrimes,
  getCrimesCount,
  getCrimeIds,
  getCrimesSeason,
  getCrime,
  insertCrime,
  updateCrime,
  deleteCrime,
};
