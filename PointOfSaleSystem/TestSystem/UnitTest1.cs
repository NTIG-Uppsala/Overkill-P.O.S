using System.Diagnostics;
using FlaUI.UIA3;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;

namespace TestSystem
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var app = FlaUI.Core.Application.Launch(@"..\..\..\..\PointOfSaleSystem\bin\Debug\net8.0-windows\PointOfSaleSystem.exe");
            using (var automation = new UIA3Automation())
            {
                var window = app.GetMainWindow(automation);
                ConditionFactory cf = new ConditionFactory(new UIA3PropertyLibrary());
                Trace.WriteLine(window.Title);

                var testPlus1Espresso = window.FindFirstDescendant(cf.ByAutomationId("plus1Espresso"))?.AsButton();
                var testCustomerOrderListBox = window.FindFirstDescendant(cf.ByAutomationId("customerOrderListBox"))?.AsListBox();
                var testTotalPriceTextBlock = window.FindFirstDescendant(cf.ByAutomationId("TotalPriceTextBlock"))?.AsLabel();

                testPlus1Espresso.Click();
                testPlus1Espresso.Click();
                testPlus1Espresso.Click();
                Assert.AreEqual("3 | Espresso", testCustomerOrderListBox.Items[0].Text);

                var resetButton = window.FindFirstDescendant(cf.ByAutomationId("ResetButton"))?.AsButton();

                resetButton.Click();
                Assert.AreEqual(0, testCustomerOrderListBox.Items.Length);
                Assert.AreEqual("Total Price: 0 SEK", testTotalPriceTextBlock.Text);
            }
        }


        [TestMethod]
        public void TestMethod2()
        {
            var app = FlaUI.Core.Application.Launch(@"..\..\..\..\PointOfSaleSystem\bin\Debug\net8.0-windows\PointOfSaleSystem.exe");
            using (var automation = new UIA3Automation())
            {
                var window = app.GetMainWindow(automation);
                ConditionFactory cf = new ConditionFactory(new UIA3PropertyLibrary());
                Trace.WriteLine(window.Title);

                var testPlus1Espresso = window.FindFirstDescendant(cf.ByAutomationId("plus1Espresso"))?.AsButton();
                var testCustomerOrderListBox = window.FindFirstDescendant(cf.ByAutomationId("customerOrderListBox"))?.AsListBox();
                var testTotalPriceTextBlock = window.FindFirstDescendant(cf.ByAutomationId("TotalPriceTextBlock"))?.AsLabel();

                testPlus1Espresso.Click();
                Assert.AreEqual("Total Price: 32 SEK", testTotalPriceTextBlock.Text);

                var resetButton = window.FindFirstDescendant(cf.ByAutomationId("ResetButton"))?.AsButton();

                resetButton.Click();
                Assert.AreEqual(0, testCustomerOrderListBox.Items.Length);
                Assert.AreEqual("Total Price: 0 SEK", testTotalPriceTextBlock.Text);
            }
        }

        [TestMethod]
        public void TestMethod3()
        {
            var app = FlaUI.Core.Application.Launch(@"..\..\..\..\PointOfSaleSystem\bin\Debug\net8.0-windows\PointOfSaleSystem.exe");
            using (var automation = new UIA3Automation())
            {
                var window = app.GetMainWindow(automation);
                ConditionFactory cf = new ConditionFactory(new UIA3PropertyLibrary());
                Trace.WriteLine(window.Title);

                var testPlus1Tea = window.FindFirstDescendant(cf.ByAutomationId("plus1Tea"))?.AsButton();
                var testCustomerOrderListBox = window.FindFirstDescendant(cf.ByAutomationId("customerOrderListBox"))?.AsListBox();
                var testTotalPriceTextBlock = window.FindFirstDescendant(cf.ByAutomationId("TotalPriceTextBlock"))?.AsLabel();

                testPlus1Tea.Click();
                testPlus1Tea.Click();
                testPlus1Tea.Click();
                Assert.AreEqual("3 | Tea", testCustomerOrderListBox.Items[0].Text);
                Assert.AreEqual("Total Price: 75 SEK", testTotalPriceTextBlock.Text);

                var resetButton = window.FindFirstDescendant(cf.ByAutomationId("ResetButton"))?.AsButton();

                resetButton.Click();
                Assert.AreEqual(0, testCustomerOrderListBox.Items.Length);
                Assert.AreEqual("Total Price: 0 SEK", testTotalPriceTextBlock.Text);
            }
        }

        [TestMethod]
        public void TestMethod4()
        {
            var app = FlaUI.Core.Application.Launch(@"..\..\..\..\PointOfSaleSystem\bin\Debug\net8.0-windows\PointOfSaleSystem.exe");
            using (var automation = new UIA3Automation())
            {
                var window = app.GetMainWindow(automation);
                ConditionFactory cf = new ConditionFactory(new UIA3PropertyLibrary());
                Trace.WriteLine(window.Title);

                var testPlus1Mocha = window.FindFirstDescendant(cf.ByAutomationId("plus1Mocha"))?.AsButton();
                var testCustomerOrderListBox = window.FindFirstDescendant(cf.ByAutomationId("customerOrderListBox"))?.AsListBox();
                var testTotalPriceTextBlock = window.FindFirstDescendant(cf.ByAutomationId("TotalPriceTextBlock"))?.AsLabel();

                testPlus1Mocha.Click();
                testPlus1Mocha.Click();
                Assert.AreEqual("2 | Mocha", testCustomerOrderListBox.Items[0].Text);
                Assert.AreEqual("Total Price: 70 SEK", testTotalPriceTextBlock.Text);

                var resetButton = window.FindFirstDescendant(cf.ByAutomationId("ResetButton"))?.AsButton();

                resetButton.Click();
                Assert.AreEqual(0, testCustomerOrderListBox.Items.Length);
                Assert.AreEqual("Total Price: 0 SEK", testTotalPriceTextBlock.Text);
            }
        }
    }
}