const express = require('express');
const path = require('path');
const logger = require('morgan');
const favicon = require('serve-favicon');
const cookieParser = require('cookie-parser');
const bodyParser = require('body-parser');
const session = require('express-session');
const fileUpload = require('express-fileupload');
const dbInitializer = require('./db');
const routes = require('./routes');

const PORT = 3000;
const app = express();

// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'hbs');

app.use(logger('dev'));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(session({
  secret: 'secret_key',
  resave: true,
  saveUninitialized: true,
}));
app.use(fileUpload());
app.use(favicon(path.join(__dirname, 'public', 'favicon.ico')));
app.use(express.static(path.join(__dirname, 'public')));

dbInitializer((pool) => {
  routes(app, pool);
  app.listen(PORT, () => {
    console.log(`Server is listening on port: ${PORT}`);
  });
});
