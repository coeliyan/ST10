using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ST10_Syg
{
    class UploadMethods
    {
        // Upload current composition and condition to server and return response
        public static string UploadUroComposition(Uri server, String uri, 
            Condition currentCondition, Composition currentComposition)
        {
            string response = "";
            try
            {
                // Create handler to send response back when uploading to server
                var messageHandler = new HttpClientEventHandler();
                messageHandler.OnAfterResponse += (object sender, AfterHttpResponseEventArgs e) =>
                {
                    response = e.RawResponse.StatusCode.ToString();
                };
                FhirClient fc = new FhirClient(server, null, messageHandler);
                //Upload the local composition and condition to server
                Condition condition = fc.Create<Condition>(currentCondition);
                currentComposition.Event = new List<Composition.EventComponent>()
                {
                    new Composition.EventComponent()
                    {
                        Code = new List<CodeableConcept>()
                        {
                            new CodeableConcept("http://hl7.org/fhir/sid/icd-10", "/DC619M/")
                        },
                        Detail = new List<ResourceReference>()
                        {
                            new ResourceReference(uri + "Condition/" + condition.Id)
                        }
                    }
                };
                fc.Create<Composition>(currentComposition);
            }
            catch (FhirOperationException error)
            {
                MessageBox.Show(error.Message, "FhirOperationException",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return response;
        }
    }
}
