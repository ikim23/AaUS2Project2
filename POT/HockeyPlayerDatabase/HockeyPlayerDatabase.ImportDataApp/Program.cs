using System;
using System.Linq;
using HockeyPlayerDatabase.Model;

namespace HockeyPlayerDatabase.ImportDataApp
{
    public class Program
    {
        public static void Main(string[] arguments)
        {
            string clubsFile = null;
            string playersFile = null;
            var clearDatabase = false;
            var args = arguments.GetEnumerator();
            while (args.MoveNext())
            {
                switch (args.Current)
                {
                    case "-clubs":
                        if (args.MoveNext()) clubsFile = (string)args.Current;
                        break;
                    case "-players":
                        if (args.MoveNext()) playersFile = (string)args.Current;
                        break;
                    case "-clearDatabase":
                        clearDatabase = true;
                        break;
                    default:
                        Console.WriteLine($"Unknown parameter: {args.Current}");
                        break;
                }
            }

            using (var ctx = new HockeyContext())
            {
                if (clearDatabase)
                {
                    Console.WriteLine("Clearing DB");
                    var players = ctx.Players.ToList();
                    ctx.Players.RemoveRange(players);
                    var clubs = ctx.Clubs.ToList();
                    ctx.Clubs.RemoveRange(clubs);
                }
                if (clubsFile != null)
                {
                    Console.WriteLine("Inserting Clubs");
                    var clubs = CsvParser.ParseClubs(clubsFile);
                    ctx.Clubs.AddRange(clubs);
                }
                if (playersFile != null)
                {
                    ctx.SaveChanges(); // save clubs
                    var clubs = ctx.Clubs.ToDictionary(club => club.Name, club => club.Id);
                    Console.WriteLine("Inserting Players");
                    var players = CsvParser.ParsePlayers(playersFile, clubs);
                    ctx.Players.AddRange(players);
                }
                Console.WriteLine("Saving changes");
                ctx.SaveChanges();
            }
        }
    }
}
