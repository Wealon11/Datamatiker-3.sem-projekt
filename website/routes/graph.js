module.exports = function(webservice) {
    const express = require('express');
    const router = express.Router();
    const { CanvasRenderService } = require('chartjs-node-canvas');
    const chartservice = require('../services/historyservice');

    let start;
    let end;

    router.get('/', function(req, res) {
        start = req.query.start;
        end = req.query.end;
        res.render('graph');
    });

    router.get('/pic', function(req, res) {
        let startTime = Date.parse(start);
        let endTime = Date.parse(end);

        let startDate = chartservice.setTimeStart(startTime);
        console.log('startDate:');
        console.log(startDate);
        let endDate = chartservice.setTimeEnd(endTime);
        console.log('endDate:');
        console.log(endDate);

        webservice.showListTemp(startDate, endDate, function(temperaturelist) {
            let chart = new CanvasRenderService(600, 600);
            chart.renderToBuffer(chartservice.setup(temperaturelist))
                .then(buffer => {
                    res.set('Content-Type', 'image/png');
                    res.send(buffer);
                    console.log("buffer sent");
                });
        });
    });

    return router;
};