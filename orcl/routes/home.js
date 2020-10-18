const { getCounts } = require('../db/stats');

module.exports = (router, pool) => {
  router.get('/', async (req, res) => {
    const conn = await pool.getConnection();
    const counts = await getCounts(conn);
    await conn.close();
    res.render('home', { counts });
  });
};
