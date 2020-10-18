const Joi = require('joi');

const addParticipantForm = Joi.object({
  personalNumber: Joi.string().length(10).regex(/\d/).required(),
  crimeId: Joi.number().integer().required(),
  type: Joi.string().max(1).required(),
  fromDate: Joi.string().regex(/(19|20)\d\d-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])/).required(),
  toDate: Joi.string().regex(/(19|20)\d\d-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])/).allow(null),
  witness: Joi.binary().allow(null),
  witnessFileName: Joi.string().max(50).allow(null),
  reason: Joi.binary().allow(null),
  reasonFileName: Joi.string().max(50).allow(null),
  date: Joi.string().regex(/(19|20)\d\d-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])/).allow(null),
  punishmentLength: Joi.number().integer().allow(null),
  punishmentPlace: Joi.string().max(30).allow(null),
});

const editParticipantForm = Joi.object({
  personalNumber: Joi.string().length(10).regex(/\d/).required(),
  crimeId: Joi.number().integer().required(),
  type: Joi.string().max(1).required(),
  fromDate: Joi.string().regex(/(19|20)\d\d-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])/).required(),
  toDate: Joi.string().regex(/(19|20)\d\d-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])/).allow(null),
  witness: Joi.binary().allow(null),
  witnessFileName: Joi.string().max(50).allow(null),
  reason: Joi.binary().allow(null),
  reasonFileName: Joi.string().max(50).allow(null),
  date: Joi.string().regex(/(19|20)\d\d-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])/).allow(null),
  punishmentLength: Joi.number().integer().allow(null),
  punishmentPlace: Joi.string().max(30).allow(null),
});

module.exports = {
  addParticipantForm,
  editParticipantForm,
};
