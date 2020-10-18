const _ = require('lodash');
const dateformat = require('dateformat');

function parseDate(data) {
  if (!_.isDate(data)) return data;
  return dateformat(data, 'yyyy-mm-dd');
}

function strToDate(data) {
  if (!_.isString(data)) return data;
  return new Date(data);
}

module.exports = {
  parseDate,
  strToDate,
};
