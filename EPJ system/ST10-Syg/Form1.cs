using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Model;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ST10_Syg
{
    public partial class Form1 : Form
    { 

        // Change the variable under client to change between servers
        // without having to change the rest of the code.
        private static Uri firely = new Uri("https://server.fire.ly");
        private static Uri local = new Uri("http://localhost:4080/");
        private static readonly FhirClient client = new FhirClient(local);

        //What server created resources should be uploaded on:
        //private static string uri = "https://server.fire.ly/";
        private static string uri = "http://localhost:4080/";

        // Firely:
        //private static Practitioner currentPrac = client.Read<Practitioner>(uri + "Practitioner/"
        //+ "15144414-8878-4d2c-8409-b20d5e94beb4");
        //private static Location currentLoc = client.Read<Location>(uri + "Location/"
        //        + "edfc0578-3151-43ae-8f0a-8776704068b3");

        // Local instances already on the server:
        private static Practitioner currentPrac = client.Read<Practitioner>(uri + "Practitioner/"
        + "01f6ea26-30ed-4214-aabb-dbaea662e14f");
        private static Location currentLoc = client.Read<Location>(uri + "Location/"
                + "bf9848a5-43f2-4c67-a17f-57260044b2c4");

        private static Patient currentPatient = SearchMethods.FindPatient(client); // Find the patient on the server

        public List<Composition> compositionList = new List<Composition>();
        public List<string> idList;
        public List<DiagnosticReport> drList = new List<DiagnosticReport>();
        public Composition currentComposition = new Composition();
        public Condition currentCondition = new Condition();
        private int newGroupBoxInstanceCounter = 0; // used to keep tap on kontinuation number
        //public static string response = "";

        public Form1()
        {
            InitializeComponent();
            SetupLayout.UpdateUIWithPatientInfo(currentPatient, label1, label2, label3);
            UpdateDR();

            // Setup composition from urologist:
            Tuple<Condition, Composition> result = SetupLayout.DisplayCompositionUro(currentPrac, currentLoc, 
                currentPatient, newGroupBoxInstanceCounter, uri, panel1);
            // Save created instances to current:
            currentCondition = result.Item1;
            currentComposition = result.Item2;
        }

        // Method for ending consultation
        private void endButton_Click(object sender, EventArgs e)
        {
            // Upload current composition and condition to server and return response
            string response = UploadMethods.UploadUroComposition(local, uri, currentCondition, currentComposition);
            
            // If succesfull display this, else display error
            if (response == "Created")
            {
                MessageBox.Show("Succes", "", MessageBoxButtons.OK);
                this.Close();
            }
            else
            {
                MessageBox.Show(response, "",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Method for updating UI with newest information
        private void collectButton_Click(object sender, EventArgs e)
        {
            UpdatePASUIwithComposition();
        }
           



        // Update UI with compositions from server
        public void UpdatePASUIwithComposition()
        {
            int newCompositions = 0;
            try
            {
                // Get compositions on server
                List<Composition> compositionListCurrent = SearchMethods.GetCompositions(client, new string[]
                    { "Composition?subject=Patient/" + currentPatient.Id}, 10);
                int listCount = compositionList.Count();

                // If there is compositions add their id to the list
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
                        // If the composition is not created by the current location
                        if (composition.Section[0].Entry[0].Reference != uri + "Location/" + currentLoc.Id)
                        {
                            // If there is compositions downloaded
                            if (listCount != 0)
                            {
                                // If the composition has not been downloaded before
                                if (!idList.Contains(composition.Id))
                                {
                                    compositionList.Add(composition); //Add to download list
                                    Observation observation = client.Read<Observation>(composition.Event[0].Detail[0].Reference);
                                    Location location = client.Read<Location>(composition.Section[0].Entry[0].Reference);
                                    SetupLayout.DisplayCompositionKiro(composition, observation, location, newGroupBoxInstanceCounter,
                                        client, uri, panel1); //Display on UI
                                    newGroupBoxInstanceCounter++;
                                    newCompositions++;
                                }
                            }
                            else // If there hasn't been downloaded any compositions display without checking
                            {
                                compositionList.Add(composition); //Add to download list
                                Observation observation = client.Read<Observation>(composition.Event[0].Detail[0].Reference);
                                Location location = client.Read<Location>(composition.Section[0].Entry[0].Reference);
                                SetupLayout.DisplayCompositionKiro(composition, observation, location, newGroupBoxInstanceCounter,
                                        client, uri, panel1); //Display on UI
                                newGroupBoxInstanceCounter++;
                                newCompositions++;
                            }
                        }
                    }
                }
                //Display message to user if there aren't any new compositions
                if (newCompositions == 0)
                {
                    MessageBox.Show("Ingen nye journalnotater.", "", MessageBoxButtons.OK);
                }
            }
            catch (FhirOperationException e)
            {
                MessageBox.Show(e.Message, "FhirOperationException",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Open PACS system by opening the relevant tab
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage2)
            {
                DICOMtest.Form1.UpdateInfo(SetupLayout.NameToString(currentPatient.Name),
                        SetupLayout.IdToStringCPR(currentPatient.Identifier));
                var formPACS = new DICOMtest.Form1();
                formPACS.Dock = DockStyle.Fill;
                formPACS.TopLevel = false;
                formPACS.TopMost = true;
                this.panel2.Controls.Add(formPACS);
                formPACS.Show();
                
            }
            
        }

        public void UpdateDR()
        {
            try
            {
                //Find current diagnostic reports for the patient
                List<DiagnosticReport> drListCurrent = SearchMethods.GetDiagnosticReports(client, new string[]
                    { "DiagnosticReport?subject=Patient/" + currentPatient.Id}, 10);
                int listCount = drList.Count();
                
                //If there are new DR add to list
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
                        // If there are downloaded DR before, check if there are any new
                        if (listCount != 0)
                        {
                            if (!idList.Contains(diagnosticReport.Id))
                            {
                                drList.Add(diagnosticReport);
                                SetupLayout.DisplayDR(client, diagnosticReport, newGroupBoxInstanceCounter, uri, panel1);
                            }
                        }
                        else //else add directly to UI and downloaded list
                        {
                            drList.Add(diagnosticReport);
                            SetupLayout.DisplayDR(client, diagnosticReport, newGroupBoxInstanceCounter, uri, panel1);
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
