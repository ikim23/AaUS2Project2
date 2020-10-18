const _ = require('lodash');
const {
  getRegions,
  getCities,
  insertCity,
  deleteCity,
} = require('../../db/address');
const { addCityForm } = require('../../validation/address');

const VIEW_DIR = 'address/city';

module.exports = (router, pool) => {
  router.get('/cities', async (req, res) => {
    const conn = await pool.getConnection();
    const cities = await getCities(conn);
    await conn.close();
    res.render(`${VIEW_DIR}/cities`, { cities });
  });

  router.get('/add/city', async (req, res) => {
    const conn = await pool.getConnection();
    const regions = await getRegions(conn);
    await conn.close();
    const form = req.session.addCityForm;
    if (form) {
      req.session.addRegionForm = null;
      res.render(`${VIEW_DIR}/city-insert`, _.merge({}, { regions }, form));
    } else {
      res.render(`${VIEW_DIR}/city-insert`, { regions });
    }
  });

  router.post('/add/city', async (req, res) => {
    const data = req.body;
    addCityForm.validate(data, async (err, city) => {
      if (err) {
        req.session.addCityForm = {
          city,
          invalid: _.set({}, err.details[0].context.key, true),
        };
        res.redirect('/add/city');
      } else {
        const conn = await pool.getConnection();
        await insertCity(conn, city);
        await conn.close();
        res.redirect('/cities');
      }
    });
  });

  router.post('/delete/city/:id', async (req, res) => {
    const cityCode = req.params.id;
    const conn = await pool.getConnection();
    await deleteCity(conn, cityCode);
    await conn.close();
    res.redirect('/cities');
  });
};
