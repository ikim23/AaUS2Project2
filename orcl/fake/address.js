// eslint-disable-next-line import/no-extraneous-dependencies
const faker = require('faker');
const _ = require('lodash');
const { rand, max } = require('./utils');

function fakeCountry() {
  return {
    id: faker.unique(faker.address.countryCode),
    name: max(_.partial(faker.unique, faker.address.country), 30),
  };
}

function fakeRegion(countries) {
  return {
    countryId: rand(countries).id,
    regionId: faker.unique(faker.address.stateAbbr),
    name: max(_.partial(faker.unique, faker.address.state), 30),
  };
}

function fakeCity(regions) {
  return {
    regionId: rand(regions).regionId,
    cityCode: faker.unique(_.partial(faker.address.zipCode, '#####')),
    city: max(_.partial(faker.unique, faker.address.city), 30),
  };
}

function fakeDistrict(cities) {
  return {
    districtId: faker.lorem.word(),
    district: faker.address.cityPrefix(),
    cityCode: rand(cities).cityCode,
  };
}

module.exports = {
  fakeCountry,
  fakeRegion,
  fakeCity,
  fakeDistrict,
};
