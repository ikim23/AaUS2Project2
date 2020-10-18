using System;
using System.Collections.Generic;
using PersonalHealthRecordUI.Models;

namespace PersonalHealthRecordUI.Logic
{
    public interface IApi
    {
        // 11. pridanie nemocnice
        bool AddHospital(string name);
        // 6. pridanie pacienta
        bool AddPatient(PatientModel patient);
        // 3. vykonanie zaznamu o zaciatku hospitalizacie pacienta (identifikovany svojim rodnym cislom) v nemocnici (identifikovana svojim nazvom)
        bool StartHospitalization(string hospitalName, int personalNumber, string diagnosis);
        // 4. vykonanie zaznamu o ukonceni hospitalizacie pacienta (identifikovany svojim rodnym cislom) v nemocnici (identifikovana svojim nazvom)
        bool EndHospitalization(string hospitalName, int personalNumber);
        // 1. vyhladanie zaznamov pacienta (identifikovany svjim rodnym cislom) v zadanej nemocnici (identifikovana svojim nazvom)
        IEnumerable<HospitalizationModel> GetHospitalizations(string hospitalName, int personalNumber);
        // 2. vyhladanie zaznamov pacienta/ov v zadanej nemocnici (identifikovana svojim nazvom) podla mena a priezviska
        IEnumerable<HospitalizationModel> GetHospitalizations(string hospitalName, string name, string surname);
        // 5. vypis hospitalizovanych pacientov v nemocnici (identifikovana svojim nazvom) v sadanom casovom obdobi (od, do)
        IEnumerable<PatientModel> GetPatients(string hospitalName, DateTime from, DateTime to);
        // 7. vytvorenie podkladov pre uctovne oddelenie na tvorbu faktur
        IEnumerable<AgencyReportModel> GetAccountantReport(string hospitalName, DateTime month);
        // 8. vypis aktualne hospitalizovanych pacientov v nemocnici (identifikovana svojim nazvom)
        IEnumerable<PatientModel> GetHospitalizedPatients(string hospitalName);
        // 9./10. vypis aktualne hospitalizovanych pacientov v nemocnici (identifikovana svojim nazvom) zotriedenych podla rodnych cisel, ktori su poistencami zadanej zdravotnej poistovne (identifikovana svojim kodom)
        IEnumerable<PatientModel> GetHospitalizedPatients(string hospitalName, string agencyCode);
        // 12. vypis nemocnic usporiadanych podla nazvov
        IEnumerable<HospitalModel> GetHospitals();
        // 13. zrusenie nemocnice (cela agenda sa presunie do inej nemocnice, ktoru secifikuje pouzivatel (identifikovana svojim nazvom), vratane pacientov a historickych zaznamov)
        bool RemoveHospital(string oldHospital, string newHospital);

        DateTime SysDate { get; set; }
        IEnumerable<PatientModel> GetPatients();
        IEnumerable<HospitalizationModel> GetHospitalizations(int personalNumber);
        void Generate(int hospitals, int patients, int records, int ongoingRecords);
        void Load();
        void Save();
    }
}
