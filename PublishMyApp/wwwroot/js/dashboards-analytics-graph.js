

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
    chartTitle: "Accounts Receivable Summary",
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
    chartTitle: "Monthly Cash Flow",
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
    chartTitle: "Accounts Payable & Receivable Summary",
    endpoint: "/Home/TransactionSummary",
    xField: "partyName",
    seriesConfig: [
        { name: "Given", field: "totalGiven", color: "#2196F3" },
        { name: "Received", field: "totalReceived", color: "#F44336" },

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
renderGroupedBarChart({
    containerId: "weekrevenueChart",
    chartTitle: "Accounts Receivable Summary",
    endpoint: "/Home/TransactionSummary",
    xField: "monthName",
    seriesConfig: [
        { name: "Received", field: "totalGiven", color: "#008000" },
        { name: "Given", field: "totalReceived", color: "#FFA500" }

    ],
    valuePrefix: "Rs. ",
    height: 420
});

renderBubbleChartUniversal({
    containerId: "bubbleChart",
    endpoint: "/Home/TransactionSummary",
    xField: "totalGiven",
    yField: "totalReceived",
    zField: "totalReceived",
    seriesField: "customerName",  // optional
    title: "Given vs Received Bubble",
    valuePrefix: "Rs. "
});

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
