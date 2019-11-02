var express = require('express');
var router = express.Router();

/* GET api listing. */
router.get('/', function(req, res, next) {
  res.send('respond with a resource');
});

router.get('/test', function(req, res, next) {
  res.send('this is a POST route.');
});

router.post("/test", function(req,res){
  if(!req.body.username || !req.body.password){
    res.status(400).json({message: 'missing required body values'});
    console.log("Error on request body: ",JSON.stringify(req.body));
  }
  else{
    res.status(200).json({message: 'done'});
    console.log("sucess, request body: ",JSON.stringify(req.body));
  }
});

router.get('/uav/:username', function(req, res, next) {
  const name = req.params.username;
  req.db.from('users').select('*')
  .where('Name', name)
  .then((rows) =>{
    if(rows.length != 0){
      res.status(200).json({"Error": false, "Message": "name taken"});
    }
    else{
      res.status(200).json({"Error": false, "Message": "name free"});
    }
  })
  .catch((e) =>{
    console.log(e);
    res.status(400).json({"Error": true, "Message": "There was an error executing the sql query, "+e.message});
  });
});


module.exports = router;
