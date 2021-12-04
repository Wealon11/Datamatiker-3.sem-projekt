/* eslint no-undef: "off" */

const request = require('supertest');

class FakeWebservice {
    getMails(callback) {
        callback(JSON.parse('[ "ex@ample.com" ]'));
    }
}

describe('mail route', function() {
    let app;
    let server;
    let ws = new FakeWebservice();

    beforeEach(function() {
        const config = {
            webservice: ws
        };
        app = require('../app')(config);
        server = app.listen(3000);
    });

    afterEach(function() {
        server.close();
        ws = new FakeWebservice();
    });

    it('should return 200', function(done) {
        request(server)
            .get('/mail')
            .expect('Content-Type', 'text/html; charset=utf-8')
            .expect(200, done);
    });
});