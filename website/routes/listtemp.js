const { stringify } = require('querystring');
const listservice = require('../services/historyservice');

 module.exports = function(webservice) {
    const express = require('express');
    const router = express.Router();
    
    router.get('/', function(req, res) {
      let start = listservice.setTimeStart(Date.parse(req.query.start));
      let end = listservice.setTimeEnd(Date.parse(req.query.end));

      const cb = function(temperaturelist) {
        res.render('listtemp', {temp:temperaturelist});
      };

      webservice.showListTemp(start, end, cb);
    });
  
    return router;
  };
  
 