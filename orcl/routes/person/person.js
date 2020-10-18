const _ = require('lodash');
const {
  getPersons,
  getPersonsCount,
  getPerson,
  insertPerson,
  updatePerson,
  deletePerson,
  getBmData,
} = require('../../db/person');
const { getCities } = require('../../db/address');
const { personalNumberValidator, addPersonForm } = require('../../validation/person');
const { nullIfEmpty, getPages } = require('../utils');

const VIEW_DIR = 'person';

module.exports = (router, pool) => {
  router.get('/persons', async (req, res) => {
    const query = _.get(req, 'query.q');
    const recordsPerPage = _.parseInt(req.query.r) || 25;
    const page = _.parseInt(req.query.p) || 1;
    const start = ((page - 1) * recordsPerPage) + 1;
    const end = (start + recordsPerPage) - 1;
    const conn = await pool.getConnection();
    const persons = await getPersons(conn, start, end, query);
    const count = await getPersonsCount(conn, query);
    const lastPage = _.ceil(count / recordsPerPage);
    const pages = getPages('/persons', page, lastPage, recordsPerPage, query);
    await conn.close();
    res.render(`${VIEW_DIR}/persons`, { persons, pages });
  });

  router.get('/person/:personalNumber', async (req, res) => {
    const { personalNumber } = req.params;
    personalNumberValidator.validate(personalNumber, async (err) => {
      if (err) {
        res.redirect('/persons');
      } else {
        const conn = await pool.getConnection();
        const person = await getPerson(conn, personalNumber);
        const data = await getBmData(conn, personalNumber);
        await conn.close();
        res.render(`${VIEW_DIR}/person`, { person, data });
      }
    });
  });

  router.get('/add/person', async (req, res) => {
    const conn = await pool.getConnection();
    const cities = await getCities(conn);
    await conn.close();
    const form = req.session.addPersonForm;
    if (form) {
      req.session.addPersonForm = null;
      res.render(`${VIEW_DIR}/person-insert`, _.merge({}, { cities }, form));
    } else {
      res.render(`${VIEW_DIR}/person-insert`, { cities });
    }
  });

  router.post('/add/person', async (req, res) => {
    const data = _.merge({}, req.body, {
      street: nullIfEmpty(req.body.street),
    });
    addPersonForm.validate(data, async (err, person) => {
      if (err) {
        req.session.addPersonForm = {
          person,
          invalid: _.set({}, err.details[0].context.key, true),
        };
        res.redirect('/add/person');
      } else {
        const conn = await pool.getConnection();
        await insertPerson(conn, person);
        await conn.close();
        res.redirect(`/person/${person.personalNumber}`);
      }
    });
  });

  router.get('/edit/person/:personalNumber', async (req, res) => {
    const { personalNumber } = req.params;
    const conn = await pool.getConnection();
    const cities = await getCities(conn);
    const form = req.session.addPersonForm;
    if (form) {
      req.session.addPersonForm = null;
      await conn.close();
      res.render(`${VIEW_DIR}/person-edit`, _.merge({}, { cities }, form));
    } else {
      const person = await getPerson(conn, personalNumber);
      await conn.close();
      res.render(`${VIEW_DIR}/person-edit`, { person, cities });
    }
  });

  router.post('/edit/person/:personalNumber', async (req, res) => {
    const { personalNumber } = req.params;
    const data = _.merge({}, req.body, {
      personalNumber,
      street: nullIfEmpty(req.body.street),
    });
    addPersonForm.validate(data, async (err, person) => {
      if (err) {
        req.session.addPersonForm = {
          person,
          invalid: _.set({}, err.details[0].context.key, true),
        };
        res.redirect(`/edit/person/${personalNumber}`);
      } else {
        const conn = await pool.getConnection();
        await updatePerson(conn, person);
        await conn.close();
        res.redirect(`/person/${personalNumber}`);
      }
    });
  });

  router.post('/delete/person/:personalNumber', async (req, res) => {
    const { personalNumber } = req.params;
    const conn = await pool.getConnection();
    await deletePerson(conn, personalNumber);
    await conn.close();
    res.redirect('/persons');
  });
};
