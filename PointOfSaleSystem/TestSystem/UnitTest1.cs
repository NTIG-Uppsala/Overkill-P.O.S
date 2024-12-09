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
            Assert.IsNotNull(button, $"Button with AutomationId '{automationId}' not found.");
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

            Assert.IsNotNull(customerOrderListView, "Customer order list view not found.");
            Assert.IsNotNull(totalPriceTextBlock, "Total price text block not found.");

            // Validate the number of rows
            Assert.AreEqual(expectedQuantities.Length, customerOrderListView.Items.Length, "Mismatch in number of rows.");

            for (int i = 0; i < expectedQuantities.Length; i++)
            {
                string expectedName = expectedNames[i];
                string expectedQuantity = expectedQuantities[i];

                // Locate the QuantityText and ProductNameText using dynamic AutomationId
                var quantityTextBlock = window.FindFirstDescendant(cf.ByAutomationId($"QuantityText_{expectedName}"))?.AsLabel();
                var productNameTextBlock = window.FindFirstDescendant(cf.ByAutomationId($"ProductNameText_{expectedName}"))?.AsLabel();

                Assert.IsNotNull(quantityTextBlock, $"Quantity TextBlock for '{expectedName}' not found.");
                Assert.AreEqual(expectedQuantity, quantityTextBlock.Text, $"Mismatch in Quantity for '{expectedName}'.");

                Assert.IsNotNull(productNameTextBlock, $"Product Name TextBlock for '{expectedName}' not found.");
                Assert.AreEqual(expectedName, productNameTextBlock.Text, $"Mismatch in Product Name for '{expectedName}'.");
            }

            // Validate the total price
            Assert.AreEqual(expectedTotalPrice, totalPriceTextBlock.Text, "Mismatch in Total Price.");
        }


        // Verifies that the order list is empty and the total price is reset.
        private void VerifyReset()
        {
            var customerOrderListView = window.FindFirstDescendant(cf.ByAutomationId("customerOrderListView"))?.AsListBox();
            var totalPriceTextBlock = window.FindFirstDescendant(cf.ByAutomationId("totalPriceTextBlock"))?.AsLabel();

            Assert.IsNotNull(customerOrderListView, "Customer order list view not found.");
            Assert.AreEqual(0, customerOrderListView.Items.Length, "Order list is not empty after reset.");

            Assert.IsNotNull(totalPriceTextBlock, "Total price text block not found.");
            Assert.AreEqual("Total Price: 0.00 SEK", totalPriceTextBlock.Text, "Total price not reset properly.");
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
                "Total Price: 32.00 SEK"
            );
        }

        [TestMethod] // Adds two teas to the order and then resets the order.
        public void VerifyTeaResetOrder()
        {
            ClickButton("Tea", 2);

            VerifyOrder(
                new string[] { "2" },
                new string[] { "Tea" },
                "Total Price: 51.98 SEK"
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
                "Total Price: 77.25 SEK"
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
                "Total Price: 61.58 SEK"
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

        [TestMethod] // Adds three different products, pays for them, and verifies the purchase history.
        public void VerifyMultipleProductsPurchaseHistory()
        {
            ClickButton("Espresso", 1);
            ClickButton("Latte", 1);
            ClickButton("Mocha", 1);

            ClickButton("payButton", 1);

            Assert.AreEqual("Payment confirmed", GetMessageBoxText());
            CloseMessageBox();

            ClickButton("historyButton", 1);

            Assert.AreEqual("1 | Espresso\n1 | Latte\n1 | Mocha", GetMessageBoxText());
            CloseMessageBox();
        }

        [TestMethod] // Opens the purchase history without paying for any products and verifies it is empty.
        public void VerifyEmptyPurchaseHistory()
        {
            ClickButton("historyButton", 1);

            Assert.AreEqual("No purchase history available.", GetMessageBoxText());
            CloseMessageBox();
        }

        [TestMethod] // Adds products to the list but does not pay for them, then checks the history.
        public void VerifyPurchaseHistoryWithoutPayment()
        {
            ClickButton("Latte", 1);
            ClickButton("Americano", 1);

            ClickButton("historyButton", 1);

            Assert.AreEqual("No purchase history available.", GetMessageBoxText());
            CloseMessageBox();
        }

        [TestMethod] // Add Cappuccino, increment quantity, decrement quantity, and verify the order.
        public void VerifyIncrementDecrementQuantity()
        {
            ClickButton("Cappuccino", 1);
            ClickButton("IncrementButton_Cappuccino", 2);
            ClickButton("DecrementButton_Cappuccino", 1);

            VerifyOrder(
                new string[] { "2" },
                new string[] { "Cappuccino" },
                "Total Price: 60.66 SEK"
                );
        }

        [TestMethod] // Add Espresso, add Cappuccino, decrement quantity of Espresso (delete), and verify the order.
        public void VerifyDecrementDeleteQuantity()
        {
            ClickButton("Espresso", 1);
            ClickButton("Cappuccino", 1);
            ClickButton("DecrementButton_Espresso", 1);

            VerifyOrder(
                new string[] { "1" },
                new string[] { "Cappuccino" },
                "Total Price: 30.33 SEK"
            );
        }

        [TestMethod] // Add Espresso, add Cappuccino, decrement quantity of Cappuccino (delete), and verify the order.
        public void VerifyDecrementDeleteQuantitySecondItem()
        {
            ClickButton("Espresso", 1);
            ClickButton("Cappuccino", 1);
            ClickButton("DecrementButton_Cappuccino", 1);

            VerifyOrder(
                new string[] { "1" },
                new string[] { "Espresso" },
                "Total Price: 32.00 SEK"
            );
        }

        [TestMethod] // Add Espresso, undo, verify the order.
        public void VerifyUndo()
        {
            ClickButton("Espresso", 1);
            ClickButton("undoButton", 1);

            VerifyReset();
        }

        [TestMethod] // Add Espresso, add Cappuccino, undo, verify the order.
        public void VerifyUndoMultipleItems()
        {
            ClickButton("Espresso", 1);
            ClickButton("Cappuccino", 1);
            ClickButton("undoButton", 1);
            VerifyOrder(
                new string[] { "1" },
                new string[] { "Espresso" },
                "Total Price: 32.00 SEK"
            );
        }

        [TestMethod] // Add Espresso, increment quantity, undo, verify the order.

        public void VerifyUndoIncrement()
        {
            ClickButton("Espresso", 1);
            ClickButton("IncrementButton_Espresso", 1);
            ClickButton("undoButton", 1);
            VerifyOrder(
                new string[] { "1" },
                new string[] { "Espresso" },
                "Total Price: 32.00 SEK"
            );
        }

        [TestMethod] // Add Espresso, decrement quantity, undo, verify the order.
        public void VerifyUndoDecrement()
        {
            ClickButton("Espresso", 1);
            ClickButton("DecrementButton_Espresso", 1);
            ClickButton("undoButton", 1);
            VerifyOrder(
                new string[] { "1" },
                new string[] { "Espresso" },
                "Total Price: 32.00 SEK"
            );
        }

        [TestMethod] // Add Espresso, reset, undo, verify the order.
        public void VerifyUndoReset()
        {
            ClickButton("Espresso", 1);
            ClickButton("resetButton", 1);
            ClickButton("undoButton", 1);
            VerifyOrder(
                new string[] { "1" },
                new string[] { "Espresso" },
                "Total Price: 32.00 SEK"
            );
        }

        [TestMethod] // Add Espresso, reset, undo three times, verify the order (should be empty).
        public void VerifyUndoResetMultiple()
        {
            ClickButton("Espresso", 1);
            ClickButton("resetButton", 1);
            ClickButton("undoButton", 1);
            VerifyOrder(
                new string[] { "1" },
                new string[] { "Espresso" },
                "Total Price: 32.00 SEK"
            );
            ClickButton("undoButton", 1);
            VerifyReset();
            ClickButton("undoButton", 1);
            VerifyReset();
        }


        [TestMethod] // Add Espresso, pay, undo pay (fail/not possible), verify the order.
        public void VerifyUndoPay()
        {
            ClickButton("Espresso", 1);
            ClickButton("payButton", 1);
            CloseMessageBox();
            ClickButton("undoButton", 1);
            Assert.AreEqual("You can't undo a payment", GetMessageBoxText());
            CloseMessageBox();
        }

        // ============================================================================

    }
}