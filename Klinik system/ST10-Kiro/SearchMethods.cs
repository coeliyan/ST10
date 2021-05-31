using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace ST10_Kiro
{
    class SearchMethods
    {
        // Method to find patient with given CPR
        public static Patient FindPatient(FhirClient client)
        {
            Patient patient1 = new Patient();
            List<Patient> patientList = new List<Patient>();
            try
            {
                patientList = SearchMethods.GetPatients(client, new string[]
                    { "identifier=120446-7891" }, 10);
                foreach (Patient patient in patientList)
                {
                    patient1 = patient;
                }
            }
            catch (FhirOperationException e)
            {
                MessageBox.Show(e.Message, "FhirOperationException",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return patient1;
        }

        //Method to search the server for patients given criteria
        public static List<Patient> GetPatients(
        FhirClient fhirClient,
        string[] patientCriteria = null,
        int maxPatients = 20)
        {
            List<Patient> patients = new List<Patient>();

            Bundle patientBundle;

            //If there isn't given any criteria search the server without
            if ((patientCriteria == null) || (patientCriteria.Length == 0))
            {
                patientBundle = fhirClient.Search<Patient>();
            }
            else //Search server by criteria
            {
                patientBundle = fhirClient.Search<Patient>(patientCriteria);
            }

            while (patientBundle != null)
            {
                // list each patient in the bundle
                foreach (Bundle.EntryComponent entry in patientBundle.Entry)
                {
                    if (entry.Resource != null)
                    {
                        if (entry.Resource.GetType().ToString() == "Hl7.Fhir.Model.Patient")
                        {
                            Patient patient = (Patient)entry.Resource;

                            patients.Add(patient);
                        }
                    }

                    // If max count is reached stop searching
                    if (patients.Count >= maxPatients)
                    {
                        break;
                    }
                }

                if (patients.Count >= maxPatients)
                {
                    break;
                }

                // get more results
                patientBundle = fhirClient.Continue(patientBundle);
            }

            return patients;
        }

        //Method to search the server for practitioners given criteria, works the same way as above
        public static List<Practitioner> GetPractitioners(
        FhirClient fhirClient,
        string[] practitionerCriteria = null,
        int maxPractitioners = 20)
        {
            List<Practitioner> practitioners = new List<Practitioner>();

            Bundle practitionerBundle;
            if ((practitionerCriteria == null) || (practitionerCriteria.Length == 0))
            {
                practitionerBundle = fhirClient.Search<Patient>();
            }
            else
            {
                practitionerBundle = fhirClient.Search<Practitioner>(practitionerCriteria);
            }

            while (practitionerBundle != null)
            {
                foreach (Bundle.EntryComponent entry in practitionerBundle.Entry)
                {
                    if (entry.Resource != null)
                    {
                        if (entry.Resource.GetType().ToString() == "Hl7.Fhir.Model.Practitioner")
                        {
                            Practitioner practitioner = (Practitioner)entry.Resource;

                            practitioners.Add(practitioner);
                        }
                    }

                    if (practitioners.Count >= maxPractitioners)
                    {
                        break;
                    }
                }

                if (practitioners.Count >= maxPractitioners)
                {
                    break;
                }

                practitionerBundle = fhirClient.Continue(practitionerBundle);
            }

            return practitioners;
        }

        //Method to search the server for diagnostic reports given criteria, works the same way as above
        public static List<DiagnosticReport> GetDiagnosticReports(
        FhirClient fhirClient,
        string[] drCriteria = null,
        int maxDR = 20)
        {
            List<DiagnosticReport> diagnosticReports = new List<DiagnosticReport>();

            Bundle drBundle;
            if ((drCriteria == null) || (drCriteria.Length == 0))
            {
                drBundle = fhirClient.Search<DiagnosticReport>();
            }
            else
            {
                drBundle = fhirClient.Search<DiagnosticReport>(drCriteria);
            }

            while (drBundle != null)
            {
                foreach (Bundle.EntryComponent entry in drBundle.Entry)
                {
                    if (entry.Resource != null)
                    {
                        if (entry.Resource.GetType().ToString() == "Hl7.Fhir.Model.DiagnosticReport")
                        {
                            DiagnosticReport diagnosticReport = (DiagnosticReport)entry.Resource;

                            diagnosticReports.Add(diagnosticReport);
                        }
                    }

                    if (diagnosticReports.Count >= maxDR)
                    {
                        break;
                    }
                }

                if (diagnosticReports.Count >= maxDR)
                {
                    break;
                }

                drBundle = fhirClient.Continue(drBundle);
            }

            return diagnosticReports;
        }

        //Method to search the server for observations given criteria, works the same way as above
        public static List<Observation> GetObservations(
        FhirClient fhirClient,
        string[] observationCriteria = null,
        int maxObservations = 20)
        {
            List<Observation> observations = new List<Observation>();

            Bundle obsBundle;
            if ((observationCriteria == null) || (observationCriteria.Length == 0))
            {
                obsBundle = fhirClient.Search<Observation>();
            }
            else
            {
                obsBundle = fhirClient.Search<Observation>(observationCriteria);
            }

            while (obsBundle != null)
            {
                foreach (Bundle.EntryComponent entry in obsBundle.Entry)
                {
                    if (entry.Resource != null)
                    {
                        if (entry.Resource.GetType().ToString() == "Hl7.Fhir.Model.Observation")
                        {
                            Observation observation = (Observation)entry.Resource;

                            observations.Add(observation);
                        }
                    }

                    if (observations.Count >= maxObservations)
                    {
                        break;
                    }
                }

                if (observations.Count >= maxObservations)
                {
                    break;
                }

                obsBundle = fhirClient.Continue(obsBundle);
            }

            return observations;
        }

        //Method to search the server for compositions given criteria, works the same way as above
        public static List<Composition> GetCompositions(
        FhirClient fhirClient,
        string[] compostionCriteria = null,
        int maxCompositions = 20)
        {
            List<Composition> compositions = new List<Composition>();

            Bundle cBundle;
            if ((compostionCriteria == null) || (compostionCriteria.Length == 0))
            {
                cBundle = fhirClient.Search<Composition>();
            }
            else
            {
                cBundle = fhirClient.Search<Composition>(compostionCriteria);
            }

            while (cBundle != null)
            {

                foreach (Bundle.EntryComponent entry in cBundle.Entry)
                {
                    if (entry.Resource != null)
                    {
                        if (entry.Resource.GetType().ToString() == "Hl7.Fhir.Model.Composition")
                        {
                            Composition composition = (Composition)entry.Resource;
                            compositions.Add(composition);
                        }
                    }

                    if (compositions.Count >= maxCompositions)
                    {
                        break;
                    }
                }

                if (compositions.Count >= maxCompositions)
                {
                    break;
                }

                cBundle = fhirClient.Continue(cBundle);
            }

            return compositions;
        }

        //Method to search the server for conditions given criteria, works the same way as above
        public static List<Condition> GetConditions(
        FhirClient fhirClient,
        string[] conditionCriteria = null,
        int maxConditions = 20)
        {
            List<Condition> conditions = new List<Condition>();

            Bundle cBundle;
            if ((conditionCriteria == null) || (conditionCriteria.Length == 0))
            {
                cBundle = fhirClient.Search<Condition>();
            }
            else
            {
                cBundle = fhirClient.Search<Condition>(conditionCriteria);
            }

            while (cBundle != null)
            {
                foreach (Bundle.EntryComponent entry in cBundle.Entry)
                {
                    if (entry.Resource != null)
                    {
                        if (entry.Resource.GetType().ToString() == "Hl7.Fhir.Model.Condition")
                        {
                            Condition condition = (Condition)entry.Resource;

                            conditions.Add(condition);
                        }
                    }

                    if (conditions.Count >= maxConditions)
                    {
                        break;
                    }
                }

                if (conditions.Count >= maxConditions)
                {
                    break;
                }

                cBundle = fhirClient.Continue(cBundle);
            }

            return conditions;
        }
    }
}
