using System;
using System.Dynamic;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using HockeyPlayerDatabase.Interfaces;
using HockeyPlayerDatabase.Model;
using LinqKit;
using Microsoft.Win32;

namespace HockeyPlayerDatabase.MainApp.ViewModels
{
    public class MainViewModel : Screen
    {
        public BindableCollection<Player> Players { get; set; }
        public Player SelectedPlayer
        {
            get => _selectedPlayer;
            set
            {
                _selectedPlayer = value;
                NotifyOfPropertyChange(() => CanEdit);
                NotifyOfPropertyChange(() => CanRemove);
                NotifyOfPropertyChange(() => CanOpenUrl);
            }
        }
        public int PlayersCount
        {
            get => _playersCount;
            set
            {
                _playersCount = value;
                NotifyOfPropertyChange();
            }
        }
        public bool CanEdit => SelectedPlayer != null;
        public bool CanRemove => SelectedPlayer != null;
        public bool CanOpenUrl => SelectedPlayer != null;
        private readonly HockeyContext _ctx;
        private Player _selectedPlayer;
        private int _playersCount;

        public MainViewModel()
        {
            _ctx = new HockeyContext();
            Players = new BindableCollection<Player>();
            LoadPlayers();
        }

        public void Apply(int krp, string firstName, string lastName, int yearFrom, int yearTo, bool cadet, bool midges, bool junior, bool senior, string club)
        {
            var predicate = PredicateBuilder.New<Player>(p => true);
            if (krp > 0)
            {
                predicate = predicate.And(p => p.KrpId == krp);
            }
            if (!string.IsNullOrWhiteSpace(firstName))
            {
                predicate = predicate.And(p => p.FirstName.Contains(firstName));
            }
            if (!string.IsNullOrWhiteSpace(lastName))
            {
                predicate = predicate.And(p => p.LastName.Contains(lastName));
            }
            if (yearFrom > 1900)
            {
                predicate = predicate.And(p => p.YearOfBirth >= yearFrom);
            }
            if (yearTo > 1900 && yearTo <= DateTime.Today.Year)
            {
                predicate = predicate.And(p => p.YearOfBirth <= yearTo);
            }
            if (cadet || midges || junior || senior)
            {
                var filter = new[] { cadet, midges, junior, senior };
                var allowedCategories = new[] { AgeCategory.Cadet, AgeCategory.Midgest, AgeCategory.Junior, AgeCategory.Senior }
                    .Where((category, i) => filter[i]).ToList();
                predicate = predicate.And(p => p.AgeCategory != null && allowedCategories.Contains((AgeCategory)p.AgeCategory));
            }
            if (!string.IsNullOrWhiteSpace(club))
            {
                predicate = predicate.And(p => p.Club.Name.Contains(club));
            }
            var players = _ctx.Players.AsExpandable().Where(predicate);
            Players.Clear();
            Players.AddRange(players);
        }

        public void Add()
        {
            OpenPlayerWindow("Add Player", OnPlayerAdd);
        }

        public void OnPlayerAdd(Player player)
        {
            _ctx.Players.Add(player);
            _ctx.SaveChanges();
            LoadPlayers();
        }

        public void Edit()
        {
            OpenPlayerWindow("Edit Player", OnPlayerEdit, SelectedPlayer);
        }

        public void OnPlayerEdit(Player player)
        {
            _ctx.Entry(SelectedPlayer).CurrentValues.SetValues(player);
            _ctx.SaveChanges();
            LoadPlayers();
        }

        public void Remove()
        {
            var result = MessageBox.Show($"Do you want to remove player {SelectedPlayer.FullName}?", "Remove Player",
                MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel);
            if (result == MessageBoxResult.OK)
            {
                _ctx.Players.Remove(SelectedPlayer);
                _ctx.SaveChanges();
                LoadPlayers();
            }
        }

        public void OpenUrl()
        {
            var url = SelectedPlayer.Club.Url;
            System.Diagnostics.Process.Start(url);
        }

        public void Export()
        {
            var saveDialog = new SaveFileDialog
            {
                Filter = "XML files (*.xml)|*.xml",
                InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };
            var result = saveDialog.ShowDialog();
            if (result != null && result.Value)
            {
                var fileName = saveDialog.FileName;
                _ctx.SaveToXml(fileName);
            }
        }

        public void Exit()
        {
            TryClose();
        }

        public void OpenPlayerWindow(string title, OnPlayerOk onPlayerOk, Player player = null)
        {
            var windowManager = new WindowManager();
            dynamic settings = new ExpandoObject();
            settings.Title = title;
            var clubs = _ctx.Clubs.ToList();
            var window = new PlayerViewModel(clubs, onPlayerOk, player?.Copy());
            windowManager.ShowWindow(rootModel: window, settings: settings);
        }

        public void LoadPlayers()
        {
            Players.Clear();
            Players.AddRange(_ctx.Players);
            PlayersCount = Players.Count;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            _ctx.Dispose();
        }
    }
}
