using System;
using System.Collections.Generic;
using DataStructures;
using PersonalHealthRecord.Generator;
using PersonalHealthRecord.Model;

namespace PersonalHealthRecord
{
    public class System : ISystem
    {
        public static System Instance => _instance ?? (_instance = new System());
        private static System _instance;

        public DateTime SysDate
        {
            get
            {
                var now = DateTime.Now;
                return _sysDate.AddMinutes(now.Minute)
                               .AddSeconds(now.Second)
                               .AddMilliseconds(now.Millisecond);
            }
            set
            {
                if (_sysDate < value)
                {
                    var oldDate = _sysDate;
                    _sysDate = value;
                    OnSysDateChange(oldDate, _sysDate);
                }
            }
        }

        internal IEnumerable<Patient> Patients => _patients.LevelOrder();
        internal IEnumerable<Hospital> Hospitals => _hospitals.LevelOrder();

        private DateTime _sysDate = DateTime.Today;
        private readonly RbTree<string, Hospital> _hospitals = new RbTree<string, Hospital>();
        private readonly RbTree<int, Patient> _patients = new RbTree<int, Patient>();
        private readonly DataSaver _dataSaver;

        private System()
        {
            _dataSaver = new DataSaver(this);
        }

        public bool AddHospital(string name)
        {
            try
            {
                _hospitals.Insert(name, new Hospital(name));
            }
            catch (ArgumentException)
            {
                throw new Exception($"Hospital with name {name} already exists.");
            }
            return true;
        }

        public bool AddPatient(string name, string surname, DateTime birthday, int personalNumber, string agencyCode)
        {
            try
            {
                _patients.Insert(personalNumber, new Patient(name, surname, birthday, personalNumber, agencyCode));
            }
            catch (ArgumentException)
            {
                throw new Exception($"Patient with personal number {personalNumber} already exists.");
            }
            return true;
        }

        public bool StartHospitalization(string hospitalName, int personalNumber, string diagnosis)
        {
            var patient = GetPatient(personalNumber);
            var hospitalization = patient.StartHospitalization(SysDate, diagnosis);
            var hospital = GetHospital(hospitalName);
            hospital.StartHospitalization(hospitalization);
            return true;
        }

        public bool EndHospitalization(string hospitalName, int personalNumber)
        {
            var patient = GetPatient(personalNumber);
            var hospitalization = patient.EndHospitalization(SysDate);
            var hospital = GetHospital(hospitalName);
            hospital.EndHospitalization(hospitalization);
            return true;
        }

        public IEnumerable<string[]> GetHospitalizations(string hospitalName, int personalNumber)
        {
            var hospital = GetHospital(hospitalName);
            IEnumerable<Hospitalization> hospitalizations;
            try
            {
                hospitalizations = hospital.GetHospitalizations(personalNumber);
            }
            catch (KeyNotFoundException)
            {
                throw new Exception($"Hospital {hospitalName} doesn't contain hospitalizations for patient {personalNumber}.");
            }
            foreach (var hospitalization in hospitalizations)
                yield return hospitalization.ToArrayWithPatient();
        }

        public IEnumerable<string[]> GetHospitalizations(string hospitalName, string name, string surname)
        {
            var hospital = GetHospital(hospitalName);
            IEnumerable<Hospitalization> hospitalizations;
            try
            {
                hospitalizations = hospital.GetHospitalizations(name, surname);
            }
            catch (KeyNotFoundException)
            {
                throw new Exception($"Hospital {hospitalName} doesn't contain hospitalizations for patient {name} {surname}.");
            }
            foreach (var hospitalization in hospitalizations)
                yield return hospitalization.ToArrayWithPatient();
        }

        public IEnumerable<string[]> GetPatients(string hospitalName, DateTime from, DateTime to)
        {
            var hospital = GetHospital(hospitalName);
            var patients = hospital.GetHospitalizedPatients(from, to);
            foreach (var patient in patients)
                yield return patient.ToArray();
        }

        public IEnumerable<string[][]> GetAccountantReport(string hospitalName, DateTime month)
        {
            var hospital = GetHospital(hospitalName);
            return hospital.GetAccountantReport(month);
        }

        public IEnumerable<string[]> GetHospitalizedPatients(string hospitalName)
        {
            var hospital = GetHospital(hospitalName);
            var patients = hospital.GetHospitalizedPatients();
            foreach (var patient in patients)
                yield return patient.ToArray();
        }

        public IEnumerable<string[]> GetHospitalizedPatients(string hospitalName, string agencyCode)
        {
            var hospital = GetHospital(hospitalName);
            IEnumerable<Patient> patients;
            try
            {
                patients = hospital.GetHospitalizedPatients(agencyCode);
            }
            catch (Exception)
            {
                throw new Exception($"Hospital {hospitalName} doesn't have hospitalized patients from agency {agencyCode}.");
            }
            foreach (var patient in patients)
                yield return patient.ToArray();
        }

        public IEnumerable<string> GetHospitals()
        {
            var hospitals = _hospitals.InOrder();
            foreach (var hospital in hospitals)
                yield return hospital.ToString();
        }

        public bool RemoveHospital(string oldHospital, string newHospital)
        {
            var remove = GetHospital(oldHospital);
            var target = GetHospital(newHospital);
            target.CopyData(remove);
            _hospitals.Remove(oldHospital);
            return true;
        }

        public IEnumerable<string[]> GetPatients()
        {
            var patients = _patients.InOrder();
            foreach (var patient in patients)
                yield return patient.ToArray();
        }

        public IEnumerable<string[]> GetHospitalizations(int personalNumber)
        {
            var patient = GetPatient(personalNumber);
            var hospitalizations = patient.Hospitalizations;
            foreach (var hospitalization in hospitalizations)
                yield return hospitalization.ToArray();
        }

        public void Generate(int hospitals, int patients, int records, int ongoingRecords)
        {
            _patients.Clear();
            _hospitals.Clear();
            var dataGenerator = new DataGenerator(hospitals, patients, records, ongoingRecords, this);
            dataGenerator.Generate();
        }

        public void Save() => _dataSaver.Save();

        public void Load() => _dataSaver.Load();

        internal void LoadHospital(Hospital hospital) => _hospitals.Insert(hospital.Name, hospital);

        internal void LoadPatient(Patient patient) => _patients.Insert(patient.PersonalNumber, patient);

        internal void LoadHospitalization(Hospitalization hospitalization)
        {
            var patient = GetPatient(hospitalization.PersonalNumber);
            var modHospitalization = patient.LoadHospitalization(hospitalization);
            var hospital = GetHospital(hospitalization.Hospital);
            hospital.LoadHospitalization(modHospitalization);
        }

        private Hospital GetHospital(string name) => _hospitals.Find(name);

        private Patient GetPatient(int personalNumber) => _patients.Find(personalNumber);

        private void OnSysDateChange(DateTime oldDate, DateTime newDate)
        {
            var hospitals = _hospitals.InOrder();
            foreach (var hospital in hospitals)
                hospital.OnSysDateChange(oldDate, newDate);
        }
    }
}
