using System;
using System.Collections.Generic;
using PersonalHealthRecord.Model;

namespace PersonalHealthRecord
{
    public interface ISystem
    {
        // 1. vyhladanie zaznamov pacienta (identifikovany cislom preukazu)
        IEnumerable<string[]> GetHospitalizations(int cardId);
        // 2. vykonanie zaznamu o zaciatku hospitalizacie pacienta (identifikovany cislom preukazu)
        bool StartHospitalization(int cardId);
        // 3. vykonanie zaznamu o ukonceni hospitalizacie pacienta (identifikovany cislom preukazu)
        bool EndHospitalization(int cardId);
        // 4. vypis pacientov, ktorych cisla preukazov su zo zadaneho intervalu
        IEnumerable<string[]> GetPatients(int cardIdFrom, int cardIdTo);
        // 5. editacia osobnych udajov pacienta (identifikovany cislom preukazu)
        bool UpdatePatient(int cardId, string firstName, string lastName, DateTime birthday);
        // 6. pridanie pacienta
        bool AddPatient(string firstName, string lastName, DateTime birthday, int cardId);
        // 7. odstranenie pacienta (identifikovany cislom preukazu)
        bool RemovePatient(int cardId);

        IEnumerable<string[]> GetPatients();
        void Generate(int patients, int records, int ongoingRecords);
        void LoadPatients(IEnumerable<Patient> patients);
    }
}
