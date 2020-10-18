const country = require('./country');
const region = require('./region');
const city = require('./city');
const district = require('./district');

module.exports = (router, pool) => {
  country(router, pool);
  region(router, pool);
  city(router, pool);
  district(router, pool);
};
