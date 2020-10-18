using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BPlusTree.Blocks
{
    public class UiUtils
    {
        public static Grid CreateGrid(IBlock block)
        {
            var grid = new Grid();
            var colLeft = new ColumnDefinition();
            var colRight = new ColumnDefinition();
            grid.ColumnDefinitions.Add(colLeft);
            grid.ColumnDefinitions.Add(colRight);
            var colorSwaper = GetColor().GetEnumerator();
            var type = block.GetType();
            var props = type.GetProperties();
            var rowIndex = 0;
            foreach (var prop in props)
            {
                colorSwaper.MoveNext();
                var value = prop.GetValue(block);
                AddGridRow(grid, rowIndex++, prop.Name, value, colorSwaper.Current);
            }
            colorSwaper.Dispose();
            return grid;
        }

        public static void AddGridRow(Grid grid, int rowIndex, string name, object value, Color background, int paddingLeft = 0)
        {
            var row = new RowDefinition();
            row.Height = new GridLength(30);
            grid.RowDefinitions.Add(row);

            var labelName = new Label();
            labelName.Margin = new Thickness { Left = paddingLeft };
            labelName.Background = new SolidColorBrush(background);
            labelName.Content = name;
            Grid.SetRow(labelName, rowIndex);
            Grid.SetColumn(labelName, 0);
            grid.Children.Add(labelName);

            if (value == null) return;
            var labelValue = new Label();
            labelValue.Background = new SolidColorBrush(background);
            labelValue.Content = value;
            Grid.SetRow(labelValue, rowIndex);
            Grid.SetColumn(labelValue, 1);
            grid.Children.Add(labelValue);
        }

        public static IEnumerable<Color> GetColor()
        {
            while (true)
            {
                yield return Colors.DarkGray;
                yield return Colors.AntiqueWhite;
            }
        }
    }
}
