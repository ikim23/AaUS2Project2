using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PersonalHealthRecord.Model;

namespace PersonalHealthRecord.Generator
{
    public class DataGenerator
    {
        private readonly int _patientCount;
        private readonly int _recordCount;
        private readonly int _ongoingRecordCount;
        private readonly ISystem _system;
        private readonly RandomValue _randVal = RandomValue.Instance;

        public DataGenerator(int patientCount, int recordCount, int ongoingRecordCount, ISystem system)
        {
            _patientCount = patientCount;
            _recordCount = recordCount;
            _ongoingRecordCount = ongoingRecordCount;
            _system = system;
        }

        public void Generate()
        {
            var patients = GeneratePatients();
            GenerateHospitalizations(patients);
            _system.LoadPatients(patients);
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
                var hospitalization = Mapper.ToHospitalization(startDate, startDate.AddDays(1), "Lorem ipsum dolor sit amet, consectetur adipiscing elit.");
                cardIds.MoveNext();
                var patient = patients[cardIds.Current];
                patient.Hospitalizations.Add(hospitalization);
                startDate = startDate.AddDays(1);
            }
            for (var i = 0; i < _ongoingRecordCount; i++)
            {
                var hospitalization = Mapper.ToHospitalization(startDate, null, "Lorem ipsum dolor sit amet, consectetur adipiscing elit.");
                cardIds.MoveNext();
                var patient = patients[cardIds.Current];
                patient.Hospitalizations.Add(hospitalization);
                startDate = startDate.AddDays(1);
            }
        }
    }

    internal class ArrayEnumerator<T> : IEnumerator<T>
    {
        public int Index;
        public T[] Array;
        public T Current => Array[Index];

        public ArrayEnumerator(T[] array)
        {
            Index = 0;
            Array = array;
        }

        public bool MoveNext()
        {
            Index++;
            if (Index >= Array.Length) Index = 0;
            return true;
        }

        public void Dispose()
        {
        }

        public void Reset() => Index = 0;

        object IEnumerator.Current => Current;
    }
}
