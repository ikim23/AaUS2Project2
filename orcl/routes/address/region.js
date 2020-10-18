const _ = require('lodash');
const {
  getCountries,
  getRegions,
  insertRegion,
  deleteRegion,
} = require('../../db/address');
const { addRegionForm } = require('../../validation/address');

const VIEW_DIR = 'address/region';

module.exports = (router, pool) => {
  router.get('/regions', async (req, res) => {
    const conn = await pool.getConnection();
    const regions = await getRegions(conn);
    await conn.close();
    res.render(`${VIEW_DIR}/regions`, { regions });
  });

  router.get('/add/region', async (req, res) => {
    const conn = await pool.getConnection();
    const countries = await getCountries(conn);
    await conn.close();
    const form = req.session.addRegionForm;
    if (form) {
      req.session.addRegionForm = null;
      res.render(`${VIEW_DIR}/region-insert`, _.merge({}, { countries }, form));
    } else {
      res.render(`${VIEW_DIR}/region-insert`, { countries });
    }
  });

  router.post('/add/region', async (req, res) => {
    const data = req.body;
    addRegionForm.validate(data, async (err, region) => {
      if (err) {
        req.session.addRegionForm = {
          region,
          invalid: _.set({}, err.details[0].context.key, true),
        };
        res.redirect('/add/region');
      } else {
        const conn = await pool.getConnection();
        await insertRegion(conn, region);
        await conn.close();
        res.redirect('/regions');
      }
    });
  });

  router.post('/delete/region/:id', async (req, res) => {
    const regionId = req.params.id;
    const conn = await pool.getConnection();
    await deleteRegion(conn, regionId);
    await conn.close();
    res.redirect('/regions');
  });
};
