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
        private static string appPath = @"..\..\..\..\PointOfSaleSystem\bin\Debug\net8.0-windows\PointOfSaleSystem.exe";
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

        // Clicks a button with the specified automation ID a given number of times.
        private void ClickButton(string automationId, int count)
        {
            var button = window.FindFirstDescendant(cf.ByAutomationId(automationId))?.AsButton();
            for (int i = 0; i < count; i++)
            {
                button.Click();
            }
        }

        // Clicks the reset button a given number of times.
        private void ClickResetButton(int count)
        {
            var resetButton = window.FindFirstDescendant(cf.ByAutomationId("ResetButton"))?.AsButton();
            for (int i = 0; i < count; i++)
            {
                resetButton.Click();
            }
        }

        // Verifies that the order list and total price match the expected values.
        private void VerifyOrder(string[] expectedOrderTexts, string expectedTotalPrice)
        {
            var testCustomerOrderListBox = window.FindFirstDescendant(cf.ByAutomationId("customerOrderListBox"))?.AsListBox();
            var testTotalPriceTextBlock = window.FindFirstDescendant(cf.ByAutomationId("TotalPriceTextBlock"))?.AsLabel();

            Assert.AreEqual(expectedOrderTexts.Length, testCustomerOrderListBox.Items.Length);

            for (int i = 0; i < expectedOrderTexts.Length; i++)
            {
                Assert.AreEqual(expectedOrderTexts[i], testCustomerOrderListBox.Items[i].Text);
            }

            Assert.AreEqual(expectedTotalPrice, testTotalPriceTextBlock.Text);
        }

        // Verifies that the order list is empty and the total price is reset.
        private void VerifyReset()
        {
            var testCustomerOrderListBox = window.FindFirstDescendant(cf.ByAutomationId("customerOrderListBox"))?.AsListBox();
            var testTotalPriceTextBlock = window.FindFirstDescendant(cf.ByAutomationId("TotalPriceTextBlock"))?.AsLabel();

            Assert.AreEqual(0, testCustomerOrderListBox.Items.Length);
            Assert.AreEqual("Total Price: 0 SEK", testTotalPriceTextBlock.Text);
        }

        [TestMethod] // Adds one espresso to the order.
        public void VerifyEspressoOrder()
        {
            Trace.WriteLine(window.Title);

            ClickButton("plus1Espresso", 1);
            VerifyOrder(new string[] { "1 | Espresso" }, "Total Price: 32 SEK");
        }

        [TestMethod] // Adds two teas to the order and then resets the order.
        public void VerifyTeaResetOrder()
        {
            Trace.WriteLine(window.Title);

            ClickButton("plus1Tea", 2);
            VerifyOrder(new string[] { "2 | Tea" }, "Total Price: 50 SEK");

            ClickResetButton(1);
            VerifyReset();
        }

        [TestMethod] // Adds one mocha to the order, resets the order, and then adds three macchiatos.
        public void VerifyMochaResetMacchiatoOrder()
        {
            Trace.WriteLine(window.Title);

            ClickButton("plus1Mocha", 1);

            ClickResetButton(1);

            ClickButton("plus1Macchiato", 3);

            VerifyOrder(new string[] { "3 | Macchiato" }, "Total Price: 75 SEK");
        }

        [TestMethod] // Clicks the reset button multiple times.
        public void VerifyMultipleResetClicks()
        {
            Trace.WriteLine(window.Title);

            ClickResetButton(3);
            VerifyReset();
        }

        [TestMethod] // Adds a diverse set of items to the order and then resets the order.
        public void VerifyDiverseOrder()
        {
            Trace.WriteLine(window.Title);

            ClickButton("plus1Latte", 1);
            ClickButton("plus1Americano", 1);
            ClickButton("plus1FlatWhite", 1);
            VerifyOrder(new string[] { "1 | Latte", "1 | Americano", "1 | Flat White" }, "Total Price: 60 SEK");

            ClickResetButton(1);
            VerifyReset();
        }
    }
}