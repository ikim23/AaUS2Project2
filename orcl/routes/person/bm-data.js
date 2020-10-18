const _ = require('lodash');
const {
  getBmData,
  insertBmData,
  updateBmData,
  deleteBmData,
} = require('../../db/person');
const { bmDataForm } = require('../../validation/person');
const { nullIfEmpty } = require('../utils');

const VIEW_DIR = 'person';

function requestToBmData(req) {
  return {
    personalNumber: req.body.personalNumber,
    height: _.parseInt(req.body.height) || null,
    weight: _.parseInt(req.body.weight) || null,
    skinColor: nullIfEmpty(req.body.skinColor),
    hairColor: nullIfEmpty(req.body.hairColor),
    eyeColor: nullIfEmpty(req.body.eyeColor),
    photo: _.get(req, 'files.photo.data', null),
    photoFilename: _.get(req, 'files.photo.name', null),
    fingerprint: _.get(req, 'files.fingerprint.data', null),
    fingerprintFilename: _.get(req, 'files.fingerprint.name', null),
  };
}

module.exports = (router, pool) => {
  router.get('/add/bm-data/:personalNumber', async (req, res) => {
    const data = {
      action: 'add',
      data: {
        personalNumber: req.params.personalNumber,
      },
    };
    const form = req.session.addBmDataForm;
    if (form) {
      req.session.addBmDataForm = null;
      res.render(`${VIEW_DIR}/bm-data-form`, _.merge({}, data, form));
    } else {
      res.render(`${VIEW_DIR}/bm-data-form`, data);
    }
  });

  router.post('/add/bm-data/:personalNumber', async (req, res) => {
    const data = requestToBmData(req);
    bmDataForm.validate(data, async (err) => {
      if (err) {
        req.session.addBmDataForm = {
          data,
          invalid: _.set({}, err.details[0].context.key, true),
        };
        res.redirect(`/add/bm-data/${data.personalNumber}`);
      } else {
        const conn = await pool.getConnection();
        await insertBmData(conn, data);
        await conn.close();
        res.redirect(`/person/${data.personalNumber}`);
      }
    });
  });

  router.get('/edit/bm-data/:personalNumber', async (req, res) => {
    const form = req.session.editBmDataForm;
    if (form) {
      req.session.editBmDataForm = null;
      res.render(`${VIEW_DIR}/bm-data-form`, _.merge({}, { action: 'edit' }, form));
    } else {
      const { personalNumber } = req.params;
      const conn = await pool.getConnection();
      const data = await getBmData(conn, personalNumber);
      await conn.close();
      res.render(`${VIEW_DIR}/bm-data-form`, { action: 'edit', data });
    }
  });

  router.post('/edit/bm-data/:personalNumber', async (req, res) => {
    const data = requestToBmData(req);
    bmDataForm.validate(data, async (err) => {
      if (err) {
        req.session.editBmDataForm = {
          data,
          invalid: _.set({}, err.details[0].context.key, true),
        };
        res.redirect(`/edit/bm-data/${data.personalNumber}`);
      } else {
        const conn = await pool.getConnection();
        await updateBmData(conn, data);
        await conn.close();
        res.redirect(`/person/${data.personalNumber}`);
      }
    });
  });

  router.post('/delete/bm-data/:personalNumber', async (req, res) => {
    const { personalNumber } = req.params;
    const conn = await pool.getConnection();
    await deleteBmData(conn, personalNumber);
    await conn.close();
    res.redirect(`/person/${personalNumber}`);
  });
};
