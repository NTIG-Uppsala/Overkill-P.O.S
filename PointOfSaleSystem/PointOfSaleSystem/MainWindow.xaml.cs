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
    public partial class MainWindow : Window
    {
        private List<Product> customerOrder = new List<Product>();

        private decimal totalPrice = 0;

        // ========== PRODUCT CLASS ==================================================

        // Represents a product with a name, price, and quantity
        public class Product
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }

            public Product(string name, decimal price, int quantity = 1)
            {
                Name = name;
                Price = price;
                Quantity = quantity;
            }

            public decimal TotalPrice => Price * Quantity;
        }

        // ============================================================================

        // ========== PRODUCT LIST ====================================================

        // List of products ordered.
        private readonly List<Product> products = new List<Product>
        {
            new Product("Espresso", 32.0m),
            new Product("Latte", 20.33m),
            new Product("Cappuccino", 30.33m),
            new Product("Americano", 18.50m),
            new Product("Mocha", 35.50m),
            new Product("Flat White", 22.75m),
            new Product("Macchiato", 25.75m),
            new Product("Tea", 25.99m),
            new Product("Hot Chocolate", 28.99m)
        };

        // ============================================================================

        // ========== HELPER METHODS ==================================================

        // Creates product buttons dynamically.
        private void CreateProductButtons()
        {
            ButtonGrid.Children.Clear();

            for (int i = 0; i < products.Count; i++)
            {
                Button productButton = new Button
                {
                    Content = products[i].Name,
                    Margin = new Thickness(5),
                    FontSize = 14,
                    Background = new SolidColorBrush(Colors.LightGray),
                    Foreground = new SolidColorBrush(Colors.Black),
                    Tag = products[i]
                };

                productButton.SetValue(AutomationProperties.AutomationIdProperty, products[i].Name.Replace(" ", ""));

                productButton.Click += ProductButton_Click;

                // Position in 3x3 grid layout. Filled from left to right => top to bottom.
                int row = i / 3;
                int column = i % 3;

                ButtonGrid.Children.Add(productButton);
                Grid.SetRow(productButton, row);
                Grid.SetColumn(productButton, column);
            }
        }

        // Handles product button click events.
        private void ProductButton_Click(object sender, RoutedEventArgs e)
        {
            // Cast the sender as Button to get the product info from the Tag.
            Button productButton = sender as Button;
            Product clickedProduct = productButton.Tag as Product;

            // Check if the product is already in the customer order.
            Product existingProduct = customerOrder.Find(p => p.Name == clickedProduct.Name);
            if (existingProduct != null)
            {
                // If it exists, increase the quantity.
                existingProduct.Quantity++;
            }
            else
            {
                // If it doesn't exist, add a new entry.
                Product newProduct = new Product(clickedProduct.Name, clickedProduct.Price);
                customerOrder.Add(newProduct);
            }

            // Update the total price.
            totalPrice += clickedProduct.Price;

            // Refresh the ListBox display to show the updated order.
            customerOrderListBox.Items.Clear();
            foreach (var product in customerOrder)
            {
                customerOrderListBox.Items.Add($"{product.Quantity} | {product.Name}");
            }
            totalPriceTextBlock.Text = $"Total Price: {String.Format("{0:0.00}", totalPrice)} SEK";
        }

        // Updates the customer order ListBox.
        private void UpdateCustomerOrderListBox()
        {
            customerOrderListBox.Items.Clear();
            foreach (var item in customerOrderListBox.Items)
            {
                customerOrderListBox.Items.Add(item);
            }
        }

        //Method to handle pay button click
        private void payButton_Click(object sender, RoutedEventArgs e)
        {
            if (customerOrder.Count == 0)
            {
                // Display message if no items are selected
                MessageBox.Show("You don't have any items selected", "Payment Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Clear the product list
            customerOrder.Clear();

            // Reset the total price
            totalPrice = 0;

            // Clear the ListBox to reflect the reset state
            customerOrderListBox.Items.Clear();

            // Update the total price text block
            totalPriceTextBlock.Text = "Total Price: 0 SEK";

            // Display payment confirmation message
            MessageBox.Show("Payment confirmed", "Payment", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Handles reset button click events.
        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            customerOrder.Clear();
            totalPrice = 0;
            customerOrderListBox.Items.Clear();
            totalPriceTextBlock.Text = "Total Price: 0 SEK";
        }

        // ============================================================================

        // ========== CONSTRUCTOR =====================================================

        public MainWindow()
        {
            InitializeComponent();
            CreateProductButtons();
        }

        // ============================================================================

    }
}
