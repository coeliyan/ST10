using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using Hl7.Fhir;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Model;
using Hl7.Fhir.Validation;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.ElementModel;
using System.Text.RegularExpressions;

namespace ST10_Kiro
{
    public partial class Form1 : Form
    {
        
        //Change the variable under client to change between servers
        //without having to change the rest of the code.
        private static readonly Uri firely = new Uri("https://server.fire.ly");
        private static readonly Uri local = new Uri("http://localhost:4080/");
        
        private static readonly FhirClient client = new FhirClient(local);
        public static string response = "";



        //What server created resources should be uploaded on:
        //private static readonly string  uri = "https://server.fire.ly/";
        private static readonly string  uri = "http://localhost:4080/";

        //Firely:
        //private static Practitioner currentPrac = client.Read<Practitioner>(uri + "Practitioner/"
        //        + "dbee2678-cc4f-43e3-a204-5f77b2cf79f4");
        //private static Location currentLoc = client.Read<Location>(uri + "Location/"
        //        + "11f1b020-5d6a-4c0f-a09d-854881f8e22c");

        //Local instances alread created:
        private static Practitioner currentPrac = client.Read<Practitioner>(uri + "Practitioner/"
                + "b1fa2927-a3d3-4fdf-b28f-58a875a1b8c3");
        private static Location currentLoc = client.Read<Location>(uri + "Location/"
                + "f2f3be96-aa6f-441e-be0e-d6d34f1dd4f4");

        // Find the given patient
        public static Patient currentPatient = SearchMethods.FindPatient(client);

        public static List<DiagnosticReport> drList = new List<DiagnosticReport>();
        public List<Composition> compositionList = new List<Composition>();
        public List<string> drIDList = new List<string>();
        public List<string> idList;
        public int columnNumber = 1;

        public Form1()
        {
            InitializeComponent();
            LayCollection.AddPatientInfoToUI(caveBox, patientLabel, currentPatient);
            //UploadMethods.CreatedResources(client, uri); //For creating and uploading instances that should existing on the server beforehand.
            LayCollection.SetupAnamnese(anamneseGridView); // Composition table setup
            LayCollection.SetupXrayGrid(xRayGridView); // DiagnosticReport table setup
            

            //Create new instance of composition for chiropractor
            columnNumber = LayCollection.AnamneseKiroContact(new FhirDateTime(2021, 04, 06),
                 "Rygsmerter", "Ikke relevant", "Smerter i den nedre del af rygsøjlen",
                 "Ikke relevant", "Ingen", "", "2021-03-06", "Pensionist",
                 "Vides ikke, har dog været i haven", "Smerter er blevet værre og værre igennem den sidste md. ",
                 "Hvile", "Bevægelse", "", "", "", "", "", "God", "", "", "", "", "", "", "", "", "", "", "", "",
                 "Pensionist", "", "Enlig", "Pensionist", "", "Pt. har smerter i den nedre del af ryggen. " +
                 "Der er udført manipulationsbehandling på L3-L4. Konsultation igen om 3 dage", anamneseGridView, columnNumber);

            UpdateAnamneseUI();
            UpdateDR();

        }

        // Button to update UI with new compositions
        private void button2_MouseClick(object sender, MouseEventArgs e)
        {
            int newCompositions = UpdateAnamneseUI();
            if (newCompositions == 0)
            {
                MessageBox.Show("Ingen nye journalnotater.", "", MessageBoxButtons.OK);
            };
        }

        // Button to send current composition to server
        private void button4_Click(object sender, EventArgs e)
        {
            string response = UploadMethods.UploadKiroComposition(uri, currentPrac, currentPatient,
                anamneseGridView, local, currentLoc);

            // Display messagebox depending on outcome of upload
            if (response == "Created")
            {
                MessageBox.Show("Succes", "", MessageBoxButtons.OK);
                this.Close();
            }
            else
            {
                MessageBox.Show(response, "",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                response = "";
            }
        }

        // Method to open PACS when diagnosticreport is clicked
        private void xRayGridView_DoubleClick(object sender, EventArgs e)
        {
            int row = 0;
            for (int i = 0; i < xRayGridView.SelectedRows.Count; i++)
            {
                if (xRayGridView.SelectedRows[i].Selected)
                {
                    try
                    {
                        // Read which report is clicked
                        row = xRayGridView.CurrentRow.Index;
                        DiagnosticReport diagnosticReport = drList[row];
                        ImagingStudy imagingStudy = client.Read<ImagingStudy>(LayCollection.UriFromReference(diagnosticReport.ImagingStudy));
                        string currentStudyUID = imagingStudy.Identifier[0].Value.ToString();
                        // Send ID on report to PACS program
                        DICOMtest.Form1.UpdateStudyUIDTransfer(drIDList);
                        DICOMtest.Form1.UpdateInfo(currentStudyUID,
                            LayCollection.NameToString(currentPatient.Name),
                            LayCollection.IdToStringCPR(currentPatient.Identifier));
                        var test = new DICOMtest.Form1();
                        test.Show();
                    }
                    catch (FhirOperationException error)
                    {
                        MessageBox.Show(error.Message, "FhirOperationException",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            xRayGridView.ClearSelection();
        }


        // Method to update composition on UI from updated information from server
        public int UpdateAnamneseUI()
        {
            int newCompositions = 0;
            try
            {
                // Search for compostions
                List<Composition> compositionListCurrent = SearchMethods.GetCompositions(client, new string[]
                    { "Composition?subject=Patient/" + currentPatient.Id}, 10);
                int listCount = compositionList.Count();

                // If any compositions are already downloaded from server add their id to list
                if (listCount != 0)
                {
                    idList = new List<string>();
                    foreach (Composition composition in compositionList)
                    {
                        idList.Add(composition.Id);
                    }
                }

                foreach (Composition composition in compositionListCurrent)
                {
                    if (composition.Subject.Reference == uri + "Patient/" + currentPatient.Id)
                    {
                        // If the composition is not from the current location
                        if (composition.Section[0].Entry[0].Reference != uri + "Location/" + currentLoc.Id)
                        {
                            // If there already exits downloaded compositions
                            if (listCount != 0)
                            {
                                // If there are any new compositions add to UI
                                if (!idList.Contains(composition.Id))
                                {
                                    compositionList.Add(composition);
                                    Condition relatedCondition = client.Read<Condition>(composition.Event[0].Detail[0].Reference);
                                    Practitioner relatedPractitioner = client.Read<Practitioner>(LayCollection.UriFromReference(composition.Author));
                                    Location relatedLocation = client.Read<Location>(composition.Section[0].Entry[0].Reference);
                                    columnNumber = LayCollection.NewAnamneseServer(composition, relatedPractitioner, relatedCondition,
                                        relatedLocation, drList, columnNumber, caveBox, anamneseGridView);
                                    newCompositions++;
                                }
                            }
                            else // Add all found compositions to UI
                            {
                                compositionList.Add(composition);
                                Condition relatedCondition = client.Read<Condition>(composition.Event[0].Detail[0].Reference);
                                Practitioner relatedPractitioner = client.Read<Practitioner>(LayCollection.UriFromReference(composition.Author));
                                Location relatedLocation = client.Read<Location>(composition.Section[0].Entry[0].Reference);
                                columnNumber = LayCollection.NewAnamneseServer(composition, relatedPractitioner, relatedCondition,
                                        relatedLocation, drList, columnNumber, caveBox, anamneseGridView);
                                newCompositions++;
                            }
                        }
                    }
                }
            }
            catch (FhirOperationException e)
            {
                MessageBox.Show(e.Message, "FhirOperationException",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return newCompositions;
        }

        // Update diagnosticreport table with reports from server
        public void UpdateDR()
        {
            try
            {
                // Search for diagnosticreports on server
                List<DiagnosticReport> drListCurrent = SearchMethods.GetDiagnosticReports(client, new string[]
                    { "DiagnosticReport?subject=Patient/" + currentPatient.Id}, 10);
                int listCount = drList.Count();

                // If diagnosticreports are already downloaded add their id to list
                if (listCount != 0)
                {
                    idList = new List<string>();
                    foreach (DiagnosticReport diagnosticReportExist in drList)
                    {
                        idList.Add(diagnosticReportExist.Id);
                    }
                }

                foreach (DiagnosticReport diagnosticReport in drListCurrent)
                {
                    if (diagnosticReport.Id != "test")
                    {
                        // If there are priorly downloaded reports
                        if (listCount != 0)
                        {
                            // If the downloaded diagnosticreports does not match priorly downloaded reports add them to UI
                            if (!idList.Contains(diagnosticReport.Id))
                            {
                                drList.Add(diagnosticReport);
                                ImagingStudy imagingStudy = client.Read<ImagingStudy>(LayCollection.UriFromReference(diagnosticReport.ImagingStudy));
                                Practitioner interpreter = client.Read<Practitioner>(LayCollection.UriFromReference(imagingStudy.Interpreter));
                                drIDList = LayCollection.NewRontgen(diagnosticReport, imagingStudy, interpreter, xRayGridView, drIDList);
                            }
                        }
                        else // Add all to UI
                        {
                            drList.Add(diagnosticReport);
                            ImagingStudy imagingStudy = client.Read<ImagingStudy>(LayCollection.UriFromReference(diagnosticReport.ImagingStudy));
                            Practitioner interpreter = client.Read<Practitioner>(LayCollection.UriFromReference(imagingStudy.Interpreter));
                            drIDList = LayCollection.NewRontgen(diagnosticReport, imagingStudy, interpreter, xRayGridView, drIDList);
                        }
                    }
                }
            }
            catch (FhirOperationException e)
            {
                MessageBox.Show(e.Message, "FhirOperationException",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}

