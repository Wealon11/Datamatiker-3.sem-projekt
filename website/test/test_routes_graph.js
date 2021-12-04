/* eslint no-undef: "off" */

const request = require('supertest');
const assert = require('assert');
const url = require('url');

describe('graph route', function() {
	this.timeout(15000);
	let app;
	let server;

	beforeEach(function() {
		const config = {
			webservice: {
				showListTemp: function(startdate, enddate, callback) {
					callback([
						{ID: 1, temps: 15, Date: '2016-01-01'},
						{ID: 2, temps: 17, Date: '2016-01-02'},
						{ID: 3, temps: 19, Date: '2016-01-03'}
					]);
				}
			}
		};
		app = require('../app')(config);
		server = app.listen(3000);
	});

	afterEach(function() {
		server.close();
	});

	it('should return a PNG image', function(done) {
		request(server)
			.get('/graph/pic')
			.expect("Content-Type", "image/png")
			.expect(200, done);
	});

	it('should return 200', function(done) {
		request(server)
		.get('/graph')
		.expect("Content-Type", "text/html; charset=utf-8")
		.expect(200, done);
	});

	it('should contain start and end parameters', function(done) {
		let path = url.parse('/graph?start=1970-01-01&end=2000-01-01', true);
		let start = '1970-01-01';
		let end = '2000-01-01';
		assert.strictEqual(path.query.start, start);
		assert.strictEqual(path.query.end, end);

		request(server)
		.get('/graph?start=1970-01-01&end=2000-01-01')
		.expect(200, done);
	});
});
