using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using BPlusTree;
using BPlusTree.Blocks;
using BPlusTree.DataStructures;
using BPlusTree.Writables;

namespace BPlusTreeVisualizer
{
    public partial class MainWindow
    {
        private BlockSwiper _swiper;

        public MainWindow()
        {
            InitializeComponent();
            KeyCombo.Items.Add(typeof(WritableInt));
            KeyCombo.Items.Add(typeof(WritableLong));
            KeyCombo.Items.Add(typeof(WritableString));
            KeyCombo.Items.Add(typeof(WritableDateTime));
            ValueCombo.Items.Add(typeof(WritableInt));
            ValueCombo.Items.Add(typeof(Patient));
            ValueCombo.Items.Add(typeof(Hospitalization));
            // remove
            KeyCombo.SelectedIndex = 0;
            ValueCombo.SelectedIndex = 1;
            FileText.Text = @"C:\Users\ikim23\source\repos\AaUS2Project2\b8-int-patients.bin";
            BlockSizeText.Text = 8.ToString();
        }

        private void OpenClick(object sender, RoutedEventArgs e)
        {
            if (CheckParams(out var param))
            {
                _swiper = new BlockSwiper(param);
                Panel.Content = _swiper.Current();
                BlockIndex.Content = _swiper.BlockIndex;
                PrevBtn.IsEnabled = _swiper.HasPrev();
                NextBtn.IsEnabled = _swiper.HasNext();
            }
        }

        private void PrevClick(object sender, RoutedEventArgs e)
        {
            Panel.Content = _swiper.Prev();
            BlockIndex.Content = _swiper.BlockIndex;
            PrevBtn.IsEnabled = _swiper.HasPrev();
            NextBtn.IsEnabled = _swiper.HasNext();
        }

        private void NextClick(object sender, RoutedEventArgs e)
        {
            Panel.Content = _swiper.Next();
            BlockIndex.Content = _swiper.BlockIndex;
            PrevBtn.IsEnabled = _swiper.HasPrev();
            NextBtn.IsEnabled = _swiper.HasNext();
        }

        private bool CheckParams(out InputParams param)
        {
            param = new InputParams();
            param.KeyType = (Type)KeyCombo.SelectedItem;
            if (param.KeyType == null)
            {
                MessageBox.Show("Key Class is not selected.");
                return false;
            }
            param.ValueType = (Type)ValueCombo.SelectedItem;
            if (param.ValueType == null)
            {
                MessageBox.Show("Value Class is not selected.");
                return false;
            }
            param.Path = FileText.Text;
            if (!File.Exists(param.Path))
            {
                MessageBox.Show($"File at path {param.Path} does not exist.");
                return false;
            }
            if (!int.TryParse(BlockSizeText.Text, out var blockSize))
            {
                MessageBox.Show($"'{BlockSizeText.Text}' is not a number.");
                return false;
            }
            param.BlockSize = blockSize;
            return true;
        }
    }

    internal class BlockSwiper
    {
        private IBlockFactory Factory { get; }
        public long CurrentAddress { get; internal set; }
        public IBlock CurrentBlock { get; internal set; }
        public int BlockIndex { get; internal set; }
        public int BlockSize;

        public BlockSwiper(InputParams param)
        {
            var factoryType = typeof(BlockFactory<,>).MakeGenericType(param.KeyType, param.ValueType);
            Factory = (IBlockFactory)Activator.CreateInstance(factoryType, param.BlockSize, param.Path);
            CurrentAddress = 0;
            BlockIndex = 0;
            CurrentBlock = Factory.ReadBlock(CurrentAddress);
        }

        public bool HasNext() => CurrentAddress + CurrentBlock.ByteSize < Factory.Length();

        public bool HasPrev() => CurrentAddress > 0;

        public Grid Current() => CurrentBlock.CreateGrid();

        public Grid Next()
        {
            BlockSize = Math.Max(BlockSize, CurrentBlock.ByteSize);
            CurrentAddress += BlockSize;
            BlockIndex++;
            CurrentBlock = Factory.ReadBlock(CurrentAddress);
            return CurrentBlock.CreateGrid();
        }

        public Grid Prev()
        {
            BlockSize = Math.Max(BlockSize, CurrentBlock.ByteSize);
            CurrentAddress -= BlockSize;
            BlockIndex--;
            if (CurrentAddress < 0) CurrentAddress = 0;
            CurrentBlock = Factory.ReadBlock(CurrentAddress);
            return CurrentBlock.CreateGrid();
        }
    }

    internal class InputParams
    {
        public Type KeyType { get; set; }
        public Type ValueType { get; set; }
        public int BlockSize { get; set; }
        public string Path { get; set; }
    }
}
