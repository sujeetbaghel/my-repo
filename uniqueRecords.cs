using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CustomPlugin
{
    public class MyPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity appointment = (Entity)context.InputParameters["Target"];

                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                if (appointment.Contains("crbcc_case"))
                {
                    EntityReference caseReference = (EntityReference)appointment["crbcc_case"];

                    // Check for existing leads
                    QueryExpression query = new QueryExpression("crbcc_lead")
                    {
                        ColumnSet = new ColumnSet("crbcc_leadid"),
                        Criteria = new FilterExpression
                        {
                            FilterOperator = LogicalOperator.And,
                            Conditions =
                            {
                                new ConditionExpression("crbcc_appointment", ConditionOperator.Equal, appointment.Id),
                                new ConditionExpression("crbcc_case", ConditionOperator.Equal, caseReference.Id)
                            }
                        }
                    };

                    var existingLeads = service.RetrieveMultiple(query);
                    if (existingLeads.Entities.Count == 0) // Only create a new lead if one doesn't exist
                    {
                        Entity caseEntity = service.Retrieve(caseReference.LogicalName, caseReference.Id, new ColumnSet("crbcc_contact"));

                        EntityReference contactReference = null;
                        if (caseEntity.Contains("crbcc_contact"))
                        {
                            contactReference = (EntityReference)caseEntity["crbcc_contact"];
                        }

                        Entity leadEntity = new Entity("crbcc_lead");

                        leadEntity["crbcc_appointment"] = new EntityReference("appointment", appointment.Id);
                        leadEntity["crbcc_case"] = caseReference;

                        if (contactReference != null)
                        {
                            leadEntity["crbcc_contact"] = contactReference;
                        }

                        service.Create(leadEntity);
                    }
                }
            }
        }
    }
}
