var express = require('express');
var router = express.Router();

/* GET home page. */
router.get('/', function(req, res, next) {
  res.render('index', { title: 'Backend', desc: 'a simple express based backend for the timetable talk mobile app.' });
});

module.exports = router;
