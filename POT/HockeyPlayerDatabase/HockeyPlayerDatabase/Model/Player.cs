using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HockeyPlayerDatabase.Interfaces;

namespace HockeyPlayerDatabase.Model
{
    public class Player : IPlayer
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
        public string TitleBefore { get; set; }
        public int YearOfBirth { get; set; }
        public int KrpId { get; set; }
        public AgeCategory? AgeCategory { get; set; }
        public int? ClubId { get; set; }
        [ForeignKey("ClubId")]
        public virtual Club Club { get; set; }
        [NotMapped]
        public int Age => DateTime.Today.Year - YearOfBirth;

        public Player Copy()
        {
            return new Player
            {
                Id = Id,
                FirstName = FirstName,
                LastName = LastName,
                TitleBefore = TitleBefore,
                YearOfBirth = YearOfBirth,
                KrpId = KrpId,
                AgeCategory = AgeCategory,
                ClubId = ClubId,
                Club = Club
            };
        }
    }
}
