module.exports = function(webservice) {
    const express = require('express');
    const router = express.Router();

    router.get('/', function(req, res) {
        const cb = function(mailList) {
            res.render('mail', {mails:mailList});
        };

        webservice.getMails(cb);
    });

    router.post('/', function(req, res) {
        const cb = function(ok) {
            if(ok) {
                res.sendStatus(200);
            } else {
                res.sendStatus(500);
            }
            res.end();
        };
        webservice.addMail(req.body.newemail, cb);
    });

    router.delete('/', function(req, res) {
        const cb = function(ok) {
            if(ok) {
                res.sendStatus(200);
            } else {
                res.sendStatus(500);
            }
            res.end();
        };
        webservice.deleteMail(decodeURIComponent(req.query.addr), cb);
    });

    return router;
};