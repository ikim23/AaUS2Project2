/* eslint-disable newline-per-chained-call */
const Joi = require('joi');

const personalNumberValidator = Joi.string().length(10).regex(/\d/);

const addPersonForm = Joi.object({
  name: Joi.string().alphanum().min(1).max(30).required(),
  surname: Joi.string().alphanum().min(1).max(30).required(),
  personalNumber: Joi.string().length(10).regex(/\d/).required(),
  cityCode: Joi.string().length(5).regex(/\d/).required(),
  street: Joi.string().min(1).max(30).allow(null).required(),
});

const bmDataForm = Joi.object({
  personalNumber: Joi.string().length(10).regex(/\d/).allow(null).required(),
  height: Joi.number().integer().positive().allow(null).required(),
  weight: Joi.number().integer().positive().allow(null).required(),
  skinColor: Joi.string().length(7).regex(/#[0-9a-fA-F]{6}/).allow(null).required(),
  hairColor: Joi.string().length(7).regex(/#[0-9a-fA-F]{6}/).allow(null).required(),
  eyeColor: Joi.string().length(7).regex(/#[0-9a-fA-F]{6}/).allow(null).required(),
  photo: Joi.binary().allow(null).required(),
  photoFilename: Joi.string().min(5).max(50).allow(null).required(),
  fingerprint: Joi.binary().allow(null).required(),
  fingerprintFilename: Joi.string().min(5).max(50).allow(null).required(),
});

module.exports = {
  personalNumberValidator,
  addPersonForm,
  bmDataForm,
};
