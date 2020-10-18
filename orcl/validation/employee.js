/* eslint-disable newline-per-chained-call */
const Joi = require('joi');

const addEmployeeForm = Joi.object({
  name: Joi.string().alphanum().min(1).max(30).required(),
  surname: Joi.string().alphanum().min(1).max(30).required(),
  personalNumber: Joi.string().length(10).regex(/\d/).required(),
  cityCode: Joi.string().length(5).regex(/\d/).required(),
  street: Joi.string().min(1).max(30).allow(null).required(),
  position: Joi.string().alphanum().min(1).max(15).required(),
  rank: Joi.string().alphanum().min(1).max(15).allow(null).required(),
  fromDate: Joi.string().regex(/(19|20)\d\d-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])/).required(),
  salary: Joi.number().integer().positive().required(),
});

const editEmployeeForm = Joi.object({
  id: Joi.number().integer().positive().required(),
  name: Joi.string().alphanum().min(1).max(30).required(),
  surname: Joi.string().alphanum().min(1).max(30).required(),
  cityCode: Joi.string().length(5).regex(/\d/).required(),
  street: Joi.string().min(1).max(30).allow(null).required(),
  position: Joi.string().alphanum().min(1).max(15).required(),
  rank: Joi.string().alphanum().min(1).max(15).allow(null).required(),
  fromDate: Joi.string().regex(/(19|20)\d\d-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])/).required(),
  toDate: Joi.string().regex(/(19|20)\d\d-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])/).allow(null).required(),
});

const addSalaryForm = Joi.object({
  id: Joi.number().integer().positive().required(),
  fromDate: Joi.string().regex(/(19|20)\d\d-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])/).required(),
  salary: Joi.number().positive().required(),
});

module.exports = {
  addEmployeeForm,
  editEmployeeForm,
  addSalaryForm,
};
