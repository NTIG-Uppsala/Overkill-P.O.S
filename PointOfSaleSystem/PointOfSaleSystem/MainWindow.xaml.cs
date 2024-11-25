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
        private decimal totalPrice = 0;

        // Represents a product with a name, price, and quantity
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

                // Position in a 3x3 grid layout
                int row = i / 3;
                int column = i % 3;

                ButtonGrid.Children.Add(productButton);
                Grid.SetRow(productButton, row);
                Grid.SetColumn(productButton, column);
            }
        }

        private void ProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button productButton || productButton.Tag is not Product clickedProduct)
                return;

            var existingProduct = customerOrder.FirstOrDefault(p => p.Name == clickedProduct.Name);
            if (existingProduct != null)
            {
                existingProduct.IncrementQuantity();
            }
            else
            {
                customerOrder.Add(new Product(clickedProduct.Name, clickedProduct.Price));
            }

            totalPrice += clickedProduct.Price;

            UpdateOrderDisplay();
        }

        private void UpdateOrderDisplay()
        {
            customerOrderListBox.Items.Clear();
            foreach (var product in customerOrder)
            {
                customerOrderListBox.Items.Add($"{product.Quantity} | {product.Name}");
            }
            totalPriceTextBlock.Text = $"Total Price: {totalPrice:0.00} SEK";
        }

        private void payButton_Click(object sender, RoutedEventArgs e)
        {
            if (customerOrder.Count == 0)
            {
                ShowMessage("You don't have any items selected", "Payment Error", MessageBoxImage.Warning);
                return;
            }

            ResetOrder();
            ShowMessage("Payment confirmed", "Payment", MessageBoxImage.Information);
        }

        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetOrder();
        }

        private void ResetOrder()
        {
            customerOrder.Clear();
            totalPrice = 0;
            UpdateOrderDisplay();
        }

        private static void ShowMessage(string message, string title, MessageBoxImage icon)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, icon);
        }
    }
}
