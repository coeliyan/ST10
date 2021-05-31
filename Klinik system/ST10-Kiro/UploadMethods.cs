using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace ST10_Kiro
{
    class UploadMethods
    {
        // Method to upload current composition from chiropractor to server
        public static string UploadKiroComposition(string uri, Practitioner currentPrac, Patient currentPatient,
            DataGridView anamneseGridView, Uri local, Location currentLoc)
        {
            // Create current observation 
            string response = "";
            Observation observation = CreateResources.CreateSymptom(uri, currentPrac,
                   currentPatient, "161891005", new FhirDateTime(anamneseGridView.Rows[7].Cells[1].Value.ToString()),
                   anamneseGridView.Rows[2].Cells[1].Value.ToString(),
                   anamneseGridView.Rows[12].Cells[1].Value.ToString(), anamneseGridView.Rows[11].Cells[1].Value.ToString(),
                   anamneseGridView.Rows[9].Cells[1].Value.ToString(), anamneseGridView.Rows[10].Cells[1].Value.ToString());
            FhirDateTime date = new FhirDateTime(anamneseGridView.Columns[1].HeaderText);
            string conclusion = anamneseGridView.Rows[38].Cells[1].Value.ToString();
            try
            {
                // Create handler to send response back
                var messageHandler = new HttpClientEventHandler();
                messageHandler.OnAfterResponse += (object sender, AfterHttpResponseEventArgs e) =>
                {
                    response = e.RawResponse.StatusCode.ToString();
                };
                FhirClient fc = new FhirClient(local, null, messageHandler);
                // Upload current observation
                Observation uploadedObservation = fc.Create<Observation>(observation);
                // Create current composition
                Composition composition = CreateResources.CreateCompositionWithObservation(uri, date,
                    conclusion, currentPatient, currentPrac, currentLoc, uploadedObservation);
                //Upload current composition
                fc.Create<Composition>(composition);
            }
            catch (FhirOperationException error)
            {
                MessageBox.Show(error.Message, "FhirOperationException",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return response;
        }

        // Method to create instances that should exist on server beforehand
        public static void CreatedResources(FhirClient client, string uri)
        {
            List<string> ids = new List<string>();

            Practitioner kiro = client.Create<Practitioner>(CreateResources.CreatePractitioner(
                "Rasmussen", "Alexandra", "05Y9F"));
            Practitioner uro = client.Create<Practitioner>(CreateResources.CreatePractitioner(
                "Poulsen", "Ulf", "02Y8F"));
            Practitioner radi = client.Create<Practitioner>(CreateResources.CreatePractitioner(
                "Jeppesen", "Klaus", "01Y6F"));
            Patient patient = client.Create<Patient>(CreateResources.CreatePatient());

            Location locationAUH = client.Create<Location>(CreateResources.CreateLocationWithID("Aalborg UH",
                "https://medinfo.dk/sks/brows.php", "801040"));
            Location locationKiroKlinik = client.Create<Location>(CreateResources.CreateLocationWithID(
                "Helledie Kiropraktisk Klinik",
                "https://sundhedsdatastyrelsen.dk/da/registre-og-services/om-sor", "77777"));

            Endpoint endpoint = client.Create<Endpoint>(CreateResources.CreateEndpoint("dicom-c-get",
                "dcm", "www.dicomserver.co.uk "));
            ImagingStudy imagingStudyMR = client.Create<ImagingStudy>(CreateResources.CreateImagingStudy(
                uri, "1.3.6.1.4.1.9590.100.1.2.102354115603628795327681137482115272724", "MR", "DZ031J",
                "1.3.6.1.4.1.9590.100.1.2.102354115603628795327681137482115272724", "181422007",
                new FhirDateTime(2018, 03, 09),
                "1.3.6.1.4.1.9590.100.1.2.408893648929939944924945733503707919945",
                "1.2.840.10008.5.1.4.1.1.7", patient, radi, endpoint, locationAUH));
            DiagnosticReport diagnosticReportMR = client.Create<DiagnosticReport>(CreateResources.CreateDiagnosticReport(
                uri, "UXMD92", "366292007", "Størrelse og lokation: T3", new FhirDateTime(2018, 03, 09),
                radi, patient, imagingStudyMR));

            ImagingStudy imagingStudyCT = client.Create<ImagingStudy>(CreateResources.CreateImagingStudy(
                uri, "1.3.6.1.4.1.9590.100.1.2.102354115603628795327681137482115272724", "CT", "DR31",
                "1.3.6.1.4.1.9590.100.1.2.102354115603628795327681137482115272724", "181422007",
                new FhirDateTime(2018, 03, 09),
                "1.3.6.1.4.1.9590.100.1.2.408893648929939944924945733503707919945",
                "1.2.840.10008.5.1.4.1.1.7", patient, radi, endpoint, locationAUH));
            DiagnosticReport diagnosticReportCT = client.Create<DiagnosticReport>(CreateResources.CreateDiagnosticReport(
                uri, "UXCD62", "25950000", "Infektion i urinvejene er udelukket",
                new FhirDateTime(2018, 03, 09), radi, patient, imagingStudyCT));

            ImagingStudy imagingStudyKS = client.Create<ImagingStudy>(CreateResources.CreateImagingStudy(
                uri, "1.3.6.1.4.1.9590.100.1.2.102354115603628795327681137482115272724", "NM", "DZ031N",
                "1.3.6.1.4.1.9590.100.1.2.102354115603628795327681137482115272724", "181422007",
                new FhirDateTime(2018, 03, 09),
                "1.3.6.1.4.1.9590.100.1.2.408893648929939944924945733503707919945",
                "1.2.840.10008.5.1.4.1.1.7", patient, radi, endpoint, locationAUH));
            DiagnosticReport diagnosticReportKS = client.Create<DiagnosticReport>(CreateResources.CreateDiagnosticReport(
                uri, "WKBGW19XX", "261928007", "M1: Der er metastaser (spredning til knogler eller andre organer)",
                new FhirDateTime(2018, 03, 09), radi, patient, imagingStudyKS));

            Debug.WriteLine("Patient: " + patient.Id);
            Debug.WriteLine("Kiro: " + kiro.Id);
            Debug.WriteLine("KK: " + locationKiroKlinik.Id);
            Debug.WriteLine("Uro: " + uro.Id);
            Debug.WriteLine("AUH: " + locationAUH.Id);

        }
    }
}
