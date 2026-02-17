
	document.addEventListener("DOMContentLoaded", function () {
		fetch('/Home/TotalContacts')
			.then(response => response.json())
			.then(data => {
				document.getElementById("partyCount").innerText = data.totalParties;
			})
			.catch(error => console.error(error));
	});


	document.addEventListener("DOMContentLoaded", function () {
		fetch('/Home/TotalGiven')
			.then(response => response.json())
			.then(data => {
				document.getElementById("totalgiven").innerText = data.totalGiven;
			})
			.catch(error => console.error(error));
	});

	document.addEventListener("DOMContentLoaded", function () {
		fetch('/Home/TotalReceived')
			.then(response => response.json())
			.then(data => {
				document.getElementById("totalReceived").innerText = data.totalReceived;
			})
			.catch(error => console.error(error));
	});


	document.addEventListener("DOMContentLoaded", function () {
		fetch('/Home/Balance')
			.then(response => response.json())
			.then(data => {
				document.getElementById("balanceamount").innerText = data.netBalance;
			})
			.catch(error => console.error(error));
	});



	document.addEventListener("DOMContentLoaded", function () {
		fetch('/Home/TotalCustomers')
			.then(response => response.json())
			.then(data => {
				document.getElementById("totalCustomers").innerText = data.totalCustomers;
			})
			.catch(error => console.error(error));
	});


	document.addEventListener("DOMContentLoaded", function () {
		fetch('/Home/TotalSuppliers')
			.then(response => response.json())
			.then(data => {
				document.getElementById("totalSuppliers").innerText = data.totalSuppliers;
			})
			.catch(error => console.error(error));
	});

	document.addEventListener("DOMContentLoaded", function () {
		fetch('/Home/TotalActiveAccount')
			.then(response => response.json())
			.then(data => {
				document.getElementById("activeAccounts").innerText = data.totalActiveAccounts;
			})
			.catch(error => console.error(error));
	});


