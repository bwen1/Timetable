var express = require('express');
var router = express.Router();

/* GET api listing. */
router.get('/', function(req, res, next) {
  res.send('respond with a resource');
});

router.post('/friendrequest', function (req, res, next) {
  const name = req.body.id;
  const friendname = req.body.friendid;
  req.db('friends')
      .insert({ ID1: name, ID2: friendname, status: 'PENDING_TO' })
      .then(() => {
          res.status(200).json({ "Error": false, "Message": "Request sent" });
      })
      .catch((e) => {
          console.log(e);
          res.status(400).json({ "Error": true, "Message": "There was an error executing the sql query, " + e.message });
      });
});

router.post('/updaterequest', function (req, res, next) {
  const name = req.body.id;
  const to = req.body.to;
  const newstatus = req.body.newstatus;
  var wh;
  const friendname = req.body.friendid;
  if(to){
    wh = {
      ID2: friendname,
      ID1: name
    };
  }
  else{
    wh = {
      ID2: name,
      ID1: friendname
    }
  }
  req.db('friends')
      .where(wh)
      .update({status: newstatus})
      .then(() => {
          res.status(200).json({ "Error": false, "Message": "Request status updated" });
      })
      .catch((e) => {
          console.log(e);
          res.status(400).json({ "Error": true, "Message": "There was an error executing the sql query, " + e.message });
      });
});

router.post('/removefriend', function (req, res, next) {
  const name = req.body.id;
  const friendname = req.body.friendid;
  req.db('friends')
  .where(function () {
    this
      .where({ID1: friendname, ID2: name})
      .orWhere({ID1: name, ID2: friendname})
  })
      .del()
      .then(() => {
          res.status(200).json({ "Error": false, "Message": "Request sent" });
      })
      .catch((e) => {
          console.log(e);
          res.status(400).json({ "Error": true, "Message": "There was an error executing the sql query, " + e.message });
      });
});

module.exports = router;