using System.Collections.Generic;
using System.IO;
using PersonalHealthRecord.Model;
using ServiceStack.Text;

namespace PersonalHealthRecord
{
    internal class DataSaver
    {
        private static readonly string FileHospitals = "hospitals.csv";
        private static readonly string FilePatients = "patients.csv";
        private static readonly string FileHospitalizations = "hospitalizations.csv";

        private readonly System _system;
        private bool _loaded;

        public DataSaver(System system)
        {
            _system = system;
        }

        public void Save()
        {
            SaveHospitals();
            SavePatients();
            SaveHospitalizations();
        }

        public void Load()
        {
            if (_loaded) return;
            _loaded = true;
            LoadHospitals();
            LoadPatients();
            LoadHospitalizations();
        }

        private void SaveHospitals()
        {
            var stream = new FileStream(FileHospitals, FileMode.Create);
            var hospitals = _system.Hospitals;
            CsvSerializer.SerializeToStream(hospitals, stream);
        }

        private void SavePatients()
        {
            var stream = new FileStream(FilePatients, FileMode.Create);
            var patients = _system.Patients;
            CsvSerializer.SerializeToStream(patients, stream);
        }

        private void SaveHospitalizations()
        {
            var stream = new FileStream(FileHospitalizations, FileMode.Create);
            var hospitalizations = GetAllHospitalizations();
            CsvSerializer.SerializeToStream(hospitalizations, stream);
        }

        private IEnumerable<Hospitalization> GetAllHospitalizations()
        {
            var patients = _system.Patients;
            foreach (var patient in patients)
            {
                var hospitalizations = patient.Hospitalizations;
                foreach (var hospitalization in hospitalizations)
                    yield return hospitalization;
            }
        }

        private void LoadHospitals()
        {
            if (!File.Exists(FileHospitals)) return;
            var stream = new FileStream(FileHospitals, FileMode.Open);
            var hospitals = CsvSerializer.DeserializeFromStream<IEnumerable<Hospital>>(stream);
            foreach (var hospital in hospitals)
                _system.LoadHospital(hospital);
        }

        private void LoadPatients()
        {
            if (!File.Exists(FilePatients)) return;
            var stream = new FileStream(FilePatients, FileMode.Open);
            var patients = CsvSerializer.DeserializeFromStream<IEnumerable<Patient>>(stream);
            foreach (var patient in patients)
                _system.LoadPatient(patient);
        }

        private void LoadHospitalizations()
        {
            if (!File.Exists(FileHospitalizations)) return;
            var stream = new FileStream(FileHospitalizations, FileMode.OpenOrCreate);
            var hospitalizations = CsvSerializer.DeserializeFromStream<IEnumerable<Hospitalization>>(stream);
            foreach (var hospitalization in hospitalizations)
                _system.LoadHospitalization(hospitalization);
        }
    }
}
