const _ = require('lodash');
const {
  getEmployees,
  getEmployeesCount,
  getEmployee,
  insertEmployee,
  updateEmployee,
  deleteEmployee,
} = require('../../db/employee');
const { getSalaries } = require('../../db/salary');
const { getCities } = require('../../db/address');
const { addEmployeeForm, editEmployeeForm } = require('../../validation/employee');
const { nullIfEmpty, getPages } = require('../utils');

const VIEW_DIR = 'employee';

async function getSelectedCities(conn, employee) {
  const allCities = await getCities(conn);
  const cities = _.map(allCities, (city) => {
    if (city.cityCode === employee.cityCode) {
      return _.assign(city, { selected: true });
    }
    return city;
  });
  return cities;
}

module.exports = (router, pool) => {
  router.get('/employees', async (req, res) => {
    const query = _.get(req, 'query.q');
    const recordsPerPage = _.parseInt(req.query.r) || 25;
    const page = _.parseInt(req.query.p) || 1;
    const start = ((page - 1) * recordsPerPage) + 1;
    const end = (start + recordsPerPage) - 1;
    const conn = await pool.getConnection();
    const employees = await getEmployees(conn, start, end, query);
    const count = await getEmployeesCount(conn, query);
    const lastPage = _.ceil(count / recordsPerPage);
    const pages = getPages('/employees', page, lastPage, recordsPerPage, query);
    await conn.close();
    res.render(`${VIEW_DIR}/employees`, { employees, pages });
  });

  router.get('/employee/:id', async (req, res, next) => {
    const employeeId = _.parseInt(req.params.id);
    if (_.isNaN(employeeId)) {
      next();
    } else {
      const conn = await pool.getConnection();
      const employee = await getEmployee(conn, employeeId);
      const salaries = await getSalaries(conn, employeeId);
      await conn.close();
      res.render(`${VIEW_DIR}/employee`, { employee, salaries });
    }
  });

  router.get('/add/employee', async (req, res) => {
    const conn = await pool.getConnection();
    const cities = await getCities(conn);
    const form = req.session.addEmployeeForm;
    if (form) {
      req.session.addEmployeeForm = null;
      res.render(`${VIEW_DIR}/employee-insert`, _.merge({}, { cities }, form));
    } else {
      await conn.close();
      res.render(`${VIEW_DIR}/employee-insert`, { cities });
    }
  });

  router.post('/add/employee', async (req, res) => {
    const data = _.merge({}, req.body, {
      street: nullIfEmpty(req.body.street),
      rank: nullIfEmpty(req.body.rank),
      salary: _.parseInt(req.body.salary),
    });
    addEmployeeForm.validate(data, async (err, employee) => {
      if (err) {
        req.session.addEmployeeForm = {
          employee,
          invalid: _.set({}, err.details[0].context.key, true),
        };
        res.redirect('/add/employee');
      } else {
        const conn = await pool.getConnection();
        const employeeId = await insertEmployee(conn, employee);
        await conn.close();
        res.redirect(`/employee/${employeeId}`);
      }
    });
  });

  router.get('/edit/employee/:id', async (req, res, next) => {
    const form = req.session.editEmployeeForm;
    if (form) {
      req.session.editEmployeeForm = null;
      const conn = await pool.getConnection();
      const cities = await getSelectedCities(conn, form.employee);
      await conn.close();
      res.render(`${VIEW_DIR}/employee-edit`, _.merge({}, { cities }, form));
    } else {
      const employeeId = _.parseInt(req.params.id);
      if (_.isNaN(employeeId)) {
        next();
      } else {
        const conn = await pool.getConnection();
        const employee = await getEmployee(conn, employeeId);
        const cities = await getSelectedCities(conn, employee);
        await conn.close();
        res.render(`${VIEW_DIR}/employee-edit`, { employee, cities });
      }
    }
  });

  router.post('/edit/employee/:id', async (req, res) => {
    const employeeId = _.parseInt(req.params.id);
    const data = _.merge({}, req.body, {
      id: employeeId,
      street: nullIfEmpty(req.body.street),
      rank: nullIfEmpty(req.body.rank),
      toDate: nullIfEmpty(req.body.toDate),
    });
    editEmployeeForm.validate(data, async (err, employee) => {
      if (err) {
        req.session.editEmployeeForm = {
          employee,
          invalid: _.set({}, err.details[0].context.key, true),
        };
        res.redirect(`/edit/employee/${employeeId}`);
      } else {
        const conn = await pool.getConnection();
        await updateEmployee(conn, employee);
        await conn.close();
        res.redirect(`/employee/${employeeId}`);
      }
    });
  });

  router.post('/delete/employee/:id', async (req, res) => {
    const employeeId = _.parseInt(req.params.id);
    const conn = await pool.getConnection();
    await deleteEmployee(conn, employeeId);
    await conn.close();
    res.redirect('/employees');
  });
};
