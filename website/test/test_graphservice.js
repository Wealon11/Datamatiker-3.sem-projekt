/* eslint no-undef: "off" */

const chartservice = require('../services/historyservice');
const assert = require('assert');

describe('graph service', function() {
    it('graph setup', function() {
        let chartsetup = chartservice.setup([]);
        assert.strictEqual(chartsetup.type, 'line');
        const options = {
            scales: {
                xAxes: [
                    {
                        type: 'time',
                        distribution: 'linear',
                        time: {
                            unit: 'month'
                        }
                    },
                    {
                        scaleLabel: {
                            labelString: 'Time'
                        }
                    }]
            }
        };
        assert.deepStrictEqual(chartsetup.options, options);
    });

    it('temperature converted correctly', function() {
        let chartsetup = chartservice.setup([{SensorID: 'raspberrypi', temps: 20, ID: 1, Date: '2018-01-01'}]);
        let coords = [{t: '2018-01-01', y: 20}];
        assert.strictEqual(chartsetup.data.datasets.length, 1);
        let dataset = chartsetup.data.datasets[0];
        assert.deepStrictEqual(dataset.data, coords);
    });

    it('should set start time to user-defined time', function() {
        let time = new Date('2000-01-01');
        assert.strictEqual(chartservice.setTimeStart(time), time.toISOString().split('T')[0]);
    });

    it('should set start time to default time', function() {
        let time = Date.parse('asdf');
        assert.strictEqual(chartservice.setTimeStart(time), '1970-01-01');
    });

    it('should set end time to user-defined time', function() {
        let time = new Date('2000-01-01');
        assert.strictEqual(chartservice.setTimeEnd(time), time.toISOString().split('T')[0]);
    });

    it('should set end time to default time', function() {
        let time = Date.parse('asdf');
        console.log('date: ');
        console.log(new Date());
        assert.strictEqual(chartservice.setTimeEnd(time), new Date().toISOString().split('T')[0]);
    });
});
