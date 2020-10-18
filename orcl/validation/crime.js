/* eslint-disable newline-per-chained-call */
const Joi = require('joi');

const addCrimeForm = Joi.object({
  type: Joi.string().alphanum().min(1).max(30).required(),
  districtId: Joi.string().max(30),
  damageValue: Joi.number().integer().positive().allow(null),
  description: Joi.binary().allow(null),
  descFileName: Joi.string().max(50).allow(null),
  explained: Joi.string().max(1),
  openDate: Joi.string().regex(/(0[1-9]|[12]\d|3[01]). (0[1-9]|1[0-2]). (19|20)\d\d/).required(),
  closeDate: Joi.string().regex(/(0[1-9]|[12]\d|3[01]). (0[1-9]|1[0-2]). (19|20)\d\d/).allow(null),
});

const editCrimeForm = Joi.object({
  id: Joi.number().integer().required(),
  type: Joi.string().alphanum().min(1).max(30).required(),
  districtId: Joi.string().max(30),
  damageValue: Joi.number().integer().positive().allow(null),
  description: Joi.binary().allow(null),
  descFileName: Joi.string().max(50).allow(null),
  explained: Joi.string().max(1),
  openDate: Joi.string().regex(/(0[1-9]|[12]\d|3[01]). (0[1-9]|1[0-2]). (19|20)\d\d/).required(),
  closeDate: Joi.string().regex(/(0[1-9]|[12]\d|3[01]). (0[1-9]|1[0-2]). (19|20)\d\d/).allow(null),
});

module.exports = {
  addCrimeForm,
  editCrimeForm,
};
