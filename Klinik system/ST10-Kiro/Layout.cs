using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ST10_Kiro
{
    class LayCollection
    {
        // Add current patient information to the UI
        public static void AddPatientInfoToUI(TextBox caveBox, Label patientLabel, Patient currentPatient)
        {
            caveBox.Text = ""; // Ensure the text in CAVE is empty at start up
            patientLabel.Text = LayCollection.IdToStringCPR(currentPatient.Identifier) + " - " +
                LayCollection.NameToString(currentPatient.Name); //Add patient name and CPR to UI
        }

        // Method to add diagnostic reports to UI
        public static List<string> NewRontgen(DiagnosticReport diagnosticReport, ImagingStudy imagingStudy, 
            Practitioner interpreter, DataGridView xRayGridView, List<string> drIDList)
        {
            string[] row = {diagnosticReport.Effective.ToString(), LayCollection.NameToString(interpreter.Name),
                imagingStudy.ReasonCode[0].Coding[0].Code, imagingStudy.Modality[0].Code + ", " + diagnosticReport.Code.Coding[0].Code,
                diagnosticReport.Conclusion + " (" +
                diagnosticReport.ConclusionCode[0].Coding[0].Code + ")"};
            xRayGridView.Rows.Add(row);
            drIDList.Add(imagingStudy.Identifier[0].Value.ToString());

            return drIDList;
        }

        // Method to add a new composition to UI from server
        public static int NewAnamneseServer(Composition composition, Practitioner relatedPractitioner, Condition relatedCondition,
            Location relatedLocation, List<DiagnosticReport> drList, int columnNumber,
            TextBox caveBox, DataGridView anamneseGridView)
        {
            caveBox.Text += "\r\n" + caveBox.Text + DiagnosisCodeConverter(relatedCondition.Code.Coding[0].Code);
            // Create new column for composition and add data from server
            DataGridViewColumn newHeader = new DataGridViewTextBoxColumn();
            newHeader.HeaderText = composition.DateElement.ToString() + Environment.NewLine
                + LayCollection.NameToString(relatedPractitioner.Name) + Environment.NewLine
                + relatedLocation.Name;
            anamneseGridView.Columns.Add(newHeader);
            anamneseGridView.Rows[20].Cells[columnNumber].Value = relatedCondition.Code.Coding[0].Code; //condition.Code.Coding[0].Code - Diagnosekode fra Condition/Prostatakræft
            anamneseGridView.Rows[38].Cells[columnNumber].Value = Regex.Replace(composition.Section[0].Text.Div, @"<(.|\n)*?>", string.Empty); // Konklusion - Composition/UroOpfølgning
            if (drList.Count > 0)
            {
                anamneseGridView.Rows[32].Cells[columnNumber].Value = "Forefindes, se Røntgen fane"; // Tidl. billeddiagnostik
            }

            return columnNumber++;
        }

        //Method to find description to ICD code
        static string DiagnosisCodeConverter(string code)
        {
            string desription = "";
            if (code.Equals("/DC619/"))
            {
                desription = "Prostatakræft";
            }
            else if (code.Equals("/DC619M/"))
            {
                desription = "Prostatakræft med metastaser";
            }
            else if (code.Equals("/DC619X/"))
            {
                desription = "Lokalrecidiv fra prostatakræft";
            }
            else if (code.Equals("/DC619Z/"))
            {
                desription = "Kastrationsresistent prostatakræft (CRPC)";
            }

            return desription;
        }

        // Get the uri from a reference in resource
        public static string UriFromReference(List<ResourceReference> list)
        {
            string s = "";
            foreach (ResourceReference resourceReference in list)
            {
                s = resourceReference.Reference;
            }

            return s;
        }

        // Transform name from list to string
        public static string NameToString(List<HumanName> list)
        {
            string s = "";
            foreach (HumanName humanName in list)
            {
                s = humanName.Family.ToString() + ", " + humanName.GivenElement[0].ToString();
            }
            return s;
        }

        // Transform CPR from list to string
        public static string IdToStringCPR(List<Identifier> list)
        {
            string s = "";
            foreach (Identifier id in list)
            {
                s = id.Value;
            }
            return s;
        }

        // Setup table for diagnostic reports
        public static void SetupXrayGrid(DataGridView dataGridView)
        {
            DataGridViewColumn column0 = new DataGridViewTextBoxColumn();
            column0.HeaderText = "Dato";
            column0.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            column0.Width = 100;
            dataGridView.Columns.Add(column0);

            DataGridViewColumn column1 = new DataGridViewTextBoxColumn();
            column1.HeaderText = "Initial";
            column1.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            column1.Width = 150;
            dataGridView.Columns.Add(column1);

            DataGridViewColumn column2 = new DataGridViewTextBoxColumn();
            column2.HeaderText = "Beskrivelse";
            dataGridView.Columns.Add(column2);

            DataGridViewColumn column3 = new DataGridViewTextBoxColumn();
            column3.HeaderText = "RU Dx";
            dataGridView.Columns.Add(column3);

            DataGridViewColumn column4 = new DataGridViewTextBoxColumn();
            column4.HeaderText = "Indikation";
            dataGridView.Columns.Add(column4);
        }

        // Setup table for compositions
        public static void SetupAnamnese(DataGridView dataGridView)
        {
            DataGridViewColumn column = new DataGridViewTextBoxColumn();
            column.HeaderText = "";
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            column.Width = 200;
            
            dataGridView.Columns.Add(column);
            dataGridView.Rows.Add("Aktuel problemstilling");
            dataGridView.Rows.Add("Klage");
            dataGridView.Rows.Add("Symptom beskrivelse");
            dataGridView.Rows.Add("Fødselsforløb");
            dataGridView.Rows.Add("Øvrige symp.");
            dataGridView.Rows.Add("Traume");
            dataGridView.Rows.Add("");
            dataGridView.Rows.Add("Debut");
            dataGridView.Rows.Add("Daglig funktion");
            dataGridView.Rows.Add("Årsag til problem");
            dataGridView.Rows.Add("Forløb");
            dataGridView.Rows.Add("Forbedrende");
            dataGridView.Rows.Add("Forværrende");
            dataGridView.Rows.Add("Aktuel medicin");
            dataGridView.Rows.Add("Øvrig medicin");
            dataGridView.Rows.Add("");
            dataGridView.Rows.Add("Døgnrytme");
            dataGridView.Rows.Add("Tidligere beh.");
            dataGridView.Rows.Add("Tidligere US");
            dataGridView.Rows.Add("Almen tilstand");
            dataGridView.Rows.Add("Tidligere sygdomme");
            dataGridView.Rows.Add("Motion");
            dataGridView.Rows.Add("Operation/indl.");
            dataGridView.Rows.Add("Familiære dispositioner");
            dataGridView.Rows.Add("Allergi/asthma");
            dataGridView.Rows.Add("Medicin");
            dataGridView.Rows.Add("Tidl. indlæggelser");
            dataGridView.Rows.Add("");
            dataGridView.Rows.Add("Tidl. operationer");
            dataGridView.Rows.Add("Sekundær problem");
            dataGridView.Rows.Add("Tidl. traumer");
            dataGridView.Rows.Add("Andet");
            dataGridView.Rows.Add("Tidl. billeddiagnostik");
            dataGridView.Rows.Add("Beskæftigelse");
            dataGridView.Rows.Add("Børn/familie");
            dataGridView.Rows.Add("Partnerstatus");
            dataGridView.Rows.Add("Arbejdsstatus");
            dataGridView.Rows.Add("Forventninger");
            dataGridView.Rows.Add("Resume");
        }

        // Method to create a new composition for chiropractor
        public static int AnamneseKiroContact(FhirDateTime date, string aktuelProblemstilling, string klage,
    string symptomBeskrivelse, string fodselsforlob, string otherSymptoms,
    string traume, string debut, string dagligFunktion,
    string reasonForProblem, string forlob, string forbedrende, string forværrende,
    string aktuelMedicin, string øvrigMedicin, string døgnrytme, string tidligereBehandling,
    string tidligereUS, string almenTilstand, string tidligereSygdomme,
    string motion, string operationIndlæggelser, string familiæreDispositioner,
    string allergiAsthma, string medicin, string tidlIndlæggelser, string tidlOperationer,
    string sekundærProblem, string tidlTraumer, string andet, string tidlBilleddiagnostik,
    string beskæftigelse, string børnFamilie, string partnerStatus, string arbejdsStatus,
    string forventninger, string resume, DataGridView anamneseGridView, int columnNumber)
        {
            DataGridViewColumn header = new DataGridViewTextBoxColumn();
            header.HeaderText = date.ToString();
            anamneseGridView.Columns.Add(header); // Composition.Date 
            anamneseGridView.Rows[0].Cells[columnNumber].Value = aktuelProblemstilling; // Observation.Value
            anamneseGridView.Rows[1].Cells[columnNumber].Value = klage; // Hardcoded
            anamneseGridView.Rows[2].Cells[columnNumber].Value = symptomBeskrivelse; // Observation.code
            anamneseGridView.Rows[3].Cells[columnNumber].Value = fodselsforlob; // Hardcoded
            anamneseGridView.Rows[4].Cells[columnNumber].Value = otherSymptoms; // Hardcoded
            anamneseGridView.Rows[5].Cells[columnNumber].Value = traume; // Hardcoded
            anamneseGridView.Rows[6].Cells[columnNumber].Value = "";
            anamneseGridView.Rows[7].Cells[columnNumber].Value = debut; // Observation.Effective
            anamneseGridView.Rows[8].Cells[columnNumber].Value = dagligFunktion; // Hardcoded
            anamneseGridView.Rows[9].Cells[columnNumber].Value = reasonForProblem; // Observation.Slice.Note:Cause
            anamneseGridView.Rows[10].Cells[columnNumber].Value = forlob; // Observation.Slice.Note:Progression
            anamneseGridView.Rows[11].Cells[columnNumber].Value = forbedrende; // Observation.Slice.Note:Alliveating
            anamneseGridView.Rows[12].Cells[columnNumber].Value = forværrende; // Observation.Slice.Note:Worsening
            anamneseGridView.Rows[13].Cells[columnNumber].Value = aktuelMedicin; // Hardcoded
            anamneseGridView.Rows[14].Cells[columnNumber].Value = øvrigMedicin; // Hardcoded
            anamneseGridView.Rows[15].Cells[columnNumber].Value = "";
            anamneseGridView.Rows[16].Cells[columnNumber].Value = døgnrytme; // Hardcoded
            anamneseGridView.Rows[17].Cells[columnNumber].Value = tidligereBehandling; // Hardcoded
            anamneseGridView.Rows[18].Cells[columnNumber].Value = tidligereUS; // Hardcoded
            anamneseGridView.Rows[19].Cells[columnNumber].Value = almenTilstand; // Hardcoded
            anamneseGridView.Rows[20].Cells[columnNumber].Value = tidligereSygdomme; // Hardcoded
            anamneseGridView.Rows[21].Cells[columnNumber].Value = motion; // Hardcoded
            anamneseGridView.Rows[22].Cells[columnNumber].Value = operationIndlæggelser; // Hardcoded
            anamneseGridView.Rows[23].Cells[columnNumber].Value = familiæreDispositioner; // Hardcoded
            anamneseGridView.Rows[24].Cells[columnNumber].Value = allergiAsthma; // Hardcoded
            anamneseGridView.Rows[25].Cells[columnNumber].Value = medicin; // Hardcoded
            anamneseGridView.Rows[26].Cells[columnNumber].Value = tidlIndlæggelser; // Hardcoded
            anamneseGridView.Rows[27].Cells[columnNumber].Value = "";
            anamneseGridView.Rows[28].Cells[columnNumber].Value = tidlOperationer; // Hardcoded
            anamneseGridView.Rows[29].Cells[columnNumber].Value = sekundærProblem; // Hardcoded
            anamneseGridView.Rows[30].Cells[columnNumber].Value = tidlTraumer; // Hardcoded
            anamneseGridView.Rows[31].Cells[columnNumber].Value = andet; // Hardcoded
            anamneseGridView.Rows[32].Cells[columnNumber].Value = tidlBilleddiagnostik; // Hardcoded
            anamneseGridView.Rows[33].Cells[columnNumber].Value = beskæftigelse; // Hardcoded
            anamneseGridView.Rows[34].Cells[columnNumber].Value = børnFamilie; // Hardcoded
            anamneseGridView.Rows[35].Cells[columnNumber].Value = partnerStatus; // Hardcoded
            anamneseGridView.Rows[36].Cells[columnNumber].Value = arbejdsStatus; // Hardcoded
            anamneseGridView.Rows[37].Cells[columnNumber].Value = forventninger; // Hardcoded
            anamneseGridView.Rows[38].Cells[columnNumber].Value = resume; // Composition.Konklusion

            return columnNumber++;
        }

    }
}
