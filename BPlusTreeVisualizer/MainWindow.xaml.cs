using System.Windows;
using System.Windows.Controls;
using BPlusTree.Blocks;
using BPlusTree.DataStructures;
using Microsoft.Win32;

namespace BPlusTreeVisualizer
{
    public partial class MainWindow
    {
        private BlockSwiper _swiper;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Binary files (*.bin)|*.bin",
                InitialDirectory = @"C:\Users\ikim23\source\repos\AaUS2Project2\DataStructuresUnitTest\bin\Debug"
                //InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
            };
            var result = dialog.ShowDialog();
            if (result != null && result.Value)
            {
                var file = FileText.Text = dialog.FileName;
                if (!string.IsNullOrWhiteSpace(file))
                {
                    _swiper = new BlockSwiper(file);
                    Panel.Content = _swiper.Current();
                    BlockIndex.Content = $"{_swiper.BlockIndex}/{_swiper.MaxIndex}";
                    PrevBtn.IsEnabled = true;
                    NextBtn.IsEnabled = true;
                }
            }
        }

        private void PrevClick(object sender, RoutedEventArgs e)
        {
            Panel.Content = _swiper.Prev();
            BlockIndex.Content = $"{_swiper.BlockIndex}/{_swiper.MaxIndex}";
        }

        private void NextClick(object sender, RoutedEventArgs e)
        {
            Panel.Content = _swiper.Next();
            BlockIndex.Content = $"{_swiper.BlockIndex}/{_swiper.MaxIndex}";
        }
    }

    internal class BlockSwiper
    {
        private IBlockFactory Factory { get; }
        public long CurrentAddress { get; internal set; }
        public IBlock CurrentBlock { get; internal set; }
        public int BlockIndex { get; internal set; }
        public int MaxIndex { get; }

        public BlockSwiper(string file)
        {
            Factory = BPlusTree.DataStructures.Factory.Create(file);
            CurrentAddress = 0;
            BlockIndex = 0;
            MaxIndex = (int)(Factory.Length() - Factory.ControlBlockByteSize) / Factory.BlockByteSize;
            CurrentBlock = Factory.ReadBlock(CurrentAddress);
        }

        public Grid Current() => CurrentBlock.CreateGrid();

        public Grid Next()
        {
            NextAddress();
            CurrentBlock = Factory.ReadBlock(CurrentAddress);
            return CurrentBlock.CreateGrid();
        }

        public Grid Prev()
        {
            PrevAddress();
            CurrentBlock = Factory.ReadBlock(CurrentAddress);
            return CurrentBlock.CreateGrid();
        }

        private void NextAddress()
        {
            if (BlockIndex == 0)
            {
                CurrentAddress += Factory.ControlBlockByteSize;
            }
            else
            {
                CurrentAddress += Factory.BlockByteSize;
            }
            BlockIndex++;
            if (CurrentAddress >= Factory.Length())
            {
                CurrentAddress = 0;
                BlockIndex = 0;
            }
        }

        private void PrevAddress()
        {
            if (BlockIndex == 0)
            {
                CurrentAddress = Factory.Length() - Factory.BlockByteSize;
                BlockIndex = MaxIndex;
            }
            else if (BlockIndex == 1)
            {
                CurrentAddress = 0;
                BlockIndex = 0;
            }
            else
            {
                CurrentAddress -= Factory.BlockByteSize;
                BlockIndex--;
            }
        }
    }
}
