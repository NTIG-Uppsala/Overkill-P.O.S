using System.ComponentModel;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;

namespace PointOfSaleSystem
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private decimal _totalPrice;

        public decimal TotalPrice
        {
            get => _totalPrice;
            set
            {
                if (_totalPrice != value)
                {
                    _totalPrice = value;
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel = new MainWindowViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;
            CreateProductButtons();
        }

        private void UpdateTotalPrice(decimal amount)
        {
            _viewModel.TotalPrice += amount;
        }

        private readonly List<Product> customerOrder = new List<Product>();
        private readonly List<Product> purchaseHistory = new List<Product>();
        private decimal totalPrice = 0;
        private bool paymentMade = false; // Flag to track if payment has been made

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

            public void IncrementQuantity() => Quantity++;

            public void DecrementQuantity() => Quantity--;
        }

        public enum ActionType
        {
            Add,
            Increment,
            Decrement,
            Reset
        }

        public class ActionRecord
        {
            public Product Product { get; set; }
            public ActionType ActionType { get; set; }
            public List<Product> PreviousOrderState { get; set; }
            public decimal PreviousTotalPrice { get; set; }
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
            new("Hot Chocolate", 28.99m),
        };

        private readonly Stack<ActionRecord> actionStack = new Stack<ActionRecord>();

        // ----------------- INITIALIZATION -------------------------------------------------------

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
                    Tag = product,
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
            var previousTotalPrice = _viewModel.TotalPrice;
            var previousOrderState = customerOrder.Select(p => new Product(p.Name, p.Price, p.Quantity)).ToList();

            actionStack.Push(new ActionRecord
            {
                ActionType = ActionType.Reset,
                PreviousOrderState = previousOrderState,
                PreviousTotalPrice = previousTotalPrice,
            });
            paymentMade = false;
            customerOrder.Clear();
            _viewModel.TotalPrice = 0;
            UpdateCustomerOrderListView();
        }


        private void UpdateCustomerOrderListView()
        {
            customerOrderListView.ItemsSource = null;
            customerOrderListView.ItemsSource = customerOrder;
            customerOrderListView.Items.Refresh();
        }

        private void ScrollToLastListViewItem(Product lastAddedProduct)
        {
            if (lastAddedProduct != null)
            {
                customerOrderListView.SelectedItem = lastAddedProduct;
                customerOrderListView.ScrollIntoView(lastAddedProduct);
            }
            else
            {
                customerOrderListView.SelectedIndex = customerOrderListView.Items.Count - 1;
                customerOrderListView.ScrollIntoView(customerOrderListView.SelectedItem);
            }
        }

        // ----------------- BUTTON CLICK EVENTS --------------------------------------------------
        private void ProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (paymentMade)
            {
                paymentMade = false; // Reset the payment made flag when a new order is started
            }

            Button productButton = sender as Button;
            Product clickedProduct = productButton.Tag as Product;

            Product existingProduct = customerOrder.Find(p => p.Name == clickedProduct.Name);
            if (existingProduct != null)
            {
                existingProduct.IncrementQuantity();
                actionStack.Push(new ActionRecord { Product = existingProduct, ActionType = ActionType.Increment });
            }
            else
            {
                customerOrder.Add(new Product(clickedProduct.Name, clickedProduct.Price));
                existingProduct = customerOrder.Last();
                actionStack.Push(new ActionRecord { Product = existingProduct, ActionType = ActionType.Add });
            }

            UpdateTotalPrice(clickedProduct.Price);

            UpdateCustomerOrderListView();
            ScrollToLastListViewItem(existingProduct);
        }

        private void payButton_Click(object sender, RoutedEventArgs e)
        {
            if (customerOrder.Count == 0)
            {
                ShowMessage("You don't have any items selected", "Payment Error", MessageBoxImage.Warning);
                return;
            }

            foreach (var item in customerOrder)
            {
                purchaseHistory.Add(new Product(item.Name, item.Price, item.Quantity));
            }

            // Clear the undo stack to make payment irreversible
            actionStack.Clear();

            ResetOrder();
            paymentMade = true; // Set the payment made flag
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
                actionStack.Push(new ActionRecord { Product = product, ActionType = ActionType.Increment });

                UpdateCustomerOrderListView();
                UpdateTotalPrice(product.Price);
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
                    actionStack.Push(new ActionRecord { Product = product, ActionType = ActionType.Decrement });
                }
                else
                {
                    customerOrder.Remove(product);
                    actionStack.Push(new ActionRecord { Product = product, ActionType = ActionType.Decrement });
                }

                UpdateCustomerOrderListView();
                UpdateTotalPrice(-product.Price);
            }
        }

        private void historyButton_Click(object sender, RoutedEventArgs e)
        {
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

        private void undoButton_Click(object sender, RoutedEventArgs e)
        {
            if (paymentMade)
            {
                ShowMessage("You can't undo a payment", "Undo Error", MessageBoxImage.Warning);
                return;
            }

            if (actionStack.Count == 0)
            {
                ShowMessage("No actions to undo or undo is disabled after payment", "Undo Error", MessageBoxImage.Warning);
                return;
            }

            ActionRecord lastAction = actionStack.Pop();
            Product lastProduct = lastAction.Product;
            lastProduct = customerOrder.Find(p => p.Name == lastProduct.Name);

            decimal totalPrice = 0;

            switch (lastAction.ActionType)
            {
                case ActionType.Add:
                    customerOrder.RemoveAll(p => p.Name == lastProduct.Name);
                    totalPrice -= lastProduct.Price;
                    break;
                case ActionType.Increment:
                    if (lastProduct.Quantity == 0)
                    {
                        customerOrder.Remove(lastProduct);
                        totalPrice -= lastProduct.Price;
                    }
                    lastProduct.DecrementQuantity();
                    totalPrice -= lastProduct.Price;
                    break;
                case ActionType.Decrement:
                    if (!customerOrder.Contains(lastProduct))
                    {
                        customerOrder.Add(lastProduct);
                        totalPrice += lastProduct.Price;
                        break;
                    }
                    lastProduct.IncrementQuantity();
                    totalPrice += lastProduct.Price;
                    break;
                case ActionType.Reset:
                    customerOrder.Clear();
                    customerOrder.AddRange(lastAction.PreviousOrderState);
                    totalPrice = lastAction.PreviousTotalPrice;
                    break;
            }

            UpdateCustomerOrderListView();
            UpdateTotalPrice(totalPrice);
            ScrollToLastListViewItem(lastProduct);
        }
    }
}
