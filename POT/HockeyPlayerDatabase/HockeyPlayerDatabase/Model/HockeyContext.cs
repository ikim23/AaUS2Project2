using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Xml;
using HockeyPlayerDatabase.Interfaces;

namespace HockeyPlayerDatabase.Model
{
    public class HockeyContext : DbContext, IHockeyReport<Club, Player>
    {
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Player> Players { get; set; }

        public HockeyContext() : base("name=HockeyContext")
        {
        }

        public IQueryable<Club> GetClubs() => Clubs;

        public IQueryable<Player> GetPlayers() => Players;

        public IEnumerable<Club> GetSortedClubs(int maxResultCount)
        {
            var keys = Players.GroupBy(p => p.ClubId)
                .OrderByDescending(g => g.Count())
                .Take(maxResultCount)
                .Select(g => g.Key);
            return Clubs.Where(c => keys.Contains(c.Id));
        }

        public IEnumerable<Player> GetSortedPlayers(int maxResultCount)
        {
            return Players.OrderBy(p => p.LastName)
                .ThenByDescending(p => p.FirstName)
                .Take(maxResultCount);
        }

        public double GetPlayerAverageAge()
        {
            return Players.Average(player => player.Age);
        }

        public Player GetYoungestPlayer()
        {
            return Players.OrderBy(p => p.Age)
                .ThenByDescending(p => p.KrpId)
                .First();
        }

        public Player GetOldestPlayer()
        {
            return Players.OrderByDescending(p => p.Age)
                .ThenBy(p => p.KrpId)
                .First();
        }

        public IEnumerable<int> GetClubPlayerAges()
        {
            return Players.GroupBy(p => p.ClubId)
                .OrderByDescending(g => g.Count())
                .First()
                .GroupBy(p => p.Age)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key);
        }

        public IEnumerable<Player> GetPlayersByAge(int minAge, int maxAge)
        {
            return Players.Where(player => player.Age >= minAge && player.Age <= minAge);
        }

        public ReportResult GetReportByClub(int clubId)
        {
            var playerCount = Players.Count(p => p.ClubId == clubId);
            var averageAge = Players
                .Where(p => p.ClubId == clubId)
                .Average(p => p.Age);
            var youngestPlayer = Players
                .Where(p => p.ClubId == clubId)
                .OrderBy(p => p.Age)
                .ThenByDescending(p => p.KrpId)
                .First();
            var oldestPlayer = Players
                .Where(p => p.ClubId == clubId)
                .OrderByDescending(p => p.Age)
                .ThenBy(p => p.KrpId)
                .First();
            return new ReportResult(playerCount, averageAge, youngestPlayer.FullName, oldestPlayer.FullName, youngestPlayer.Age, oldestPlayer.Age);
        }

        public IDictionary<AgeCategory, ReportResult> GetReportByAgeCategory()
        {
            var dictionary = new Dictionary<AgeCategory, ReportResult>();
            var categories = Enum.GetValues(typeof(AgeCategory)).Cast<AgeCategory>();
            foreach (var category in categories)
            {
                var playerCount = Players.Count(p => p.AgeCategory == category);
                var averageAge = Players
                    .Where(p => p.AgeCategory == category)
                    .Average(p => p.Age);
                var youngestPlayer = Players
                    .Where(p => p.AgeCategory == category)
                    .OrderBy(p => p.Age)
                    .ThenByDescending(p => p.KrpId)
                    .First();
                var oldestPlayer = Players
                    .Where(p => p.AgeCategory == category)
                    .OrderByDescending(p => p.Age)
                    .ThenBy(p => p.KrpId)
                    .First();
                var report = new ReportResult(playerCount, averageAge, youngestPlayer.FullName, oldestPlayer.FullName, youngestPlayer.Age, oldestPlayer.Age);
                dictionary[category] = report;
            }
            return dictionary;
        }

        public void SaveToXml(string fileName)
        {
            var document = new XmlDocument();
            var clubsXml = document.CreateElement("Clubs");
            foreach (var club in Clubs)
            {
                clubsXml.AppendChild(ClubXml(club, document));
            }
            var playersXml = document.CreateElement("Players");
            foreach (var player in Players)
            {
                playersXml.AppendChild(PlayerXml(player, document));
            }
            document.AppendChild(document.CreateXmlDeclaration("1.0", "UTF-8", null));
            var hockeyXml = document.CreateElement("Hockey");
            hockeyXml.AppendChild(clubsXml);
            hockeyXml.AppendChild(playersXml);
            document.AppendChild(hockeyXml);
            document.Save(fileName);
        }

        private XmlElement ClubXml(IClub club, XmlDocument document)
        {
            var clubXml = document.CreateElement("Club");
            var clubId = document.CreateElement("ID");
            var clubName = document.CreateElement("Name");
            var clubAddress = document.CreateElement("Address");
            var clubUrl = document.CreateElement("URL");
            clubId.InnerText = club.Id.ToString();
            clubName.InnerText = club.Name;
            clubAddress.InnerText = club.Address;
            clubUrl.InnerText = club.Url;
            clubXml.AppendChild(clubId);
            clubXml.AppendChild(clubName);
            clubXml.AppendChild(clubAddress);
            clubXml.AppendChild(clubUrl);
            return clubXml;
        }

        private XmlElement PlayerXml(IPlayer player, XmlDocument document)
        {
            var playerXml = document.CreateElement("Player");
            var id = document.CreateElement("ID");
            var firstName = document.CreateElement("FirstName");
            var lastName = document.CreateElement("LastName");
            var titleBefore = document.CreateElement("TitleBefore");
            var yearOfBirth = document.CreateElement("YearOfBirth");
            var krp = document.CreateElement("KRP");
            id.InnerText = player.Id.ToString();
            firstName.InnerText = player.FirstName;
            lastName.InnerText = player.LastName;
            titleBefore.InnerText = player.TitleBefore;
            yearOfBirth.InnerText = player.YearOfBirth.ToString();
            krp.InnerText = player.KrpId.ToString();
            playerXml.AppendChild(id);
            playerXml.AppendChild(firstName);
            playerXml.AppendChild(lastName);
            playerXml.AppendChild(titleBefore);
            playerXml.AppendChild(yearOfBirth);
            playerXml.AppendChild(krp);
            if (player.AgeCategory != null)
            {
                var ageCategory = document.CreateElement("AgeCategory");
                ageCategory.InnerText = player.AgeCategory.ToString();
                playerXml.AppendChild(ageCategory);
            }
            if (player.ClubId != null)
            {
                var club = document.CreateElement("Club");
                club.InnerText = player.ClubId.ToString();
                playerXml.AppendChild(club);
            }
            return playerXml;
        }
    }
}
