const _ = require('lodash');
const express = require('express');
const bodyParser = require('body-parser');
const cors = require('cors');
const mysql = require('promise-mysql');

const pool = mysql.createPool({
  host: 'db',
  database: 'todos_db',
  user: 'user',
  password: 'user',
  connectionLimit: 5,
});

const PORT = 3000;
const app = express();
app.use(bodyParser.json());
app.use(cors());

app.get('/todos', (req, res) => {
  pool.query('select * from todos')
    .then((todos) => {
      res.send(todos);
    });
});

app.post('/todo', (req, res) => {
  const todo = req.body;
  if (_.has(todo, 'task')) {
    pool.query('insert into todos set ?', _.assign(todo, { done: 0 }))
      .then(() => {
        res.sendStatus(201);
      })
      .catch((e) => {
        console.error(e);
        res.sendStatus(500);
      });
  } else {
    res.sendStatus(400);
  }
});

app.listen(PORT, () => {
  console.log(`Server is listening on port: ${PORT}`);
});
