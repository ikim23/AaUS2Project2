const employee = require('./employee');
const salary = require('./salary');

module.exports = (router, pool) => {
  employee(router, pool);
  salary(router, pool);
};
