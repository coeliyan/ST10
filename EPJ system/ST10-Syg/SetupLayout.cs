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
    public class SetupLayout
    {
        // Update UI with patient information
        public static void UpdateUIWithPatientInfo(Patient currentPatient, Label label1, Label label2, Label label3)
        {
            label1.Text = SetupLayout.IdToStringCPR(currentPatient.Identifier) + "\r\n" + SetupLayout.NameToString(currentPatient.Name)
                + "\r\n" + "Mand";
            label2.Text = "123456-1234" + "\r\n" + "Rasmussen, Ruth" + "\r\n" + "Kvinde";
            label3.Text = "345678-9190" + "\r\n" + "Jensen, Birthe" + "\r\n" + "Kvinde";
        }

        // Create composition UI box and add to UI, specifically for data from server
        public static GroupBox CreateBoxCollect(System.Windows.Forms.DockStyle dock, int nameCounter, string dato, string kontinuationsNr, string tilhørendeSpeciale,
            string aktueltDiverse, string konklusionResume, string behandlerNavn, string lokationsNavn)
        {
            TextBox behandler = new TextBox();
            behandler.BackColor = System.Drawing.SystemColors.Window;
            behandler.BorderStyle = System.Windows.Forms.BorderStyle.None;
            behandler.Dock = System.Windows.Forms.DockStyle.Fill;
            behandler.Location = new System.Drawing.Point(3, 3);
            behandler.Name = "behandler" + nameCounter.ToString();
            behandler.ReadOnly = true;
            behandler.Size = new System.Drawing.Size(327, 22);
            behandler.TabIndex = 3;
            behandler.Text = behandlerNavn;
            
            TextBox lokation = new TextBox();
            lokation.BackColor = System.Drawing.SystemColors.Window;
            lokation.BorderStyle = System.Windows.Forms.BorderStyle.None;
            lokation.Dock = System.Windows.Forms.DockStyle.Fill;
            lokation.Location = new System.Drawing.Point(336, 3);
            lokation.Name = "lokation" + nameCounter.ToString();
            lokation.ReadOnly = true;
            lokation.Size = new System.Drawing.Size(327, 22);
            lokation.TabIndex = 4;
            lokation.Text = lokationsNavn;

            TableLayoutPanel tableLayoutPanel3 = new TableLayoutPanel();
            tableLayoutPanel3.ColumnCount = 2;
            tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel3.Controls.Add(lokation, 0, 0);
            tableLayoutPanel3.Controls.Add(behandler, 0, 0);
            tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel3.Location = new System.Drawing.Point(3, 353);
            tableLayoutPanel3.Name = "tableLayoutPanel-" + nameCounter.ToString();
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel3.Size = new System.Drawing.Size(666, 24);
            tableLayoutPanel3.TabIndex = 3;

            TextBox speciale = new TextBox();
            speciale.BackColor = System.Drawing.SystemColors.Window;
            speciale.BorderStyle = System.Windows.Forms.BorderStyle.None;
            speciale.Dock = System.Windows.Forms.DockStyle.Fill;
            speciale.Location = new System.Drawing.Point(447, 30);
            speciale.Name = "speciale" + nameCounter.ToString();
            speciale.ReadOnly = true;
            speciale.Size = new System.Drawing.Size(216, 22);
            speciale.TabIndex = 0;
            speciale.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            speciale.Font = new Font(speciale.Font, FontStyle.Bold);
            speciale.Text = tilhørendeSpeciale;

            TextBox kontNr = new TextBox();
            kontNr.BackColor = System.Drawing.SystemColors.Window;
            kontNr.BorderStyle = System.Windows.Forms.BorderStyle.None;
            kontNr.Dock = System.Windows.Forms.DockStyle.Fill;
            kontNr.Location = new System.Drawing.Point(447, 3);
            kontNr.Name = "kontNr" + nameCounter.ToString();
            kontNr.ReadOnly = true;
            kontNr.Size = new System.Drawing.Size(216, 22);
            kontNr.TabIndex = 3;
            kontNr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            kontNr.Font = new Font(kontNr.Font, FontStyle.Bold);
            kontNr.Text = kontinuationsNr;

            TextBox date = new TextBox();
            date.BackColor = System.Drawing.SystemColors.Window;
            date.BorderStyle = System.Windows.Forms.BorderStyle.None;
            date.Dock = System.Windows.Forms.DockStyle.Fill;
            date.Location = new System.Drawing.Point(3, 30);
            date.Name = "date" + nameCounter.ToString();
            date.ReadOnly = true;
            date.Size = new System.Drawing.Size(216, 22);
            date.TabIndex = 4;
            date.Font = new Font(date.Font, FontStyle.Bold);
            date.Text = dato;

            TableLayoutPanel tableLayoutPanel2 = new TableLayoutPanel();
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            tableLayoutPanel2.Controls.Add(date, 0, 1);
            tableLayoutPanel2.Controls.Add(kontNr, 2, 0);
            tableLayoutPanel2.Controls.Add(speciale, 2, 1);
            tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2-" + nameCounter.ToString();
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel2.Size = new System.Drawing.Size(666, 54);
            tableLayoutPanel2.TabIndex = 0;

            TextBox aktuelt = new TextBox();
            aktuelt.BackColor = System.Drawing.SystemColors.Window;
            aktuelt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            aktuelt.Dock = System.Windows.Forms.DockStyle.Fill;
            aktuelt.Location = new System.Drawing.Point(3, 63);
            aktuelt.Multiline = true;
            aktuelt.Name = "aktuelt" + nameCounter.ToString();
            aktuelt.ReadOnly = true;
            aktuelt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            aktuelt.Size = new System.Drawing.Size(666, 197);
            aktuelt.TabIndex = 1;
            aktuelt.Text = aktueltDiverse;

            TextBox konklusion = new TextBox();
            konklusion.BackColor = System.Drawing.SystemColors.Window;
            konklusion.BorderStyle = System.Windows.Forms.BorderStyle.None;
            konklusion.Dock = System.Windows.Forms.DockStyle.Fill;
            konklusion.Location = new System.Drawing.Point(3, 266);
            konklusion.Multiline = true;
            konklusion.Name = "konklusion" + nameCounter.ToString();
            konklusion.ReadOnly = true;
            konklusion.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            konklusion.Size = new System.Drawing.Size(666, 81);
            konklusion.TabIndex = 2;
            konklusion.Text = konklusionResume;

            TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel.Controls.Add(konklusion, 0, 2);
            tableLayoutPanel.Controls.Add(aktuelt, 0, 1);
            tableLayoutPanel.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanel.Controls.Add(tableLayoutPanel3, 0, 3);
            tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            tableLayoutPanel.Location = new System.Drawing.Point(3, 25);
            tableLayoutPanel.Name = "tableLayoutPanel" + nameCounter.ToString();
            tableLayoutPanel.RowCount = 5;
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel.Size = new System.Drawing.Size(672, 400);
            tableLayoutPanel.TabIndex = 1;

            GroupBox TestGroupBox = new GroupBox();
            TestGroupBox.AutoSize = true;
            TestGroupBox.Controls.Add(tableLayoutPanel);
            TestGroupBox.Dock = dock;
            TestGroupBox.Location = new System.Drawing.Point(0, 0);
            TestGroupBox.Name = "groupBox" + nameCounter.ToString();
            TestGroupBox.Size = new System.Drawing.Size(678, 428);
            TestGroupBox.TabIndex = 2;
            TestGroupBox.TabStop = false;

            return TestGroupBox;
        }

        // Create composition UI box and add to UI, specifically for local data
        public static GroupBox CreateBoxLocal(int nameCounter, string dato, string tid, string kontinuationsNr, string tilhørendeSpeciale,
    string aktueltDiverse, string konklusionResume, string behandlerNavn, string lokationsNavn, string icdCode)
        {
            TextBox diagnosisCode = new TextBox();
            diagnosisCode.BackColor = System.Drawing.SystemColors.Window;
            diagnosisCode.BorderStyle = System.Windows.Forms.BorderStyle.None;
            diagnosisCode.Dock = System.Windows.Forms.DockStyle.Fill;
            diagnosisCode.Location = new System.Drawing.Point(3, 3);
            diagnosisCode.Name = "diagnosisCode" + nameCounter.ToString();
            diagnosisCode.ReadOnly = true;
            diagnosisCode.Size = new System.Drawing.Size(338, 22);
            diagnosisCode.TabIndex = 4;
            diagnosisCode.Text = icdCode;

            TableLayoutPanel tableLayoutPanel9 = new TableLayoutPanel();
            tableLayoutPanel9.ColumnCount = 2;
            tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel9.Controls.Add(diagnosisCode, 0, 0);
            tableLayoutPanel9.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel9.Location = new System.Drawing.Point(3, 343);
            tableLayoutPanel9.Name = "tableLayoutPanel9-" + nameCounter.ToString();
            tableLayoutPanel9.RowCount = 1;
            tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel9.Size = new System.Drawing.Size(689, 24);
            tableLayoutPanel9.TabIndex = 4;

            TextBox behandler = new TextBox();
            behandler.BackColor = System.Drawing.SystemColors.Window;
            behandler.BorderStyle = System.Windows.Forms.BorderStyle.None;
            behandler.Dock = System.Windows.Forms.DockStyle.Fill;
            behandler.Location = new System.Drawing.Point(3, 3);
            behandler.Name = "behandler" + nameCounter.ToString();
            behandler.ReadOnly = true;
            behandler.Size = new System.Drawing.Size(338, 22);
            behandler.TabIndex = 3;
            behandler.Text = behandlerNavn;

            TextBox lokation = new TextBox();
            lokation.BackColor = System.Drawing.SystemColors.Window;
            lokation.BorderStyle = System.Windows.Forms.BorderStyle.None;
            lokation.Dock = System.Windows.Forms.DockStyle.Fill;
            lokation.Location = new System.Drawing.Point(347, 3);
            lokation.Name = "lokation" + nameCounter.ToString();
            lokation.ReadOnly = true;
            lokation.Size = new System.Drawing.Size(339, 22);
            lokation.TabIndex = 4;
            lokation.Text = lokationsNavn;

            TableLayoutPanel tableLayoutPanel8 = new TableLayoutPanel();
            tableLayoutPanel8.ColumnCount = 2;
            tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel8.Controls.Add(lokation, 0, 0);
            tableLayoutPanel8.Controls.Add(behandler, 0, 0);
            tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel8.Location = new System.Drawing.Point(3, 373);
            tableLayoutPanel8.Name = "tableLayoutPanel8-" + nameCounter.ToString();
            tableLayoutPanel8.RowCount = 1;
            tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel8.Size = new System.Drawing.Size(689, 24);
            tableLayoutPanel8.TabIndex = 3;

            TextBox speciale = new TextBox();
            speciale.BackColor = System.Drawing.SystemColors.Window;
            speciale.BorderStyle = System.Windows.Forms.BorderStyle.None;
            speciale.Dock = System.Windows.Forms.DockStyle.Fill;
            speciale.Location = new System.Drawing.Point(461, 30);
            speciale.Name = "speciale" + nameCounter.ToString();
            speciale.ReadOnly = true;
            speciale.Size = new System.Drawing.Size(225, 22);
            speciale.TabIndex = 0;
            speciale.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            speciale.Font = new Font(speciale.Font, FontStyle.Bold);
            speciale.Text = tilhørendeSpeciale;

            TextBox kontNr = new TextBox();
            kontNr.BackColor = System.Drawing.SystemColors.Window;
            kontNr.BorderStyle = System.Windows.Forms.BorderStyle.None;
            kontNr.Dock = System.Windows.Forms.DockStyle.Fill;
            kontNr.Location = new System.Drawing.Point(461, 3);
            kontNr.Name = "kontNr" + nameCounter.ToString();
            kontNr.ReadOnly = true;
            kontNr.Size = new System.Drawing.Size(225, 22);
            kontNr.TabIndex = 3;
            kontNr.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            kontNr.Font = new Font(kontNr.Font, FontStyle.Bold);
            kontNr.Text = kontinuationsNr;

            TextBox date = new TextBox();
            date.BackColor = System.Drawing.SystemColors.Window;
            date.BorderStyle = System.Windows.Forms.BorderStyle.None;
            date.Dock = System.Windows.Forms.DockStyle.Fill;
            date.Location = new System.Drawing.Point(3, 30);
            date.Name = "date" + nameCounter.ToString();
            date.ReadOnly = true;
            date.Size = new System.Drawing.Size(223, 22);
            date.TabIndex = 4;
            date.Font = new Font(date.Font, FontStyle.Bold);
            date.Text = dato;

            TextBox time = new TextBox();
            time.BackColor = System.Drawing.SystemColors.Window;
            time.BorderStyle = System.Windows.Forms.BorderStyle.None;
            time.Dock = System.Windows.Forms.DockStyle.Fill;
            time.Location = new System.Drawing.Point(232, 30);
            time.Name = "time" + nameCounter.ToString();
            time.ReadOnly = true;
            time.Size = new System.Drawing.Size(223, 22);
            time.TabIndex = 5;
            time.Font = new Font(time.Font, FontStyle.Bold);
            time.Text = tid;

            TableLayoutPanel tableLayoutPanel7 = new TableLayoutPanel();
            tableLayoutPanel7.ColumnCount = 3;
            tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel7.Controls.Add(time, 0, 1);
            tableLayoutPanel7.Controls.Add(date, 0, 1);
            tableLayoutPanel7.Controls.Add(kontNr, 2, 0);
            tableLayoutPanel7.Controls.Add(speciale, 2, 1);
            tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel7.Location = new System.Drawing.Point(3, 3);
            tableLayoutPanel7.Name = "tableLayoutPanel7-" + nameCounter.ToString();
            tableLayoutPanel7.RowCount = 2;
            tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            tableLayoutPanel7.Size = new System.Drawing.Size(689, 54);
            tableLayoutPanel7.TabIndex = 0;

            TextBox aktuelt = new TextBox();
            aktuelt.BackColor = System.Drawing.SystemColors.Window;
            aktuelt.BorderStyle = System.Windows.Forms.BorderStyle.None;
            aktuelt.Dock = System.Windows.Forms.DockStyle.Fill;
            aktuelt.Location = new System.Drawing.Point(3, 63);
            aktuelt.Multiline = true;
            aktuelt.Name = "aktuelt" + nameCounter.ToString();
            aktuelt.ReadOnly = true;
            aktuelt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            aktuelt.Size = new System.Drawing.Size(689, 190);
            aktuelt.TabIndex = 1;
            aktuelt.Text = aktueltDiverse;

            TextBox konklusion = new TextBox();
            konklusion.BackColor = System.Drawing.SystemColors.Window;
            konklusion.BorderStyle = System.Windows.Forms.BorderStyle.None;
            konklusion.Dock = System.Windows.Forms.DockStyle.Fill;
            konklusion.Location = new System.Drawing.Point(3, 259);
            konklusion.Multiline = true;
            konklusion.Name = "konklusion" + nameCounter.ToString();
            konklusion.ReadOnly = true;
            konklusion.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            konklusion.Size = new System.Drawing.Size(689, 78);
            konklusion.TabIndex = 2;
            konklusion.Text = konklusionResume;

            TableLayoutPanel tableLayoutPanel4 = new TableLayoutPanel();
            tableLayoutPanel4.ColumnCount = 1;
            tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tableLayoutPanel4.Controls.Add(konklusion, 0, 2);
            tableLayoutPanel4.Controls.Add(aktuelt, 0, 1);
            tableLayoutPanel4.Controls.Add(tableLayoutPanel7, 0, 0);
            tableLayoutPanel4.Controls.Add(tableLayoutPanel8, 0, 4);
            tableLayoutPanel4.Controls.Add(tableLayoutPanel9, 0, 3);
            tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Top;
            tableLayoutPanel4.Location = new System.Drawing.Point(3, 25);
            tableLayoutPanel4.Name = "tableLayoutPanel4-" + nameCounter.ToString();
            tableLayoutPanel4.RowCount = 5;
            tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            tableLayoutPanel4.Size = new System.Drawing.Size(695, 400);
            tableLayoutPanel4.TabIndex = 4;

            GroupBox TestGroupBox = new GroupBox();
            TestGroupBox.AutoSize = true;
            TestGroupBox.Controls.Add(tableLayoutPanel4);
            TestGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            TestGroupBox.Location = new System.Drawing.Point(0, 0);
            TestGroupBox.Name = "groupBox" + nameCounter.ToString();
            TestGroupBox.Size = new System.Drawing.Size(678, 428);
            TestGroupBox.TabIndex = 2;
            TestGroupBox.TabStop = false;

            return TestGroupBox;
        }

        // Update UI with a composition from chiropractor
        public static void DisplayCompositionKiro(Composition composition, Observation observation, Location location, int counter,
            FhirClient client, string uri, Panel panel1)
        {
            string aktuelt = observation.Value.ToString() + " (" +
                observation.Code.Coding[0].Code.ToString() + ")" + "\r\n" +
                "Debut: " + observation.Effective.ToString() + "\r\n" +
                observation.Component[0].Value + "\r\n" +
                observation.Component[1].Value + "\r\n" +
                observation.Component[2].Value + "\r\n" +
                observation.Component[3].Value;
            int kontinuationsNr = counter + 19;
            // Create a new box and input the observation
            panel1.Controls.Add(SetupLayout.CreateBoxCollect(System.Windows.Forms.DockStyle.Top, counter,
                composition.DateElement.ToString(), kontinuationsNr.ToString(),
                "Kiropraktor", aktuelt, Regex.Replace(composition.Section[0].Text.Div, @"<(.|\n)*?>", string.Empty),
                SetupLayout.NameToString(client.Read<Practitioner>(
                uri + "Practitioner/" + SetupLayout.UriFromReference(composition.Author)).Name),
                location.Name));
        }

        // Update UI with composition from urologist
        public static Tuple<Condition, Composition> DisplayCompositionUro(Practitioner currentPrac, Location currentLocation,
            Patient currentPatient, int counter, string uri, Panel panel1)
        {
            string date = "2019-03-09";
            string time = "09.40";
            string speciale = "Urologi";
            string aktuelt = "Ambulant kontrol" + "\r\n" +
                "Allergisk overfor morfin, voldsom kvalme" + "\r\n" +
                "Far døde af blodprop i hjertet som 62-årig. Ingen cancer i familien" + "\r\n" +
                "1982, kompliceret brud af crus dextrum efter trafikuheld siden 1991, " +
                "type - 2 diabetes, går til kontrol, velbehandlet" + "\r\n" +
                "Har type 2 diabetes." + "\r\n" +
                "Ingen symptomer hos patient. PSA er stabilt og ikke tegn på recidiv." + "\r\n" +
                "Drikker 2-3 genstande om ugen." + "\r\n" +
                "Ikke-ryger." + "\r\n" +
                "Anvender ikke rusmidler." + "\r\n" +
                "Tager Metformin “Actavis” 850 mg er 1 tablet 2-3 gange dagligt." + "\r\n" +
                "Bor alene i hus, god kontakt til børn, pensioneret." + "\r\n" +
                "God almen tilstand." + "\r\n" +
                "Blodtryk målt til 134/87. " + "\r\n" +
                "EKG ser normalt ud." + "\r\n" +
                "Negativ U-stix";
            string konklusion = "75-årig mand diagnosticeret med prostatakræft med spredning " +
                "til knoglerne. Foretaget hormonbehandling med kirurgisk kastration. " +
                "God respons på behandling. PSA faldet fra 200 ng/ml til 0.8 ng/ml. " +
                "Fortsat kontrol hos egen læge fremadrettet.";
            int kontinuationsNr = counter + 18;
            string behandler = SetupLayout.NameToString(currentPrac.Name);
            string lokation = currentLocation.Name;
            string diagnoseKode = "/DC619M/";
            panel1.Controls.Add(SetupLayout.CreateBoxLocal(counter, date, time, kontinuationsNr.ToString(),
                speciale, aktuelt, konklusion, behandler, lokation, diagnoseKode));
            Condition currentCondition = ConformanceInstances.CreateCondition(uri, currentPrac, currentPatient,
                diagnoseKode, new FhirDateTime(2018, 03, 24));
            Composition currentComposition = ConformanceInstances.CreateCompositionWithCondition(uri, new FhirDateTime(date),
               konklusion, currentPatient, currentPrac, currentLocation, currentCondition);

            var result = Tuple.Create<Condition, Composition>(currentCondition, currentComposition);
            return result;
        }

        public static void DisplayDR(FhirClient client, DiagnosticReport diagnosticReport, int counter, string uri, Panel panel1)
        {
            try
            {
                ImagingStudy imagingStudy = client.Read<ImagingStudy>(SetupLayout.UriFromReference(diagnosticReport.ImagingStudy));
                Location location = client.Read<Location>(imagingStudy.Location.Reference);
                int kontinuationsNr = counter + 17 - panel1.Controls.Count;
                panel1.Controls.Add(SetupLayout.CreateBoxCollect(System.Windows.Forms.DockStyle.Bottom, counter, diagnosticReport.Effective.ToString(),
                    kontinuationsNr.ToString(), "Radiologi", "Årsag til undersøgelse: " +
                    diagnosticReport.Code.Coding[0].Code, diagnosticReport.Conclusion,
                    SetupLayout.NameToString(client.Read<Practitioner>(
                    uri + "Practitioner/" + SetupLayout.UriFromReference(diagnosticReport.ResultsInterpreter)).Name),
                    location.Name));
            }
            catch (FhirOperationException e)
            {
                MessageBox.Show(e.Message, "FhirOperationException",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Convert URI from reference to string
        public static string UriFromReference(List<ResourceReference> list)
        {
            string s = "";
            foreach (ResourceReference resourceReference in list)
            {
                s = resourceReference.Reference;
            }

            return s;
        }


        // Convert CPR from list to string
        public static string IdToStringCPR(List<Identifier> list)
        {
            string s = "";
            foreach (Identifier id in list)
            {
                s = id.Value;
            }
            return s;
        }


        // Convert name from list to string
        public static string NameToString(List<HumanName> list)
        {
            string s = "";
            foreach (HumanName humanName in list)
            {
                s = humanName.Family.ToString() + ", " + humanName.GivenElement[0].ToString();
            }
            return s;
        }
    }
}
