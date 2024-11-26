using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;

namespace PointOfSaleSystem
{
    public partial class MainWindow : Window
    {
        private readonly List<Product> customerOrder = new List<Product>();
        private readonly List<Product> purchaseHistory = new List<Product>();
        private decimal totalPrice = 0;

        public class Product
        {
            public string Name { get; }
            public decimal Price { get; }
            public int Quantity { get; private set; }

            public Product(string name, decimal price, int quantity = 1)
            {
                Name = name;
                Price = price;
                Quantity = quantity;
            }

            public decimal TotalPrice => Price * Quantity;

            public void IncrementQuantity() => Quantity++;

            public void DecrementQuantity() => Quantity--;
        }

        private readonly List<Product> products = new()
        {
            new("Espresso", 32.0m),
            new("Latte", 20.33m),
            new("Cappuccino", 30.33m),
            new("Americano", 18.50m),
            new("Mocha", 35.50m),
            new("Flat White", 22.75m),
            new("Macchiato", 25.75m),
            new("Tea", 25.99m),
            new("Hot Chocolate", 28.99m)
        };

        // ----------------- INITIALIZATION -------------------------------------------------------
        public MainWindow()
        {
            InitializeComponent();
            CreateProductButtons();
        }

        private void CreateProductButtons()
        {
            ButtonGrid.Children.Clear();

            for (int i = 0; i < products.Count; i++)
            {
                var product = products[i];
                var productButton = new Button
                {
                    Content = product.Name,
                    Margin = new Thickness(5),
                    FontSize = 14,
                    Background = new SolidColorBrush(Colors.LightGray),
                    Foreground = new SolidColorBrush(Colors.Black),
                    Tag = product
                };

                productButton.SetValue(AutomationProperties.AutomationIdProperty, product.Name.Replace(" ", ""));
                productButton.Click += ProductButton_Click;

                int row = i / 3;
                int column = i % 3;

                ButtonGrid.Children.Add(productButton);
                Grid.SetRow(productButton, row);
                Grid.SetColumn(productButton, column);
            }
        }

        // ----------------- HELPER METHODS -------------------------------------------------------
        private static void ShowMessage(string message, string title, MessageBoxImage icon)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, icon);
        }

        private void ResetOrder()
        {
            customerOrder.Clear();
            totalPrice = 0;
            UpdateCustomerOrderListView();
            UpdateTotalPriceTextBlock();
        }

        private void UpdateCustomerOrderListView()
        {
            customerOrderListView.ItemsSource = null;
            customerOrderListView.ItemsSource = customerOrder;
        }

        private void UpdateTotalPriceTextBlock()
        {
            totalPriceTextBlock.Text = $"Total Price: {totalPrice:0.00} SEK";
        }

        // ----------------- BUTTON CLICK EVENTS --------------------------------------------------
        private void ProductButton_Click(object sender, RoutedEventArgs e)
        {
            Button productButton = sender as Button;
            Product clickedProduct = productButton.Tag as Product;

            Product existingProduct = customerOrder.Find(p => p.Name == clickedProduct.Name);
            if (existingProduct != null)
            {
                existingProduct.IncrementQuantity();
            }
            else
            {
                customerOrder.Add(new Product(clickedProduct.Name, clickedProduct.Price));
            }

            totalPrice += clickedProduct.Price;

            UpdateCustomerOrderListView();
            UpdateTotalPriceTextBlock();
        }

        private void payButton_Click(object sender, RoutedEventArgs e)
        {
            if (customerOrder.Count == 0)
            {
                ShowMessage("You don't have any items selected", "Payment Error", MessageBoxImage.Warning);
                return;
            }

            // Add purchased items to the purchase history
            foreach (var item in customerOrder)
            {
                purchaseHistory.Add(new Product(item.Name, item.Price, item.Quantity));
            }

            ResetOrder();
            ShowMessage("Payment confirmed", "Payment", MessageBoxImage.Information);
        }

        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetOrder();
        }

        private void IncrementQuantity_Click(object sender, RoutedEventArgs e)
        {
            Button incrementButton = sender as Button;
            Product product = incrementButton.Tag as Product;

            if (product != null)
            {
                product.IncrementQuantity();
                totalPrice += product.Price;

                UpdateCustomerOrderListView();
                UpdateTotalPriceTextBlock();
            }
        }

        private void DecrementQuantity_Click(object sender, RoutedEventArgs e)
        {
            Button decrementButton = sender as Button;
            Product product = decrementButton.Tag as Product;

            if (product != null)
            {
                if (product.Quantity > 1)
                {
                    product.DecrementQuantity();
                    totalPrice -= product.Price;
                }
                else
                {
                    customerOrder.Remove(product);
                    totalPrice -= product.Price;
                }

                UpdateCustomerOrderListView();
                UpdateTotalPriceTextBlock();
            }
        }
        private void historyButton_Click(object sender, RoutedEventArgs e)
        {
            // Display the purchase history
            if (purchaseHistory.Count == 0)
            {
                ShowMessage("No purchase history available.", "Purchase History", MessageBoxImage.Information);
                return;
            }

            var mergedHistory = purchaseHistory
                .GroupBy(p => p.Name)
                .Select(g => new { Name = g.Key, Quantity = g.Sum(p => p.Quantity) })
                .ToList();

            var history = string.Join("\n", mergedHistory.Select(p => $"{p.Quantity} | {p.Name}"));
            ShowMessage(history, "Purchase History", MessageBoxImage.Information);
        }
    }
}
