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
        private void VerifyOrder(string[] expectedOrderTexts, string expectedTotalPrice)
        {
            var customerOrderListBox = window.FindFirstDescendant(cf.ByAutomationId("customerOrderListBox"))?.AsListBox();
            var totalPriceTextBlock = window.FindFirstDescendant(cf.ByAutomationId("totalPriceTextBlock"))?.AsLabel();

            Assert.AreEqual(expectedOrderTexts.Length, customerOrderListBox.Items.Length);

            for (int i = 0; i < expectedOrderTexts.Length; i++)
            {
                Assert.AreEqual(expectedOrderTexts[i], customerOrderListBox.Items[i].Text);
            }

            Assert.AreEqual(expectedTotalPrice, totalPriceTextBlock.Text);
        }

        // Verifies that the order list is empty and the total price is reset.
        private void VerifyReset()
        {
            var customerOrderListBox = window.FindFirstDescendant(cf.ByAutomationId("customerOrderListBox"))?.AsListBox();
            var totalPriceTextBlock = window.FindFirstDescendant(cf.ByAutomationId("totalPriceTextBlock"))?.AsLabel();

            Assert.AreEqual(0, customerOrderListBox.Items.Length);
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

            VerifyOrder(new string[] { "1 | Espresso" }, "Total Price: 32,00 SEK");
        }

        [TestMethod] // Adds two teas to the order and then resets the order.
        public void VerifyTeaResetOrder()
        {
            ClickButton("Tea", 2);

            VerifyOrder(new string[] { "2 | Tea" }, "Total Price: 51,98 SEK");

            ClickButton("resetButton", 1);

            VerifyReset();
        }

        [TestMethod] // Adds one mocha to the order, resets the order, and then adds three macchiatos.
        public void VerifyMochaResetMacchiatoOrder()
        {
            ClickButton("Mocha", 1);
            ClickButton("resetButton", 1);
            ClickButton("Macchiato", 3);

            VerifyOrder(new string[] { "3 | Macchiato" }, "Total Price: 77,25 SEK");
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

            VerifyOrder(new string[] { "1 | Latte", "1 | Americano", "1 | Flat White" }, "Total Price: 61,58 SEK");
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


        // ============================================================================

    }
}