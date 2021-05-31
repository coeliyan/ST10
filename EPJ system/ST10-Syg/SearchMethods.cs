using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace ST10_Syg
{
    class SearchMethods
    {
        // Method to find patient by CPR
        public static Patient FindPatient(FhirClient client)
        {
            Patient currentPatient = new Patient();
            List<Patient> patientList = new List<Patient>();
            try
            {
                patientList = SearchMethods.GetPatients(client, new string[]
                    { "identifier=120446-7891" }, 10);
                foreach (Patient patient in patientList)
                {
                    currentPatient = patient;
                }
            }
            catch (FhirOperationException e)
            {
                MessageBox.Show(e.Message, "FhirOperationException",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return currentPatient;
        }

        // Method to find patients based on given criteria
        public static List<Patient> GetPatients(
        FhirClient fhirClient,
        string[] patientCriteria = null,
        int maxPatients = 20)
        {
            List<Patient> patients = new List<Patient>();

            Bundle patientBundle;
            //If none criteria is given search without, else use the given criteria to search for patients.
            if ((patientCriteria == null) || (patientCriteria.Length == 0))
            {
                patientBundle = fhirClient.Search<Patient>();
            }
            else
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

                    // If max count is reach stop 
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

        // Method to find practitioners, works the same way as above method
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

        // Search for diagnostic reports, works the same way
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

        // Method to find observations, works the same way
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

        // Search for compositions, works the same way
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

        // Search for conditions, works the same way
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
