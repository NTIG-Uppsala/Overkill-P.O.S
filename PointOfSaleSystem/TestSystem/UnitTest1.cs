using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.UIA3;
using System.Diagnostics;

namespace TestSystem
{
    [TestClass] // While running the tests, DO NOT move the mouse and/or interact with your computer.
    public class UnitTest1
    {

        // ========== TEST SETUP ======================================================

        private static string appPath = @"..\..\..\..\PointOfSaleSystem\bin\Release\net8.0-windows\PointOfSaleSystem.exe";
        private Application app;
        private UIA3Automation automation;
        private Window window;
        private ConditionFactory cf;

        [TestInitialize] // Initializes the application and automation objects before each test method.
        public void TestInitialize()
        {
            app = FlaUI.Core.Application.Launch(appPath);
            automation = new UIA3Automation();
            window = app.GetMainWindow(automation);
            cf = new ConditionFactory(new UIA3PropertyLibrary());
        }

        [TestCleanup] // Closes the window and disposes of the automation object after each test method.
        public void TestCleanup()
        {
            automation.Dispose();
            app.Close();
        }

        // ============================================================================

        // ========== HELPER METHODS ==================================================

        // Clicks a button with the specified automation ID a given number of times.
        private void ClickButton(string automationId, int count)
        {
            var button = window.FindFirstDescendant(cf.ByAutomationId(automationId))?.AsButton();
            for (int i = 0; i < count; i++)
            {
                button.Click();
            }
        }

        // Verifies that the order list and total price match the expected values.
        private void VerifyOrder(string[] expectedQuantities, string[] expectedNames, string expectedTotalPrice)
        {
            var customerOrderListView = window.FindFirstDescendant(cf.ByAutomationId("customerOrderListView"))?.AsListBox();
            var totalPriceTextBlock = window.FindFirstDescendant(cf.ByAutomationId("totalPriceTextBlock"))?.AsLabel();

            // Validate the number of rows
            Assert.AreEqual(expectedQuantities.Length, customerOrderListView.Items.Length, "Mismatch in number of rows.");

            for (int i = 0; i < expectedQuantities.Length; i++)
            {
                // Access the current row
                var row = customerOrderListView.Items[i];

                // Access the Quantity TextBlock
                var quantityTextBlock = row.FindFirstDescendant(cf.ByAutomationId("QuantityText"))?.AsLabel();
                Assert.IsNotNull(quantityTextBlock, $"Quantity TextBlock not found in row {i}.");
                Assert.AreEqual(expectedQuantities[i], quantityTextBlock.Text, $"Mismatch in Quantity for row {i}.");

                // Access the Product Name TextBlock
                var productNameTextBlock = row.FindFirstDescendant(cf.ByAutomationId("ProductNameText"))?.AsLabel();
                Assert.IsNotNull(productNameTextBlock, $"Product Name TextBlock not found in row {i}.");
                Assert.AreEqual(expectedNames[i], productNameTextBlock.Text, $"Mismatch in Product Name for row {i}.");
            }

            // Validate the total price
            Assert.AreEqual(expectedTotalPrice, totalPriceTextBlock.Text, "Mismatch in Total Price.");
        }


        // Verifies that the order list is empty and the total price is reset.
        private void VerifyReset()
        {
            var customerOrderListView = window.FindFirstDescendant(cf.ByAutomationId("customerOrderListView"))?.AsListBox();
            var totalPriceTextBlock = window.FindFirstDescendant(cf.ByAutomationId("totalPriceTextBlock"))?.AsLabel();

            Assert.AreEqual(0, customerOrderListView.Items.Length);
            Assert.AreEqual("Total Price: 0,00 SEK", totalPriceTextBlock.Text);
        }

        // Gets the text of the message box.
        private string GetMessageBoxText()
        {
            var messageBox = window.ModalWindows.FirstOrDefault();
            return messageBox?.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Text))?.AsLabel().Text;
        }

        // Closes the message box.
        private void CloseMessageBox()
        {
            var messageBox = window.ModalWindows.FirstOrDefault();
            messageBox?.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Button))?.AsButton().Click();
        }

        // ============================================================================

        // ========== TEST METHODS ====================================================

        [TestMethod] // Adds one espresso to the order.
        public void VerifyEspressoOrder()
        {
            ClickButton("Espresso", 1);

            VerifyOrder(
                new string[] { "1" },
                new string[] { "Espresso" },
                "Total Price: 32,00 SEK"
            );
        }

        [TestMethod] // Adds two teas to the order and then resets the order.
        public void VerifyTeaResetOrder()
        {
            ClickButton("Tea", 2);

            VerifyOrder(
                new string[] { "2" },
                new string[] { "Tea" },
                "Total Price: 51,98 SEK"
            );

            ClickButton("resetButton", 1);

            VerifyReset();
        }

        [TestMethod] // Adds one mocha to the order, resets the order, and then adds three macchiatos.
        public void VerifyMochaResetMacchiatoOrder()
        {
            ClickButton("Mocha", 1);
            ClickButton("resetButton", 1);
            ClickButton("Macchiato", 3);

            VerifyOrder(
                new string[] { "3" },
                new string[] { "Macchiato" },
                "Total Price: 77,25 SEK"
            );
        }

        [TestMethod] // Clicks the reset button multiple times.
        public void VerifyMultipleResetClicks()
        {
            ClickButton("resetButton", 3);

            VerifyReset();
        }

        [TestMethod] // Adds a diverse set of items to the order and then resets the order.
        public void VerifyDiverseOrder()
        {
            ClickButton("Latte", 1);
            ClickButton("Americano", 1);
            ClickButton("FlatWhite", 1);

            VerifyOrder(
                new string[] { "1", "1", "1" },
                new string[] { "Latte", "Americano", "Flat White" },
                "Total Price: 61,58 SEK"
            );
        }

        [TestMethod] // Clicks the pay button without any items and verifies the error message.
        public void VerifyPayWithoutItems()
        {
            ClickButton("payButton", 1);

            Assert.AreEqual("You don't have any items selected", GetMessageBoxText());
            CloseMessageBox();
        }

        [TestMethod] // Adds items to the order and verifies the payment confirmation message.
        public void VerifyPayWithItems()
        {
            ClickButton("Latte", 1);
            ClickButton("Americano", 1);

            ClickButton("payButton", 1);

            Assert.AreEqual("Payment confirmed", GetMessageBoxText());
            CloseMessageBox();

            VerifyReset();
        }

        [TestMethod] // Adds multiple items, pays, and verifies the order list is reset and payment confirmation message.
        public void VerifyMultipleItemsPay()
        {
            ClickButton("Espresso", 2);
            ClickButton("Mocha", 1);

            ClickButton("payButton", 1);

            Assert.AreEqual("Payment confirmed", GetMessageBoxText());
            CloseMessageBox();

            VerifyReset();
        }

        [TestMethod] // Add Hot Chocolate, increment quantity, and verify the order.
        public void VerifyIncrementQuantity()
        {
            ClickButton("HotChocolate", 1);
            ClickButton("IncrementButton", 1);

            VerifyOrder(
                new string[] { "2" },
                new string[] { "Hot Chocolate" },
                "Total Price: 57,98 SEK"
            );
        }

        [TestMethod] // Add Cappuccino, increment quantity, decrement quantity, and verify the order.
        public void VerifyIncrementDecrementQuantity()
        {
            ClickButton("Cappuccino", 1);
            ClickButton("IncrementButton", 2);
            ClickButton("DecrementButton", 1);

            VerifyOrder(
                new string[] { "2" },
                new string[] { "Cappuccino" },
                "Total Price: 60,66 SEK"
                );
        }

        [TestMethod] // Add Espresso, add Cappuccino, decrement quantity of Espresso (delete), and verify the order.
        public void VerifyDecrementDeleteQuantity()
        {
            ClickButton("Espresso", 1);
            ClickButton("Cappuccino", 1);
            ClickButton("DecrementButton", 1);

            VerifyOrder(
                new string[] { "1" },
                new string[] { "Cappuccino" },
                "Total Price: 30,33 SEK"
            );
        }

        // ============================================================================

    }
}