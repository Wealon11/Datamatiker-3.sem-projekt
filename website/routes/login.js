module.exports = function(webservice) {
    const express = require('express');
    const router = express.Router();

    router.get('/login',function(req,res) {
        res.render('login',{title: "login", loginErrorMsg: null});
    });

    router.post('/login', function(req, res) {
        webservice.validateCredentials(req.body.username, req.body.password, function(status) {
            const loginOK = status === 2;
            if (loginOK) {
                req.session.authenticatedAs = req.body.username;
                res.redirect('/');
            } else {
                res.render('login', 
                {title: "login", loginErrorMsg: `login failed (status ${status}`}, 
                function(err, html) {
                    res.statusCode = 400; 
                    res.send(html);
                });
            }
            res.end();
        });
    });

    router.post('/logout', function(req, res) {
        req.session.destroy(function() {
            res.redirect('/');
        });
    });

    return router;
};

