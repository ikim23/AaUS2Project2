using System;
using System.Collections.Generic;
using System.Linq;
using PersonalHealthRecord.Model;

namespace PersonalHealthRecord.Generator
{
    internal class DataGenerator
    {
        private readonly int _patientCount;
        private readonly int _recordCount;
        private readonly int _ongoingRecordCount;
        private readonly RandomValue _randVal = RandomValue.Instance;

        public DataGenerator(int patientCount, int recordCount, int ongoingRecordCount)
        {
            _patientCount = patientCount;
            _recordCount = recordCount;
            _ongoingRecordCount = ongoingRecordCount;
        }

        public List<Patient> Generate()
        {
            var patients = GeneratePatients();
            GenerateHospitalizations(patients);
            return patients;
        }

        private List<Patient> GeneratePatients()
        {
            var firstNames = new ArrayEnumerator<string>(_randVal.FirstName);
            var lastNames = new ArrayEnumerator<string>(_randVal.LastName);
            var patients = new List<Patient>(_patientCount);
            for (var cardId = 1; cardId <= _patientCount; cardId++)
            {
                firstNames.MoveNext();
                lastNames.MoveNext();
                var patient = Mapper.ToPatient(firstNames.Current, lastNames.Current, _randVal.GetBirthday(), cardId);
                patients.Add(patient);
            }
            return patients;
        }

        private void GenerateHospitalizations(List<Patient> patients)
        {
            var cardIds = new ArrayEnumerator<int>(Enumerable.Range(0, _patientCount).ToArray());
            var startDate = DateTime.Today.AddDays(-(_recordCount + _ongoingRecordCount + 7));
            for (var i = 0; i < _recordCount; i++)
            {
                var hospitalization = Mapper.ToHospitalization(startDate, startDate.AddDays(1), "Lorem ipsum dolor sit amet consectetur.");
                cardIds.MoveNext();
                var patient = patients[cardIds.Current];
                patient.Hospitalizations.Add(hospitalization);
                startDate = startDate.AddDays(1);
            }
            for (var i = 0; i < _ongoingRecordCount; i++)
            {
                var hospitalization = Mapper.ToHospitalization(startDate, null, "Lorem ipsum dolor sit amet consectetur.");
                cardIds.MoveNext();
                var patient = patients[cardIds.Current];
                patient.Hospitalizations.Add(hospitalization);
                startDate = startDate.AddDays(1);
            }
        }
    }
}
