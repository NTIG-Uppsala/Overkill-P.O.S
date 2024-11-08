using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Automation;
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
        private List<Product> customerOrder = new List<Product>();
        private double totalPrice = 0.0;

        public class Product
        {
            public string Name { get; set; }
            public double Price { get; set; }
            public int Quantity { get; set; }  // New property to track quantity

            public Product(string name, double price, int quantity = 1)
            {
                Name = name;
                Price = price;
                Quantity = quantity;
            }

            public double TotalPrice => Price * Quantity; // Helper property for total cost of this item
        }



        private readonly List<Product> products = new List<Product>
        {
            new Product("Tea", 25.0),
            new Product("Latte", 20.0),
            new Product("Americano", 18.0),
            new Product("Cappuccino", 30.0),
            new Product("Mocha", 35.0),
            new Product("Hot Chocolate", 28.0),
            new Product("Macchiato", 25.0),
            new Product("Flat White", 22.0),
            new Product("Espresso", 32.0)
        };

        public MainWindow()
        {
            InitializeComponent();
            CreateProductButtons();
        }

        private void CreateProductButtons()
        {
            ButtonGrid.Children.Clear(); // Clear any existing buttons

            // Loop through products and create buttons
            for (int i = 0; i < products.Count; i++)
            {
                // Create a new button for each product
                Button productButton = new Button
                {
                    Content = $"+1 {products[i].Name}",
                    Margin = new Thickness(5),
                    FontSize = 16,
                    Background = new SolidColorBrush(Colors.LightGray),
                    Foreground = new SolidColorBrush(Colors.Black),
                    Tag = products[i]
                };

                // Set AutomationId based on product name to match test cases
                productButton.SetValue(AutomationProperties.AutomationIdProperty, $"plus1{products[i].Name.Replace(" ", "")}");

                // Attach click event handler
                productButton.Click += ProductButton_Click;

                // Position in 3x3 grid
                int row = i / 3;
                int column = i % 3;

                ButtonGrid.Children.Add(productButton);
                Grid.SetRow(productButton, row);
                Grid.SetColumn(productButton, column);
            }
        }

        private void ProductButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Product product = button?.Tag as Product;

            if (product == null)
                return;

            // Check if the product already exists in the order list
            var existingItem = customerOrder.FirstOrDefault(item => item.Name == product.Name);

            if (existingItem != null)
            {
                // Increment quantity if it already exists
                existingItem.Quantity++;

                // Update ListBox display for the existing item
                int index = customerOrderListBox.Items.IndexOf($"{existingItem.Quantity - 1} | {existingItem.Name}");
                customerOrderListBox.Items[index] = $"{existingItem.Quantity} | {existingItem.Name}";
            }
            else
            {
                // Add the product as a new item if it doesn't exist
                var newProduct = new Product(product.Name, product.Price, 1);  // Start with quantity 1
                customerOrder.Add(newProduct);

                // Display in ListBox
                customerOrderListBox.Items.Add($"{newProduct.Quantity} | {newProduct.Name}");
            }

            // Update total price
            totalPrice += product.Price;
            TotalPriceTextBlock.Text = $"Total Price: {totalPrice} SEK";
        }

        private void UpdateCustomerOrderListBox()
        {
            customerOrderListBox.Items.Clear();
            foreach (var item in customerOrderListBox.Items)
            {
                customerOrderListBox.Items.Add(item);
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            customerOrderListBox.Items.Clear();
            TotalPriceTextBlock.Text = "Total Price: 0 SEK";
        }
    }
}
