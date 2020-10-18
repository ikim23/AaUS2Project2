// eslint-disable-next-line import/no-extraneous-dependencies
const faker = require('faker');
const _ = require('lodash');
const dateformat = require('dateformat');
const fs = require('fs');

function rand(array) {
  return array[_.random(0, array.length - 1)];
}

function fakeCrime(districts) {
  const oDate = faker.date.past();
  const cDate = faker.date.between(oDate, new Date());
  const descFileN = `${faker.random.number({ min: 1, max: 2 })}.txt`;
  return {
    districtId: rand(districts).districtId,
    type: faker.random.arrayElement(['kradez', 'vrazda', 'lupez', 'podvod', 'sprenevera', 'zabitie', 'uzera', 'pytliactvo', 'vlastizrada', 'vydieranie', 'unos', 'podplacanie', 'vytrznictvo', 'prevadzacstvo', 'kupliarstvo']),
    description: fs.readFileSync(`${__dirname}/file/witness/${descFileN}`),
    descFileName: descFileN,
    damageValue: faker.random.number({ min: 500, max: 1000000000 }),
    explained: faker.random.arrayElement(['0', '1']),
    openDate: dateformat(oDate, 'yyyy-mm-dd'),
    closeDate: dateformat(cDate, 'yyyy-mm-dd'),
  };
}

module.exports = {
  fakeCrime,
};
