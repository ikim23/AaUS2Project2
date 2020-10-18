const _ = require('lodash');
const {
  getParticipants,
  getParticipantsCount,
  getParticipant,
  insertParticipant,
  updateParticipant,
  deleteParticipant,
} = require('../db/crime_participant');
const { getCities } = require('../db/address');
const { addParticipantForm, editParticipantForm } = require('../validation/participant');
const { nullIfEmpty, getPages } = require('./utils');

module.exports = (router, pool) => {
  router.get('/participants', async (req, res) => {
    const query = _.get(req, 'query.q');
    const recordsPerPage = _.parseInt(req.query.r) || 25;
    const page = _.parseInt(req.query.p) || 1;
    const start = ((page - 1) * recordsPerPage) + 1;
    const end = (start + recordsPerPage) - 1;
    const conn = await pool.getConnection();
    const participants = await getParticipants(conn, start, end, query);
    const count = await getParticipantsCount(conn, query);
    const lastPage = _.ceil(count / recordsPerPage);
    const pages = getPages('/participants', page, lastPage, recordsPerPage, query);
    await conn.close();
    res.render('participants', { participants, pages });
  });

  router.get('/participant/:personalNumber.:crimeId.:type.:fromDate', async (req, res) => {
    const {
      personalNumber,
      crimeId,
      type,
      fromDate,
    } = req.params;
    const conn = await pool.getConnection();
    const participant = await getParticipant(conn, personalNumber, crimeId, type, fromDate);
    await conn.close();
    res.render('participant', { participant });
  });

  router.get('/add/participant', async (req, res) => {
    const conn = await pool.getConnection();
    const cities = await getCities(conn);
    await conn.close();
    const form = req.session.addParticipantForm;
    if (form) {
      req.session.addParticipantForm = null;
      res.render('participant-insert', _.merge({}, { cities }, form));
    } else {
      res.render('participant-insert', { cities });
    }
  });

  router.post('/add/participant', async (req, res) => {
    const data = _.merge({}, req.body, {
      toDate: nullIfEmpty(req.body.toDate),
      witness: _.get(req, 'files.witness.data', ''),
      witnessFileName: _.get(req, 'files.witness.name', null),
      reason: _.get(req, 'files.reason.data', ''),
      reasonFileName: _.get(req, 'files.reason.name', null),
      date: nullIfEmpty(req.body.date),
      punishmentLength: nullIfEmpty(req.body.punishmentLength),
      punishmentPlace: nullIfEmpty(req.body.punishmentPlace),
    });
    addParticipantForm.validate(data, async (err, participant) => {
      if (err) {
        req.session.addParticipantForm = {
          participant,
          invalid: _.set({}, err.details[0].context.key, true),
        };
        res.redirect('/add/participant');
      } else {
        const conn = await pool.getConnection();
        await insertParticipant(conn, participant);
        await conn.close();
        res.redirect('/participants');
      }
    });
  });

  router.get('/edit/participant/:personalNumber.:crimeId.:type.:fromDate', async (req, res) => {
    const {
      personalNumber,
      crimeId,
      type,
      fromDate,
    } = req.params;
    const conn = await pool.getConnection();
    const cities = await getCities(conn);
    const form = req.session.editParticipantForm;
    if (form) {
      req.session.editParticipantForm = null;
      await conn.close();
      res.render('participant-edit', _.merge({}, { cities }, form));
    } else {
      const participant = await getParticipant(conn, personalNumber, crimeId, type, fromDate);
      await conn.close();
      res.render('participant-edit', { participant, cities });
    }
  });

  router.post('/edit/participant/:personalNum.:cId.:t.:fromD', async (req, res) => {
    const {
      personalNum,
      cId,
      t,
      fromD,
    } = req.params;
    const data = _.merge({}, req.body, {
      personalNumber: personalNum,
      crimeId: cId,
      type: t,
      fromDate: fromD,
      toDate: nullIfEmpty(req.body.toDate),
      witness: _.get(req, 'files.witness.data', ''),
      witnessFileName: _.get(req, 'files.witness.name', null),
      reason: _.get(req, 'files.reason.data', ''),
      reasonFileName: _.get(req, 'files.reason.name', null),
      date: nullIfEmpty(req.body.date),
      punishmentLength: nullIfEmpty(req.body.punishmentLength),
      punishmentPlace: nullIfEmpty(req.body.punishmentPlace),
    });
    editParticipantForm.validate(data, async (err, participant) => {
      if (err) {
        req.session.editParticipantForm = {
          participant,
          invalid: _.set({}, err.details[0].context.key, true),
        };
        res.redirect('/edit/participant');
      } else {
        const conn = await pool.getConnection();
        await updateParticipant(conn, participant);
        await conn.close();
        res.redirect('/participants');
      }
    });
  });

  router.post('/delete/participant/:personalNum.:cId.:t.:fromD', async (req, res) => {
    const {
      personalNum,
      cId,
      t,
      fromD,
    } = req.params;
    const conn = await pool.getConnection();
    await deleteParticipant(conn, personalNum, cId, t, fromD);
    await conn.close();
    res.redirect('/participants');
  });
};
