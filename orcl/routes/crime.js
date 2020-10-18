const _ = require('lodash');
const dateformat = require('dateformat');
const {
  getCrimes,
  getCrimesCount,
  getCrime,
  insertCrime,
  updateCrime,
  deleteCrime,
} = require('../db/crime');
const { getDistricts } = require('../db/address');
const { addCrimeForm, editCrimeForm } = require('../validation/crime');
const { getPages } = require('./utils');

function oneIfTrue(param) {
  if (param != null) return '1';
  return '0';
}

function parseDate(date) {
  return _.isNull(date) ? null : dateformat(date, 'dd. mm. yyyy');
}

module.exports = (router, pool) => {
  router.get('/crimes', async (req, res) => {
    const query = _.get(req, 'query.q');
    const recordsPerPage = _.parseInt(req.query.r) || 25;
    const page = _.parseInt(req.query.p) || 1;
    const start = ((page - 1) * recordsPerPage) + 1;
    const end = (start + recordsPerPage) - 1;
    const conn = await pool.getConnection();
    const crimes = await getCrimes(conn, start, end, query);
    const count = await getCrimesCount(conn, query);
    const lastPage = _.ceil(count / recordsPerPage);
    const pages = getPages('/crimes', page, lastPage, recordsPerPage, query);
    await conn.close();
    res.render('crimes', { crimes, pages });
  });

  router.get('/crime/:id', async (req, res, next) => {
    const crimeId = _.parseInt(req.params.id);
    if (_.isNaN(crimeId)) {
      next();
    } else {
      const conn = await pool.getConnection();
      const crime = await getCrime(conn, crimeId);
      await conn.close();
      res.render('crime', { crime });
    }
  });

  router.get('/add/crime', async (req, res) => {
    const conn = await pool.getConnection();
    const districts = await getDistricts(conn);
    const form = req.session.addCrimeForm;
    if (form) {
      req.session.addCrimeForm = null;
      res.render('crime-insert', _.merge({}, { districts }, form));
    } else {
      await conn.close();
      res.render('crime-insert', { districts });
    }
  });

  router.post('/add/crime', async (req, res) => {
    const data = _.merge({}, req.body, {
      districtId: _.get(req, 'body.districtId'),
      type: _.get(req, 'body.type'),
      description: _.get(req, 'files.description.data', ''),
      descFileName: _.get(req, 'files.description.name', null),
      explained: oneIfTrue(req.body.explained),
      damageValue: _.isNaN(_.parseInt(req.body.damageValue)) ? null : _.parseInt(req.body.damageValue),
      openDate: parseDate(_.get(req, 'body.openDate')),
      closeDate: parseDate(_.get(req, 'body.closeDate')),
    });
    addCrimeForm.validate(data, async (err, crime) => {
      if (err) {
        req.session.addCrimeForm = {
          crime,
          invalid: _.set({}, err.details[0].context.key, true),
        };
        res.redirect('/add/crime');
      } else {
        const conn = await pool.getConnection();
        await insertCrime(conn, crime);
        await conn.close();
        res.redirect('/crimes');
      }
    });
  });

  router.get('/edit/crime/:id', async (req, res, next) => {
    const form = req.session.editCrimeForm;
    if (form) {
      req.session.editCrimeForm = null;
      res.render('crime-edit', form);
    } else {
      const crimeId = _.parseInt(req.params.id);
      if (_.isNaN(crimeId)) {
        next();
      } else {
        const conn = await pool.getConnection();
        const crime = await getCrime(conn, crimeId);
        const allDistricts = await getDistricts(conn);
        const districts = _.map(allDistricts, (district) => {
          if (district.districtId === crime.districtId) {
            return _.assign(district, { selected: true });
          }
          return district;
        });
        await conn.close();
        res.render('crime-edit', { crime, districts });
      }
    }
  });

  router.post('/edit/crime/:id', async (req, res) => {
    const crimeId = _.parseInt(req.params.id);
    const data = _.merge({}, req.body, {
      id: crimeId,
      districtId: _.get(req, 'body.districtId'),
      type: _.get(req, 'body.type'),
      description: _.get(req, 'files.description.data', ''),
      descFileName: _.get(req, 'files.description.name', null),
      explained: oneIfTrue(req.body.explained),
      damageValue: _.isNaN(_.parseInt(req.body.damageValue)) ? null : _.parseInt(req.body.damageValue),
      openDate: parseDate(_.get(req, 'body.openDate')),
      closeDate: parseDate(_.get(req, 'body.closeDate')),
    });
    editCrimeForm.validate(data, async (err, crime) => {
      if (err) {
        req.session.editCrimeForm = {
          crime,
          invalid: _.set({}, err.details[0].context.key, true),
        };
        res.redirect(`/edit/crime/${crimeId}`);
      } else {
        const conn = await pool.getConnection();
        await updateCrime(conn, crime);
        await conn.close();
        res.redirect('/crimes');
      }
    });
  });

  router.post('/delete/crime/:id', async (req, res) => {
    const crimeId = _.parseInt(req.params.id);
    const conn = await pool.getConnection();
    await deleteCrime(conn, crimeId);
    await conn.close();
    res.redirect('/crimes');
  });
};
