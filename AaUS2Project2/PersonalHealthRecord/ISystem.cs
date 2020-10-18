using System;
using System.Collections.Generic;

namespace PersonalHealthRecord
{
    public interface ISystem
    {
        // 1. vyhladanie zaznamov pacienta (identifikovany cislom preukazu)
        IEnumerable<string[]> GetHospitalizations(int cardId);
        // 2. vykonanie zaznamu o zaciatku hospitalizacie pacienta (identifikovany cislom preukazu)
        void StartHospitalization(int cardId, DateTime start, string diagnosis);
        // 3. vykonanie zaznamu o ukonceni hospitalizacie pacienta (identifikovany cislom preukazu)
        void EndHospitalization(int cardId, DateTime end);
        // 4. vypis pacientov, ktorych cisla preukazov su zo zadaneho intervalu
        IEnumerable<string[]> GetPatients(int cardIdFrom, int cardIdTo);
        // 5. editacia osobnych udajov pacienta (identifikovany cislom preukazu)
        void UpdatePatient(int cardId, string firstName, string lastName, DateTime birthday);
        void UpdatePatientWithId(int oldCardId, int cardId, string firstName, string lastName, DateTime birthday);
        // 6. pridanie pacienta
        void AddPatient(string firstName, string lastName, DateTime birthday, int cardId);
        // 7. odstranenie pacienta (identifikovany cislom preukazu)
        void RemovePatient(int cardId);

        IEnumerable<string[]> GetPatients();
        void Generate(int blockSize, int patientCount, int recordCount, int ongoingRecordCount);
    }
}
