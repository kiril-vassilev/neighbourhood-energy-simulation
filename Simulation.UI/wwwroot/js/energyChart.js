window.energyChart = {
    chart: null,

    render: function (data, labels) {
        const canvas = document.getElementById("chart");
        if (!canvas || typeof Chart === "undefined") {
            return;
        }

        if (!this.chart) {
            this.chart = new Chart(canvas, {
                type: "line",
                data: {
                    labels: labels,
                    datasets: [{
                        label: "kW",
                        data: data
                    }]
                }
            });
        } else {
            this.chart.data.labels = labels;
            this.chart.data.datasets[0].data = data;
            this.chart.update();
        }
    }
};
