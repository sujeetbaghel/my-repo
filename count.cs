using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;

namespace CustomPlugin
{
    public class UpdateCaseCountPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Get the execution context
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Check if the context has an appointment record
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity appointment)
            {
                // Initialize the service
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                // Check if the appointment has a case lookup
                if (appointment.Contains("crbcc_case"))
                {
                    EntityReference caseReference = (EntityReference)appointment["crbcc_case"];

                    // Count the number of appointments linked to this case
                    var appointmentCount = CountAppointmentsForCase(service, caseReference.Id);

                    // Update the case entity with the appointment count
                    UpdateCaseWithAppointmentCount(service, caseReference.Id, appointmentCount);
                }
            }
        }

        private int CountAppointmentsForCase(IOrganizationService service, Guid caseId)
        {
            // Create a context for LINQ queries
            using (var context = new OrganizationServiceContext(service))
            {
                // Query appointments linked to the specified case
                var count = (from appointment in context.CreateQuery("appointment")
                             where appointment["crbcc_case"] != null &&
                                   ((EntityReference)appointment["crbcc_case"]).Id == caseId
                             select appointment).Count();

                return count;
            }
        }

        private void UpdateCaseWithAppointmentCount(IOrganizationService service, Guid caseId, int appointmentCount)
        {
            // Create an entity object for the case
            Entity caseEntity = new Entity("crbcc_case", caseId);
            
            // Update the count field (Assuming it's named 'crbcc_appointmentcount')
            caseEntity["crbcc_appointmentcount"] = appointmentCount;

            // Update the case entity
            service.Update(caseEntity);
        }
    }
}
