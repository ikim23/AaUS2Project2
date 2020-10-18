const _ = require('lodash');
const { nullIfEmpty, getCrimeNames } = require('./utils');
const { getCrimesSeason } = require('../db/crime');

const VIEW_DIR = 'report';

module.exports = (router, pool) => {
  router.get('/report/crimes/season', async (req, res) => {
    const crimeNames = getCrimeNames();
    res.render(`${VIEW_DIR}/report-crimes-season`, { crimeNames });
  });

  router.post('/report/crimes/season', async (req, res) => {
    const params = req.body;
    const conn = await pool.getConnection();
    const crimes = await getCrimesSeason(conn, params);
    await conn.close();
    const crimeNames = getCrimeNames();
    res.render(`${VIEW_DIR}/report-crimes-season`, { crimeNames, crimes });
  });
};
