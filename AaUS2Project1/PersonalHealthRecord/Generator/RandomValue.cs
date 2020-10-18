using System;

namespace PersonalHealthRecord.Generator
{
    public class RandomValue
    {
        public static RandomValue Instance => _instance ?? (_instance = new RandomValue());
        private static RandomValue _instance;

        public string[] FirstName { get; internal set; }
        public string[] LastName { get; internal set; }
        public string[] InsuranceAgencyCode { get; internal set; }
        public string[] City { get; internal set; }
        private readonly Random _random = new Random();

        private RandomValue()
        {
            FirstName = Properties.Resources.FirstName.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            LastName = Properties.Resources.LastName.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            InsuranceAgencyCode = Properties.Resources.InsuranceAgencyCode.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            City = Properties.Resources.City.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }

        public string GetFirstName()
        {
            var i = _random.Next(0, FirstName.Length);
            return FirstName[i];
        }

        public string GetLastName()
        {
            var i = _random.Next(0, LastName.Length);
            return LastName[i];
        }

        public DateTime GetBirthday()
        {
            var ageInDays = _random.Next(3 * 365, 90 * 365);
            var birthday = DateTime.Today.AddDays(-ageInDays);
            return birthday;
        }

        public string GetPersonalNumber(DateTime? birthday = null)
        {
            var b = birthday ?? DateTime.Today;
            var fourDigit = _random.Next(1000, 10000);
            return $"{b:ddMMyy}{fourDigit}";
        }

        public string GeInsuranceAgencyCode()
        {
            var i = _random.Next(0, InsuranceAgencyCode.Length);
            return InsuranceAgencyCode[i];
        }
    }
}
