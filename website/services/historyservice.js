function setup(temperatures) {
    let chartSetup = {
        type: 'line',
        data: {
            datasets: []
        },
        options: {
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
        }
    };

    //create keyList of sensorIDs
    let keyList = [];
    temperatures.forEach(function(reading) {
        if (!keyList.includes(reading.SensorID)) {
            keyList.push(reading.SensorID);
        }
    });

    //filter temperatures into array of arrays filteredArrays by sensorID
    let filteredArrays = [];
    keyList.forEach(function(sensorID) {
        filteredArrays.push(temperatures.filter(function(obj) {
            return obj.SensorID === sensorID;
        }));
    });

    //loop through filteredArrays
    filteredArrays.forEach(function(sensor) {
        //create dataset objects
        let dataset = {
            label: sensor[0].SensorID,
            backgroundColor: 'rgba(255, 99, 132, 1)',
            borderColor: 'rgba(255, 99, 132, 1)',
            borderWidth: 1,
            fill: false,
            data: []
        };

        //loop through each array inside filteredArrays
        sensor.forEach(function(element) {
            let coord = { t: element.Date, y: element.temps };
            dataset.data.push(coord);
        });
        //push coord of element into dataset.data
        chartSetup.data.datasets.push(dataset);
    });
    return chartSetup;
}

function setTimeStart(time) {
    if(!isNaN(time)) {
        return new Date(time).toISOString().split('T')[0];
    } else {
        return new Date('1970-01-01').toISOString().split('T')[0];
    }
}

function setTimeEnd(time) {
    if(!isNaN(time)) {
        return new Date(time).toISOString().split('T')[0];
    } else {
        return new Date().toISOString().split('T')[0];
    }
}

module.exports = {
    setup,
    setTimeStart,
    setTimeEnd
};