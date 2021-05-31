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
        public static string patientName_transfer = ""; 
        public static string patientCPR_transfer = "";
        public static string patientName = "undefined";

        static string pathToDicomDownloadFile = Path.Combine(@"C:\Users\lisaa\Documents\ST10\Firely\DICOMtest2\DICOMtest2\Test Files\KirPACS\Downloads");
        static string pathToDicomLocalFile = Path.Combine(@"C:\Users\lisaa\Documents\ST10\Firely\DICOMtest2\DICOMtest2\Test Files\KirPACS\Local");
        //static string pathToDicomDownloadFile = Path.Combine(@"C:\Users\sille\Desktop\dcm_files\KirPACS\Downloads");
        //static string pathToDicomLocalFile = Path.Combine(@"C:\Users\sille\Desktop\dcm_files\KirPACS\Local");


        public static string updatedSelectedStudyUID = "";
        public static List<string> studyUIDTransferList = new List<string>();
        
        public Form1()
        {

            InitializeComponent();

            /*Anvendes ved test*/
            patientName_transfer = "undefined"; //File with actual medical image
            //string patientName = "Anony Mous"; //Test file with blue circle
            patientCPR_transfer = "012345-6789";
            studyUIDTransferList.Clear();
            //string studyUIDTest1 = "1.2.826.0.1.3680043.8.1055.1.20111103112244831.40200514.30965937"; //Anonymized
            //string studyUIDTest2 = "1.2.826.0.1.3680043.8.498.13230779778012324449356534479549187420"; //Anony Mous
            string studyUIDTest3 = "1.3.6.1.4.1.9590.100.1.2.3230817638493334023401054782792588833"; //undefined
            //studyUIDTransferList.Add(studyUIDTest1);
            //studyUIDTransferList.Add(studyUIDTest2);
            studyUIDTransferList.Add(studyUIDTest3);

            LayoutClass.LogToDebugConsole("TEST3: " + studyUIDTransferList[0]);
            //LayoutClass.LogToDebugConsole("TEST2: " + studyUIDTransferList[1]);

            updatedSelectedStudyUID = studyUIDTest3;
            /*Slut af test variable*/

            LayoutClass.LogToDebugConsole("OVERFØRT: " + updatedSelectedStudyUID + ", " + patientName_transfer + ", " + patientCPR_transfer);

            try
            {
                //Get studies based on list from information system
                DICOMMethods.CreateGetSaveDicomClient(studyUIDTransferList, patientName, pathToDicomLocalFile, pathToDicomDownloadFile);
            }

            catch (Exception e)
            {
                //In real life, do something about this exception
                LayoutClass.LogToDebugConsole($"Error occured during DICOM verification request -> {e.StackTrace}");
            }
            try
            {
                DICOMMethods.Wait(1000);
                AddToMyListBox();
                OpenFileForStudyUIDTransfer();
            }
            catch (Exception e)
            {
                LayoutClass.LogToDebugConsole($"Error occured during DICOM file read operation -> {e.StackTrace}", label1);
                LayoutClass.LogToDebugConsole($"Error occured during DICOM file read operation -> {e.StackTrace}");
            }
        }

        /*UI*/
        public void AddToMyListBox()
        {
            string[] fileArrayDownload = new string[] { };
            string[] fileArrayLocal = new string[] { };
            var fileListDownload = new List<string>();
            var fileListLocal = new List<string>();
            // If downloaded files for patient exits, add to array
            if (Directory.Exists($@"{pathToDicomDownloadFile}\{patientName}"))
            {
                fileArrayDownload = Directory.GetFiles($@"{pathToDicomDownloadFile}\{patientName}");

            }
            // If local files for patient exits, add to array
            if (Directory.Exists($@"{pathToDicomLocalFile}\{patientName}"))
            {
                fileArrayLocal = Directory.GetFiles($@"{pathToDicomLocalFile}\{patientName}");
            }

            // Extract studyUID from each file
            var arrayLocal = new List<string>();
            for (int i = 0; i < fileArrayLocal.Length; i++)
            {
                fileListLocal.Add(fileArrayLocal[i]);
                string[] splitfileArrayLocal = fileArrayLocal[i].Split(@"\");
                string splitfileArrayLocalStudyUId = splitfileArrayLocal[8];
                arrayLocal.Add(splitfileArrayLocalStudyUId);
            }
            var arrayDownload = new List<string>();
            for (int x = 0; x < fileArrayDownload.Length; x++)
            {
                fileListDownload.Add(fileArrayDownload[x]);
                string[] splitfileArrayDownload = fileListDownload[x].Split(@"\");
                string splitfileArrayDownloadStudyUId = splitfileArrayDownload[8];
                arrayDownload.Add(splitfileArrayDownloadStudyUId);
            }
            // Stop the ListBox from drawing while items are added.
            listBox1.BeginUpdate();
            listBox1.ResetText();
            LayoutClass.LogToDebugConsole($"Længde af arrays: {fileListDownload.Count} og {fileListLocal.Count}");

            //Add each local file to listBox
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

            // Ensure there are no dublicates in download in relation to local
            for (int x = 0; x < fileArrayDownload.Length; x++)
            {
                for (int i = 0; i < fileArrayLocal.Length; i++)
                {
                    if (arrayLocal[i] == arrayDownload[x])
                    {
                        LayoutClass.LogToDebugConsole($"Local: {arrayLocal[i]} - Download: {arrayDownload[x]}");
                        arrayDownload.Remove(arrayDownload[x]);
                        fileListDownload.Remove(fileListDownload[x]);
                    }
                }
            }

            // Add each downloaded file to listBox
            for (int x = 0; x < fileListDownload.Count; x++)
            {
                LayoutClass.LogToDebugConsole($"x: {x}");
                LayoutClass.LogToDebugConsole($"Downloaded: {fileListDownload[x]}");
                var file1 = DicomFile.Open(fileListDownload[x], readOption: FileReadOption.ReadAll);
                var dicomDataset1 = file1.Dataset;

                string modality1 = dicomDataset1.GetSingleValueOrDefault<string>(DicomTag.Modality, "undefined");
                var studyDate1 = dicomDataset1.GetSingleValueOrDefault<string>(DicomTag.StudyDate, "undefined");
                var studyIUid1 = dicomDataset1.GetSingleValueOrDefault<string>(DicomTag.StudyInstanceUID, "undefined");

                listBox1.Items.Add($"{modality1} - {studyDate1} - {studyIUid1} - Indhentet fil");

            }
            // End the update process and force a repaint of the ListBox.
            listBox1.EndUpdate();
        }

        // When new study is selected update UI
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LayoutClass.UpdateLabels(label1, label2, label3, label4, label5, label6, label7, label8, label9, label10,
                label11, label12, label13, label14, label15, label16, label17, label18, label19, label20, label21,
                label22, label23, label24, label25, label26, label27, label28, label29, label30, label31, label32,
                label33, label34, button1, patientName_transfer, patientCPR_transfer);

            //Get path to selected dcm file
            var UI = listBox1.SelectedItem.ToString();
            string[] split = UI.Split(" - ");
            string studyIUid_extract = split[2];
            string fileLocation = split[3];

            LayoutClass.LogToDebugConsole($"Split string: {studyIUid_extract}");
            string pathToDicomFile = "";

            if (fileLocation == "Lokal fil")
            {
                pathToDicomFile = Path.Combine($@"{pathToDicomLocalFile}\{patientName}\{studyIUid_extract}.dcm");
            }
            else
            {
                pathToDicomFile = Path.Combine($@"{pathToDicomDownloadFile}\{patientName}\{studyIUid_extract}.dcm");
            }

            LayoutClass.LogToDebugConsole($"Attempting to extract information from DICOM file:{pathToDicomFile}...");
            var file = DicomFile.Open(pathToDicomFile, readOption: FileReadOption.ReadAll);
            DICOMMethods.ExtractDataset(file, label10, label11, label12, label13, label14, label15, label16, label17,
                label18, label19, label20, label21, label22, label23, label24, label25, label26, label27, label28,
                label29, label30, label31, label32, label33, label34, pictureBox1, pathToDicomFile);
        }

        //Method used when closing system
        private void button1_Click(object sender, EventArgs e)
        {
            var UI = listBox1.SelectedItem.ToString();
            string[] split = UI.Split(" - ");
            string studyIUid_extract = split[2];
            string fileLocation = split[3];

            LayoutClass.LogToDebugConsole($"Split string: {studyIUid_extract}");

            string pathToDicomFileLocal = Path.Combine($@"{pathToDicomDownloadFile}\{patientName}\{studyIUid_extract}.dcm");
            
            //If selected file is not Indhentet fil display error to user, else normal closing message
            if (fileLocation == "Indhentet fil")
            {
                string message = "Luk PACS";
                string caption = "PACS lukkes";
                MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
                DialogResult result;

                // Displays the MessageBox.
                result = MessageBox.Show(message, caption, buttons);
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    // Closes the parent form.
                    this.Close();
                }
            }
            else
            {
                string message = "Fejl i dcm-fil";
                string caption = "PACS lukkes";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Displays the MessageBox.
                result = MessageBox.Show(message, caption, buttons);
            }
        }

        //Get list of studyUIDs from information system
        public static void UpdateStudyUIDTransfer(List<string> studyUIDTransferInputList)
        {
            for (int x= 0; x < studyUIDTransferInputList.Count ; x++)
            {
                studyUIDTransferList.Add(studyUIDTransferInputList[x]);
            }
            
        }

        //Get patient and selected study from information system
        public static void UpdateInfo(string studyUIDTransferInput, string patientNameTransferInput, string patientCPRTransferInput)
        {
            updatedSelectedStudyUID = studyUIDTransferInput;
            patientName_transfer = patientNameTransferInput;
            patientCPR_transfer = patientCPRTransferInput;
        }

        //Ensure correct study is selected
        public void OpenFileForStudyUIDTransfer()
         {
            listBox1.SelectedIndex = -1;
            if (studyUIDTransferList.Count >= 0)
            {
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    string item = listBox1.Items[i].ToString();
                    string[] split = item.Split();
                    string selectedStudyUID = split[4];

                    for (int x = 0; x < updatedSelectedStudyUID.Length; x++)
                    {
                        if (updatedSelectedStudyUID == selectedStudyUID)
                        {
                            listBox1.SetSelected(i, true);
                            break;
                        }
                    }
                                    
                }
            }
        }
    }
}
