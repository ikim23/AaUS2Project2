using System;
using System.Collections.Generic;
using System.Linq;
using BPlusTree.DataStructures;
using BPlusTree.Writables;
using PersonalHealthRecord.Model;

namespace PersonalHealthRecord
{
    public class System : ISystem
    {
        public static string FormatDate(DateTime date) => $"{date:dd.MM.yyyy}";
        private readonly BPlusTree<WritableInt, Patient> _patients = new BPlusTree<WritableInt, Patient>(5);

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

        public bool StartHospitalization(int cardId)
        {
            throw new NotImplementedException();
        }

        public bool EndHospitalization(int cardId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string[]> GetPatients(int cardIdFrom, int cardIdTo)
        {
            throw new NotImplementedException();
        }

        public bool UpdatePatient(int cardId, string firstName, string lastName, DateTime birthday)
        {
            throw new NotImplementedException();
        }

        public bool AddPatient(string firstName, string lastName, DateTime birthday, int cardId)
        {
            var patient = Mapper.ToPatient(firstName, lastName, birthday, cardId);
            _patients.Insert(new WritableInt(cardId), patient);
            return true;
        }

        public bool RemovePatient(int cardId)
        {
            _patients.Remove(new WritableInt(cardId));
            return false;
        }

        public void Generate(int patients, int records, int ongoingRecords)
        {
            throw new NotImplementedException();
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
