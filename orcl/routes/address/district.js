const _ = require('lodash');
const {
  getCities,
  getDistricts,
  insertDistrict,
  deleteDistrict,
} = require('../../db/address');
const { addDistrictForm } = require('../../validation/address');

const VIEW_DIR = 'address/district';

module.exports = (router, pool) => {
  router.get('/districts', async (req, res) => {
    const conn = await pool.getConnection();
    const districts = await getDistricts(conn);
    await conn.close();
    res.render(`${VIEW_DIR}/districts`, { districts });
  });

  router.get('/add/district', async (req, res) => {
    const conn = await pool.getConnection();
    const cities = await getCities(conn);
    await conn.close();
    const form = req.session.addDistrictForm;
    if (form) {
      req.session.addRegionForm = null;
      res.render(`${VIEW_DIR}/district-insert`, _.merge({}, { cities }, form));
    } else {
      res.render(`${VIEW_DIR}/district-insert`, { cities });
    }
  });

  router.post('/add/district', async (req, res) => {
    const data = req.body;
    addDistrictForm.validate(data, async (err, district) => {
      if (err) {
        req.session.addDistrictForm = {
          district,
          invalid: _.set({}, err.details[0].context.key, true),
        };
        res.redirect('/add/district');
      } else {
        const conn = await pool.getConnection();
        await insertDistrict(conn, district);
        await conn.close();
        res.redirect('/districts');
      }
    });
  });

  router.post('/delete/district/:id', async (req, res) => {
    const districtId = req.params.id;
    const conn = await pool.getConnection();
    await deleteDistrict(conn, districtId);
    await conn.close();
    res.redirect('/districts');
  });
};
