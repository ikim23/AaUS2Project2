using System;
using System.Collections.Generic;
using DataStructures;

namespace PersonalHealthRecord.Model
{
    internal class Hospital
    {
        public string Name { get; set; }

        private readonly RbTree<int, List<Hospitalization>> _personalNumberHospitalizations;
        private readonly RbTree<string, List<Hospitalization>> _nameHospitalizations;
        private readonly RbTree<string, RbTree<int, Patient>> _agencyCodeHospitalizedPatients;
        private readonly RbTree<DateTime, Hospitalization> _startDateHospitalizations;
        private readonly RbTree<DateTime, Hospitalization> _endDateHospitalizations;
        private readonly RbTree<DateTime, Hospitalization> _actualHospitalizations;
        private readonly RbTree<string, RbTree<string, RbTree<int, List<Hospitalization>>>> _accountantReports;

        public Hospital()
        {
            _personalNumberHospitalizations = new RbTree<int, List<Hospitalization>>();
            _nameHospitalizations = new RbTree<string, List<Hospitalization>>();
            _agencyCodeHospitalizedPatients = new RbTree<string, RbTree<int, Patient>>();
            _startDateHospitalizations = new RbTree<DateTime, Hospitalization>();
            _endDateHospitalizations = new RbTree<DateTime, Hospitalization>();
            _actualHospitalizations = new RbTree<DateTime, Hospitalization>();
            _accountantReports = new RbTree<string, RbTree<string, RbTree<int, List<Hospitalization>>>>();
        }

        public Hospital(string name) : this()
        {
            Name = name;
        }

        public IEnumerable<Hospitalization> GetHospitalizations(int personalNumber)
        {
            var hospitalizations = _personalNumberHospitalizations.Find(personalNumber);
            return hospitalizations;
        }

        internal IEnumerable<Hospitalization> GetHospitalizations(string name, string surname)
        {
            var key = $"{name} {surname}";
            var hospitalizations = _nameHospitalizations.Find(key);
            return hospitalizations;
        }

        public IEnumerable<Patient> GetHospitalizedPatients(DateTime start, DateTime end)
        {
            var tree = new RbTree<int, Patient>();
            var startDate = _startDateHospitalizations.GetInterval(start, end);
            DateTime? firstDate = null;
            foreach (var hospitalization in startDate)
            {
                if (firstDate == null && hospitalization.End != null) firstDate = hospitalization.Start;
                var patient = hospitalization.Patient;
                tree.GetOrInsert(patient.PersonalNumber, patient);
            }
            var endDate = _endDateHospitalizations.GetInterval(start, end);
            foreach (var hospitalization in endDate)
            {
                if (hospitalization.Start == firstDate) break;
                var patient = hospitalization.Patient;
                tree.GetOrInsert(patient.PersonalNumber, patient);
            }
            var actual = _actualHospitalizations.GetInterval(DateTime.MinValue, start);
            foreach (var hospitalization in actual)
            {
                var patient = hospitalization.Patient;
                tree.GetOrInsert(patient.PersonalNumber, patient);
            }
            return tree.InOrder();
        }

        public void StartHospitalization(Hospitalization hospitalization)
        {
            hospitalization.Hospital = Name;
            var patient = hospitalization.Patient;
            var listPersonalNumberHospitalizations = _personalNumberHospitalizations.GetOrInsert(patient.PersonalNumber, new List<Hospitalization>());
            listPersonalNumberHospitalizations.Add(hospitalization);
            var listNameHospitalizations = _nameHospitalizations.GetOrInsert(patient.FullName, new List<Hospitalization>());
            listNameHospitalizations.Add(hospitalization);
            var hospitalizedPatients = _agencyCodeHospitalizedPatients.GetOrInsert(patient.AgencyCode, new RbTree<int, Patient>());
            hospitalizedPatients.Insert(patient.PersonalNumber, patient);
            _startDateHospitalizations.Insert(hospitalization.Start, hospitalization);
            _actualHospitalizations.Insert(hospitalization.Start, hospitalization);
            AddAccountantRecords(hospitalization);
        }

        public void EndHospitalization(Hospitalization hospitalization)
        {
            var patient = hospitalization.Patient;
            var hospitalizedPatients = _agencyCodeHospitalizedPatients.Find(patient.AgencyCode);
            hospitalizedPatients.Remove(patient.PersonalNumber);
            _endDateHospitalizations.Insert(hospitalization.Start, hospitalization);
            _actualHospitalizations.Remove(hospitalization.Start);
            AddAccountantRecords(hospitalization, hospitalization.End);
        }

        public void LoadHospitalization(Hospitalization hospitalization)
        {
            var patient = hospitalization.Patient;
            var listPersonalNumberHospitalizations = _personalNumberHospitalizations.GetOrInsert(patient.PersonalNumber, new List<Hospitalization>());
            listPersonalNumberHospitalizations.Add(hospitalization);
            var listNameHospitalizations = _nameHospitalizations.GetOrInsert(patient.FullName, new List<Hospitalization>());
            listNameHospitalizations.Add(hospitalization);
            _startDateHospitalizations.Insert(hospitalization.Start, hospitalization);
            if (hospitalization.End == null)
            {
                var hospitalizedPatients = _agencyCodeHospitalizedPatients.GetOrInsert(patient.AgencyCode, new RbTree<int, Patient>());
                hospitalizedPatients.Insert(patient.PersonalNumber, patient);
                _actualHospitalizations.Insert(hospitalization.Start, hospitalization);
            }
            else
            {
                _endDateHospitalizations.Insert(hospitalization.End ?? DateTime.MinValue, hospitalization);
            }
            AddAccountantRecords(hospitalization);
        }

        public IEnumerable<Patient> GetHospitalizedPatients()
        {
            var agencies = _agencyCodeHospitalizedPatients.InOrder();
            foreach (var agency in agencies)
                foreach (var patient in agency.InOrder())
                    yield return patient;
        }

        public IEnumerable<Patient> GetHospitalizedPatients(string agencyCode)
        {
            RbTree<int, Patient> agency;
            try
            {
                agency = _agencyCodeHospitalizedPatients.Find(agencyCode);
            }
            catch (Exception)
            {
                throw new Exception($"Hospital doesn't have hospitalized patients from agency {agencyCode}.");
            }
            foreach (var patient in agency.InOrder())
                yield return patient;
        }

        public override string ToString() => Name;

        public void CopyData(Hospital hospital)
        {
            // Hospitalizations by personal number
            var pnHospitalizations = hospital._personalNumberHospitalizations.InOrderTuple();
            foreach (var tuple in pnHospitalizations)
            {
                var personalNumber = tuple.Item1;
                var newHospitalizations = tuple.Item2;
                var hospitalizations = _personalNumberHospitalizations.GetOrInsert(personalNumber, new List<Hospitalization>());
                foreach (var hospitalization in newHospitalizations)
                {
                    hospitalization.Hospital = Name;
                    hospitalizations.Add(hospitalization);
                }
            }
            // Hospitalizations by name
            var nameHospitalizations = hospital._nameHospitalizations.InOrderTuple();
            foreach (var tuple in nameHospitalizations)
            {
                var name = tuple.Item1;
                var newHospitalizations = tuple.Item2;
                var hospitalizations = _nameHospitalizations.GetOrInsert(name, new List<Hospitalization>());
                foreach (var hospitalization in newHospitalizations)
                {
                    hospitalization.Hospital = Name;
                    hospitalizations.Add(hospitalization);
                }
            }
            // Actually hospitalized patients
            var agencyCodeHospitalizedPatients = hospital._agencyCodeHospitalizedPatients.InOrderTuple();
            foreach (var tuple in agencyCodeHospitalizedPatients)
            {
                var agencyCode = tuple.Item1;
                var pnPatients = tuple.Item2.InOrderTuple();
                var pnHospitalizedPatients = _agencyCodeHospitalizedPatients.GetOrInsert(agencyCode, new RbTree<int, Patient>());
                foreach (var pnPatientTuple in pnPatients)
                {
                    var personalNumber = pnPatientTuple.Item1;
                    var patient = pnPatientTuple.Item2;
                    pnHospitalizedPatients.Insert(personalNumber, patient);
                }
            }
            // Hospitalizations by start date
            var startDateHospitalizations = hospital._startDateHospitalizations.InOrder();
            foreach (var hospitalization in startDateHospitalizations)
            {
                hospitalization.Hospital = Name;
                _startDateHospitalizations.Insert(hospitalization.Start, hospitalization);
            }
            // Hospitalizations by start date [ongoing]
            var actualHospitalizations = hospital._actualHospitalizations.InOrder();
            foreach (var hospitalization in actualHospitalizations)
            {
                hospitalization.Hospital = Name;
                _actualHospitalizations.Insert(hospitalization.Start, hospitalization);
            }
            // Hospitalizations by end date
            var endDateHospitalizations = hospital._endDateHospitalizations.InOrder();
            foreach (var hospitalization in endDateHospitalizations)
            {
                hospitalization.Hospital = Name;
                _endDateHospitalizations.Insert(hospitalization.End ?? DateTime.MinValue, hospitalization);
            }
            // Accountant data
            var monthAgency = hospital._accountantReports.InOrderTuple();
            foreach (var monthAgencyTuple in monthAgency)
            {
                var month = monthAgencyTuple.Item1;
                var agencyDayHospitalizations = monthAgencyTuple.Item2.InOrderTuple();
                foreach (var agencyDay in agencyDayHospitalizations)
                {
                    var agency = agencyDay.Item1;
                    var days = agencyDay.Item2.InOrderTuple();
                    foreach (var tuple in days)
                    {
                        var day = tuple.Item1;
                        var hospitalizations = tuple.Item2;

                        var monthData = _accountantReports.GetOrInsert(month, new RbTree<string, RbTree<int, List<Hospitalization>>>());
                        var agencyData = monthData.GetOrInsert(agency, new RbTree<int, List<Hospitalization>>());
                        var dayData = agencyData.GetOrInsert(day, new List<Hospitalization>());
                        dayData.AddRange(hospitalizations);
                    }
                }

            }
        }

        public IEnumerable<string[][]> GetAccountantReport(DateTime month)
        {
            var monthKey = $"{month:yyyyMM}";
            var agencyRecords = _accountantReports.GetOrInsert(monthKey, new RbTree<string, RbTree<int, List<Hospitalization>>>());
            foreach (var tupleAgencyRecords in agencyRecords.InOrderTuple())
            {
                var agency = tupleAgencyRecords.Item1;
                var records = tupleAgencyRecords.Item2;
                var data = new List<string[]>();
                foreach (var tupleDayHospitalizations in records.InOrderTuple())
                {
                    var day = tupleDayHospitalizations.Item1;
                    var hospitalizations = tupleDayHospitalizations.Item2;
                    foreach (var hospitalization in hospitalizations)
                    {
                        var item = new[]
                        {
                            agency,
                            new DateTime(month.Year, month.Month, day).ToString(),
                            hospitalization.Patient.Name,
                            hospitalization.Patient.Surname,
                            hospitalization.Diagnosis
                        };
                        data.Add(item);
                    }
                }
                yield return data.ToArray();
            }
        }

        public void OnSysDateChange(DateTime oldDate, DateTime newDate)
        {
            var patients = GetHospitalizedPatients();
            foreach (var patient in patients)
            {
                var actualHospitalization = patient.Hospitalizations.Last.Value;
                AddAccountantRecords(actualHospitalization, oldDate.AddDays(1));
            }
        }

        private void AddAccountantRecords(Hospitalization hospitalization, DateTime? fromDate = null)
        {
            var date = fromDate ?? hospitalization.Start;
            var endDate = hospitalization.End ?? System.Instance.SysDate;
            var agencyCode = hospitalization.Patient.AgencyCode;
            var prevMonth = string.Empty;
            var agencyReports = new RbTree<int, List<Hospitalization>>();
            while (date <= endDate)
            {
                var month = $"{date:yyyyMM}";
                if (month != prevMonth)
                {
                    var monthReports = _accountantReports.GetOrInsert(month, new RbTree<string, RbTree<int, List<Hospitalization>>>());
                    agencyReports = monthReports.GetOrInsert(agencyCode, new RbTree<int, List<Hospitalization>>());
                }

                var dayReports = agencyReports.GetOrInsert(date.Day, new List<Hospitalization>());
                dayReports.Add(hospitalization);

                date = date.AddDays(1);
                prevMonth = month;
            }
        }
    }
}
