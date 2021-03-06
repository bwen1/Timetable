var express = require('express');
var router = express.Router();

/* GET api listing. */
router.get('/', function (req, res, next) {
  res.send('respond with a resource');
});

router.post('/addevent', function (req, res, next) {
  const name = req.body.id;
  const share = req.body.shared;
  const evname = request.body.eventname;
  const note = req.body.notes;
  const loc = req.body.location;
  const timest = req.body.timestart;
  const timeed = req.body.timeend;
  const day = req.body.day;
  req.db('event')
    .insert({ ID: name, shared: share, EventName: evname, notes: note, Location: loc, TimeStart: timest, TimeEnd: timeed, Day: day })
    .then(() => {
      req.db.from('event')
        .select('EventID')
        .where({ ID: name, eventName: evname, TimeStart: timest })
        .limit(1)
        .then((row) => {
          res.status(200).json({ "Error": false, "Message": "Event added", "EventID": row[0].EventID });
        })
        .catch((e) => {
          console.log(e);
          res.status(400).json({ "Error": true, "Message": "There was an error executing the sql query, " + e.message });
        });
    })
    .catch((e) => {
      console.log(e);
      res.status(400).json({ "Error": true, "Message": "There was an error executing the sql query, " + e.message });
    });
});

router.post('/editevent', function (req, res, next) {
  const name = req.body.id;
  const evid = req.body.eventid;
  const share = req.body.shared;
  const evname = request.body.eventname;
  const note = req.body.notes;
  const loc = req.body.location;
  const timest = req.body.timestart;
  const timeed = req.body.timeend;
  const day = req.body.day;
  req.db('event')
    .where({ EventID: evid })
    .update({ ID: name, shared: share, EventName: evname, notes: note, Location: loc, TimeStart: timest, TimeEnd: timeed, Day: day })
    .then(() => {
      res.status(200).json({ "Error": false, "Message": "Event edited" });
    })
    .catch((e) => {
      console.log(e);
      res.status(400).json({ "Error": true, "Message": "There was an error executing the sql query, " + e.message });
    });
});

router.post('/removeevent', function (req, res, next) {
  const id = req.body.eventid;
  req.db('event')
    .where({ EventID: id })
    .del()
    .then(() => {
      res.status(200).json({ "Error": false, "Message": "Event removed" });
    })
    .catch((e) => {
      console.log(e);
      res.status(400).json({ "Error": true, "Message": "There was an error executing the sql query, " + e.message });
    });
});

router.get('/events', function (req, res, next) {
  const query = req.query;
  var friend_list;
  if (!query.friends) {
    res.status(400).json({ "Error": true, "Message": "Missing friend list" })
  }
  else {
    friend_list = query.friends.split(',');
    req.db.from('event')
      .select('*')
      .whereIn('ID', friend_list)
      .where({ shared: true })
      .then((rows) => {
        res.status(200).json({ "Error": false, "Message": "events gathered", "Events": rows });
      })
      .catch((e) => {
        console.log(e);
        res.status(400).json({ "Error": true, "Message": "There was an error executing the sql query, " + e.message });
      });
  }

});

router.get('/myevents/:id', function (req, res, next) {
  const id = req.params.id;
    req.db.from('event')
      .select('*')
      .where("ID", id )
      .then((rows) => {
        res.status(200).json({ "Error": false, "Message": "my events gathered", "Events": rows });
      })
      .catch((e) => {
        console.log(e);
        res.status(400).json({ "Error": true, "Message": "There was an error executing the sql query, " + e.message });
      });
  

});

module.exports = router;