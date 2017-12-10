﻿using System;
using System.Windows;
using Caliburn.Micro;
using PersonalHealthRecord.Generator;
using PersonalHealthRecordUI.Models;

namespace PersonalHealthRecordUI.ViewModels
{
    public class PatientViewModel : Screen
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public int CardId { get; set; }
        public bool NewPatient { get; }
        private readonly IOnPatientViewListener _listener;

        public PatientViewModel(IOnPatientViewListener listener)
        {
            NewPatient = true;
            _listener = listener;
            var rand = RandomValue.Instance;
            FirstName = rand.GetFirstName();
            LastName = rand.GetLastName();
            Birthday = rand.GetBirthday();
        }

        public PatientViewModel(IOnPatientViewListener listener, PatientModel patient)
        {
            NewPatient = false;
            _listener = listener;
            FirstName = patient.FirstName;
            LastName = patient.LastName;
            Birthday = patient.Birthday;
            CardId = patient.CardId;
        }

        public bool CanOk(string firstName, string lastName, DateTime birthday, int cardId)
        {
            return !string.IsNullOrWhiteSpace(firstName) &&
                   !string.IsNullOrWhiteSpace(lastName) &&
                   cardId > 0 &&
                   birthday < DateTime.Now;
        }

        public void Ok(string firstName, string lastName, DateTime birthday, int cardId)
        {
            try
            {
                var patient = new PatientModel
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Birthday = birthday,
                    CardId = cardId
                };
                if (NewPatient) _listener.OnPatientAdd(patient);
                else _listener.OnPatientEdit(patient);
                TryClose();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void Cancel()
        {
            TryClose();
        }
    }

    public interface IOnPatientViewListener
    {
        void OnPatientAdd(PatientModel patient);
        void OnPatientEdit(PatientModel patient);
    }
}