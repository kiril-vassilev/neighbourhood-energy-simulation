window.energyChart = {
    chart: null,

    render: function (data, dataWithBattery, labels) {
        const canvas = document.getElementById("chart");
        if (!canvas || typeof Chart === "undefined") {
            return;
        }

        if (!this.chart) {
            this.chart = new Chart(canvas, {
                type: "line",
                data: {
                    labels: labels,
                    datasets: [
                        {
                            label: "Load (kW)",
                            data: data,
                            borderColor: "rgb(255, 99, 132)",
                            backgroundColor: "rgba(255, 99, 132, 0.1)",
                            tension: 0.1
                        },
                        {
                            label: "Load with Battery (kW)",
                            data: dataWithBattery,
                            borderColor: "rgb(54, 162, 235)",
                            backgroundColor: "rgba(54, 162, 235, 0.1)",
                            tension: 0.1
                        }
                    ]
                }
            });
        } else {
            this.chart.data.labels = labels;
            this.chart.data.datasets[0].data = data;
            this.chart.data.datasets[1].data = dataWithBattery;
            this.chart.update();
        }
    }
};
