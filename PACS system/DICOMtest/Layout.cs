using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace DICOMtest
{
    class LayoutClass
    {
        // Add patient info to labels
        public static void InitUI(Label label7, Label label8, string patientName_transfer, string patientCPR_transfer)
        {
            label7.Text = $"Patient navn: {patientName_transfer}";
            label8.Text = $"CPR: {patientCPR_transfer}";
        }

        // Add info to given label
        public static void LogToDebugConsole(string informationToLog, Label label)
        {
            label.Text = label.Text + informationToLog + "\r\n";
        }

        // Write to console
        public static void LogToDebugConsole(string informationToLog)
        {
            Debug.WriteLine(informationToLog);
        }
    }
}
