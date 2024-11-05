using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PointOfSaleSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, double> itemPrices = new Dictionary<string, double>
        {
            { "Kaffe", 32.0 },
            { "Te", 25.0 }
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void plus1Kaffe_Click(object sender, RoutedEventArgs e)
        {
            UpdateItemCount("Kaffe");
        }

        private void plus1Te_Click(object sender, RoutedEventArgs e)
        {
            UpdateItemCount("Te");
        }

        private void UpdateItemCount(string itemName)
        {
            bool itemFound = false;

            for (int i = 0; i < ResultListBox.Items.Count; i++)
            {
                string currentItem = ResultListBox.Items[i].ToString();
                if (currentItem.Contains(itemName))
                {
                    string[] split = currentItem.Split(' ');
                    int antal = int.Parse(split[0]);
                    antal++;
                    ResultListBox.Items[i] = antal + " " + itemName;
                    itemFound = true;
                    break;
                }
            }

            if (!itemFound)
            {
                ResultListBox.Items.Add("1 " + itemName);
            }

            UpdateTotalPrice();
        }

        private void UpdateTotalPrice()
        {
            double totalPrice = 0.0;

            foreach (var item in ResultListBox.Items)
            {
                string[] split = item.ToString().Split(' ');
                int antal = int.Parse(split[0]);
                string itemName = split[1];
                totalPrice += antal * itemPrices[itemName];
            }

            TotalPriceTextBlock.Text = $"Totalpris: {totalPrice} kr";
        }
        private void NollställButton_Click(object sender, RoutedEventArgs e)
        {
            ResultListBox.Items.Clear();
            TotalPriceTextBlock.Text = "Totalpris: 0 kr";
        }

    }
}
