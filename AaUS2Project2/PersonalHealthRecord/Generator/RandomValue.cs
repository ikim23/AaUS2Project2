using System;

namespace PersonalHealthRecord.Generator
{
    public class RandomValue
    {
        public static RandomValue Instance => _instance ?? (_instance = new RandomValue());
        private static RandomValue _instance;

        public string[] FirstName { get; internal set; }
        public string[] LastName { get; internal set; }
        private readonly Random _random = new Random();

        private RandomValue()
        {
            FirstName = Properties.Resources.FirstName.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            LastName = Properties.Resources.LastName.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
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
    }
}
