using System;
using System.Collections.Generic;
using PersonalHealthRecord;
using PersonalHealthRecordUI.Models;

namespace PersonalHealthRecordUI.Logic
{
    public class Api : IApi
    {
        public static IApi Instance => _instance ?? (_instance = new Api());
        private static IApi _instance;

        public DateTime SysDate
        {
            get => _sys.SysDate;
            set => _sys.SysDate = value;
        }

        private Api()
        {
        }

        private readonly ISystem _sys = PersonalHealthRecord.System.Instance;

        public bool AddHospital(string name) => _sys.AddHospital(name);

        public bool AddPatient(PatientModel patient) => _sys.AddPatient(patient.FirstName, patient.LastName, patient.Birthday, patient.PersonalNumber, patient.InsuranceAgencyCode);

        public bool StartHospitalization(string hospitalName, int personalNumber, string diagnosis) => _sys.StartHospitalization(hospitalName, personalNumber, diagnosis);

        public bool EndHospitalization(string hospitalName, int personalNumber) => _sys.EndHospitalization(hospitalName, personalNumber);

        public IEnumerable<HospitalizationModel> GetHospitalizations(string hospitalName, int personalNumber)
        {
            var hospitalizations = _sys.GetHospitalizations(hospitalName, personalNumber);
            foreach (var hospitalization in hospitalizations)
                yield return new HospitalizationModel(hospitalization[0], hospitalization[1], hospitalization[2], hospitalization[3], hospitalization[4], hospitalization[5], hospitalization[6]);
        }

        public IEnumerable<HospitalizationModel> GetHospitalizations(string hospitalName, string name, string surname)
        {
            var hospitalizations = _sys.GetHospitalizations(hospitalName, name, surname);
            foreach (var hospitalization in hospitalizations)
                yield return new HospitalizationModel(hospitalization[0], hospitalization[1], hospitalization[2], hospitalization[3], hospitalization[4], hospitalization[5], hospitalization[6]);
        }

        public IEnumerable<PatientModel> GetPatients(string hospitalName, DateTime from, DateTime to)
        {
            var patients = _sys.GetPatients(hospitalName, from, to);
            foreach (var patient in patients)
                yield return new PatientModel(patient[0], patient[1], patient[2], patient[3], patient[4]);
        }

        public IEnumerable<AgencyReportModel> GetAccountantReport(string hospitalName, DateTime month)
        {
            var data = _sys.GetAccountantReport(hospitalName, month);
            foreach (var agencyData in data)
            {
                var report = new AgencyReportModel();
                foreach (var record in agencyData)
                {
                    var item = new AgencyReportItemModel
                    {
                        Day = DateTime.Parse(record[1]),
                        FirstName = record[2],
                        LastName = record[3],
                        Diagnosis = record[4]
                    };
                    report.AgencyCode = record[0];
                    report.Hospitalizations.Add(item);
                }
                yield return report;
            }
        }

        public IEnumerable<PatientModel> GetHospitalizedPatients(string hospitalName)
        {
            var patients = _sys.GetHospitalizedPatients(hospitalName);
            foreach (var patient in patients)
                yield return new PatientModel(patient[0], patient[1], patient[2], patient[3], patient[4]);
        }

        public IEnumerable<PatientModel> GetHospitalizedPatients(string hospitalName, string agencyCode)
        {
            var patients = _sys.GetHospitalizedPatients(hospitalName, agencyCode);
            foreach (var patient in patients)
                yield return new PatientModel(patient[0], patient[1], patient[2], patient[3], patient[4]);
        }

        public IEnumerable<HospitalModel> GetHospitals()
        {
            var hospitals = _sys.GetHospitals();
            foreach (var hospitalName in hospitals)
                yield return new HospitalModel(hospitalName);
        }

        public bool RemoveHospital(string oldHospital, string newHospital) => _sys.RemoveHospital(oldHospital, newHospital);

        public IEnumerable<PatientModel> GetPatients()
        {
            var patients = _sys.GetPatients();
            foreach (var patient in patients)
                yield return new PatientModel(patient[0], patient[1], patient[2], patient[3], patient[4]);
        }

        public IEnumerable<HospitalizationModel> GetHospitalizations(int personalNumber)
        {
            var hospitalizations = _sys.GetHospitalizations(personalNumber);
            foreach (var hospitalization in hospitalizations)
                yield return new HospitalizationModel(hospitalization[0], hospitalization[1], hospitalization[2], hospitalization[3]);
        }

        public void Generate(int hospitals, int patients, int records, int ongoingRecords) => _sys.Generate(hospitals, patients, records, ongoingRecords);

        public void Load() => _sys.Load();

        public void Save() => _sys.Save();
    }
}
