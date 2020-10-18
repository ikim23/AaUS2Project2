/* eslint-disable */
const _ = require('lodash');
const faker = require('faker');
const { getConnection, batchInsert } = require('./utils');
const { fakeCountry, fakeRegion, fakeCity } = require('./address');
const { fakePerson, fakeBmData, fakeEmployee, fakeParticipant } = require('./person');
const { fakeCrime } = require('./crime');
const { insertPerson, insertBmData, getPersonalNumbers } = require('../db/person');
const { insertEmployee } = require('../db/employee');
const {
  insertCountry,
  insertRegion,
  insertCity,
  getCities,
  getDistricts,
} = require('../db/address');
const { insertDistrict } = require('../db/district');
const { insertCrime, getCrimeIds } = require('../db/crime');
const { insertParticipant } = require('../db/crime_participant');

async function insertAddresses() {
  const countries = _.times(10, fakeCountry);
  const regions = _.times(30, () => fakeRegion(countries));
  const cities = _.times(100, () => fakeCity(regions));
  const conn = await getConnection();
  await Promise.all(_.map(countries, country => insertCountry(conn, country)));
  await conn.commit();
  await Promise.all(_.map(regions, region => insertRegion(conn, region)));
  await conn.commit();
  await Promise.all(_.map(cities, city => insertCity(conn, city)));
  await conn.commit();
  await conn.close();
}

async function insertPersonsWithBmData(count) {
  const conn = await getConnection();
  const cities = await getCities(conn);
  const persons = _.times(count, () => fakePerson(cities));
  // const bmData = _.map(persons, person => fakeBmData(person.personalNumber));
  await Promise.all(_.map(persons, person => insertPerson(conn, person)));
  await conn.commit();
  // await Promise.all(_.map(bmData, data => insertBmData(conn, data)));
  // await conn.commit();
  await conn.close();
}

async function insertEmployees(count) {
  const conn = await getConnection();
  const cities = await getCities(conn);
  const employees = _.times(count, () => fakeEmployee(cities));
  await Promise.all(_.map(employees, employee => insertEmployee(conn, employee)));
  await conn.commit();
  await conn.close();
}

async function insertCrimes(count) {
  const conn = await getConnection();
  const districts = await getDistricts(conn);
  const crimes = _.times(count, () => fakeCrime(districts));
  await Promise.all(_.map(crimes, crime => insertCrime(conn, crime)));
  await conn.commit();
  await conn.close();
}

async function insertParticipants(count) {
  const conn = await getConnection();
  const cities = await getCities(conn);
  const personalNumbers = await getPersonalNumbers(conn);
  const crimeIds = await getCrimeIds(conn);
  const participants = _.times(count, () => fakeParticipant(cities, personalNumbers, crimeIds));
  await Promise.all(_.map(participants, participant => insertParticipant(conn, participant)));
  await conn.commit();
  await conn.close();
}

async function insertDistricts(countPerCity) {
  const conn = await getConnection();
  const cities = await getCities(conn);
  for (const city of cities) {
    const districtNames = _.uniq(_.times(countPerCity, faker.address.streetName));
    const districts = _.map(districtNames, districtName => ({
      cityCode: city.cityCode,
      name: districtName,
    }));
    await Promise.all(_.map(districts, district => insertDistrict(conn, district)));
    await conn.commit();
  }
  await conn.close();
}

// insertAddresses();
// batchInsert(insertPersonsWithBmData, 100, 90000);
// batchInsert(insertEmployees, 100, 1000);
// batchInsert(insertCrimes, 100, 10000);
batchInsert(insertParticipants, 100, 3000);
// insertDistricts(10);
// insertParticipants(5);
