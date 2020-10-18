const _ = require('lodash');
const oracledb = require('oracledb');

function rand(array) {
  return array[_.random(0, array.length - 1)];
}

function max(func, length) {
  const result = func();
  if (_.isString(result) && result.length > length) {
    return max(func, length);
  }
  return result;
}

async function getConnection() {
  return oracledb.getConnection({
    user: process.env.USER,
    password: process.env.PASS,
    connectString: process.env.CONN_STRING,
  });
}

/* eslint-disable */
async function batchInsert(insertFunc, batchSize, count) {
  const numBatch = _.ceil(count / batchSize);
  console.log(`Total of ${numBatch} batches will be executed`);
  for (let i = 0; i < numBatch; i++) {
    try {
      await insertFunc(batchSize);
    } catch (e) {
      console.error(e);
    }
    console.log(`Batch ${i + 1} done`);
  }
}
/* eslint-enable */

module.exports = {
  rand,
  max,
  getConnection,
  batchInsert,
};
