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

router.get('/friends/:user', function (req, res, next) {
  const query = req.query;
  const id = req.params.user;
  var frids = [];
  var fstatus = [];
  var friend_list = [];
    req.db.from('friends')
      .select('*')
      .where(function () {
        this
          .where('ID1', id)
          .orWhere('ID2', id)
      })
      .then((rows) => {
        console.log(rows);
        rows.forEach(row => {
          if(row.ID1 == id){
            frids.push(row.ID2);
            fstatus.push(row.status);
          }
          else if(row.ID2 == id){
            frids.push(row.ID1);
            if(row.status == "PENDING_TO")
              fstatus.push("PENDING_FROM");
            else if (row.status == "PENDING_FROM")
              fstatus.push("PENDING_TO");
            else if (row.status == "BLOCKED_TO")
              fstatus.push("BLOCKED_FROM")
            else
              fstatus.push("BLOCKED_TO")
          }
          
        });
        console.log(frids);
        req.db.from('users')
        .select('Name', 'ID')
        .whereIn('ID', frids)
        .then((ros) =>{
          console.log(ros);
          for(var i = 0; i < ros.length; i++){
            friend_list.push({
              ID: ros[i].ID,
              Name: ros[i].Name,
              status: fstatus[i]
            });
          }
          console.log(friend_list);
          res.status(200).json({"Error": false, "Message": "Data retrived ", "data": friend_list})
        })
        .catch((e) =>{
          res.status(400).json({"Error": true, "Message": "There was an error "})
        });
      })
      .catch((e) => {
        console.log(e);
        res.status(400).json({ "Error": true, "Message": "There was an error executing the sql query, " + e.message });
      });
  

});

module.exports = router;