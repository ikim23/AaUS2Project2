const _ = require('lodash');
const oracledb = require('oracledb');
const { parseDate, strToDate } = require('./utils');

async function getEmployees(conn, startIdx = 1, endIdx = 50, query = null) {
  let result;
  if (query) {
    result = await conn.execute(`
    SELECT rn, id_zam, meno, priezvisko, rod_cislo, hodnost, funkcia, od, do
    FROM (
      SELECT id_zam, meno, priezvisko, rod_cislo, hodnost, funkcia, od, do,
      ROW_NUMBER() OVER (order by priezvisko, meno) as rn
      FROM osoba
      JOIN zamestnanec USING (rod_cislo)
      WHERE LOWER(meno || ' ' || priezvisko) LIKE LOWER('%${query}%')
    ) WHERE rn BETWEEN :startIdx AND :endIdx`, { startIdx, endIdx });
  } else {
    result = await conn.execute(`
    SELECT rn, id_zam, meno, priezvisko, rod_cislo, hodnost, funkcia, od, do
    FROM (
      SELECT id_zam, meno, priezvisko, rod_cislo, hodnost, funkcia, od, do,
      ROW_NUMBER() OVER (order by priezvisko, meno) as rn
      FROM osoba
      JOIN zamestnanec USING (rod_cislo)
    ) WHERE rn BETWEEN :startIdx AND :endIdx`, { startIdx, endIdx });
  }
  const employees = _.map(result.rows, ([rowNumber, id, name, surname, personalNumber, rank, position, fromDate, toDate]) => ({
    rowNumber,
    id,
    name,
    surname,
    personalNumber,
    position,
    rank,
    fromDate: parseDate(fromDate),
    toDate: parseDate(toDate),
  }));
  return employees;
}

async function getEmployeesCount(conn, query = null) {
  let result;
  if (query) {
    result = await conn.execute(`
    SELECT COUNT(*)
    FROM zamestnanec
    JOIN osoba USING (rod_cislo)
    WHERE LOWER(meno || ' ' || priezvisko) LIKE LOWER('%${query}%')`);
  } else {
    result = await conn.execute('SELECT COUNT(*) FROM zamestnanec');
  }
  return result.rows[0];
}

async function getEmployee(conn, id) {
  const result = await conn.execute(`
  SELECT meno, priezvisko, rod_cislo, funkcia, hodnost, od, do, ulica, psc, nazov
  FROM zamestnanec 
  JOIN osoba USING (rod_cislo) 
  JOIN mesto USING (psc)
  WHERE id_zam = :id`, { id });
  const [name, surname, personalNumber, position, rank, fromDate, toDate, street, cityCode, city] = result.rows[0];
  return {
    id,
    name,
    surname,
    personalNumber,
    position,
    rank,
    fromDate: parseDate(fromDate),
    toDate: parseDate(toDate),
    street,
    cityCode,
    city,
  };
}

async function insertEmployee(conn, employee) {
  const params = _.merge({}, employee, {
    fromDate: strToDate(employee.fromDate),
    employeeId: { dir: oracledb.BIND_OUT, type: oracledb.NUMBER },
  });
  const result = await conn.execute(`
  BEGIN 
    insert_zamestnanec(
      :name, :surname, :personalNumber, :cityCode, :street,
      :position, :rank, :fromDate, :salary, :employeeId
    );
  END;`, params);
  return result.outBinds.employeeId;
}

async function updateEmployee(conn, employee) {
  const params = _.merge({}, employee, {
    fromDate: strToDate(employee.fromDate),
    toDate: strToDate(employee.toDate),
  });
  const result = await conn.execute(`
  BEGIN 
    update_zamestnanec(
      :id, :name, :surname, :cityCode, :street, :position, :rank, :fromDate, :toDate
    );
  END;`, params);
  return result;
}

async function deleteEmployee(conn, id) {
  const result = await conn.execute(`
  BEGIN
    delete_zamestnanec(:id);
  END;`, { id });
  return result;
}

module.exports = {
  getEmployees,
  getEmployeesCount,
  getEmployee,
  insertEmployee,
  updateEmployee,
  deleteEmployee,
};
