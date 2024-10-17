function opportunity(formControl){
    var recordId = formControl.data.entity.getId();
    recordId = recordId.replace("{", "").replace("}", "");
     // Get the Case entity's primary attribute (Assuming 'title' is the primary attribute for the Case entity)
     var primaryColumnValue = formControl.getAttribute("crbcc_name").getValue();
    
     if (primaryColumnValue) {
         // Define the parameters for the Opportunity form
         var formParameters = {};
         
         formParameters["crbcc_case"] = [{
            id: recordId, // The GUID of the Case
            entityType: "crbcc_case", // Logical name of the Case entity (usually 'incident' for Case entity)
            name: primaryColumnValue // The primary attribute of the Case (e.g., Case title)
        }];
         
         // Open the Opportunity form and pass the parameters
         var entityOptions = {
             entityName: "appointment",
             useQuickCreateForm: false
         };
         
         Xrm.Navigation.openForm(entityOptions, formParameters).then(
             function (success) {
                 console.log("Opportunity form opened successfully.");
             },
             function (error) {
                 console.log("Error opening Opportunity form: " + error.message);
             }
         );
     } else {
         // Handle case where Case title is not available
         Xrm.Utility.alertDialog("The Case does not have a title.");
     }
 }
    