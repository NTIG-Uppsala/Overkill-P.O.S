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
            { "Espresso", 32.0 },
            { "Tea", 25.0 },
            { "Latte", 20.0 },
            { "Americano", 18.0 },
            { "Cappuccino", 30.0 },
            { "Mocha", 35.0 },
            { "Hot Chocolate", 28.0 },
            { "Macchiato", 25.0 },
            { "Flat White", 22.0 }
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void plus1Espresso_Click(object sender, RoutedEventArgs e)
        {
            UpdateItemCount("Espresso");
        }

        private void plus1Tea_Click(object sender, RoutedEventArgs e)
        {
            UpdateItemCount("Tea");
        }

        private void plus1Latte_Click(object sender, RoutedEventArgs e)
        {
            UpdateItemCount("Latte");
        }

        private void plus1Americano_Click(object sender, RoutedEventArgs e)
        {
            UpdateItemCount("Americano");
        }

        private void plus1Cappuccino_Click(object sender, RoutedEventArgs e)
        {
            UpdateItemCount("Cappuccino");
        }

        private void plus1Mocha_Click(object sender, RoutedEventArgs e)
        {
            UpdateItemCount("Mocha");
        }

        private void plus1HotChocolate_Click(object sender, RoutedEventArgs e)
        {
            UpdateItemCount("Hot Chocolate");
        }

        private void plus1Macchiato_Click(object sender, RoutedEventArgs e)
        {
            UpdateItemCount("Macchiato");
        }

        private void plus1FlatWhite_Click(object sender, RoutedEventArgs e)
        {
            UpdateItemCount("Flat White");
        }

        private void UpdateItemCount(string itemName)
        {
            bool itemFound = false;

            for (int i = 0; i < customerOrderListBox.Items.Count; i++)
            {
                string currentItem = customerOrderListBox.Items[i].ToString();
                string[] split = currentItem.Split('|');
                string currentItemName = split[1].Trim();

                if (currentItemName == itemName)
                {
                    int antal = int.Parse(split[0].Trim());
                    antal++;
                    customerOrderListBox.Items[i] = $"{antal} | {itemName}";
                    itemFound = true;
                    break;
                }
            }

            if (!itemFound)
            {
                customerOrderListBox.Items.Add($"1 | {itemName}");
            }

            UpdateTotalPrice();
        }

        private void UpdateTotalPrice()
        {
            double totalPrice = 0.0;

            foreach (var item in customerOrderListBox.Items)
            {
                string[] split = item.ToString().Split('|');
                int antal = int.Parse(split[0].Trim());
                string itemName = split[1].Trim();
                totalPrice += antal * itemPrices[itemName];
            }

            TotalPriceTextBlock.Text = $"Total Price: {totalPrice} SEK";
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            customerOrderListBox.Items.Clear();
            TotalPriceTextBlock.Text = "Total Price: 0 SEK";
        }
    }
}
