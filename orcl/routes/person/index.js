const person = require('./person');
const bmData = require('./bm-data');

module.exports = (router, pool) => {
  person(router, pool);
  bmData(router, pool);
};
