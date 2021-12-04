/* eslint no-undef: "off" */

const assert = require('assert');
const request = require('supertest');

class FakeWebservice {
	constructor() {
		this.newMaxTemp = null;
		this.loginRequest = {};
	}

	setMaxTemp(maxTemp, callback) {
		this.newMaxTemp = maxTemp;
		callback(true);
	}

	validateCredentials(username, password, callback) {
		this.loginRequest.username = username;
		this.loginRequest.password = password;
		if (this.loginRequest.username === "chris" && this.loginRequest.password === "password") {
			callback(2);
		} else {
			callback(0);
		}
	}
}

describe('max temp page', function() {
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

	it('should redirect to login page if not logged in', function(done) {
		request(server)
			.get('/')
			.expect(302, done);
	});

	it('POST should send maxtemp update request to webservice', function(done) {
		request(server)
			.post('/')
			.send("maxtemp=42")
			.then(res => {
				assert.strictEqual(res.statusCode, 200);
				assert.strictEqual(ws.newMaxTemp, 42);
				done();
			});
	});
});

describe('login page', function() {
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

	it('should load', function(done) {
		request(server)
			.get('/login')
			.expect('content-type', 'text/html; charset=utf-8')
			.expect(200, done);
	});

	it('should send credentials for validation to webservice on POST', function(done) {
		request(server)
			.post('/login')
			.send('username=chris&password=password')
			.then(function(res) {
				assert.strictEqual(res.statusCode, 302);
				assert.strictEqual("chris", ws.loginRequest.username);
				assert.strictEqual("password", ws.loginRequest.password);
				done();
			}).catch(function(){
				done(new Error());
			});
	});

	it('login page should not redirect after login failure', function(done) {
		request(server)
		.post('/login')
		.send('username=bobdylan&password=ZeroCalories')
		.then(function(res){
			assert.strictEqual(res.statusCode, 400);
			done();
		}).catch(function(){
			done(new Error());
		});
	});
});

describe('mail page', function() {
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
});