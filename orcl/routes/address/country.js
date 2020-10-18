const _ = require('lodash');
const {
  getCountries,
  insertCountry,
  deleteCountry,
} = require('../../db/address');
const { addCountryForm } = require('../../validation/address');

const VIEW_DIR = 'address/country';

module.exports = (router, pool) => {
  router.get('/countries', async (req, res) => {
    const conn = await pool.getConnection();
    const countries = await getCountries(conn);
    await conn.close();
    res.render(`${VIEW_DIR}/countries`, { countries });
  });

  router.get('/add/country', (req, res) => {
    const form = req.session.addCountryForm;
    if (form) {
      req.session.addCountryForm = null;
      res.render(`${VIEW_DIR}/country-insert`, form);
    } else {
      res.render(`${VIEW_DIR}/country-insert`);
    }
  });

  router.post('/add/country', async (req, res) => {
    const data = req.body;
    addCountryForm.validate(data, async (err, country) => {
      if (err) {
        req.session.addCountryForm = {
          country,
          invalid: _.set({}, err.details[0].context.key, true),
        };
        res.redirect('/add/country');
      } else {
        const conn = await pool.getConnection();
        await insertCountry(conn, country);
        await conn.close();
        res.redirect('/countries');
      }
    });
  });

  router.post('/delete/country/:id', async (req, res) => {
    const countryId = req.params.id;
    const conn = await pool.getConnection();
    await deleteCountry(conn, countryId);
    await conn.close();
    res.redirect('/countries');
  });
};
