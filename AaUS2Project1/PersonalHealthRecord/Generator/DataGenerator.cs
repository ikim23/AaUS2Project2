using System;
using System.Collections.Generic;
using PersonalHealthRecord.Model;

namespace PersonalHealthRecord.Generator
{
    public class DataGenerator
    {
        private readonly int _hospitals;
        private readonly int _patients;
        private readonly int _records;
        private readonly int _ongoingRecords;
        private readonly System _system;
        private readonly RandomValue _randVal = RandomValue.Instance;

        public DataGenerator(int hospitals, int patients, int records, int ongoingRecords, System system)
        {
            _hospitals = hospitals;
            _patients = patients;
            _records = records;
            _ongoingRecords = ongoingRecords;
            _system = system;
        }

        public void Generate()
        {
            var hospitals = GenerateHospitals();
            GeneratePatients();
            GenerateHospitalizations(hospitals);
        }

        private IEnumerable<string> GenerateHospitals()
        {
            var cities = _randVal.City;
            Func<int, string> suffixCreator;
            if (cities.Length > _hospitals) suffixCreator = idx => "";
            else suffixCreator = idx => idx.ToString();
            var hospitals = new LinkedList<string>();
            int cityIdx = 0, id = 1;
            for (var i = 0; i < _hospitals; i++)
            {
                if (cityIdx >= cities.Length)
                {
                    cityIdx = 0;
                    id++;
                }
                var hospital = new Hospital($"{cities[cityIdx++]}{suffixCreator.Invoke(id)}");

                hospitals.AddLast(hospital.Name);
                _system.LoadHospital(hospital);
            }
            return hospitals;
        }

        private void GeneratePatients()
        {
            var firstNames = _randVal.FirstName;
            var lastNames = _randVal.LastName;
            var agencies = _randVal.InsuranceAgencyCode;
            var first = 0;
            var last = 0;
            var agency = 0;
            for (var i = 0; i < _patients; i++)
            {
                if (first >= firstNames.Length) first = 0;
                if (last >= lastNames.Length) last = 0;
                if (agency >= agencies.Length) agency = 0;

                var patient = new Patient
                {
                    Name = firstNames[first++],
                    Surname = lastNames[last++],
                    Birthday = _system.SysDate.AddDays(-i),
                    PersonalNumber = i + 1,
                    AgencyCode = agencies[agency++]
                };

                _system.LoadPatient(patient);
            }
        }

        private void GenerateHospitalizations(IEnumerable<string> hospitals)
        {
            var enHospital = hospitals.GetEnumerator();
            var personalNumber = 1;
            var random = new Random();
            var dayShift = 7;
            var startDate = _system.SysDate.AddDays(-(((_records + _ongoingRecords) / _patients + 1) * dayShift));
            for (var i = 0; i < _records; i++)
            {
                if (!enHospital.MoveNext())
                {
                    enHospital.Reset();
                    enHospital.MoveNext();
                }
                if (personalNumber++ >= _patients)
                {
                    personalNumber = 1;
                    startDate = startDate.AddDays(dayShift);
                }
                startDate = startDate.AddMilliseconds(1);
                var hospitalization = new Hospitalization
                {
                    Hospital = enHospital.Current,
                    PersonalNumber = personalNumber,
                    Start = startDate,
                    End = startDate.AddDays(random.Next(1, dayShift)),
                    Diagnosis = "Lorem ipsum dolor sit amet, consectetur adipiscing elit."
                };
                _system.LoadHospitalization(hospitalization);
            }
            personalNumber = 1;
            for (var i = 0; i < _ongoingRecords; i++)
            {
                if (!enHospital.MoveNext())
                {
                    enHospital.Reset();
                    enHospital.MoveNext();
                }
                if (personalNumber++ >= _patients)
                {
                    personalNumber = 1;
                    startDate = startDate.AddDays(dayShift);
                }
                startDate = startDate.AddMilliseconds(1);
                var hospitalization = new Hospitalization
                {
                    Hospital = enHospital.Current,
                    PersonalNumber = personalNumber,
                    Start = startDate,
                    Diagnosis = "Lorem ipsum dolor sit amet, consectetur adipiscing elit."
                };
                _system.LoadHospitalization(hospitalization);
            }
        }
    }
}
