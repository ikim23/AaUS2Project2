// eslint-disable-next-line import/no-extraneous-dependencies
const faker = require('faker');
const _ = require('lodash');
const df = require('dateformat');
const fs = require('fs');

function rand(array) {
  return array[_.random(0, array.length - 1)];
}

function dateAddYears(date, years) {
  const now = new Date();
  const newDate = new Date(date.getFullYear() + years, date.getMonth(), date.getDate());
  return newDate < now ? newDate : now;
}

function fakePersonalNumber() {
  const birthday = faker.date.between('1937-01-01', '1997-01-01');
  return {
    birthday,
    value: `${df(birthday, 'yymmdd')}${faker.random.number({ min: 1000, max: 9999 })}`,
  };
}

function fakePerson(cities) {
  return {
    personalNumber: fakePersonalNumber().value,
    cityCode: rand(cities).cityCode,
    name: faker.name.firstName(),
    surname: faker.name.lastName(),
    street: faker.address.streetAddress(),
  };
}

function fakeEmployee(cities) {
  const { birthday, value } = fakePersonalNumber();
  return Object.assign(
    fakePerson(cities),
    {
      personalNumber: value,
      position: faker.name.jobType(),
      rank: null,
      fromDate: df(faker.date.between(df(dateAddYears(birthday, 25), 'yyyy-mm-dd'), df(new Date(), 'yyyy-mm-dd')), 'yyyy-mm-dd'),
      salary: faker.random.number({ min: 400, max: 1500 }),
    },
  );
}

function fakeBmData(personalNumber) {
  const photoFilename = `${faker.random.number({ min: 1, max: 21 })}.jpg`;
  const fingerprintFilename = `${faker.random.number({ min: 1, max: 10 })}.jpg`;
  return {
    personalNumber,
    height: faker.random.number({ min: 150, max: 200 }),
    weight: faker.random.number({ min: 50, max: 150 }),
    skinColor: faker.internet.color(),
    hairColor: faker.internet.color(),
    eyeColor: faker.internet.color(),
    photo: fs.readFileSync(`${__dirname}/img/photo/${photoFilename}`),
    photoFilename,
    fingerprint: fs.readFileSync(`${__dirname}/img/fingerprint/${fingerprintFilename}`),
    fingerprintFilename,
  };
}

function fakeParticipant(cities, personalNumbers, crimeIds) {
  const fDate = faker.date.past();
  const tDate = faker.date.between(fDate, new Date());
  const witnessFileN = `${faker.random.number({ min: 1, max: 4 })}.txt`;
  const reasonFileN = `${faker.random.number({ min: 1, max: 3 })}.txt`;
  return {
    personalNumber: rand(personalNumbers).personalNumber,
    crimeId: rand(crimeIds).crimeId,
    type: faker.random.arrayElement(['S', 'H', 'P', 'O']),
    fromDate: df(fDate, 'yyyy-mm-dd'),
    toDate: df(tDate, 'yyyy-mm-dd'),
    witness: fs.readFileSync(`${__dirname}/file/witness/${witnessFileN}`),
    witnessFileName: witnessFileN,
    reason: fs.readFileSync(`${__dirname}/file/reason/${reasonFileN}`),
    reasonFileName: reasonFileN,
    date: null,
    punishmentLength: null,
    punishmentPlace: null,
  };
}

module.exports = {
  fakePerson,
  fakeEmployee,
  fakeBmData,
  fakeParticipant,
};
