using System;
using System.Collections.Generic;
using System.Linq;
using PersonalHealthRecord;
using PersonalHealthRecordUI.Models;

namespace PersonalHealthRecordUI.Logic
{
    public class Api
    {
        public static Api Instance => _instance ?? (_instance = new Api());
        private static Api _instance;

        private readonly ISystem _sys = new PersonalHealthRecord.System();

        private Api()
        {
        }

        public IEnumerable<PatientModel> GetPatients()
        {
            return _sys.GetPatients().Select(Mapper.ToPatientModel);
        }

        public IEnumerable<PatientModel> GetPatients(int cardIdFrom, int cardIdTo)
        {
            return _sys.GetPatients(cardIdFrom, cardIdTo).Select(Mapper.ToPatientModel);
        }

        public void AddPatient(PatientModel patient)
        {
            _sys.AddPatient(patient.FirstName, patient.LastName, patient.Birthday, patient.CardId);
        }

        public void UpdatePatient(PatientModel patient)
        {
            _sys.UpdatePatient(patient.CardId, patient.FirstName, patient.LastName, patient.Birthday);
        }

        public IEnumerable<HospitalizationModel> GetHospitalizations(int cardId)
        {
            return _sys.GetHospitalizations(cardId).Select(Mapper.ToHospitalizationModel);
        }

        public void StartHospitalization(int cardId, DateTime start, string diagnosis)
        {
            _sys.StartHospitalization(cardId, start, diagnosis);
        }

        public void EndHospitalization(int cardId, DateTime end)
        {
            _sys.EndHospitalization(cardId, end);
        }

        public void Generate(int patientsCount, int recordsCount, int ongoingRecordsCount)
        {
            _sys.Generate(patientsCount, recordsCount, ongoingRecordsCount);
        }
    }
}
