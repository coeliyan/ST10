using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace DICOMtest
{
    class LayoutClass
    {
        /*Method to enable writing to labels and the console*/
        public static void LogToDebugConsole(string informationToLog, Label label)
        {
            label.Text = label.Text + informationToLog + "\r\n";
        }
        public static void LogToDebugConsole(string informationToLog)
        {
            Debug.WriteLine(informationToLog);
        }

        public static void UpdateLabels(Label label1, Label label2, Label label3, Label label4, Label label5, Label label6,
            Label label7, Label label8, Label label9, Label label10, Label label11, Label label12, Label label13, Label label14,
            Label label15, Label label16, Label label17, Label label18, Label label19, Label label20, Label label21, Label label22,
            Label label23, Label label24, Label label25, Label label26, Label label27, Label label28, Label label29, Label label30, Label label31,
            Label label32, Label label33, Label label34, Button button1, string patientName_transfer, string patientCPR_transfer)
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
            button1.Text = "Luk PACS";
        }
    }
}
