const express = require('express');
const home = require('./home');
const employee = require('./employee');
const crime = require('./crime');
const person = require('./person');
const address = require('./address');
const participant = require('./crime_participant');
const report = require('./report');

const router = express.Router();

module.exports = (app, pool) => {
  home(router, pool);
  employee(router, pool);
  crime(router, pool);
  person(router, pool);
  address(router, pool);
  participant(router, pool);
  report(router, pool);

  // add routes to app
  app.use('/', router);

  // catch 404 and forward to error handler
  app.use((req, res, next) => {
    const err = new Error('Not Found');
    err.status = 404;
    next(err);
  });

  // error handler
  app.use((err, req, res) => {
    // set locals, only providing error in development
    res.locals.message = err.message;
    res.locals.error = req.app.get('env') === 'development' ? err : {};

    // render the error page
    res.status(err.status || 500);
    res.render('error');
  });
};
