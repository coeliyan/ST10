using Dicom;
using Dicom.Imaging;
using Dicom.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DICOMtest
{
    public partial class Form1 : Form
    {
        //string patientName_extract = "Anonymized"; //File with actual medical image
        //string patientName = "Anony Mous"; //Test file with blue circle
        //string patinetCPR_extract = "012345-6789";
        //static string pathToDicomDownloadFile = Path.Combine(@"C:\Users\sille\Desktop\dcm_files\PACS\Downloads");
        //static string pathToDicomLocalFile = Path.Combine(@"C:\Users\sille\Desktop\dcm_files\PACS\Local");
        static string pathToDicomDownloadFile = Path.Combine(@"C:\Users\lisaa\Documents\ST10\Firely\DICOMtest\DICOMtest\DICOMtest\Test Files\PACS\Downloads");
        static string pathToDicomLocalFile = Path.Combine(@"C:\Users\lisaa\Documents\ST10\Firely\DICOMtest\DICOMtest\DICOMtest\Test Files\PACS\Local");

        public static string patientName_transfer = "";
        public static string patientCPR_transfer = "";
        //public static string patientName = "Anonymized";
        public static string patientName = "undefined";

        public static string updatedSelectedStudyUID = "";

        public Form1()
        {

            InitializeComponent();
            LayoutClass.InitUI(label7, label8, patientName_transfer, patientCPR_transfer);

            try
            {
                AddToMyListBox();
            }
            catch (Exception e)
            {
                LayoutClass.LogToDebugConsole($"Error occured during DICOM file read operation -> {e.StackTrace}", label1);
            }

        }

        // Pause before continuing
        public static void Wait(int time)
        {
            Thread thread = new Thread(delegate ()
            {
                System.Threading.Thread.Sleep(time);
            });
            thread.Start();
            while (thread.IsAlive)
                Application.DoEvents();
        }

        /*UI*/
        public void AddToMyListBox()
        {
            string[] fileArrayLocal = new string[] { };
            var fileListLocal = new List<string>();
            
            //Check if directory with given patient exists
            if (Directory.Exists($@"{pathToDicomLocalFile}\{patientName}"))
            {
                fileArrayLocal = Directory.GetFiles($@"{pathToDicomLocalFile}\{patientName}");
            }
            else
            {
                LayoutClass.LogToDebugConsole("Error in filepath when adding to listbox");
            }

            //Get studyUID from all local files
            var arrayLocal = new List<string>();
            for (int i = 0; i < fileArrayLocal.Length; i++)
            {
                fileListLocal.Add(fileArrayLocal[i]);
                string[] splitfileArrayLocal = fileArrayLocal[i].Split(@"\");
                string splitfileArrayLocalStudyUId = splitfileArrayLocal[8];
                arrayLocal.Add(splitfileArrayLocalStudyUId);
            }
            
            // Stop the ListBox from drawing while items are added.
            listBox1.BeginUpdate();
            listBox1.ResetText();
            
            // For each local file
            for (int i = 0; i < fileListLocal.Count; i++)
            {
                var file = DicomFile.Open(fileListLocal[i], readOption: FileReadOption.ReadAll);
                var dicomDataset = file.Dataset;

                string modality = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.Modality, "undefined");
                var studyDate = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.StudyDate, "undefined");
                var studyIUid = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.StudyInstanceUID, "undefined");

                var local = fileListLocal[i];
                listBox1.Items.Add($"{modality} - {studyDate} - {studyIUid} - Lokal fil");
                LayoutClass.LogToDebugConsole($"Local: {arrayLocal[i]}");
            }

            // End the update process and force a repaint of the ListBox.
            listBox1.EndUpdate();
        }

            // When selecting a new study from the listbox
            private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label1.Text = "Patient info: \n";
            label2.Text = "Study info: \n";
            label3.Text = "Series info: \n";
            label4.Text = "Image info: \n";
            label5.Text = "Extra info: \n";
            label6.Text = "Vælg studie:";
            label7.Text = $"Patient navn: {patientName_transfer}";
            label8.Text = $"CPR: {patientCPR_transfer}";
            label10.Text = "";
            label11.Text = "";
            label12.Text = "";
            label13.Text = "";
            label14.Text = "";
            label15.Text = "";
            label16.Text = "";
            label17.Text = "";
            label18.Text = "";
            label19.Text = "";
            label20.Text = "";
            label21.Text = "";
            label22.Text = "";
            label23.Text = "";
            label24.Text = "";
            label25.Text = "";
            label26.Text = "";
            label27.Text = "";
            label28.Text = "";
            label29.Text = "";
            label30.Text = "";
            label31.Text = "";
            label32.Text = "";
            label33.Text = "";
            label34.Text = "";
            
            // Get UI from selected item
            var UI = listBox1.SelectedItem.ToString();
            string[] split = UI.Split(" - ");
            string studyIUid_extract = split[2];
            string fileLocation = split[3];

            LayoutClass.LogToDebugConsole($"Split string: {studyIUid_extract}");
            string pathToDicomFile = "";

            //Save the path to dcm file
            if (fileLocation=="Lokal fil")
            {
                pathToDicomFile = Path.Combine($@"{pathToDicomLocalFile}\{patientName}\{studyIUid_extract}.dcm");
            }
            else
            {
                LayoutClass.LogToDebugConsole("Error in file path");
            }

            LayoutClass.LogToDebugConsole($"Attempting to extract information from DICOM file:{pathToDicomFile}...");
            var file = DicomFile.Open(pathToDicomFile, readOption: FileReadOption.ReadAll);

            DICOMMethods.ExtractDataset(file, label10, label11, label12, label13, label14, label15, label16,
                label17, label18, label19, label20, label21, label22, label23, label24, label25, label26,
                label27, label28, label29, label30, label31, label32, label33, label34, pathToDicomFile, pictureBox1);
        }

        //Method to get patient info from FHIR
        public static void UpdateInfo(string patientNameTransferInput, string patientCPRTransferInput)
        {
            patientName_transfer = patientNameTransferInput;
            patientCPR_transfer = patientCPRTransferInput;
        }

        //Method for closing system
        private void button1_Click(object sender, EventArgs e)
        {
            var UI = listBox1.SelectedItem.ToString();
            string[] split = UI.Split(" - ");
            string studyIUid_extract = split[2];
            string fileLocation = split[3];

            LayoutClass.LogToDebugConsole($"Split string: {studyIUid_extract}");

            string pathToDicomFileLocal = Path.Combine($@"{pathToDicomLocalFile}\{patientName}\{studyIUid_extract}.dcm");
           
            //If the file is local upload to server
            if (fileLocation == "Lokal fil")
            {
                //Display message to user
                string message = "Luk PACS og send dcm-fil";
                string caption = "PACS lukkes";
                MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
                DialogResult result;

                // Displays the MessageBox.
                result = MessageBox.Show(message, caption, buttons);
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    //Send file to server
                    DICOMMethods.CreateDicomStoreClient(pathToDicomFileLocal);
                    Wait(10000);
                    this.Close();
                }
            }
            else
            {
                string message = "Luk PACS og send dcm-fil";
                string caption = "Fejl i dcm-fil";
                MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
                DialogResult result;

                // Displays the error message to user
                result = MessageBox.Show(message, caption, buttons);
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    // Closes the parent form.
                    this.Close();
                }
            }
        }
    }
}
