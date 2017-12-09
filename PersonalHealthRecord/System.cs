using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BPlusTree.DataStructures;
using BPlusTree.Writables;
using PersonalHealthRecord.Generator;
using PersonalHealthRecord.Model;

namespace PersonalHealthRecord
{
    public class System : ISystem
    {
        public static readonly string FilePath = "bptree.bin";
        private BPlusTree<WritableInt, Patient> _patients = new BPlusTree<WritableInt, Patient>(5, FilePath);

        public IEnumerable<string[]> GetPatients()
        {
            return _patients.InOrder().Select(Mapper.FromPatient);
        }

        public IEnumerable<string[]> GetHospitalizations(int cardId)
        {
            var patient = _patients.Find(new WritableInt(cardId));
            var hospitalizations = patient.Hospitalizations;
            return hospitalizations.Select(Mapper.FromHospitalization);
        }

        public void StartHospitalization(int cardId, DateTime start, string diagnosis)
        {
            var patient = _patients.Find(new WritableInt(cardId));
            var hospitalization = new Hospitalization
            {
                Start = start,
                Diagnosis = diagnosis
            };
            patient.Hospitalizations.Add(hospitalization);
            _patients.Update(new WritableInt(patient.CardId), patient);
        }

        public void EndHospitalization(int cardId, DateTime end)
        {
            var patient = _patients.Find(new WritableInt(cardId));
            var hospitalization = patient.Hospitalizations.Last();
            hospitalization.End = end;
            _patients.Update(new WritableInt(patient.CardId), patient);
        }

        public IEnumerable<string[]> GetPatients(int cardIdFrom, int cardIdTo)
        {
            return _patients
                .GetInterval(new WritableInt(cardIdFrom), new WritableInt(cardIdTo))
                .Select(Mapper.FromPatient);
        }

        public void UpdatePatient(int cardId, string firstName, string lastName, DateTime birthday)
        {
            var patient = _patients.Find(new WritableInt(cardId));
            patient.FirstName = firstName;
            patient.LastName = lastName;
            patient.Birthday = birthday;
            _patients.Update(new WritableInt(patient.CardId), patient);
        }

        public void AddPatient(string firstName, string lastName, DateTime birthday, int cardId)
        {
            var patient = Mapper.ToPatient(firstName, lastName, birthday, cardId);
            _patients.Insert(new WritableInt(cardId), patient);
        }

        public void RemovePatient(int cardId)
        {
            _patients.Remove(new WritableInt(cardId));
        }

        public void Generate(int patients, int records, int ongoingRecords)
        {
            _patients.Dispose();
            File.Delete(FilePath);
            _patients = new BPlusTree<WritableInt, Patient>(5, FilePath);
            var generator = new DataGenerator(patients, records, ongoingRecords, this);
            generator.Generate();
        }

        public void LoadPatients(IEnumerable<Patient> patients)
        {
            foreach (var patient in patients)
            {
                _patients.Insert(new WritableInt(patient.CardId), patient);
            }
        }
    }
}
