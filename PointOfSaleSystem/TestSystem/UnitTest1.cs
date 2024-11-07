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

                if (testPlus1Espresso == null)
                {
                    Trace.WriteLine("Button 'plus1Espresso' not found.");
                    Assert.Fail("Button 'plus1Espresso' not found.");
                }

                if (testCustomerOrderListBox == null)
                {
                    Trace.WriteLine("ListBox 'customerOrderListBox' not found.");
                    Assert.Fail("ListBox 'customerOrderListBox' not found.");
                }

                if (testTotalPriceTextBlock == null)
                {
                    Trace.WriteLine("TextBlock 'TotalPriceTextBlock' not found.");
                    Assert.Fail("TextBlock 'TotalPriceTextBlock' not found.");
                }

                testPlus1Espresso.Click();
                testPlus1Espresso.Click();
                testPlus1Espresso.Click();
                Assert.AreEqual("3 | Espresso", testCustomerOrderListBox.Items[0].Text);

                var resetButton = window.FindFirstDescendant(cf.ByAutomationId("ResetButton"))?.AsButton();
                if (resetButton == null)
                {
                    Trace.WriteLine("Button 'ResetButton' not found.");
                    Assert.Fail("Button 'ResetButton' not found.");
                }

                resetButton.Click();
                Assert.AreEqual(0, testCustomerOrderListBox.Items.Length);
                Assert.AreEqual("Total Price: 0 SEK", testTotalPriceTextBlock.Text);
            }
        }
    }
}
