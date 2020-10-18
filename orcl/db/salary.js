const _ = require('lodash');
const { parseDate, strToDate } = require('./utils');

async function getSalaries(conn, employeeId) {
  const result = await conn.execute(`
  SELECT id, od, do, suma 
  FROM TABLE(
    SELECT platy 
    FROM zamestnanec 
    WHERE id_zam = :employeeId
  ) ORDER BY od ASC`, { employeeId });
  const salaries = _.map(result.rows, ([id, fromDate, toDate, salary]) => ({
    id,
    fromDate: parseDate(fromDate),
    toDate: parseDate(toDate),
    salary,
  }));
  return salaries;
}

async function insertSalary(conn, salary) {
  const params = _.merge({}, salary, {
    fromDate: strToDate(salary.fromDate),
  });
  const result = await conn.execute(`  
  BEGIN
    insert_plat(:id, :salary, :fromDate);
  END;`, params);
  return result;
}

async function deleteSalary(conn, salary) {
  const result = await conn.execute(`  
  DELETE FROM TABLE(
    SELECT platy
    FROM zamestnanec
    WHERE id_zam = :employeeId
  ) WHERE id = :salaryId`, salary);
  return result;
}

async function getSalaryStat(conn, numYears = 5) {
  const thisYear = new Date().getFullYear();
  const years = _.map(_.rangeRight(numYears), i => thisYear - i);
  const queries = _.map(years, year => `SELECT ${year} as year, SUM(z.get_rocny_plat(${year})) as salary FROM zamestnanec z`);
  const query = _.join(queries, ' UNION ALL ');
  const result = await conn.execute(query);
  const data = _.map(result.rows, ([year, salary]) => ({
    year,
    salary,
  }));
  return data;
}

module.exports = {
  getSalaries,
  insertSalary,
  deleteSalary,
  getSalaryStat,
};
