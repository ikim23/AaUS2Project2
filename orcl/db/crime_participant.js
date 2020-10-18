const _ = require('lodash');
const { parseDate, strToDate } = require('./utils');

async function getParticipants(conn, startIdx = 1, endIdx = 50, query = null) {
  let result;
  if (query) {
    result = await conn.execute(`
    SELECT rn, meno, priezvisko, rod_cislo, psc, ulica, nazov, id_cinu, typ, od, do,
    svedectvo, svedectvo_filename, dovod, dovod_filename, dat_nastupu, dlzka_trestu, miesto_vyk_trestu
    FROM (
      SELECT meno, priezvisko, rod_cislo, psc, ulica, nazov, id_cinu, typ, od, do,
      svedectvo, svedectvo_filename, dovod, dovod_filename, dat_nastupu, dlzka_trestu, miesto_vyk_trestu,
      ROW_NUMBER() OVER (order by priezvisko, meno) as rn
      FROM ucastnik_cinu
      JOIN osoba USING (rod_cislo)
      JOIN mesto USING (psc)
      WHERE LOWER(meno || ' ' || priezvisko) LIKE LOWER('%${query}')
    ) WHERE rn BETWEEN :startIdx AND :endIdx`, { startIdx, endIdx });
  } else {
    result = await conn.execute(`
    SELECT rn, meno, priezvisko, rod_cislo, psc, ulica, nazov, id_cinu, typ, od, do,
    svedectvo, svedectvo_filename, dovod, dovod_filename, dat_nastupu, dlzka_trestu, miesto_vyk_trestu
    FROM (
      SELECT meno, priezvisko, rod_cislo, psc, ulica, nazov, id_cinu, typ, od, do,
      svedectvo, svedectvo_filename, dovod, dovod_filename, dat_nastupu, dlzka_trestu, miesto_vyk_trestu,
      ROW_NUMBER() OVER (order by priezvisko, meno) as rn
      FROM ucastnik_cinu
      JOIN osoba USING (rod_cislo)
      JOIN mesto USING (psc)
    ) WHERE rn BETWEEN :startIdx AND :endIdx`, { startIdx, endIdx });
  }
  const participants = _.map(result.rows, ([rowNumber, name, surname, personalNumber, cityCode, street, city, crimeId, type,
    fromDate, toDate, witness, witnessFileName, reason, reasonFileName, date, punishmentLength, punishmentPlace]) => ({
    rowNumber,
    name,
    surname,
    personalNumber,
    cityCode,
    street,
    city,
    crimeId,
    type,
    fromDate: parseDate(fromDate),
    toDate: parseDate(toDate),
    witness,
    witnessFileName,
    reason,
    reasonFileName,
    date: parseDate(date),
    punishmentLength,
    punishmentPlace,
  }));
  return participants;
}

async function getParticipantsCount(conn, query = null) {
  let result;
  if (query) {
    result = await conn.execute(`
    SELECT COUNT(*)
    FROM ucastnik_cinu
    JOIN osoba USING (rod_cislo)
    WHERE LOWER(meno || ' ' || priezvisko) LIKE LOWER('%${query}')`);
  } else {
    result = await conn.execute(`
    SELECT COUNT(*)
    FROM ucastnik_cinu`);
  }
  return result.rows[0];
}

async function getParticipant(conn, pn, id, type, from) {
  const fr = strToDate(from);
  const result = await conn.execute(`
  SELECT meno, priezvisko, rod_cislo, psc, ulica, nazov, id_cinu, typ, od, do,
  svedectvo, svedectvo_filename, dovod, dovod_filename, dat_nastupu, dlzka_trestu, miesto_vyk_trestu
  FROM ucastnik_cinu
  JOIN osoba USING (rod_cislo)
  JOIN mesto USING (psc)
  WHERE rod_cislo = :pn
  AND id_cinu = :id
  AND od = :fr
  AND typ = :type`, { pn, id, fr, type });
  const participant = result.rows[0];
  return {
    name: participant[0],
    surname: participant[1],
    personalNumber: participant[2],
    cityCode: participant[3],
    street: participant[4],
    city: participant[5],
    crimeId: participant[6],
    type: participant[7],
    fromDate: parseDate(participant[8]),
    toDate: parseDate(participant[9]),
    witness: participant[10],
    witnessFileName: participant[11],
    reason: participant[12],
    reasonFileName: participant[13],
    date: parseDate(participant[14]),
    punishmentLength: participant[15],
    punishmentPlace: participant[16],
  };
}

async function insertParticipant(conn, participant) {
  const params = _.merge({}, participant, {
    fromDate: strToDate(participant.fromDate),
    toDate: strToDate(participant.toDate),
    date: strToDate(participant.date),
  });
  const result = await conn.execute(`
  BEGIN
    insert_participant(
      :personalNumber,
      :crimeId,
      :type,
      :fromDate,
      :toDate,
      :witness,
      :witnessFileName,
      :reason,
      :reasonFileName,
      :date,
      :punishmentLength,
      :punishmentPlace
    );
  END;`, params);
  return result;
}

async function updateParticipant(conn, participant) {
  const params = _.merge({}, participant, {
    fromDate: strToDate(participant.fromDate),
    toDate: strToDate(participant.toDate),
    date: strToDate(participant.date),
  });
  const result = await conn.execute(`
  BEGIN
    update_participant(
      :personalNumber,
      :crimeId,
      :type,
      :fromDate,
      :toDate,
      :witness,
      :witnessFileName,
      :reason,
      :reasonFileName,
      :date,
      :punishmentLength,
      :punishmentPlace
    );
  END;`, params);
  return result;
}

async function deleteParticipant(conn, pn, id, type, from) {
  const result = await conn.execute(`
  BEGIN
    delete_participant(:pn, :id, :type, :from);
  END;`, { pn, id, type, from: strToDate(from) });
  return result;
}

module.exports = {
  getParticipants,
  getParticipantsCount,
  getParticipant,
  insertParticipant,
  updateParticipant,
  deleteParticipant,
};
