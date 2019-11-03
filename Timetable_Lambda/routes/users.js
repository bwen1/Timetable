var express = require('express');
var router = express.Router();

/* GET api listing. */
router.get('/', function (req, res, next) {
    res.send('respond with a resource');
});

router.get('/avcheck/:username', function (req, res, next) {
    const name = req.params.username;
    req.db.from('users').select('*')
        .where({Name: name})
        .then((rows) => {
            if (rows.length != 0) {
                res.status(200).json({ "Error": false, "Message": "Name taken"});
            }
            else {
                res.status(200).json({ "Error": false, "Message": "Name free" });
            }
        })
        .catch((e) => {
            console.log(e);
            res.status(400).json({ "Error": true, "Message": "There was an error executing the sql query, " + e.message });
        });
});

router.post('/login', function (req, res, next) {
    const name = req.body.username;
    const pass = req.body.password;
    req.db.from('users').select('*')
        .where('Name', name)
        .where('passwordHash', pass)
        .then((rows) => {
            if (rows.length != 0) {
                res.status(200).json({ "Error": false, "Message": "Login sucess", "id": rows[0].ID });
            }
            else {
                res.status(200).json({ "Error": false, "Message": "Login failed" });
            }
        })
        .catch((e) => {
            console.log(e);
            res.status(400).json({ "Error": true, "Message": "There was an error executing the sql query, " + e.message });
        });
});

router.post('/signup', function (req, res, next) {
    const name = req.body.username;
    const pass = req.body.password;
    req.db('users')
        .insert({ Name: name, passwordHash: pass })
        .then(() => {
            res.status(200).json({ "Error": false, "Message": "User registered" });
        })
        .catch((e) => {
            console.log(e);
            res.status(400).json({ "Error": true, "Message": "There was an error executing the sql query, " + e.message });
        });
});

router.post('/changename', function (req, res, next) {
    const id = req.body.id;
    const newname = req.body.newname;
    req.db('users')
        .where({ ID: id })
        .update({ Name: newname })
        .then(() => {
            res.status(200).json({ "Error": false, "Message": "Username updated" });
        })
        .catch((e) => {
            console.log(e);
            res.status(400).json({ "Error": true, "Message": "There was an error executing the sql query, " + e.message });
        });
});

router.post('/changepassword', function (req, res, next) {
    const id = req.body.id;
    const newpass = req.body.newpass;
    req.db('users')
        .where({ ID: id })
        .update({ passwordHash: newpass })
        .then(() => {
            res.status(200).json({ "Error": false, "Message": "Password updated" });
        })
        .catch((e) => {
            console.log(e);
            res.status(400).json({ "Error": true, "Message": "There was an error executing the sql query, " + e.message });
        });
});

module.exports = router;