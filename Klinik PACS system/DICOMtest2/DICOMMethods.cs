using Dicom;
using Dicom.Imaging;
using Dicom.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DICOMtest
{
    class DICOMMethods
    {
        /*CFind, CGet, and SaveFile method*/
        public static Dicom.Network.Client.DicomClient CreateGetSaveDicomClient(List<string> studyUIDTransferList, 
            string patientName, string pathToDicomLocalFile, string pathToDicomDownloadFile)
        {
            string QRServerHost = "www.dicomserver.co.uk";
            int QRServerPort = 104;
            string QRServerAET = "STORESCP";
            string AET = "FODICOMSCU";
            string testAET = "ANY-SCP";

            // Create DICOM client
            var cGetDicomClient = new Dicom.Network.Client.DicomClient(QRServerHost, QRServerPort, false, AET, QRServerAET);

            // For each study extract studyUID and get study from server
            for (int i = 0; i < studyUIDTransferList.Count; i++)
            {
                LayoutClass.LogToDebugConsole("Started CreateCGetDicomClient");

                var requestCGet = new DicomCGetRequest(studyUIDTransferList[i]);
                Debug.WriteLine("Debug:" + studyUIDTransferList[i]);

                // Get Studies
                cGetDicomClient.OnCStoreRequest += (DicomCStoreRequest req) =>
                {
                    LayoutClass.LogToDebugConsole("OnCStoreRequest started");
                    Console.WriteLine(DateTime.Now.ToString() + " recived");
                    SaveImage(req.Dataset, patientName, pathToDicomLocalFile, pathToDicomDownloadFile);

                    return Task.FromResult(new DicomCStoreResponse(req, DicomStatus.Success));
                };

                // the client has to accept storage of the images. We know that the requested images are of SOP class Secondary capture, 
                // so we add the Secondary capture to the additional presentation context
                // a more general approach would be to mace a cfind-request on image level and to read a list of distinct SOP classes of all
                // the images. these SOP classes shall be added here.
                var pcs = DicomPresentationContext.GetScpRolePresentationContextsFromStorageUids(
                    DicomStorageCategory.Image,
                    DicomTransferSyntax.ExplicitVRLittleEndian,
                    DicomTransferSyntax.ImplicitVRLittleEndian,
                    DicomTransferSyntax.ImplicitVRBigEndian);
                cGetDicomClient.AdditionalPresentationContexts.AddRange(pcs);

                //add the request payload to the C FIND SCU Client
                cGetDicomClient.AddRequestAsync(requestCGet);

                //Add a handler to be notified of any association rejections
                cGetDicomClient.AssociationRejected += CGetDicomClient_AssociationRejected;

                //Add a handler to be notified when association is successfully released - this can be triggered by the remote peer as well
                cGetDicomClient.AssociationReleased += OnAssociationReleased;

                cGetDicomClient.SendAsync();
            }
            Wait(10000);
            return cGetDicomClient;
        }

        private static void CFindDicomClient_AssociationRejected(object sender, Dicom.Network.Client.EventArguments.AssociationRejectedEventArgs e)
        {
            throw new NotImplementedException();
        }

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

        /*CStore*/
        private static Dicom.Network.Client.DicomClient CreateDicomStoreClient(string fileToTransmit)
        {
            string QRServerHost = "www.dicomserver.co.uk";
            int QRServerPort = 104;
            string QRServerAET = "STORESCP";
            string AET = "FODICOMSCU";
            string testAET = "ANY-SCP";

            var clientStore = new Dicom.Network.Client.DicomClient(QRServerHost, QRServerPort, false, AET, QRServerAET);


            //request for DICOM store operation
            var dicomCStoreRequest = new DicomCStoreRequest(fileToTransmit);

            //attach an event handler when remote peer responds to store request 
            dicomCStoreRequest.OnResponseReceived += OnStoreResponseReceivedFromRemoteHost;
            clientStore.AddRequestAsync(dicomCStoreRequest);
            clientStore.SendAsync();
            LayoutClass.LogToDebugConsole("Our DICOM CStore operation was successfully completed");

            //Add a handler to be notified of any association rejections

            clientStore.AssociationRejected += ClientStore_AssociationRejected;

            //Add a handler to be notified when association is successfully released - this can be triggered by the remote peer as well
            clientStore.AssociationReleased += OnAssociationReleased;

            return clientStore;
        }

        private static void OnStoreResponseReceivedFromRemoteHost(DicomCStoreRequest request, DicomCStoreResponse response)
        {
            LayoutClass.LogToDebugConsole("DICOM Store request was received by remote host for storage...");
            LayoutClass.LogToDebugConsole($"DICOM Store request was received by remote host for SOP instance transmitted for storage:{request.SOPInstanceUID}");
            LayoutClass.LogToDebugConsole($"Store operation response status returned was:{response.Status}");
        }
        private static void ClientStore_AssociationRejected(object sender, Dicom.Network.Client.EventArguments.AssociationRejectedEventArgs e)
        {
            throw new NotImplementedException();
        }

        // /*CFind*/
        public static void LogStudyResultsFoundToDebugConsole(DicomCFindResponse response, string patientName)
        {
            LayoutClass.LogToDebugConsole("Starting LogStudyResultsFound");
            //data will continue to come as long as the response is 'pending' 
            if (response.Status == DicomStatus.Pending)
            {
                patientName = response.Dataset.GetSingleValueOrDefault(DicomTag.PatientName, string.Empty);
                var studyDate = response.Dataset.GetSingleValueOrDefault(DicomTag.StudyDate, new DateTime());
                var studyUID = response.Dataset.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, string.Empty);

                LayoutClass.LogToDebugConsole("Matched Result...");
                LayoutClass.LogToDebugConsole($"Patient Found ->  {patientName} ");
                LayoutClass.LogToDebugConsole($"Study Date ->  {studyDate} ");
                LayoutClass.LogToDebugConsole($"Study UID ->  {studyUID} ");
                LayoutClass.LogToDebugConsole("\n");
            }

            if (response.Status == DicomStatus.Success)
            {
                LayoutClass.LogToDebugConsole(response.Status.ToString());
            }
        }

        /*CGet*/
        private static void CGetDicomClient_AssociationRejected(object sender, Dicom.Network.Client.EventArguments.AssociationRejectedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /*Save image method*/
        public static void SaveImage(DicomDataset dataset, string patientName, string pathToDicomLocalFile, 
            string pathToDicomDownloadFile)
        {
            var studyUid = dataset.GetSingleValue<string>(DicomTag.StudyInstanceUID);
            patientName = dataset.GetSingleValueOrDefault<string>(DicomTag.PatientName, "undefined");

            if (File.Exists($@"{pathToDicomLocalFile}\{patientName}\{studyUid}.dcm"))
            {
                return;
            }
            else
            {
                var path = Path.GetFullPath($@"{pathToDicomDownloadFile}");
                path = Path.Combine(path, patientName);

                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                path = Path.Combine(path, studyUid) + ".dcm";

                new DicomFile(dataset).Save(path);
            }
        }

        /*Method used for verifying the connection*/
        private static void OnAssociationReleased(object sender, EventArgs e)
        {
            LayoutClass.LogToDebugConsole("Association was released. BYE BYE!");
        }

        /*CEcho method*/
        private static Dicom.Network.DicomClient CreateDicomVerificationClient()
        {
            var client = new Dicom.Network.DicomClient();

            //register that we want to do a DICOM ping here
            var dicomCEchoRequest = new DicomCEchoRequest();

            //attach an event handler when remote peer responds to echo request
            dicomCEchoRequest.OnResponseReceived += OnEchoResponseReceivedFromRemoteHost;
            client.AddRequest(dicomCEchoRequest);

            return client;
        }

        private static void OnEchoResponseReceivedFromRemoteHost(DicomCEchoRequest request, DicomCEchoResponse response)
        {
            LayoutClass.LogToDebugConsole($"DICOM Echo Verification request was received by remote host");
            LayoutClass.LogToDebugConsole($"Response was received from remote host...");
            LayoutClass.LogToDebugConsole($"Verification response status returned was:{response.Status.ToString()}");
        }

        public static void ExtractDataset(DicomFile file, Label label10, Label label11, Label label12, Label label13,
            Label label14, Label label15, Label label16, Label label17, Label label18, Label label19, Label label20, 
            Label label21, Label label22, Label label23, Label label24, Label label25, Label label26, Label label27, 
            Label label28, Label label29, Label label30, Label label31, Label label32, Label label33, Label label34,
            PictureBox pictureBox1, string pathToDicomFile)
        {
            var dicomDataset = file.Dataset;

            //Patient info
            var patientName1 = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.PatientName, "undefined");
            var patientID = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.PatientID, "undefined");
            var patientDOB = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.PatientBirthDate, "undefined");
            var patientSex = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.PatientSex, "undefined");

            LayoutClass.LogToDebugConsole($" Patient name: {patientName1}", label10);
            LayoutClass.LogToDebugConsole($" Patient ID: {patientID}", label11);
            LayoutClass.LogToDebugConsole($" Patient DOB: {patientDOB}", label12);
            LayoutClass.LogToDebugConsole($" Patient Sex: {patientSex}", label13);

            //Study info
            var studyDate = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.StudyDate, "undefined");
            var studyTime = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.StudyTime, "undefined");
            var accessionNr = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.AccessionNumber, "undefined");
            var studyDescription = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.StudyDescription, "undefined");
            var studyID = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.StudyID, "undefined");

            LayoutClass.LogToDebugConsole($" Study Date: {studyDate} ", label14);
            LayoutClass.LogToDebugConsole($" Study Time: {studyTime}", label15);
            LayoutClass.LogToDebugConsole($" Accession Number: {accessionNr}", label16);
            LayoutClass.LogToDebugConsole($" Study Description: {studyDescription}", label17);
            LayoutClass.LogToDebugConsole($" Study ID: {studyID}", label18);

            //Series info
            var seriesDate = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.SeriesDate, "undefined");
            var seriesTime = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.SeriesTime, "undefined");
            string modality = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.Modality, "undefined");
            var seriesNr = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.SeriesNumber, "undefined");

            LayoutClass.LogToDebugConsole($" Series Date: {seriesDate} ", label19);
            LayoutClass.LogToDebugConsole($" Series Time: {seriesTime}", label20);
            LayoutClass.LogToDebugConsole($" Modality: {modality}", label21);
            LayoutClass.LogToDebugConsole($" Series Number: {seriesNr}", label22);

            //Image info
            var nrOfFrames = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.NumberOfFrames, "undefined");
            var instanceNr = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.InstanceNumber, "undefined");
            var photometricIntp = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.PhotometricInterpretation, "undefined");
            var rows = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.Rows, "undefined");
            var columns = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.Columns, "undefined");
            var pixelSpacing = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.PixelSpacing, "undefined");
            var bits = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.BitsAllocated, "undefined");

            LayoutClass.LogToDebugConsole($" Number of Frames: {nrOfFrames} ", label23);
            LayoutClass.LogToDebugConsole($" Instance Number: {instanceNr}", label24);
            LayoutClass.LogToDebugConsole($" Photometric Interpretation: {photometricIntp}", label25);
            LayoutClass.LogToDebugConsole($" Rows: {rows}", label26);
            LayoutClass.LogToDebugConsole($" Columns: {columns} ", label27);
            LayoutClass.LogToDebugConsole($" Pixel Spacing: {pixelSpacing}", label28);
            LayoutClass.LogToDebugConsole($" Bits: {bits}", label29);

            //Extra info
            var studyIUid = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.StudyInstanceUID, "undefined");
            var seriesInstanceUid = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.SeriesInstanceUID, "undefined");
            var sopClassUid = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.SOPClassUID, "undefined");
            var sopInstanceUid = dicomDataset.GetSingleValueOrDefault<string>(DicomTag.SOPInstanceUID, "undefined");
            var transferSyntaxUid = file.FileMetaInfo.TransferSyntax;

            LayoutClass.LogToDebugConsole($" StudyInstanceUid - {studyIUid}", label30);
            LayoutClass.LogToDebugConsole($" SeriesInstanceUid - {seriesInstanceUid}", label31);
            LayoutClass.LogToDebugConsole($" SopClassUid - {sopClassUid}", label32);
            LayoutClass.LogToDebugConsole($" SopInstanceUid - {sopInstanceUid}", label33);
            LayoutClass.LogToDebugConsole($" TransferSyntaxUid - {transferSyntaxUid}", label34);

            //Image
            var iimage = new DicomImage(pathToDicomFile);

            Bitmap imageBmp = iimage.RenderImage().AsClonedBitmap();
            var image = resizeImage(imageBmp, new Size(876, 628));
            image = resizeImage(image, new Size(876, 628));

            pictureBox1.Image = image;
            LayoutClass.LogToDebugConsole($"Extract operation from DICOM file successful");
        }
        public static Image resizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }
    }
}
