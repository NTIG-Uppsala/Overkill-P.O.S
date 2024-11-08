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

        // The total price of the order
        private double totalPrice = 0;

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


        //List of products
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

        //Constructor
        public MainWindow()
        {
            InitializeComponent();
            CreateProductButtons();
        }

        //Method to create product buttons
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

        //Method to handle product button click
        private void ProductButton_Click(object sender, RoutedEventArgs e)
        {
            // Cast the sender as Button to get the product info from the Tag
            Button productButton = sender as Button;
            Product clickedProduct = productButton.Tag as Product;

            // Check if the product is already in the customer order
            Product existingProduct = customerOrder.Find(p => p.Name == clickedProduct.Name);
            if (existingProduct != null)
            {
                // If it exists, increase the quantity
                existingProduct.Quantity++;
            }
            else
            {
                // If it doesn't exist, add a new entry
                Product newProduct = new Product(clickedProduct.Name, clickedProduct.Price);
                customerOrder.Add(newProduct);
            }

            // Update the total price
            totalPrice += clickedProduct.Price;

            // Refresh the ListBox display to show the updated order
            customerOrderListBox.Items.Clear();
            foreach (var product in customerOrder)
            {
                customerOrderListBox.Items.Add($"{product.Quantity} | {product.Name}");
            }

            // Update the total price text block
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

        //Method to handle reset button click
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear the product list
            customerOrder.Clear();

            // Reset the total price
            totalPrice = 0;

            // Clear the ListBox to reflect the reset state
            customerOrderListBox.Items.Clear();

            // Update the total price text block
            TotalPriceTextBlock.Text = "Total Price: 0 SEK";
        }
    }
}