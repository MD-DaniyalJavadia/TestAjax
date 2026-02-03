

renderIncomeChart({
    chartSelector: '#myChartLegend5',
    endpoint: '/Home/MonthlyTransactionSummary',
    xField: 'monthName', // X-axis
    yField: 'totalGiven', // Y-axis
    title: '',
    valuePrefix: '',
    xAxisTitle: '',
    yAxisTitle: ''
});

renderIncomeChart({
    chartSelector: '#myChartLegend4',
    endpoint: '/Home/MonthlyTransactionSummary',
    xField: 'monthName', // X   -axis
    yField: 'totalReceived', // Y-axis
    title: '',
    colors: { primary: '#FF0000', white: '#fff' },
    valuePrefix: '',
    xAxisTitle: '',
    yAxisTitle: ''
});


renderGroupedBarChart({
    containerId: "GroupChart2",
    chartTitle: "Ledger Wise Received",
    endpoint: "/Home/TransactionSummary",
    xField: "partyName",
    seriesConfig: [
        { name: "Received", field: "totalReceived", color: "#F44336" }
    ],
    valuePrefix: "Rs. ",
    height: 420
});


renderGroupedBarChart({
    containerId: "GroupChart6",
    chartTitle: "Ledger Wise Given / Received",
    endpoint: "/Home/TransactionSummary",
    xField: "monthName",
    seriesConfig: [
        { name: "Given", field: "totalGiven", color: "#2196F3" },
        { name: "Received", field: "totalReceived", color: "#F44336" }
    ],
    valuePrefix: "Rs. ",
    height: 420
});

renderGroupedBarChart({
    containerId: "GroupChart1",
    chartTitle: "Ledger-wise Given",
    endpoint: "/Home/TransactionSummary",
    xField: "partyName",
    seriesConfig: [
        { name: "Given", field: "totalGiven", color: "#2196F3" }
    ],
    valuePrefix: "Rs. ",
    height: 420
});




//renderIncomeChart({
//    chartSelector: '#myChartLegend4',
//    endpoint: '/Home/TransactionSummary',
//    xField: 'monthName', // X-axis
//    yField: 'totalReceived', // Y-axis
//    title: '',
//    colors: { primary: '#FF0000', white: '#fff' },
//    valuePrefix: '',
//    xAxisTitle: '',
//    yAxisTitle: ''
//});


renderIncomeChart2({
    chartSelector: '#myChartLegend3',
    endpoint: '/Home/MonthlyTransactionSummary',
    xField: 'monthName', // X-axis
    yFields: [
        { field: 'totalGiven', name: 'Given' },
        { field: 'totalReceived', name: 'Received' }
    ],
    labelRotation: -45,
    title: '',
    valuePrefix: '',
    xAxisTitle: '',
    yAxisTitle: ''
});
