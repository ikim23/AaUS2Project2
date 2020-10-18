/* eslint-disable newline-per-chained-call */
const Joi = require('joi');

const addCountryForm = Joi.object({
  id: Joi.string().alphanum().length(2).required(),
  name: Joi.string().min(1).max(30).required(),
});

const addRegionForm = Joi.object({
  countryId: Joi.string().alphanum().length(2).required(),
  regionId: Joi.string().alphanum().length(2).required(),
  name: Joi.string().min(1).max(30).required(),
});

const addCityForm = Joi.object({
  regionId: Joi.string().alphanum().length(2).required(),
  cityCode: Joi.string().alphanum().length(5).required(),
  city: Joi.string().min(1).max(30).required(),
});

const addDistrictForm = Joi.object({
  districtId: Joi.string().alphanum().max(30).required(),
  district: Joi.string().max(30).required(),
  cityCode: Joi.string().alphanum().length(5).required(),
});

module.exports = {
  addCountryForm,
  addRegionForm,
  addCityForm,
  addDistrictForm,
};
