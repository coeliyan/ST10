using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using Hl7.Fhir;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Model;
using Hl7.Fhir.Validation;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Utility;

namespace ST10_Kiro
{
    class CreateResources
    {
        //Create instance of Practitioner that conforms to the given profile in Meta
        public static Practitioner CreatePractitioner(string family, string given, string idValue)
        {
            Practitioner pra = new Practitioner();
            Identifier id = new Identifier();
            id.System = "https://autregweb.sst.dk/authorizationsearch.aspx";
            id.Value = idValue;
            id.Type = new CodeableConcept("http://hl7.org/fhir/R4/valueset-identifier-type.html", "MD");
            pra.Identifier.Add(id);

            HumanName name = new HumanName();
            name.Family = family;
            name.Given = new List<string>() { given };
            pra.Name.Add(name);

            pra.Meta = new Meta()
            {

                Profile = new string[] { "http://example.org/fhir/StructureDefinition/ST10Practitioner" }

            };

            return pra;
        }

        //Create instance of Patient that conforms to the given profile in Meta
        public static Patient CreatePatient()
        {
            Patient toCreate = new Patient();

            Identifier id = new Identifier();
            id.Use = Identifier.IdentifierUse.Official;
            id.System = "urn:oid:1.2.208.176.1.2";
            id.Value = "120446-7891";
            id.Type = new CodeableConcept("http://hl7.org/fhir/R4/valueset-identifier-type.html", "NNDNK");
            toCreate.Identifier.Add(id);

            HumanName name = new HumanName();
            name.Family = "Hansen";
            name.Given = new List<string>() { "Jørgen" };
            toCreate.Name.Add(name);

            toCreate.Meta = new Meta()
            {

                Profile = new string[] { "http://example.org/fhir/StructureDefinition/ST10Patient" }

            };

            return toCreate;
        }

        //Create instance of Concition that conforms to the given profile in Meta
        public static Condition CreateCondition(string uri, Practitioner uro, Patient patient, string code, FhirDateTime date)
        {
            Condition con = new Condition();
            con.Code = new CodeableConcept("http://hl7.org/fhir/sid/icd-10", code);
            con.Subject = new ResourceReference(uri + "Patient/" + patient.Id);
            con.Onset = date;
            con.Asserter = new ResourceReference(uri + "Practitioner/" + uro.Id);

            con.Meta = new Meta()
            {

                Profile = new string[] { "http://example.org/fhir/StructureDefinition/ST10Condition" }

            };

            return con;
        }

        //Create instance of Observation that conforms to the given profile in Meta
        public static Observation CreateSymptom(string uri, Practitioner kiro, Patient patient,
            string code, FhirDateTime date, string beskrivelse,
            string worsening, string alleviating, string cause, string progression)
        {
            Observation sym = new Observation();
            sym.Status = ObservationStatus.Final;
            sym.Code = new CodeableConcept("http://hl7.org/fhir/R4/valueset-clinical-findings.html", code);
            sym.Subject = new ResourceReference(uri + "Patient/" + patient.Id);
            sym.Effective = date;
            sym.Performer = new List<ResourceReference>() { new ResourceReference(uri + "Practitioner/" + kiro.Id) };
            sym.Value = new FhirString(beskrivelse);
            sym.Component = new List<Observation.ComponentComponent>()
            {
                new Observation.ComponentComponent
                {
                    Code = new CodeableConcept("http://snomed.info/sct", "162473008"),
                    Value = new FhirString(worsening)
                },

                new Observation.ComponentComponent
                {
                    Code = new CodeableConcept("http://snomed.info/sct", "162483007"),
                    Value = new FhirString(alleviating)
                },

                new Observation.ComponentComponent
                {
                    Code = new CodeableConcept("http://snomed.info/sct", "42752001"),
                    Value = new FhirString(cause)
                },

                new Observation.ComponentComponent
                {
                    Code = new CodeableConcept("http://snomed.info/sct", "385634002"),
                    Value = new FhirString(progression)
                },

            };

            sym.Meta = new Meta()
            {

                Profile = new string[] { "http://example.org/fhir/StructureDefinition/ST10Observation" }

            };

            return sym;
        }

        //Create instance of DiagnosticReport that conforms to the given profile in Meta
        public static DiagnosticReport CreateDiagnosticReport(string uri, string studyCode, string conclusionCode, string conclusion, FhirDateTime date, Practitioner radiolog, Patient patient, ImagingStudy imagingStudy)
        {
            DiagnosticReport rep = new DiagnosticReport();
            rep.Status = DiagnosticReport.DiagnosticReportStatus.Final;
            rep.Code = new CodeableConcept("https://medinfo.dk/sks/brows.php", studyCode);
            rep.Subject = new ResourceReference(uri + "Patient/" + patient.Id);
            rep.Effective = date;
            rep.ResultsInterpreter = new List<ResourceReference>()
            {
                new ResourceReference(uri + "Practitioner/" + radiolog.Id)
            };
            rep.ImagingStudy = new List<ResourceReference>()
            {
                new ResourceReference(uri + "ImagingStudy/" + imagingStudy.Id)
            };
            rep.Conclusion = conclusion;
            rep.ConclusionCode = new List<CodeableConcept>()
            {
                new CodeableConcept("http://hl7.org/fhir/ValueSet/clinical-findings", conclusionCode)
            };

            rep.Meta = new Meta()
            {

                Profile = new string[] { "http://example.org/fhir/StructureDefinition/ST10DiagnosticReport" }

            };

            return rep;
        }

        //Create instance of Composition that conforms to the given profile in Meta
        public static Composition CreateCompositionWithObservation(string uri, FhirDateTime date, string conclusion,
            Patient patient, Practitioner practitioner, Location location, Observation observation)
        {
            Composition composition = new Composition();
            composition.Status = CompositionStatus.Final;
            composition.Type = new CodeableConcept()
            {
                Coding = new List<Coding>()
                {
                    new Coding()
                    {
                        System = "http://snomed.info/sct",
                        Version = "Danish Edition 2021 03 31",
                        Code = "422735006",
                        Display = "Summary clinical document (record artifact)"
                    }
                }
            };
            composition.Subject = new ResourceReference(uri + "Patient/" + patient.Id);
            composition.DateElement = date;
            composition.Author = new List<ResourceReference>()
            {
                new ResourceReference(uri + "Practitioner/" + practitioner.Id)
            };
            composition.Title = "Journalnotat";
            composition.Event = new List<Composition.EventComponent>()
            {
                new Composition.EventComponent()
                {
                    Code = new List<CodeableConcept>()
                    {
                        new CodeableConcept("http://hl7.org/fhir/R4/valueset-clinical-findings.html", "161891005")
                    },
                    Detail = new List<ResourceReference>()
                    {
                        new ResourceReference(uri + "Observation/" + observation.Id)
                    }
                }
            };
            composition.Section = new List<Composition.SectionComponent>()
            {
                new Composition.SectionComponent()
                {
                    Text = new Narrative()
                    {
                        Status = Narrative.NarrativeStatus.Generated,
                        Div = conclusion
                    },
                    Entry = new List<ResourceReference>()
                    {
                        new ResourceReference(uri + "Location/" + location.Id)
                    }
                }
            };

            composition.Meta = new Meta()
            {

                Profile = new string[] { "http://example.org/fhir/StructureDefinition/ST10Summary" }

            };

            return composition;
        }

        //Create instance of Composition that conforms to the given profile in Meta
        public static Composition CreateCompositionWithCondition(string uri, FhirDateTime date, string conclusion,
    Patient patient, Practitioner practitioner, Location location, Condition condition)
        {
            Composition composition = new Composition();
            composition.Status = CompositionStatus.Final;
            composition.Type = new CodeableConcept()
            {
                Coding = new List<Coding>()
                {
                    new Coding()
                    {
                        System = "http://snomed.info/sct",
                        Version = "Danish Edition 2021 03 31",
                        Code = "422735006",
                        Display = "Summary clinical document (record artifact)"
                    }
                }
            };
            composition.Subject = new ResourceReference(uri + "Patient/" + patient.Id);
            composition.DateElement = date;
            composition.Author = new List<ResourceReference>()
            {
                new ResourceReference(uri + "Practitioner/" + practitioner.Id)
            };
            composition.Title = "Journalnotat";
            composition.Event = new List<Composition.EventComponent>()
            {
                new Composition.EventComponent()
                {
                    Code = new List<CodeableConcept>()
                    {
                        new CodeableConcept("http://hl7.org/fhir/sid/icd-10", "/DC619M/")
                    },
                    Detail = new List<ResourceReference>()
                    {
                        new ResourceReference(uri + "Condition/" + condition.Id)
                    }
                }
            };
            composition.Section = new List<Composition.SectionComponent>()
            {
                new Composition.SectionComponent()
                {
                    Text = new Narrative()
                    {
                        Status = Narrative.NarrativeStatus.Generated,
                        Div = conclusion
                    },
                    Entry = new List<ResourceReference>()
                    {
                        new ResourceReference(uri + "Location/" + location.Id)
                    }
                }
            };

            composition.Meta = new Meta()
            {

                Profile = new string[] { "http://example.org/fhir/StructureDefinition/ST10Summary" }

            };

            return composition;
        }

        //Create instance of Location that conforms to the given profile in Meta
        public static Location CreateLocationWithID(string name, string system, string IDnumber)
        {
            Location location = new Location();
            location.Identifier = new List<Identifier>()
            {
                new Identifier()
                {
                    System = system,
                    Value = IDnumber                }
            };

            location.Name = name;
            location.Meta = new Meta()
            {

                Profile = new string[] { "http://example.org/fhir/StructureDefinition/ST10LocationV3" }

            };

            return location;
        }

        //Create instance of ImagingStudy that conforms to the given profile in Meta
        public static ImagingStudy CreateImagingStudy(string uri, string dicomUID, string modalityCode,
            string reasonCode, string seriesUID, string bodySiteCode, FhirDateTime date, string instanceUID,
            string sopClass, Patient patient, Practitioner radiolog, Endpoint endpoint1, Location location)
        {
            ImagingStudy img = new ImagingStudy();
            Identifier id = new Identifier();
            id.Type = new CodeableConcept("http://terminology.hl7.org/CodeSystem/v2-0203", "ACSN");
            id.System = "urn:dicom:uid";
            id.Value = dicomUID;
            img.Identifier.Add(id);

            img.Status = ImagingStudy.ImagingStudyStatus.Available;
            img.Modality = new List<Coding>()
            {
                new Coding("http://dicom.nema.org/medical/dicom/current/output/chtml/part16/sect_CID_29.html", modalityCode)
            };
            img.Subject = new ResourceReference(uri + "Patient/" + patient.Id);
            img.Interpreter = new List<ResourceReference>()
            {
                new ResourceReference(uri + "Practitioner/" + radiolog.Id)
            };
            img.Endpoint = new List<ResourceReference>()
            {
                new ResourceReference(uri + "Endpoint/" + endpoint1.Id)
            };
            img.ReasonCode = new List<CodeableConcept>()
            {
                new CodeableConcept("https://medinfo.dk/sks/brows.php", reasonCode)
            };
            img.Series = new List<ImagingStudy.SeriesComponent>()
            {
                new ImagingStudy.SeriesComponent()
                {
                    Uid = seriesUID,
                    Modality = new Coding("http://dicom.nema.org/medical/dicom/current/output/chtml/part16/sect_CID_29.html", modalityCode),
                    BodySite = new Coding("http://hl7.org/fhir/ValueSet/body-site", bodySiteCode),
                    StartedElement = date,
                    Instance = new List<ImagingStudy.InstanceComponent>()
                    {
                        new ImagingStudy.InstanceComponent()
                        {
                            Uid = instanceUID,
                            SopClass = new Coding("http://dicom.nema.org/medical/dicom/current/output/chtml/part04/sect_B.5.html#table_B.5-1", sopClass)
                        }
                    }
                }
            };

            img.Location = new ResourceReference(uri + "Location/" + location.Id);

            img.Meta = new Meta()
            {

                Profile = new string[] { "http://example.org/fhir/StructureDefinition/ST10ImagingStudyV2" }

            };

            return img;
        }

        //Create instance of Endpoint that conforms to the given profile in Meta
        public static Endpoint CreateEndpoint(string connectionType, string payloadType, string url)
        {
            Endpoint end = new Endpoint();
            end.Status = Endpoint.EndpointStatus.Active;
            end.ConnectionType = new Coding("http://hl7.org/fhir/ValueSet/endpoint-connection-type", connectionType);
            end.PayloadType = new List<CodeableConcept>()
            {
                new CodeableConcept("http://hl7.org/fhir/ValueSet/endpoint-payload-type", payloadType)
            };
            end.Address = url;

            end.Meta = new Meta()
            {

                Profile = new string[] { "http://example.org/fhir/StructureDefinition/ST10Endpoint" }

            };

            return end;
        }

    }
}
