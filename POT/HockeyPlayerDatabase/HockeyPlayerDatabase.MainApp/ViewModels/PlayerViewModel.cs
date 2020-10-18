using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using HockeyPlayerDatabase.Interfaces;
using HockeyPlayerDatabase.Model;

namespace HockeyPlayerDatabase.MainApp.ViewModels
{
    public delegate void OnPlayerOk(Player player);

    public class PlayerViewModel : Screen
    {
        public Player Player { get; set; }
        public IEnumerable<AgeCategory> AgeCategories => Enum.GetValues(typeof(AgeCategory)).Cast<AgeCategory>();
        public IEnumerable<Club> Clubs { get; set; }
        public Club Club
        {
            get => Player.Club;
            set
            {
                Player.Club = value;
                Player.ClubId = value.Id;
            }
        }
        private readonly OnPlayerOk _onPlayerOk;

        public PlayerViewModel(IEnumerable<Club> clubs, OnPlayerOk onPlayerOk, Player player)
        {
            Clubs = clubs;
            Player = player ?? new Player();
            _onPlayerOk = onPlayerOk;
        }

        public void Ok()
        {
            _onPlayerOk(Player);
            TryClose();
        }

        public void Cancel()
        {
            TryClose();
        }
    }
}
