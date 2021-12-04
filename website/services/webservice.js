const url = require('url');
const http = require('http');

class WebserviceClient {

    constructor(serviceURL) {
        this.serviceURL = new url.URL(serviceURL);
    }

    setMaxTemp(maxTemp, callback) {
        const options = {
            method: 'PUT',
            host: this.serviceURL.hostname,
            port: this.serviceURL.port,
            path: this.serviceURL.pathname +'/maxtemp/',
            headers: { "content-type": "application/json" }
        };
        const req = http.request(options, function(res) {
            callback(res.statusCode === 200);

        });
        req.write(JSON.stringify({
            Maxtemps: maxTemp
        }));
        req.end();
    }

    showListTemp(startdate,enddate,callback){
        const options = {
            method: 'GET',
            host: this.serviceURL.hostname,
            port: this.serviceURL.port,
            path: this.serviceURL +`/alltemps/?start=${startdate}&end=${enddate}`
        };
        console.log('serviceURL: ' + options.path);
        const req = http.request(options, function(res){
            let body = '';
            res.on('data', function(chunk) {
                body += chunk.toString();
            });

            res.on('end', function(){
                console.log(res.statusCode);
                callback(JSON.parse(body));
            });
           
        });

        req.end();
    }

    validateCredentials(user, password, callback){
        const options = {
            method: 'POST',
            host: this.serviceURL.hostname,
            port: this.serviceURL.port,
            path: this.serviceURL.pathname + '/encrypt/',
            headers: { "content-type": "application/json" }
        };
        const req = http.request(options, function(res){
            let body = '';
            res.on('data', function(chunk) {
                body += chunk.toString();
            });
                
            res.on('end', function(){
                callback(parseInt(body));

            });
        });
        req.write(JSON.stringify({
            User: user,
            Password: password
        }));
        req.end();
    }

    getMails(callback) {
        const options = {
            method: 'GET',
            host: this.serviceURL.hostname,
            port: this.serviceURL.port,
            path: this.serviceURL +'/mail/'
        };
        console.log('serviceURL: ' + options.path);
        const req = http.request(options, function(res) {
            let body = '';
            res.on('data', function(chunk) {
                body += chunk.toString();
            });
    
            res.on('end', function() {
                console.log(body);
                console.log(res.statusCode);
                callback(JSON.parse(body));
            });
        });
        req.end();
    }

    addMail(mailAddr, callback) {
        const options = {
            method: 'POST',
            host: this.serviceURL.hostname,
            port: this.serviceURL.port,
            path: this.serviceURL.pathname + '/mail/',
            headers: { 'content-type': 'application/json' }
        };
        const req = http.request(options, function(res) {
            callback(res.statusCode === 200);

        });
        req.write(JSON.stringify({
            mail: mailAddr
        }));
        req.end();
    }

    deleteMail(mailAddr, callback) {
        const options = {
            method: 'DELETE',
            host: this.serviceURL.hostname,
            port: this.serviceURL.port,
            path: this.serviceURL.pathname + decodeURIComponent(`/mail/?addr=${mailAddr}/`),
            headers: {
                'content-type': 'application/json',
                'transfer-encoding': 'chunked'
            }
        };
        const req = http.request(options, function(res) {
            callback(res.statusCode === 200);
        });
        req.end();
    }
}

module.exports = WebserviceClient;