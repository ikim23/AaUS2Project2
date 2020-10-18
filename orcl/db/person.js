const _ = require('lodash');
const fileType = require('file-type');

async function getPersons(conn, startIdx = 1, endIdx = 50, query = null) {
  let result;
  if (query) {
    result = await conn.execute(`
    SELECT rn, rod_cislo, meno, priezvisko 
    FROM (
      SELECT rod_cislo, meno, priezvisko,
      ROW_NUMBER() OVER (order by priezvisko, meno) as rn
      FROM osoba
      WHERE LOWER(meno || ' ' || priezvisko) LIKE LOWER('%${query}%')
    ) WHERE rn BETWEEN :startIdx AND :endIdx`, { startIdx, endIdx });
  } else {
    result = await conn.execute(`
    SELECT rn, rod_cislo, meno, priezvisko 
    FROM (
      SELECT rod_cislo, meno, priezvisko,
      ROW_NUMBER() OVER (order by priezvisko, meno) as rn
      FROM osoba
    ) WHERE rn BETWEEN :startIdx AND :endIdx`, { startIdx, endIdx });
  }
  const persons = _.map(result.rows, ([rowNumber, personalNumber, name, surname]) => ({
    rowNumber,
    personalNumber,
    name,
    surname,
  }));
  return persons;
}

async function getPersonsCount(conn, query = null) {
  let result;
  if (query) {
    result = await conn.execute(`
    SELECT COUNT(*)
    FROM osoba
    WHERE LOWER(meno || ' ' || priezvisko) LIKE LOWER('%${query}%')`);
  } else {
    result = await conn.execute('SELECT COUNT(*) FROM osoba');
  }
  return result.rows[0];
}
async function getPersonalNumbers(conn) {
  const result = await conn.execute(`
  SELECT rod_cislo
  FROM osoba`);
  const personalNumbers = _.map(result.rows, ([personalNumber]) => ({ personalNumber }));
  return personalNumbers;
}

async function getPerson(conn, personalNumber) {
  const result = await conn.execute(`
  SELECT rod_cislo, meno, priezvisko, ulica, psc, nazov, fotka, fotka_filename
  FROM osoba
  JOIN mesto USING (psc)
  LEFT JOIN bm_udaje USING (rod_cislo)
  WHERE rod_cislo = :personalNumber`, { personalNumber });
  const person = result.rows[0];
  return {
    personalNumber: person[0],
    name: person[1],
    surname: person[2],
    street: person[3],
    cityCode: person[4],
    city: person[5],
    photo: _.isNull(person[6]) ? null : person[6].toString('base64'),
    photoMimeType: _.isNull(person[6]) ? null : fileType(person[6]).mime,
    photoFilename: person[7],
  };
}

async function insertPerson(conn, person) {
  const result = await conn.execute(`
  INSERT INTO osoba(meno, priezvisko, rod_cislo, psc, ulica) 
  VALUES(:name, :surname, :personalNumber, :cityCode, :street)`, person);
  return result;
}

async function updatePerson(conn, person) {
  const result = await conn.execute(`
  UPDATE osoba
  SET meno = :name,
  priezvisko = :surname,
  psc = :cityCode,
  ulica = :street
  WHERE rod_cislo = :personalNumber`, person);
  return result;
}

async function deletePerson(conn, personalNumber) {
  const result = await conn.execute(`
  BEGIN
    delete_osoba(:personalNumber);
  END;`, { personalNumber });
  return result;
}

async function getBmData(conn, personalNumber) {
  const result = await conn.execute(`
  SELECT rod_cislo, vyska, vaha, farba_pleti, farba_vlasov, farba_oci,
    fotka, fotka_filename, odtlacok_prsta, odtlacok_prsta_filename
  FROM bm_udaje
  WHERE rod_cislo = :personalNumber`, { personalNumber });
  if (result.rows.length === 0) return null;
  const data = result.rows[0];
  return {
    personalNumber,
    height: data[1],
    weight: data[2],
    skinColor: data[3],
    hairColor: data[4],
    eyeColor: data[5],
    photo: _.isNull(data[6]) ? null : data[6].toString('base64'),
    photoMimeType: _.isNull(data[6]) ? null : fileType(data[6]).mime,
    photoFilename: data[7],
    fingerprint: _.isNull(data[8]) ? null : data[8].toString('base64'),
    fingerprintMimeType: _.isNull(data[8]) ? null : fileType(data[8]).mime,
    fingerprintFilename: data[9],
  };
}

async function insertBmData(conn, data) {
  const result = await conn.execute(`
  INSERT INTO bm_udaje(rod_cislo, vyska, vaha, farba_pleti, farba_vlasov, farba_oci,
    fotka, fotka_filename, odtlacok_prsta, odtlacok_prsta_filename)
  VALUES(:personalNumber, :height, :weight, :skinColor, :hairColor, :eyeColor,
    :photo, :photoFilename, :fingerprint, :fingerprintFilename)`, data);
  return result;
}

async function updateBmData(conn, data) {
  const result = await conn.execute(`
  UPDATE bm_udaje
  SET vyska = :height,
  vaha = :weight,
  farba_pleti = :skinColor,
  farba_vlasov = :hairColor,
  farba_oci = :eyeColor,
  fotka = :photo,
  fotka_filename = :photoFilename,
  odtlacok_prsta = :fingerprint,
  odtlacok_prsta_filename = :fingerprintFilename
  WHERE rod_cislo = :personalNumber`, data);
  return result;
}

async function deleteBmData(conn, personalNumber) {
  const result = await conn.execute(`
  DELETE FROM bm_udaje
  WHERE rod_cislo = :personalNumber`, { personalNumber });
  return result;
}

module.exports = {
  getPersons,
  getPersonsCount,
  getPersonalNumbers,
  getPerson,
  insertPerson,
  updatePerson,
  deletePerson,
  getBmData,
  insertBmData,
  updateBmData,
  deleteBmData,
};
