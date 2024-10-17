function countAppointmentsForCase(executionContext) {
    var formContext = executionContext.getFormContext();
    var recordId = formContext.data.entity.getId().replace("{", "").replace("}", ""); // Clean the GUID format

    // Define the OData endpoint for the Web API
    var apiUrl = Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.2/";

    // Construct the query to count appointments for the specified case
    var query = apiUrl + "appointments?$filter=_crbcc_case_value eq " + recordId;

    // Use the fetch API to make the request
    fetch(query, {
        method: "GET",
        headers: {
            "OData-MaxVersion": "4.0",
            "OData-Version": "4.0",
            "Content-Type": "application/json; charset=utf-8",
            "Accept": "application/json",
            "Prefer": "odata.include-annotations=*"
        }
    })
    .then(response => response.json().then((json) => {
        if (response.ok) {
            return [response, json];
        } else {
            throw json.error;
        }
    }))
    .then(responseObjects => {
        var results = responseObjects[1];
        console.log(results);

        // Get the number of appointments
        var appointmentCount = results.value.length;

        // Call the function to update the case with the appointment count
        updateCaseAppointmentCount(recordId, appointmentCount);
    })
    .catch(error => {
        console.error("Error counting appointments: " + error.message);
    });
}

function updateCaseAppointmentCount(caseId, appointmentCount) {
    // Define the OData endpoint for the Web API
    var apiUrl = Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v9.2/crbcc_cases(" + caseId + ")";

    // Create the object to update the case
    var caseUpdate = {
        crbcc_appointmentcount: appointmentCount // Update the custom count field
    };

    // Use fetch API to update the case
    fetch(apiUrl, {
        method: "PATCH",
        headers: {
            "OData-MaxVersion": "4.0",
            "OData-Version": "4.0",
            "Content-Type": "application/json; charset=utf-8",
            "Accept": "application/json",
            "If-Match": "*"
        },
        body: JSON.stringify(caseUpdate)
    })
    .then(response => {
        if (response.ok) {
            console.log("Case appointment count updated successfully.");
        } else {
            return response.json().then(json => { throw json.error; });
        }
    })
    .catch(error => {
        console.error("Error updating case appointment count: " + error.message);
    });
}
