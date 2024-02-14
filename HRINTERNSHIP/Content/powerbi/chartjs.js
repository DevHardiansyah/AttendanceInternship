$(function() {
    'use strict';

    // Bar chart
    // if($('#chartjsBar').length) {
    //    $.getJSON("/BussinessIntelligence/getStatuEmployee", function (data) { 

    //   new Chart($("#chartjsBar"), {
    //     type: 'bar',
    //     data: {
    //       labels: ["China", "America", "India", "Germany", "Oman"],
    //       datasets: [
    //         {
    //           label: "Population",
    //           backgroundColor: ["#b1cfec","#7ee5e5","#66d1d1","#f77eb9","#4d8af0"],
    //           data: [2478,5267,734,2084,1433]
    //         }
    //       ]
    //     },
    //     options: {
    //       legend: { display: false },
    //     }
    //   });
    //    });
    // }
    Chart.helpers.merge(Chart.defaults.global.plugins.datalabels, {
        color: '#000',

    });
    if ($('#chartjsLine').length) {
        new Chart($('#chartjsLine'), {
            type: 'line',
            data: {
                labels: ['Jan', 'Feb', 'Mar', 'Apr', 'Mei', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
                datasets: [{
                        data: [186, 114, 106, 106, 107, 111, 133, 121],
                        label: "New Hire",
                        borderColor: "#185ADB",
                        fill: true
                    },
                    {
                        data: [200, 214, 236, 226, 250, 214, 223, 211, 283, 278, 211, 200],
                        label: "End Contract",
                        borderColor: "#FFC947",
                        fill: true
                    }
                ],
                options: {
                    responsive: true,
                }
            }
        });
    }

    if ($('#chartjsLine2').length) {
        new Chart($('#chartjsLine2'), {
            type: 'bar',
            data: {
                labels: ['Jan', 'Feb', 'Mar', 'Apr', 'Mei', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
                datasets: [{
                        data: [186, 114, 106, 106, 107, 111, 133, 121],
                        label: "New Hire",
                        backgroundColor: "#185ADB",
                        fill: true
                    },
                    {
                        data: [200, 214, 236, 226, 250, 214, 223, 211, 283, 278, 211, 200],
                        label: "End Contract",
                        backgroundColor: "#FFC947",
                        fill: true
                    }
                ],
                borderWidth: 1,
                options: {
                    responsive: true,
                }
            }
        });
    }


    // based on IDL AND DL
    if ($('#chartjsDoughnut').length) {
        $.getJSON("/BussinessIntelligence/getEmjoba", function(data) {
            new Chart($('#chartjsDoughnut'), {
                type: 'doughnut',
                data: {
                    labels: ["DL", "IDL"],
                    datasets: [{
                        label: "Population (millions)",
                        backgroundColor: ["#FFC947", "#185ADB"],
                        data: [data['DL'], data['IDL']]
                    }]
                },
                plugins: {
                    // Change options for ALL labels of THIS CHART
                    datalabels: {
                        color: '#FFF',
                    }
                },
            });
        });
    }
    // end


    if ($('#chartjsDoughnut1').length) {
        $.getJSON("/BussinessIntelligence/getGender", function(data) {
            new Chart($('#chartjsDoughnut1'), {
                type: 'doughnut',
                data: {
                    labels: ["Male", "Female"],
                    datasets: [{
                        label: "Population (millions)",
                        backgroundColor: ["#FFC947", "#185ADB"],
                        data: [data["Male"], data["Female"]]
                    }]
                },
            });
        });
    }









    if ($('#chartjsLine1').length) {
        new Chart($('#chartjsLine1'), {
            type: 'line',
            data: {
                labels: ['Jan', 'Feb', 'Mar', 'Apr', 'Mei', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
                datasets: [{
                    data: [107, 111, 133, 221, 383, 478, 86, 114, 106, 106, 11, 200],
                    label: "Turnover",
                    borderColor: "#006400",
                    backgroundColor: "rgba(0,0,0,0)",
                    fill: fs
                }],
                options: {
                    maintainAspectRatio: true,
                    responsive: true,
                }
            }
        });
    }

    if ($('#chartjsPie').length) {
        $.getJSON("/BussinessIntelligence/getStatuEmployee", function(data) {
            new Chart($('#chartjsPie'), {
                type: 'doughnut',
                data: {
                    labels: ["Permanent", "Contract"],
                    datasets: [{
                        label: "Population (millions)",
                        backgroundColor: ["#FFC947", "#185ADB"],
                        data: [data['permanent'], data['contract']]
                    }]
                },
                options: {
                    legend: {
                        display: true,

                        position: 'top',
                        labels: {
                            align: 'center',
                        }
                    },
                },
            });
        });
    }

    if ($('#plant').length) {
        $.getJSON("/BussinessIntelligence/getStatuEmployee", function(data) {
            new Chart($('#plant'), {
                type: 'doughnut',
                data: {
                    labels: ["East", "West"],
                    datasets: [{
                        label: "Population (millions)",
                        backgroundColor: ["#FFC947", "#185ADB"],
                        data: [data['permanent'], data['contract']]
                    }]
                },
                options: {
                    showAllTooltips: true,
                    legend: {
                        display: true,

                        position: 'top',
                        labels: {
                            align: 'center',
                        }
                    },
                },
            });
        });
    }

    if ($('#status').length) {
        $.getJSON("/BussinessIntelligence/getStatuEmployee", function(data) {
            new Chart($('#status'), {
                type: 'pie',
                data: {
                    labels: ["Permanent", "Contract", ],
                    datasets: [{
                        label: "Population (millions)",
                        backgroundColor: ["#FFC947", "#185ADB"],
                        data: [data['permanent'], data['contract']]
                    }]
                },
                options: {
                    legend: {
                        display: true,

                        position: 'top',
                        labels: {
                            align: 'center',
                        }
                    },
                },
            });
        });
    }




});