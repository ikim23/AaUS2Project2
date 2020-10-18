const _ = require('lodash');
const { getEmployee } = require('../../db/employee');
const {
  insertSalary,
  deleteSalary,
  getSalaryStat,
} = require('../../db/salary');
const { addSalaryForm } = require('../../validation/employee');

const VIEW_DIR = 'employee';

module.exports = (router, pool) => {
  router.get('/add/salary/:id', async (req, res) => {
    const employeeId = _.parseInt(req.params.id);
    const conn = await pool.getConnection();
    const employee = await getEmployee(conn, employeeId);
    await conn.close();
    const form = req.session.addSalaryForm;
    if (form) {
      req.session.editEmployeeForm = null;
      res.render(`${VIEW_DIR}/employee-add-salary`, _.assign(form, { employee }));
    } else {
      res.render(`${VIEW_DIR}/employee-add-salary`, { employee });
    }
  });

  router.post('/add/salary/:id', async (req, res) => {
    const employeeId = _.parseInt(req.params.id);
    const data = _.merge({}, req.body, { id: employeeId });
    addSalaryForm.validate(data, async (err, salary) => {
      if (err) {
        req.session.addSalaryForm = {
          salary,
          invalid: _.set({}, err.details[0].context.key, true),
        };
        res.redirect(`/add/salary/${employeeId}`);
      } else {
        const conn = await pool.getConnection();
        await insertSalary(conn, salary);
        await conn.close();
        res.redirect(`/employee/${employeeId}`);
      }
    });
  });

  router.post('/delete/salary/:employeeId/:salaryId', async (req, res) => {
    const employeeId = _.parseInt(req.params.employeeId);
    const salaryId = _.parseInt(req.params.salaryId);
    const conn = await pool.getConnection();
    await deleteSalary(conn, { employeeId, salaryId });
    await conn.close();
    res.redirect(`/employee/${employeeId}`);
  });

  router.get('/employees/salary-stats', (req, res) => {
    res.render(`${VIEW_DIR}/employees-salary-stats`);
  });

  router.get('/employees/salary', async (req, res) => {
    const conn = await pool.getConnection();
    const statData = await getSalaryStat(conn);
    await conn.close();
    const years = _.map(statData, 'year');
    const salaries = _.map(statData, 'salary');
    res.json({
      type: 'line',
      data: {
        labels: years,
        datasets: [{
          label: 'Ročné náklady na mzdy zamestnancov',
          data: salaries,
        }],
      },
      options: {
        responsive: false,
        scaleLabel: '<%=value%>%',
      },
    });
  });
};
