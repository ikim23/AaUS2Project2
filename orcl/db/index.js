const oracledb = require('oracledb');

module.exports = (callback) => {
  oracledb.fetchAsBuffer = [oracledb.BLOB];
  oracledb.autoCommit = true;
  oracledb.createPool({
    user: process.env.USER,
    password: process.env.PASS,
    connectString: process.env.CONN_STRING,
  }, (error, pool) => {
    callback(pool);
  });
};
