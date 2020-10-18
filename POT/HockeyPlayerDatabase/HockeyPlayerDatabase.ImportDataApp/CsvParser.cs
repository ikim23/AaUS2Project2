using System.Collections.Generic;
using System.IO;
using HockeyPlayerDatabase.Interfaces;
using HockeyPlayerDatabase.Model;

namespace HockeyPlayerDatabase.ImportDataApp
{
    public class CsvParser
    {
        public static IEnumerable<Club> ParseClubs(string file)
        {
            var list = new List<Club>();
            using (var reader = new StreamReader(file))
            {
                var line = reader.ReadLine(); // skip first line
                while ((line = reader.ReadLine()) != null)
                {
                    var p = line.Split(';');
                    var club = new Club
                    {
                        Name = p[0],
                        Address = p[1],
                        Url = p[2]
                    };
                    list.Add(club);
                }
            }
            return list;
        }

        public static IEnumerable<Player> ParsePlayers(string file, IDictionary<string, int> clubs)
        {
            var list = new List<Player>();
            using (var reader = new StreamReader(file))
            {
                var line = reader.ReadLine(); // skip first line
                while ((line = reader.ReadLine()) != null)
                {
                    var p = line.Split(';');
                    var cludId = clubs[p[5]];
                    var player = new Player
                    {
                        LastName = ToTitleCase(p[0]),
                        FirstName = p[1],
                        TitleBefore = p[2],
                        YearOfBirth = int.Parse(p[3]),
                        KrpId = int.Parse(p[4]),
                        ClubId = cludId,
                        AgeCategory = ParseAgeCategory(p[6])
                    };
                    list.Add(player);
                }
            }
            return list;
        }

        private static AgeCategory? ParseAgeCategory(string value)
        {
            switch (value)
            {
                case "Kadeti": return AgeCategory.Cadet;
                case "Juniori": return AgeCategory.Junior;
                case "Dorastenci": return AgeCategory.Midgest;
                case "Seniori": return AgeCategory.Senior;
                default: return null;
            }
        }

        private static string ToTitleCase(string s)
        {
            var lower = s.ToLower();
            return $"{s[0]}{lower.Substring(1)}";
        }
    }
}
