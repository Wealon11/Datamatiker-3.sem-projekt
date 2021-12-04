/* eslint no-undef: "off" */

const http = require('http');
const webservice = require('../services/webservice');

describe('webservice', function() {
    it('setMaxTemp', function(done) {
        let requestOK = false;
        const server = http.createServer(function(request, response) {
            let body = '';
            request.on('data', function(chunk) {
                body += chunk.toString();
            });
            request.on('end', function() {
                if (request.method === 'PUT' && request.url === '/hesten_Alan/maxtemp/' && JSON.stringify({ Maxtemps: 5 }) === body && request.headers["content-type"] === "application/json") {
                    requestOK = true;
                }

                response.statusCode = 200;
                response.end();
            });

        }).listen(0, '127.0.0.1', function() {
            const port = server.address().port;
            const ws = new webservice('http://127.0.0.1:' + port + '/hesten_Alan');
            ws.setMaxTemp(5, function(ok) {
                server.close();
                if (ok && requestOK) {
                    done();
                } else {
                    done(new Error());
                }
            });
        });
    });

    it('validateCredentials', function(done) {
        let requestOK = false;
        const server = http.createServer(function(request, response) {
            let body = '';
            request.on('data', function(chunk) {
                body += chunk.toString();
            });

            request.on('end', function() {
                const expectdata = { User: 'NotAdmin', Password: '12345password' };
                if (request.method === 'POST' && request.url === '/Zebraen_Helene/encrypt/' && JSON.stringify(expectdata) === body) {
                    requestOK = true;
                }

                response.statusCode = 200;
                response.write("2");
                response.end();
            });
        }).listen(0, '127.0.0.1', function() {
            const port = server.address().port;
            const ws = new webservice('http://127.0.0.1:' + port + '/Zebraen_Helene');
            ws.validateCredentials("NotAdmin", "12345password", function(status) {
                server.close();
                if (status === 2 && requestOK) { 
                    done();
                } else {
                    done(new Error(`Unexpected Status: status=${status}, requestOK=${requestOK}`));
                }
            });
        });
    });


    it('showListTemp', function(done){
        var array = [
            {
                "Date": "2018-05-17T13:07:15.2800000",
                "ID": 1,
                "temps": 20
            }
        ];
        const server = http.createServer(function(request, response){

            response.statusCode = 200;
            response.write(JSON.stringify(array));
            response.end();
        }).listen(0, '127.0.0.1', function(){

            const port = server.address().port;
            const ws = new webservice('http:/127.0.0.1:' + port + '/hesten_Alan');
            ws.showListTemp('a', 'b', function(tempList) {
                server.close();
                if(JSON.stringify(array) === JSON.stringify(tempList)){
                    done();
                    
                }else{
                    done(new Error());
                }
            });
        });
    });
});

