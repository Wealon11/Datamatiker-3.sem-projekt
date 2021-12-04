/* eslint no-undef: "off" */

const http = require('http');
const webservice = require('../services/webservice');

describe('mail service', function() {
    this.timeout(15000);
    
    it('getMails', function(done) {
        let array = [
            'test@tester.com'
        ];
        const server = http.createServer(function(req, res) {
            res.statusCode = 200;
            res.write(JSON.stringify(array));
            res.end();
        }).listen(0, '127.0.0.1', function() {
            const port = server.address().port;
            const ws = new webservice('http:/127.0.0.1:' + port + '/hesten_Alan');
            ws.getMails(function(mailList) {
                server.close();
                if(JSON.stringify(array) === JSON.stringify(mailList)) {
                    done();
                } else {
                    done(new Error());
                }
            });
        });    
    });
    
    it('addMail', function(done) {
        let reqOK = false;
        const server = http.createServer(function(request,response) {
            let body = '';
            request.on('data', function(chunk) {
                body += chunk.toString();
            });
            request.on('end', function() {
                // console.log(req);
                if (request.method === 'POST' && request.url === '/hesten_Alan/mail/' && JSON.stringify({ mail: "ex@ample.com" }) === body && request.headers["content-type"] === "application/json") {
                    reqOK = true;
                }
                response.statusCode = 200;
                response.end();
            });
        }).listen(0, '127.0.0.1', function() {
            const port = server.address().port;
            const ws = new webservice('http://127.0.0.1:' + port + '/hesten_Alan');
            ws.addMail('ex@ample.com', function(ok) {
                server.close();
                if(ok && reqOK) {
                    done();
                } else {
                    done(new Error());
                }
            });
        });
    });
    
    it('deleteMail', function(done) {
        let reqOK = false;
        const server = http.createServer(function(request,response) {
            if (request.method === 'DELETE' && request.url === '/hesten_Alan/mail/?addr=ex@ample.com/' && request.headers["content-type"] === "application/json") {
                reqOK = true;
            }
            response.statusCode = 200;
            response.end();
        }).listen(0, '127.0.0.1', function() {
            const port = server.address().port;
            const ws = new webservice('http://127.0.0.1:' + port + '/hesten_Alan');
            ws.deleteMail('ex@ample.com', function(ok) {
                server.close();
                if(ok && reqOK) {
                    done();
                } else {
                    done(new Error());
                }
            });
        });
    });
});
