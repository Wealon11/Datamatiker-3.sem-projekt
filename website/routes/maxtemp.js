module.exports = function(webservice) {
  const express = require('express');
  const router = express.Router();

  router.get('/', function(req, res) {
    if (req.session.authenticatedAs === undefined) {
      res.redirect('/login');
    } else {
      res.render('maxtemp');
    }
  });

  router.post('/', function(req, res) {
    const cb = function(ok) {
      if (ok) {
        res.sendStatus(200);
      } else {
        res.sendStatus(500);
      }
      res.end();
    };
    webservice.setMaxTemp(parseInt(req.body.maxtemp), cb);
  });

  return router;
};
